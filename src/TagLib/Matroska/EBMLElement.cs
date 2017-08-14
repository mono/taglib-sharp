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

using System.Collections.Generic;
using System;

namespace TagLib.Matroska
{
    /// <summary>
    /// Describes a generic EBML Element.
    /// </summary>
    public class EBMLElement
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
        /// Constructs a <see cref="EBMLElement" /> parsing from provided
        /// file data.
        /// </summary>
        /// <param name="_file"><see cref="File" /> instance to read from.</param>
        /// <param name="position">Position to start reading from.</param>
        public EBMLElement (Matroska.File _file, ulong position)
        {
            if (_file == null)
                throw new ArgumentNullException ("file");

            if (position >= (ulong) (_file.Length) - 1)
                throw new ArgumentOutOfRangeException ("position");

            // Keep a reference to the file
            file = _file;

            file.Seek ((long) position);

            // Get the header byte
            ByteVector vector = file.ReadBlock (1);
            Byte header_byte = vector [0];
            // Define a mask
            Byte mask = 0x80, id_length = 1;
            // Figure out the size in bytes
            while (id_length <= 4 && (header_byte & mask) == 0) {
                id_length++;
                mask >>= 1;
            }

            if (id_length > 4) {
                throw new CorruptFileException ("invalid EBML id size");
            }

            // Now read the rest of the EBML ID
            if (id_length > 1) {
                vector.Add (file.ReadBlock (id_length -1));
            }

            ebml_id = vector.ToUInt ();

            vector.Clear ();

            // Get the size length
            vector = file.ReadBlock (1);
            header_byte = vector [0];
            mask = 0x80;
            Byte size_length = 1;

            // Iterate through various possibilities
            while (size_length <= 8 && (header_byte & mask) == 0) {
                size_length++;
                mask >>= 1;
            }

            if (size_length > 8) {
                throw new CorruptFileException ("invalid EBML element size");
            }

            // Clear the marker bit
            vector [0] &= (Byte) (mask - 1);

            // Now read the rest of the EBML element size
            if (size_length > 1) {
                vector.Add (file.ReadBlock (size_length - 1));
            }

            ebml_size = vector.ToULong ();

            offset = position;
            data_offset = offset + id_length + size_length;
        }


        /// <summary>
        /// Insert a new <see cref="EBMLElement" /> in the file with no Data nor DataSize.
        /// </summary>
        /// <param name="_file">instance to write the EBML to.</param>
        /// <param name="position">Position to insert (writing) to.</param>
        /// <param name="ebmlid">EBML ID of the element to be created</param>
        public EBMLElement(Matroska.File _file, ulong position, MatroskaID ebmlid)
        {
            if (_file == null)
                throw new ArgumentNullException("file");

            if (_file.Mode != TagLib.File.AccessMode.Write)
                throw new CorruptFileException("File is not open for Write");

            if (position > (ulong)(_file.Length))
                throw new ArgumentOutOfRangeException("position");

            // Keep a reference to the file
            file = _file;

            // Initialize attributes
            offset = position;
            data_offset = offset;

            // Write EBML ID field
            ID = (uint)ebmlid;

        }

        /// <summary>
        /// Insert a new <see cref="EBMLElement" />  in the file.
        /// </summary>
        /// <param name="_file">instance to write the EBML to.</param>
        /// <param name="position">Position to insert (writing) to.</param>
        /// <param name="ebmlid">EBML ID of the element to be created</param>
        /// <param name="data">EBML data of the element to be created. null will not create any DataSize field.</param>
        public EBMLElement(Matroska.File _file, ulong position, MatroskaID ebmlid, ByteVector data) : this(_file, position, ebmlid)
        {
            // Write the data
            if (data != null) WriteData(data);
        }


