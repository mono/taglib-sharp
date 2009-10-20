//
// IFDReader.cs: Parses TIFF IFDs and populates an IFD structure.
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

using System;
using System.IO;
using TagLib.IFD.Entries;

namespace TagLib.IFD
{
	/// <summary>
	///     This class contains all the IFD reading and parsing code.
	/// </summary>
	public class IFDReader {

#region Private Fields

		/// <summary>
		///    The <see cref="File" /> where this IFD is found in.
		/// </summary>
		protected readonly File file;

		/// <summary>
		///    If IFD is encoded in BigEndian or not
		/// </summary>
		protected readonly bool is_bigendian;

		/// <summary>
		///    The IFD structure that will be populated
		/// </summary>
		protected readonly IFDStructure structure;

		/// <summary>
		///     A <see cref="System.Int64"/> value describing the base were the IFD offsets
		///     refer to. E.g. in Jpegs the IFD are located in an Segment and the offsets
		///     inside the IFD refer from the beginning of this segment. So base_offset must
		///     contain the beginning of the segment.
		/// </summary>
		protected readonly long base_offset;

		/// <summary>
		///     A <see cref="System.UInt32"/> value with the beginning of the IFD relative to
		///     base_offset.
		/// </summary>
		protected readonly uint ifd_offset;

#endregion

#region Constructors

		/// <summary>
		///    Constructor. Reads an IFD from given file, using the given endianness.
		/// </summary>
		/// <param name="file">
		///    A <see cref="File"/> to read from.
		/// </param>
		/// <param name="is_bigendian">
		///     A <see cref="System.Boolean"/>, it must be true, if the data of the IFD should be
		///     read as bigendian, otherwise false.
		/// </param>
		/// <param name="structure">
		///    A <see cref="IFDStructure"/> that will be populated.
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
		public IFDReader (File file, bool is_bigendian, IFDStructure structure, long base_offset, uint ifd_offset)
		{
			this.file = file;
			this.is_bigendian = is_bigendian;
			this.structure = structure;
			this.base_offset = base_offset;
			this.ifd_offset = ifd_offset;
		}

#endregion

#region Public Methods

		/// <summary>
		///    Read an IFD at the given offsets from a file.
		/// </summary>
		public void Read ()
		{
			uint next_offset = ifd_offset;
			do {
				next_offset = ReadIFD (base_offset, next_offset);
			} while (next_offset > 0);
		}

#endregion

#region Private Methods

		/// <summary>
		///    Reads an IFD from file at position <paramref name="offset"/> relative
		///    to <paramref name="base_offset"/>.
		/// </summary>
		/// <param name="base_offset">
		///    A <see cref="System.Int64"/> with the base offset which every offset
		///    in IFD is relative to.
		/// </param>
		/// <param name="offset">
		///    A <see cref="System.UInt32"/> with the offset of the IFD relative to
		///    <paramref name="base_offset"/>
		/// </param>
		/// <returns>
		///    A <see cref="System.UInt32"/> with the offset of the next IFD, the
		///    offset is also relative to <paramref name="base_offset"/>
		/// </returns>
		private uint ReadIFD (long base_offset, uint offset)
		{
			if (base_offset + offset > file.Length)
				throw new Exception (String.Format ("Invalid IFD offset {0}, length: {1}", offset, file.Length));

			var directory = new IFDDirectory ();

			file.Seek (base_offset + offset, SeekOrigin.Begin);
			ushort entry_count = ReadUShort ();

			ByteVector entry_datas = file.ReadBlock (12 * entry_count);
			uint next_offset = ReadUInt ();

			for (int i = 0; i < entry_count; i++) {
				ByteVector entry_data = entry_datas.Mid (i * 12, 12);

				ushort entry_tag = entry_data.Mid (0, 2).ToUShort (is_bigendian);
				ushort type = entry_data.Mid (2, 2).ToUShort (is_bigendian);
				uint value_count = entry_data.Mid (4, 4).ToUInt (is_bigendian);
				ByteVector offset_data = entry_data.Mid (8, 4);

				IFDEntry entry = CreateIFDEntry (entry_tag, type, value_count, base_offset, offset_data);
				directory.Add (entry.Tag, entry);
			}

			structure.directories.Add (directory);
			return next_offset;
		}

