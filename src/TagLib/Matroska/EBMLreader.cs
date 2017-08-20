//
// EBMLElement.cs:
//
// Author:
//   Julien Moutte <julien@fluendo.com>
//   Sebastien Mouy <starwer@laposte.net>
//
// Copyright (C) 2011 FLUENDO S.A.
//
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

using System;


namespace TagLib.Matroska
{
    /// <summary>
    /// Read a Matroska EBML element from a file, but also provides basic modification of an EBML element directly on the file (write).
    /// </summary>
    /// <remarks>
    ///  This was intitialy called <see cref="EBMLelement"/>, but this was in fact a file-reader.
    ///  The name <see cref="EBMLelement"/> correspond more to the class which has been created to
    ///  represent an EBML structure (regardless of file-issues) to support the EBML writing to file.
    /// </remarks>
    public class EBMLreader
    {
        #region Private Fields

        private ulong offset = 0;
        private ulong data_offset = 0;
        private uint ebml_id = 0;
        private ulong ebml_size = 0;
        private Matroska.File file = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a <see cref="EBMLreader" /> reading from provided
        /// file data.
        /// </summary>
        /// <param name="_file"><see cref="File" /> instance to read from.</param>
        /// <param name="position">Position to start reading from.</param>
        public EBMLreader(Matroska.File _file, ulong position)
        {
            if (_file == null)
                throw new ArgumentNullException("file");

            if (position >= (ulong)(_file.Length) - 1)
                throw new ArgumentOutOfRangeException("position");

            // Keep a reference to the file
            file = _file;

            file.Seek((long)position);

            // Get the header byte
            ByteVector vector = file.ReadBlock(1);
            Byte header_byte = vector[0];
            // Define a mask
            Byte mask = 0x80, id_length = 1;
            // Figure out the size in bytes
            while (id_length <= 4 && (header_byte & mask) == 0)
            {
                id_length++;
                mask >>= 1;
            }

            if (id_length > 4)
            {
                throw new CorruptFileException("invalid EBML id size");
            }

            // Now read the rest of the EBML ID
            if (id_length > 1)
            {
                vector.Add(file.ReadBlock(id_length - 1));
            }

            ebml_id = vector.ToUInt();

            vector.Clear();

            // Get the size length
            vector = file.ReadBlock(1);
            header_byte = vector[0];
            mask = 0x80;
            Byte size_length = 1;

            // Iterate through various possibilities
            while (size_length <= 8 && (header_byte & mask) == 0)
            {
                size_length++;
                mask >>= 1;
            }

            if (size_length > 8)
            {
                throw new CorruptFileException("invalid EBML element size");
            }

            // Clear the marker bit
            vector[0] &= (Byte)(mask - 1);

            // Now read the rest of the EBML element size
            if (size_length > 1)
            {
                vector.Add(file.ReadBlock(size_length - 1));
            }

            ebml_size = vector.ToULong();

            offset = position;
            data_offset = offset + id_length + size_length;
        }