        /// <summary>
        /// Insert a new <see cref="EBMLElement" />  in the file.
        /// </summary>
        /// <param name="_file">instance to write the EBML to.</param>
        /// <param name="position">Position to insert (writing) to.</param>
        /// <param name="ebmlid">EBML ID of the element to be created</param>
        /// <param name="value">EBML data as an unsigned long integer value.</param>
        public EBMLElement(Matroska.File _file, ulong position, MatroskaID ebmlid, ulong value) : this(_file, position, ebmlid)
        {
            WriteData(value);
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// EBML Element Identifier.
        /// </summary>
        public uint ID
        {
            set
            {
                if (value != ebml_id)
                {
                    // Figure out the ID size in bytes
                    uint mask = 0xFF00, id_length = 1;
                    if (ebml_id == 0)
                    {
                        id_length = 0;
                    }
                    else
                    {
                        while (id_length <= 4 && (ebml_id & mask) != 0)
                        {
                            id_length++;
                            mask <<= 8;
                        }
                        if (id_length > 4)
                        {
                            throw new CorruptFileException("invalid EBML id size");
                        }
                    }

                    // Figure out the new ID size in bytes
                    mask = 0xFF00;
                    uint new_length = 1;
                    while (new_length <= 4 && (value & mask) != 0)
                    {
                        new_length++;
                        mask <<= 8;
                    }
                    if (new_length > 4)
                    {
                        throw new CorruptFileException("invalid EBML id");
                    }

                    // Construct the new ID field
                    ByteVector vector = new ByteVector((int)new_length);
                    mask = (uint)value;
                    for (int i = (int)new_length - 1; i >= 0; i--)
                    {
                        vector[i] = (byte)(mask & 0xFF);
                        mask >>= 8;
                    }

                    // Write data-size field to file
                    file.Insert(vector, (long)offset, (long)id_length);

                    // Update fields
                    data_offset = data_offset + new_length - id_length;
                    ebml_id = value;

                }
            }
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
            set
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
                    if(newsize_length < size_length) newsize_length = size_length;

                    if (size_length > 8)
                    {
                        throw new CorruptFileException("invalid EBML element size");
                    }

                    // Construct the data-size field
                    ByteVector vector = new ByteVector((int)newsize_length);
                    mask = (uint)value;
                    for (int i = (int)newsize_length - 1; i>=0; i--)
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

        #region Public Methods


        /// <summary>
        /// Reads a string from EBML Element's data section (UTF-8).
        /// </summary>
        /// <returns>a string object containing the parsed value.</returns>
        public string ReadString ()
        {
            if (file == null) {
                return null;
            }

            file.Seek ((long) data_offset);

            ByteVector vector = file.ReadBlock ((int) ebml_size);

            return vector.ToString(StringType.UTF8);
        }

        /// <summary>
        /// Reads a boolean from EBML Element's data section.
        /// </summary>
        /// <returns>a bool containing the parsed value.</returns>
        public bool ReadBool ()
        {
            if (file == null) {
                return false;
            }

            file.Seek ((long) data_offset);

            ByteVector vector = file.ReadBlock ((int) ebml_size);

            if (vector.ToUInt () > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Reads a double from EBML Element's data section.
        /// </summary>
        /// <returns>a double containing the parsed value.</returns>
        public double ReadDouble ()
        {
            if (file == null) {
                return 0;
            }

            if (ebml_size != 4 && ebml_size != 8) {
                throw new UnsupportedFormatException ("Can not read a Double with sizes differing from 4 or 8");
            }

            file.Seek ((long) data_offset);

            ByteVector vector = file.ReadBlock ((int) ebml_size);

            double result = 0.0;

            if (ebml_size == 4) {
                result = (double) vector.ToFloat ();
            }
            else if (ebml_size == 8) {
                result = vector.ToDouble ();
            }

            return result;
        }

        /// <summary>
        /// Reads an unsigned integer (any size from 1 to 8 bytes) from EBML Element's data section.
        /// </summary>
        /// <returns>a ulong containing the parsed value.</returns>
        public ulong ReadULong ()
        {
            if (file == null) {
                return 0;
            }

            file.Seek ((long) data_offset);

            ByteVector vector = file.ReadBlock ((int) ebml_size);

            return vector.ToULong();
        }


        /// <summary>
        /// Reads a bytes vector from EBML Element's data section.
        /// </summary>
        /// <returns>a <see cref="ByteVector" /> containing the parsed value.</returns>
        public ByteVector ReadBytes ()
        {
            if (file == null) {
                return null;
            }

            file.Seek ((long) data_offset);

            ByteVector vector = file.ReadBlock ((int) ebml_size);

            return vector;
        }



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
                DataSize = 0;
                if (old_size > 0) file.RemoveBlock((long)data_offset, old_size);
            }
            else
            {
                // Force DataSize field to be created
                if (ebml_size == 0 && data.Count == 0) ebml_size = 1;

                // Write EBML Data field
                DataSize = (ulong)data.Count;
                file.Insert(data, (long)data_offset, old_size);
            }

            ret += (long)Size;
            return ret;
        }

        /// <summary>
        /// Write an unsigned integer (any size from 1 to 8 bytes) to the EBML file
        /// </summary>
        /// <param name="data">unsigned long number to write</param>
        /// <returns>Size difference compare to previous EBML size</returns>
        public long WriteData(ulong data)
        {
            const ulong mask = 0xffffffff00000000;
            bool isLong = (data & mask) != 0;

            ByteVector vector = new ByteVector(isLong ? 8 : 4);
            for (int i = vector.Count - 1; i >= 0; i--)
            {
                vector[i] = (byte)(data & 0xff);
                data >>= 8;
            }

            return WriteData(vector);
        }


        /// <summary>
        /// Add an offset to the data-size of the EBML
        /// </summary>
        /// <param name="offset">offset size (positive or negative) to be added to the DataSize</param>
        /// <returns>Size difference compare to previous EBML size</returns>
        public long ResizeData(long offset)
        {
            long ret = 0;

            if (offset != 0)
            {
                ret -= (long)Size;
                DataSize = (ulong)((long)DataSize + offset);
                ret += (long)Size;
            }

            return ret;
        }



        /// <summary>
        /// Remove the EBML element from the file
        /// </summary>
        /// <returns>Size difference compare to previous EBML size</returns>
        public long Remove()
        {
            long ret = - (long)Size;

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
