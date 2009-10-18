//
// ExifReader.cs: Parses Exif files.
//
// Author:
//   Ruben Vermeersch (ruben@savanne.be)
//   Mike Gemuende (mike@gemuende.de)
//
// Copyright (C) 2009 Ruben Vermeersch
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

using TagLib.IFD;
using TagLib.IFD.Entries;

namespace TagLib.Exif
{
	/// <summary>
	///     This class contains all the IFD reading and parsing code.
	/// </summary>
	public class ExifReader : IFDReader {

#region Constructors

		/// <summary>
		///    Constructor. Reads an Exif tag from given file, using the given
		///    endianness.
		/// </summary>
		/// <param name="file">
		///    A <see cref="File"/> to read from.
		/// </param>
		/// <param name="is_bigendian">
		///     A <see cref="System.Boolean"/>, it must be true, if the data of the IFD should be
		///     read as bigendian, otherwise false.
		/// </param>
		/// <param name="tag">
		///    A <see cref="IFDTag"/> that will be populated.
		/// </param>
		/// <param name="base_offset">
		///     A <see cref="System.Int64"/> value describing the base were the IFD offsets
		///     refer to. E.g. in Jpegs the IFD are located in an Segment and the offsets
		///     inside the IFD refer from the beginning of this segment. So <paramref
		///     name="base_offset"/> must contain the beginning of the segment.
		/// </param>
		/// <param name="ifd_offset">
		///     A <see cref="System.UInt32"/> value with the beginning of the IFD relative to
		///     <paramref name="base_offset"/>.
		/// </param>
		public ExifReader (File file, bool is_bigendian, IFDTag tag, long base_offset, uint ifd_offset)
			: base (file, is_bigendian, tag, base_offset, ifd_offset)
		{
		}

#endregion

#region Protected Methods

		/// <summary>
		///    Try to parse the given IFD entry, used to discover format-specific entries.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="System.UInt16"/> with the tag of the entry.
		/// </param>
		/// <param name="type">
		///    A <see cref="System.UInt16"/> with the type of the entry.
		/// </param>
		/// <param name="count">
		///    A <see cref="System.UInt32"/> with the data count of the entry.
		/// </param>
		/// <param name="base_offset">
		///    A <see cref="System.Int64"/> with the base offset which every offsets in the
		///    IFD are relative to.
		/// </param>
		/// <param name="offset">
		///    A <see cref="System.UInt32"/> with the offset of the entry.
		/// </param>
		/// <returns>
		///    A <see cref="IFDEntry"/> with the given parameters, or null if none was parsed, after
		///    which the normal TIFF parsing is used.
		/// </returns>
		protected override IFDEntry ParseIFDEntry (ushort tag, ushort type, uint count, long base_offset, uint offset) {
			IFDTag ifd_tag = new IFDTag ();
			IFDReader reader = new ExifReader (file, is_bigendian, ifd_tag, base_offset, offset);

			switch (tag) {
				case (ushort) IFDEntryTag.ExifIFD:
				case (ushort) IFDEntryTag.IopIFD:
				case (ushort) IFDEntryTag.GPSIFD:
					reader.Read ();
					return new SubIFDEntry (tag, type, count, ifd_tag);

				case (ushort) IFDEntryTag.MakerNoteIFD:
					// A maker note may be a Sub IFD, but it may also be in an arbitrary
					// format. We try to parse a Sub IFD, if this fails, go ahead to read
					// it as an Undefined Entry below.
					try {
						reader.Read ();
						return new SubIFDEntry (tag, type, count, ifd_tag);
					} catch {
						return null;
					}

				default:
					return null;
			}
		}

#endregion

	}
}
