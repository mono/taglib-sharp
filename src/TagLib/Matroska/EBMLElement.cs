//
// EBMLElement.cs:
//
// Author:
//   Julien Moutte <julien@fluendo.com>
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

            if (position > (ulong) (_file.Length - 4))
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

        #region Public Methods

        /// <summary>
        /// Reads a string from EBML Element's data section.
        /// </summary>
        /// <returns>a string object containing the parsed value.</returns>
        public string ReadString ()
        {
            if (file == null) {
                return null;
            }

            file.Seek ((long) data_offset);

            ByteVector vector = file.ReadBlock ((int) ebml_size);

            return vector.ToString ();
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
        /// Reads an unsigned 32 bits integer from EBML Element's data section.
        /// </summary>
        /// <returns>a uint containing the parsed value.</returns>
        public uint ReadUInt ()
        {
            if (file == null) {
                return 0;
            }

            file.Seek ((long) data_offset);

            ByteVector vector = file.ReadBlock ((int) ebml_size);

            return vector.ToUInt ();
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

        #endregion
    }
}
