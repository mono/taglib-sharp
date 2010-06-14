//
// BaseTiffFile.cs:
//
// Author:
//   Mike Gemuende (mike@gemuende.de)
//
// Copyright (C) 2010 Mike Gemuende
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

using TagLib.Image;
using TagLib.IFD;


namespace TagLib.Tiff
{

	/// <summary>
	///    This class extends <see cref="TagLib.Image.File" /> to provide some basic behavior
	///    for Tiff based file formats.
	/// </summary>
	public abstract class BaseTiffFile : TagLib.Image.File
	{

#region Public Properties

		/// <summary>
		///    Indicates if the current file is in big endian or little endian format.
		/// </summary>
		/// <remarks>
		///    The method <see cref="ReadHeader()"/> must be called from a subclass to
		///    properly initialize this property.
		/// </remarks>
		public bool IsBigEndian {
			get; private set;
		}

#endregion


#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="File" /> for a specified path in the local file
		///    system.
		/// </summary>
		/// <param name="path">
		///    A <see cref="string" /> object containing the path of the
		///    file to use in the new instance.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		protected BaseTiffFile (string path) : base (path)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="File" /> for a specified file abstraction.
		/// </summary>
		/// <param name="abstraction">
		///    A <see cref="IFileAbstraction" /> object to use when
		///    reading from and writing to the file.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="abstraction" /> is <see langword="null"
		///    />.
		/// </exception>
		protected BaseTiffFile (IFileAbstraction abstraction) : base (abstraction)
		{
		}

#endregion

#region Protected Methods

		/// <summary>
		///    Reads and validates the TIFF header at the current position.
		/// </summary>
		/// <returns>
		///    A <see cref="System.UInt32"/> with the offset value to the first
		///    IFD contained in the file.
		/// </returns>
		/// <remarks>
		///    This method should only be called, when the current read position is
		///    the beginning of the file.
		/// </remarks>
		protected uint ReadHeader ()
		{
			// TIFF header:
			//
			// 2 bytes         Indicating the endianess (II or MM)
			// 2 bytes         Tiff Magic word (42)
			// 4 bytes         Offset to first IFD

			ByteVector header = ReadBlock (8);

			if (header.Count != 8)
				throw new CorruptFileException ("Unexpected end of header");

			string order = header.Mid (0, 2).ToString ();

			if (order == "II") {
				IsBigEndian = false;
			} else if (order == "MM") {
				IsBigEndian = true;
			} else {
				throw new CorruptFileException ("Unknown Byte Order");
			}

			if (header.Mid (2, 2).ToUShort (IsBigEndian) != 42)
				throw new CorruptFileException ("TIFF Magic (42) expected");

			uint first_ifd_offset = header.Mid (4, 4).ToUInt (IsBigEndian);

			return first_ifd_offset;
		}


		/// <summary>
		///    Reads IFDs starting from the given offset.
		/// </summary>
		/// <param name="offset">
		///    A <see cref="System.UInt32"/> with the IFD offset to start
		///    reading from.
		/// </param>
		protected void ReadIFD (uint offset)
		{
			ReadIFD (offset, -1);
		}


		/// <summary>
		///    Reads a certain number of IFDs starting from the given offset.
		/// </summary>
		/// <param name="offset">
		///    A <see cref="System.UInt32"/> with the IFD offset to start
		///    reading from.
		/// </param>
		/// <param name="ifd_count">
		///    A <see cref="System.Int32"/> with the number of IFDs to read.
		/// </param>
		protected void ReadIFD (uint offset, int ifd_count)
		{
			var ifd_tag = GetTag (TagTypes.TiffIFD, true) as IFDTag;
			var reader = new IFDReader (this, IsBigEndian, ifd_tag.Structure, 0, offset, (uint) Length);

			reader.Read (ifd_count);
		}


		/// <summary>
		///    Renders a TIFF header with the given offset to the first IFD.
		///    The returned data has length 8.
		/// </summary>
		/// <param name="first_ifd_offset">
		///    A <see cref="System.UInt32"/> with the offset to the first IFD
		///    to be included in the header.
		/// </param>
		/// <returns>
		///    A <see cref="ByteVector"/> with the rendered header of length 8.
		/// </returns>
		protected ByteVector RenderHeader (uint first_ifd_offset)
		{
			ByteVector data = new ByteVector ();

			if (IsBigEndian)
				data.Add ("MM");
			else
				data.Add ("II");

			data.Add (ByteVector.FromUShort (42, IsBigEndian));
			data.Add (ByteVector.FromUInt (first_ifd_offset, IsBigEndian));

			return data;
		}

#endregion

	}
}
