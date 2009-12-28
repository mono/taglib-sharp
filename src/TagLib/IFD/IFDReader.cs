//
// IFDReader.cs: Parses TIFF IFDs and populates an IFD structure.
//
// Author:
//   Ruben Vermeersch (ruben@savanne.be)
//   Mike Gemuende (mike@gemuende.de)
//
// Copyright (C) 2009 Ruben Vermeersch
// Copyright (C) 2009 Mike Gemuende
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
using TagLib.IFD.Makernotes;

namespace TagLib.IFD
{
	/// <summary>
	///     This class contains all the IFD reading and parsing code.
	/// </summary>
	public class IFDReader {

#region Private Constants

		private static readonly string PANASONIC_HEADER = "Panasonic\0\0\0";
		private static readonly string PENTAX_HEADER = "AOC\0";
		private static readonly string NIKON_HEADER = "Nikon\0";
		private static readonly string OLYMPUS1_HEADER = "OLYMP\0";
		private static readonly string OLYMPUS2_HEADER = "OLYMPUS\0";
		private static readonly string SONY_HEADER = "SONY DSC \0\0\0";

#endregion

#region Protected Fields

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

		/// <summary>
		///    A <see cref="System.UInt32"/> with the maximal offset, which should occur in the
		///    IFD. Greater offsets, would reference beyond the considered data.
		/// </summary>
		protected readonly uint max_offset;

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
		/// <param name="max_offset">
		/// 	A <see cref="System.UInt32"/> value with maximal possible offset. This is to limit
		///     the size of the possible data;
		/// </param>
		public IFDReader (File file, bool is_bigendian, IFDStructure structure, long base_offset, uint ifd_offset, uint max_offset)
		{
			this.file = file;
			this.is_bigendian = is_bigendian;
			this.structure = structure;
			this.base_offset = base_offset;
			this.ifd_offset = ifd_offset;
			this.max_offset = max_offset;
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
				next_offset = ReadIFD (base_offset, next_offset, max_offset);
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
		/// <param name="max_offset">
		///    A <see cref="System.UInt32"/> with the maximal offset to consider for
		///    the IFD.
		/// </param>
		/// <returns>
		///    A <see cref="System.UInt32"/> with the offset of the next IFD, the
		///    offset is also relative to <paramref name="base_offset"/>
		/// </returns>
		private uint ReadIFD (long base_offset, uint offset, uint max_offset)
		{
			if (base_offset + offset > file.Length)
				throw new Exception (String.Format ("Invalid IFD offset {0}, length: {1}", offset, file.Length));

			var directory = new IFDDirectory ();

			file.Seek (base_offset + offset, SeekOrigin.Begin);
			ushort entry_count = ReadUShort ();

			if (file.Tell + 12 * entry_count > base_offset + max_offset)
				throw new Exception (String.Format ("Size of entries exceeds possible data size"));

			ByteVector entry_datas = file.ReadBlock (12 * entry_count);
			uint next_offset = ReadUInt ();

			for (int i = 0; i < entry_count; i++) {
				ByteVector entry_data = entry_datas.Mid (i * 12, 12);

				ushort entry_tag = entry_data.Mid (0, 2).ToUShort (is_bigendian);
				ushort type = entry_data.Mid (2, 2).ToUShort (is_bigendian);
				uint value_count = entry_data.Mid (4, 4).ToUInt (is_bigendian);
				ByteVector offset_data = entry_data.Mid (8, 4);

				IFDEntry entry = CreateIFDEntry (entry_tag, type, value_count, base_offset, offset_data, max_offset);

				if (entry == null)
					continue;

				if (directory.ContainsKey (entry.Tag))
					directory.Remove (entry.Tag);

				directory.Add (entry.Tag, entry);
			}

			FixupDirectory (base_offset, directory);

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
		/// <param name="max_offset">
		///    A <see cref="System.UInt32"/> with the maximal offset to consider for
		///    the IFD.
		/// </param>
		/// <returns>
		///    A <see cref="IFDEntry"/> with the given parameter.
		/// </returns>
		private IFDEntry CreateIFDEntry (ushort tag, ushort type, uint count, long base_offset, ByteVector offset_data, uint max_offset)
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
				if (type == (ushort) IFDEntryType.Byte)
					return new ByteIFDEntry (tag, offset_data[0]);

				if (type == (ushort) IFDEntryType.SByte)
					return new SByteIFDEntry (tag, (sbyte)offset_data[0]);

				if (type == (ushort) IFDEntryType.Short)
					return new ShortIFDEntry (tag, offset_data.Mid (0, 2).ToUShort (is_bigendian));

				if (type == (ushort) IFDEntryType.SShort)
					return new SShortIFDEntry (tag, (short) offset_data.Mid (0, 2).ToUShort (is_bigendian));

				if (type == (ushort) IFDEntryType.Long)
					return new LongIFDEntry (tag, offset_data.ToUInt (is_bigendian));

				if (type == (ushort) IFDEntryType.SLong)
					return new SLongIFDEntry (tag, offset_data.ToInt (is_bigendian));

			}

			if (count == 2) {
				if (type == (ushort) IFDEntryType.Short) {
					ushort [] data = new ushort [] {
						offset_data.Mid (0, 2).ToUShort (is_bigendian),
						offset_data.Mid (2, 2).ToUShort (is_bigendian)
					};

					return new ShortArrayIFDEntry (tag, data);
				}

				if (type == (ushort) IFDEntryType.SShort) {
					short [] data = new short [] {
						(short) offset_data.Mid (0, 2).ToUShort (is_bigendian),
						(short) offset_data.Mid (2, 2).ToUShort (is_bigendian)
					};

					return new SShortArrayIFDEntry (tag, data);
				}
			}

			if (count <= 4) {
				if (type == (ushort) IFDEntryType.Undefined)
					return new UndefinedIFDEntry (tag, offset_data.Mid (0, (int)count));

				if (type == (ushort) IFDEntryType.Ascii)
					return new StringIFDEntry (tag, offset_data.Mid (0, (int)count - 1).ToString ());

				if (type == (ushort) IFDEntryType.Byte)
					return new ByteVectorIFDEntry (tag, offset_data.Mid (0, (int)count));
			}


			// FIXME: create correct type.
			if (offset > max_offset)
				return new UndefinedIFDEntry (tag, new ByteVector ());

			// then handle data referenced by the offset
			file.Seek (base_offset + offset, SeekOrigin.Begin);

			if (count == 1) {
				if (type == (ushort) IFDEntryType.Rational)
					return new RationalIFDEntry (tag, ReadRational ());

				if (type == (ushort) IFDEntryType.SRational)
					return new SRationalIFDEntry (tag, ReadSRational ());
			}

			if (count > 1) {
				if (type == (ushort) IFDEntryType.Long) {
					uint [] data = ReadUIntArray (count);

					return new LongArrayIFDEntry (tag, data);
				}

				if (type == (ushort) IFDEntryType.SLong) {
					int [] data = ReadIntArray (count);

					return new SLongArrayIFDEntry (tag, data);
				}

				if (type == (ushort) IFDEntryType.Rational) {
					Rational[] entries = new Rational [count];

					for (int i = 0; i < count; i++)
						entries[i] = ReadRational ();

					return new RationalArrayIFDEntry (tag, entries);
				}
			}

			if (count > 2) {
				if (type == (ushort) IFDEntryType.Short) {
					ushort [] data = ReadUShortArray (count);

					return new ShortArrayIFDEntry (tag, data);
				}

				if (type == (ushort) IFDEntryType.SShort) {
					short [] data = ReadShortArray (count);

					return new SShortArrayIFDEntry (tag, data);
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

				if (tag == (ushort) ExifEntryTag.UserComment) {
					ByteVector data = file.ReadBlock ((int) count);

					return new UserCommentIFDEntry (tag, data);
				}

				if (type == (ushort) IFDEntryType.Undefined) {
					ByteVector data = file.ReadBlock ((int) count);

					return new UndefinedIFDEntry (tag, data);
				}
			}

			if (type == (ushort) IFDEntryType.Float)
				return null;

			// TODO: We should ignore unreadable values, erroring for now until we have sufficient coverage.
			throw new NotImplementedException (String.Format ("Unknown type/count {0}/{1}", type, count));
		}

		/// <summary>
		///    Reads a 2-byte signed short from the current file.
		/// </summary>
		/// <returns>
		///    A <see cref="short" /> value containing the short read
		///    from the current instance.
		/// </returns>
		private short ReadShort ()
		{
			return (short) file.ReadBlock (2).ToUShort (is_bigendian);
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
		///    Reads a <see cref="Rational"/> by two following unsigned
		///    int from the current file.
		/// </summary>
		/// <returns>
		///    A <see cref="Rational"/> value created by the read values.
		/// </returns>
		private Rational ReadRational ()
		{
			uint numerator = ReadUInt ();
			uint denominator = ReadUInt ();

			// correct illegal value
			if (denominator == 0) {
				numerator = 0;
				denominator = 1;
			}

			return new Rational (numerator, denominator);
		}

		/// <summary>
		///    Reads a <see cref="SRational"/> by two following unsigned
		///    int from the current file.
		/// </summary>
		/// <returns>
		///    A <see cref="SRational"/> value created by the read values.
		/// </returns>
		private SRational ReadSRational ()
		{
			int numerator = ReadInt ();
			int denominator = ReadInt ();

			// correct illegal value
			if (denominator == 0) {
				numerator = 0;
				denominator = 1;
			}

			return new SRational (numerator, denominator);
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
		///    Reads an array of 2-byte signed shorts from the current file.
		/// </summary>
		/// <returns>
		///    An array of <see cref="short" /> values containing the
		///    shorts read from the current instance.
		/// </returns>
		private short [] ReadShortArray (uint count)
		{
			short [] data = new short [count];
			for (int i = 0; i < count; i++)
				data [i] = ReadShort ();
			return data;
		}

		/// <summary>
		///    Reads an array of 4-byte int from the current file.
		/// </summary>
		/// <returns>
		///    An array of <see cref="int" /> values containing the
		///    shorts read from the current instance.
		/// </returns>
		private int [] ReadIntArray (uint count)
		{
			int [] data = new int [count];
			for (int i = 0; i < count; i++)
				data [i] = ReadInt ();
			return data;
		}

		/// <summary>
		///    Reads an array of 4-byte unsigned int from the current file.
		/// </summary>
		/// <returns>
		///    An array of <see cref="uint" /> values containing the
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
		/// <remarks>
		///    The exif standard allows to store multiple string separated
		///    by '\0' in one ASCII-field. On the other hand some programs
		///    (e.g. CanonZoomBrowser) fill some ASCII fields by trailing
		///    '\0's.
		///    We follow the Adobe practice as described in XMP Specification
		///    Part 3 (Storeage in Files), and process the ASCII string only
		///    to the first '\0'.
		/// </remarks>
		private string ReadAsciiString (int count)
		{
			// The last character is \0
			string str = file.ReadBlock (count - 1).ToString ();
			int term = str.IndexOf ('\0');

			if (term == -1)
				return str;
			else
				return str.Substring (0, term);
		}

		/// <summary>
		///    Performs some fixups to a read <see cref="IFDDirectory"/>. For some
		///    special cases multiple <see cref="IFDEntry"/> instances contained
		///    in the directory are needed. Therfore, we do the fixups after reading the
		///    whole directory to be sure, all entries are present.
		/// </summary>
		/// <param name="base_offset">
		///    A <see cref="System.Int64"/> value with the base offset, all offsets in the
		///    directory refers to.
		/// </param>
		/// <param name="directory">
		///    A <see cref="IFDDirectory"/> instance which was read and needs fixes.
		/// </param>
		private void FixupDirectory (long base_offset, IFDDirectory directory)
		{
			// The following two entries refer to thumbnail data, where one is  the offset
			// to the data and the other is the length. Unnaturally both are used to describe
			// the data. So it is needed to keep both entries in sync and keep the thumbnail data
			// for writing it back.
			// We determine the position of the data, read it and store it in an ThumbnailDataIFDEntry
			// which replaces the offset-entry to thumbnail data.
			ushort offset_tag = (ushort) IFDEntryTag.JPEGInterchangeFormat;
			ushort length_tag = (ushort) IFDEntryTag.JPEGInterchangeFormatLength;
			if (directory.ContainsKey (offset_tag) && directory.ContainsKey (length_tag)) {

				var offset_entry = directory [offset_tag] as LongIFDEntry;
				var length_entry = directory [length_tag] as LongIFDEntry;

				if (offset_entry != null && length_entry != null) {
					uint offset = offset_entry.Value;
					uint length = length_entry.Value;

					file.Seek (base_offset + offset, SeekOrigin.Begin);
					ByteVector data = file.ReadBlock ((int) length);

					directory.Remove (offset_tag);
					directory.Add (offset_tag, new ThumbnailDataIFDEntry (offset_tag, data));
				}
			}
		}

		private IFDEntry ParseMakernote (ushort tag, ushort type, uint count, long base_offset, uint offset)
		{
			long makernote_offset = base_offset + offset;
			IFDStructure ifd_structure = new IFDStructure ();

			// This is the minimum size a makernote should have
			// The shortest header is PENTAX_HEADER (4)
			// + IFD entry count (2)
			// + at least one IFD etry (12)
			// + next IFD pointer (4)
			// = 22 ....
			// we use this number to read a header which is big used
			// to identify the makernote types
			int header_size = 18;

			if (file.Length < makernote_offset)
			    throw new Exception ("offset to makernote is beyond file size");

			if (file.Length < makernote_offset + header_size)
				throw new Exception ("data is to short to contain a maker note ifd");

			// read header
			file.Seek (makernote_offset, SeekOrigin.Begin);
			ByteVector header = file.ReadBlock (header_size);

			if (header.StartsWith (PANASONIC_HEADER)) {
				IFDReader reader =
					new IFDReader (file, is_bigendian, ifd_structure, base_offset, offset + 12, max_offset);

				reader.ReadIFD (base_offset, offset + 12, max_offset);
				return new MakernoteIFDEntry (tag, ifd_structure, MakernoteType.Panasonic, PANASONIC_HEADER, 12, true, null);
			}

			if (header.StartsWith (PENTAX_HEADER)) {
				IFDReader reader =
					new IFDReader (file, is_bigendian, ifd_structure, base_offset, offset + 6, max_offset);

				reader.ReadIFD (base_offset, offset + 6, max_offset);
				return new MakernoteIFDEntry (tag, ifd_structure, MakernoteType.Pentax, header.Mid (0, 6), 6, true, null);
			}

			if (header.StartsWith (OLYMPUS1_HEADER)) {
				IFDReader reader =
					new IFDReader (file, is_bigendian, ifd_structure, base_offset, offset + 8, max_offset);

				reader.Read ();
				return new MakernoteIFDEntry (tag, ifd_structure, MakernoteType.Olympus1, header.Mid (0, 8), 8, true, null);
			}

			if (header.StartsWith (OLYMPUS2_HEADER)) {
				IFDReader reader =
					new IFDReader (file, is_bigendian, ifd_structure, makernote_offset, 12, count);

				reader.Read ();
				return new MakernoteIFDEntry (tag, ifd_structure, MakernoteType.Olympus2, header.Mid (0, 12), 12, false, null);
			}

			if (header.StartsWith (SONY_HEADER)) {
				IFDReader reader =
					new IFDReader (file, is_bigendian, ifd_structure, base_offset, offset + 12, max_offset);

				reader.ReadIFD (base_offset, offset + 12, max_offset);
				return new MakernoteIFDEntry (tag, ifd_structure, MakernoteType.Sony, SONY_HEADER, 12, true, null);
			}

			if (header.StartsWith (NIKON_HEADER)) {

				ByteVector endian_bytes = header.Mid (10, 2);

				if (endian_bytes.ToString () == "II" || endian_bytes.ToString () == "MM") {

					bool makernote_endian = endian_bytes.ToString ().Equals ("MM");
					ushort magic = header.Mid (12, 2).ToUShort (is_bigendian);

					if (magic == 42) {
						var reader = new Nikon3MakernoteReader (file, makernote_endian, ifd_structure, makernote_offset + 10, 8, count - 10);

						reader.Read ();
						return new MakernoteIFDEntry (tag, ifd_structure, MakernoteType.Nikon3, header.Mid (0, 18), 8, false, makernote_endian);
					}
				}
			}

			try {
				IFDReader reader =
					new IFDReader (file, is_bigendian, ifd_structure, base_offset, offset, max_offset);

				reader.Read ();
				return new MakernoteIFDEntry (tag, ifd_structure, MakernoteType.Canon);
			} catch {
				return null;
			}
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
		protected virtual IFDEntry ParseIFDEntry (ushort tag, ushort type, uint count, long base_offset, uint offset)
		{
			if (tag == (ushort) ExifEntryTag.MakerNote)
				return ParseMakernote (tag, type, count, base_offset, offset);

			IFDStructure ifd_structure = new IFDStructure ();
			IFDReader reader = CreateSubIFDReader (file, is_bigendian, ifd_structure, base_offset, offset, max_offset);

			// Sub IFDs are either identified by the IFD-type ...
			if (type == (ushort) IFDEntryType.IFD) {
				reader.Read ();
				return new SubIFDEntry (tag, type, (uint) ifd_structure.Directories.Length, ifd_structure);
			}

			// ... or by one of the following tags
			switch (tag) {
			case (ushort) IFDEntryTag.ExifIFD:
			case (ushort) IFDEntryTag.InteroperabilityIFD:
			case (ushort) IFDEntryTag.GPSIFD:
				reader.Read ();
				return new SubIFDEntry (tag, (ushort) IFDEntryType.Long, 1, ifd_structure);

			default:
				return null;
			}
		}

		/// <summary>
		///    Create a reader for Sub IFD entries.
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
		///    A <see cref="System.Int64"/> with the base offset which every offsets in the
		///    IFD are relative to.
		/// </param>
		/// <param name="offset">
		///    A <see cref="System.UInt32"/> with the offset of the entry.
		/// </param>
		/// <param name="max_offset">
		///    A <see cref="System.UInt32"/> with the maximal offset to consider for
		///    the IFD.
		/// </param>
		/// <returns>
		///    A <see cref="IFDReader"/> which can be used to read the specified sub IFD.
		/// </returns>
		protected virtual IFDReader CreateSubIFDReader (File file, bool is_bigendian, IFDStructure structure, long base_offset, uint offset, uint max_offset)
		{
			return new IFDReader (file, is_bigendian, structure, base_offset, offset, max_offset);
		}

#endregion

	}
}