		/// <summary>
		///    Creates an IFDEntry from the given values. This method is used for
		///    every entry. Custom parsing can be hooked in by overriding the
		///    <see cref="ParseIFDEntry(ushort,ushort,uint,long,uint)"/> method.
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
		///    A <see cref="System.Int64"/> with the base offset which every
		///    offsets in the IFD are relative to.
		/// </param>
		/// <param name="offset_data">
		///    A <see cref="ByteVector"/> containing exactly 4 byte with the data
		///    of the offset of the entry. Since this field isn't interpreted as
		///    an offset if the data can be directly stored in the 4 byte, we
		///    pass the <see cref="ByteVector"/> to easier interpret it.
		/// </param>
		/// <returns>
		///    A <see cref="IFDEntry"/> with the given parameter.
		/// </returns>
		private IFDEntry CreateIFDEntry (ushort tag, ushort type, uint count, long base_offset, ByteVector offset_data)
		{
			uint offset = offset_data.ToUInt (is_bigendian);

			// Fix the type for the IPTC tag.
			// From http://www.awaresystems.be/imaging/tiff/tifftags/iptc.html
			// "Often times, the datatype is incorrectly specified as LONG. "
			if (tag == (ushort) IFDEntryTag.IPTC && type == (ushort) IFDEntryType.Long) {
				type = (ushort) IFDEntryType.Byte;
			}

			var ifd_entry = ParseIFDEntry (tag, type, count, base_offset, offset);
			if (ifd_entry != null)
				return ifd_entry;

			// then handle the values stored in the offset data itself
			if (count == 1) {
				if (type == (ushort) IFDEntryType.Short)
					return new ShortIFDEntry (tag, offset_data.Mid (0, 2).ToUShort (is_bigendian));

				if (type == (ushort) IFDEntryType.Long)
					return new LongIFDEntry (tag, offset_data.ToUInt (is_bigendian));

			}

			if (count == 2) {
				if (type == (ushort) IFDEntryType.Short) {
					ushort [] data = new ushort [] {
						offset_data.Mid (0, 2).ToUShort (is_bigendian),
						offset_data.Mid (2, 2).ToUShort (is_bigendian)};

					return new ShortArrayIFDEntry (tag, data);
				}
			}

			if (count <= 4) {
				if (type == (ushort) IFDEntryType.Undefined)
					return new UndefinedIFDEntry (tag, offset_data);

				if (type == (ushort) IFDEntryType.Ascii)
					return new StringIFDEntry (tag, offset_data.Mid (0, (int)count - 1).ToString ());
			}


			// then handle data referenced by the offset
			file.Seek (base_offset + offset, SeekOrigin.Begin);

			if (count == 1) {
				if (type == (ushort) IFDEntryType.Rational) {
					uint numerator = ReadUInt ();
					uint denominator = ReadUInt ();

					return new RationalIFDEntry (tag, numerator, denominator);
				}

				if (type == (ushort) IFDEntryType.SRational) {
					int numerator = ReadInt ();
					int denominator = ReadInt ();

					return new SRationalIFDEntry (tag, numerator, denominator);
				}
			}

			if (count > 1) {
				if (type == (ushort) IFDEntryType.Long) {
					uint [] data = ReadUIntArray (count);

					return new LongArrayIFDEntry (tag, data);
				}
			}

			if (count > 2) {
				if (type == (ushort) IFDEntryType.Short) {
					ushort [] data = ReadUShortArray (count);

					return new ShortArrayIFDEntry (tag, data);
				}
			}

			if (count > 4) {
				if (type == (ushort) IFDEntryType.Long) {
					uint [] data = ReadUIntArray (count);

					return new LongArrayIFDEntry (tag, data);
				}

				if (type == (ushort) IFDEntryType.Byte) {
					ByteVector data = file.ReadBlock ((int) count);

					return new ByteVectorIFDEntry (tag, data);
				}

				if (type == (ushort) IFDEntryType.Ascii) {
					string data = ReadAsciiString ((int) count);

					return new StringIFDEntry (tag, data);
				}

				if (type == (ushort) IFDEntryType.Undefined) {
					ByteVector data = file.ReadBlock ((int) count);

					return new UndefinedIFDEntry (tag, data);
				}
			}

			// TODO: We should ignore unreadable values, erroring for now until we have sufficient coverage.
			throw new NotImplementedException (String.Format ("Unknown type/count {0}/{1}", type, count));
		}