        /// <summary>
        /// Create a new <see cref="EBMLreader" /> with arbitrary attributes, 
        /// without reading its information on the file.
        /// </summary>
        /// <param name="_file">instance to the EBML to be associated with the element.</param>
        /// <param name="position">Position in the file.</param>
        /// <param name="ebmlid">EBML ID of the element</param>
        public EBMLreader(Matroska.File _file, ulong position, MatroskaID ebmlid)
        {
            // Keep a reference to the file
            file = _file;

            // Initialize attributes
            offset = position;
            data_offset = offset;
            ebml_id = (uint)ebmlid;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// EBML Element Identifier.
        /// </summary>
        public uint ID
        {
            get { return ebml_id; }
        }

        /// <summary>
        /// EBML Element size in bytes.
        /// </summary>
        public ulong Size
        {
            get { return (data_offset - offset) + ebml_size; }
        }

        /// <summary>
        /// EBML Element data size in bytes.
        /// </summary>
        public ulong DataSize
        {
           get { return ebml_size; }
        }

        /// <summary>
        /// EBML Element data offset in bytes.
        /// </summary>
        public ulong DataOffset
        {
            get { return data_offset; }
        }

        /// <summary>
        /// EBML Element offset in bytes.
        /// </summary>
        public ulong Offset
        {
            get { return offset; }
        }

        #endregion

        #region Public Methods for Reading

        /// <summary>
        /// Reads a vector of bytes (raw data) from EBML Element's data section.
        /// </summary>
        /// <returns>a <see cref="ByteVector" /> containing the parsed value.</returns>
        public ByteVector ReadBytes()
        {
            if (file == null)
            {
                return null;
            }

            file.Seek((long)data_offset);

            ByteVector vector = file.ReadBlock((int)ebml_size);

            return vector;
        }

        /// <summary>
        /// Reads a string from EBML Element's data section (UTF-8).
        /// </summary>
        /// <returns>a string object containing the parsed value.</returns>
        public string ReadString()
        {
            if (file == null ) return null;
            ByteVector vector = ReadBytes();
            var ebml = new EBMLelement((MatroskaID)ebml_id, vector);
            return ebml.GetString();

        }

        /// <summary>
        /// Reads a boolean from EBML Element's data section.
        /// </summary>
        /// <returns>a bool containing the parsed value.</returns>
        public bool ReadBool()
        {
            if (file == null || ebml_size == 0) return false;
            ByteVector vector = ReadBytes();
            var ebml = new EBMLelement((MatroskaID)ebml_id, vector);
            return ebml.GetBool();

        }

        /// <summary>
        /// Reads a double from EBML Element's data section.
        /// </summary>
        /// <returns>a double containing the parsed value.</returns>
        public double ReadDouble()
        {
            if (file == null || ebml_size == 0) return 0;
            ByteVector vector = ReadBytes();
            var ebml = new EBMLelement((MatroskaID) ebml_id, vector);
            return ebml.GetDouble();
        }

        /// <summary>
        /// Reads an unsigned integer (any size from 1 to 8 bytes) from EBML Element's data section.
        /// </summary>
        /// <returns>a ulong containing the parsed value.</returns>
        public ulong ReadULong()
        {
            if (file == null || ebml_size == 0) return 0;
            ByteVector vector = ReadBytes();
            var ebml = new EBMLelement((MatroskaID)ebml_id, vector);
            return ebml.GetULong();
        }


        #endregion

        #region Public Methods for Writing
        
        /// <summary>
        /// Write data to the EBML file
        /// </summary>
        /// <param name="data">ByteVector data</param>
        /// <returns>Size difference compare to previous EBML size</returns>
        public long WriteData(ByteVector data)
        {
            long ret = -(long)Size;
            long old_size = (long)DataSize;

            if (data == null)
            {
                // Remove EBML Data field
                WriteDataSize(0);
                if (old_size > 0) file.RemoveBlock((long)data_offset, old_size);
            }
            else
            {
                // Force DataSize field to be created
                if (ebml_size == 0 && data.Count == 0) ebml_size = 1;

                // Write EBML Data field
                WriteDataSize((ulong)data.Count);
                file.Insert(data, (long)data_offset, old_size);
            }

            ret += (long)Size;
            return ret;
        }


        /// <summary>
        /// Add an offset to the data-size of the EBML
        /// </summary>
        /// <param name="offset">offset size (positive or negative) to be added to the DataSize</param>
        /// <returns>Size difference compare to previous EBML size</returns>
        public long WriteDataResize(long offset)
        {
            long ret = 0;

            if (offset != 0)
            {
                ret -= (long)Size;
                WriteDataSize((ulong)((long)DataSize + offset));
                ret += (long)Size;
            }

            return ret;
        }

        /// <summary>
        /// Write a new value for the <see cref="DataSize"/> to the EBML file
        /// </summary>
        /// <param name="value">New value of the <see cref="DataSize"/> </param>
        public void WriteDataSize(ulong value)
        {
            if (value != ebml_size)
            {
                // Figure out the ID size in bytes
                uint mask = 0xFF00, id_length = 1;
                while (id_length <= 4 && (ebml_id & mask) != 0)
                {
                    id_length++;
                    mask <<= 8;
                }
                if (id_length > 4)
                {
                    throw new CorruptFileException("invalid EBML id size");
                }

                ulong size_length = data_offset - offset - id_length;

                // Figure out the required data-size size in bytes
                ulong newsize_length = 1;
                if (value == 0x7F)
                {
                    // Special case: Avoid element-size reserved word of 0xFF (all ones)
                    newsize_length = 2;
                }
                else
                {
                    mask = 0x3F80;
                    while (newsize_length <= 8 && (value & mask) != 0)
                    {
                        newsize_length++;
                        mask <<= 7;
                    }
                }

                // Lazy change
                if (newsize_length < size_length) newsize_length = size_length;

                if (size_length > 8)
                {
                    throw new CorruptFileException("invalid EBML element size");
                }

                // Construct the data-size field
                ByteVector vector = new ByteVector((int)newsize_length);
                mask = (uint)value;
                for (int i = (int)newsize_length - 1; i >= 0; i--)
                {
                    vector[i] = (byte)(mask & 0xFF);
                    mask >>= 8;
                }
                // Set the marker bit
                vector[0] |= (byte)(0x100 >> (int)newsize_length);

                // Write data-size field to file
                file.Insert(vector, (long)offset + id_length, (long)size_length);

                // Update fields
                data_offset = data_offset + newsize_length - size_length;
                ebml_size = value;
            }
        }



        /// <summary>
        /// Remove the EBML element from the file
        /// </summary>
        /// <returns>Size difference compare to previous EBML size</returns>
        public long Remove()
        {
            long ret = -(long)Size;

            file.RemoveBlock((long)Offset, (long)Size);

            // Invalidate this object
            data_offset = offset;
            ebml_id = 0;
            ebml_size = 0;
            file = null;

            return ret;
        }


        #endregion


    }
}