		/// <summary>
		///    Reads a 2-byte unsigned short from the current file.
		/// </summary>
		/// <returns>
		///    A <see cref="ushort" /> value containing the short read
		///    from the current instance.
		/// </returns>
		private ushort ReadUShort ()
		{
			return file.ReadBlock (2).ToUShort (is_bigendian);
		}

		/// <summary>
		///    Reads a 4-byte int from the current file.
		/// </summary>
		/// <returns>
		///    A <see cref="uint" /> value containing the int read
		///    from the current instance.
		/// </returns>
		private int ReadInt ()
		{
			return file.ReadBlock (4).ToInt (is_bigendian);
		}

		/// <summary>
		///    Reads a 4-byte unsigned int from the current file.
		/// </summary>
		/// <returns>
		///    A <see cref="uint" /> value containing the int read
		///    from the current instance.
		/// </returns>
		private uint ReadUInt ()
		{
			return file.ReadBlock (4).ToUInt (is_bigendian);
		}

		/// <summary>
		///    Reads an array of 2-byte shorts from the current file.
		/// </summary>
		/// <returns>
		///    An array of <see cref="ushort" /> values containing the
		///    shorts read from the current instance.
		/// </returns>
		private ushort [] ReadUShortArray (uint count)
		{
			ushort [] data = new ushort [count];
			for (int i = 0; i < count; i++)
				data [i] = ReadUShort ();
			return data;
		}

		/// <summary>
		///    Reads an array of 4-byte unsigned int from the current file.
		/// </summary>
		/// <returns>
		///    An array of <see cref="ushort" /> values containing the
		///    shorts read from the current instance.
		/// </returns>
		private uint [] ReadUIntArray (uint count)
		{
			uint [] data = new uint [count];
			for (int i = 0; i < count; i++)
				data [i] = ReadUInt ();
			return data;
		}

		/// <summary>
		///    Reads an ASCII string from the current file.
		/// </summary>
		/// <returns>
		///    A <see cref="string" /> read from the current instance.
		/// </returns>
		private string ReadAsciiString (int count)
		{
			// The last character is \0
			return file.ReadBlock (count - 1).ToString ();
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
		protected virtual IFDEntry ParseIFDEntry (ushort tag, ushort type, uint count, long base_offset, uint offset) {
			IFDStructure ifd_structure = new IFDStructure ();
			IFDReader reader = new IFDReader (file, is_bigendian, ifd_structure, base_offset, offset);

			switch (tag) {
				case (ushort) IFDEntryTag.ExifIFD:
				case (ushort) IFDEntryTag.InteroperabilityIFD:
				case (ushort) IFDEntryTag.GPSIFD:
					reader.Read ();
					return new SubIFDEntry (tag, type, count, ifd_structure);

				case (ushort) ExifEntryTag.MakerNote:
					// A maker note may be a Sub IFD, but it may also be in an arbitrary
					// format. We try to parse a Sub IFD, if this fails, go ahead to read
					// it as an Undefined Entry below.
					try {
						reader.Read ();
						return new SubIFDEntry (tag, type, count, ifd_structure);
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
