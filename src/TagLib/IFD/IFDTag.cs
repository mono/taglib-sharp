//
// IFDTag.cs: Basic Tag-class to handle an IFD (Image File Directory) with
// its image-tags.
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
using System.Collections.Generic;
using System.IO;

using TagLib.IFD.Entries;

namespace TagLib.IFD
{
	/// <summary>
	///    Contains the metadata for one IFD (Image File Directory).
	/// </summary>
	public class IFDTag : Tag
	{
#region Private Fields

		/// <summary>
		///    The <see cref="File" /> where this IFD is found in.
		/// </summary>
		private File file;

		/// <summary>
		///    Contains the entries in this IFD.
		/// </summary>
		private Dictionary<uint, IFDEntry> entries = new Dictionary<uint, IFDEntry> ();

		/// <summary>
		///    If IFD is encoded in BigEndian or not
		/// </summary>
		private bool is_bigendian;

#endregion

#region Constructors

		/// <summary>
		///    Constructor. Reads an IFD from given file at position <see cref="ifd_offset"/>
		///    relative to <see cref="base_offset"/>.
		/// </summary>
		/// <param name="file">
		///    A <see cref="File"/> to read from.
		/// </param>
		/// <param name="base_offset">
		///     A <see cref="System.UInt32"/> value describing the base were the IFD offsets
		///     refer to. E.g. in Jpegs the IFD are located in an Segment and the offsets
		///     inside the IFD refer from the beginning of this segment. So <see cref="base_offset"/>
		///     must contain the beginning of the segment.
		/// </param>
		/// <param name="ifd_offset">
		///     A <see cref="System.Int64"/> value with the beginning of the IFD relative to
		///     <see cref="base_offset"/>.
		/// </param>
		/// <param name="is_bigendian">
		///     A <see cref="System.Boolean"/>, it must be true, if the data of the IFD should be
		///     read as bigendian, otherwise false.
		/// </param>
		/// <param name="next_offset">
		///     A <see cref="System.UInt32"/> containing the offset of the following IFD relative
		///     to <see cref="base_offset"/>.
		/// </param>
		public IFDTag (File file, long base_offset, uint ifd_offset, bool is_bigendian, out uint next_offset)
		{
			this.file = file;
			this.is_bigendian = is_bigendian;

			ReadIFD (base_offset, ifd_offset, out next_offset);
		}

		/// <summary>
		///    Constructor. Reads an IFD from given file at position <see cref="ifd_offset"/>.
		/// </summary>
		/// <param name="file">
		///    A <see cref="File"/> to read from.
		/// </param>
		/// <param name="ifd_offset">
		///     A <see cref="System.UInt32"/> value with the beginning of the IFD.
		/// </param>
		/// <param name="is_bigendian">
		///     A <see cref="System.Boolean"/>, it must be true, if the data of the IFD should be
		///     read as bigendian, otherwise false.
		/// </param>
		/// <param name="next_offset">
		///     A <see cref="System.UInt32"/> containing the offset.
		/// </param>
		public IFDTag (File file, uint ifd_offset, bool is_bigendian, out uint next_offset)
			: this (file, 0, ifd_offset, is_bigendian, out next_offset) {}

#endregion

#region Public Properties

		/// <summary>
		///    Gets the IFD entries contained in the current instance.
		/// </summary>
		/// <value>
		///    An array of <see cref="IFDEntry"/> instances.
		/// </value>
		public IFDEntry [] Entries {
			get { return new List<IFDEntry> (entries.Values).ToArray (); }
		}

		/// <summary>
		///    Gets the file, the current instance belongs to.
		/// </summary>
		public File File {
			get { return file; }
		}

		/// <summary>
		///    Gets the tag types contained in the current instance.
		/// </summary>
		/// <value>
		///    Always <see cref="TagTypes.TiffIFD" />.
		/// </value>
		public override TagTypes TagTypes {
			get { return TagTypes.TiffIFD; }
		}

#endregion

#region Public Methods

		/// <summary>
		///    Clears the values stored in the current instance.
		/// </summary>
		public override void Clear ()
		{
			throw new NotImplementedException ();
		}

		/// <summary>
		///    Checks, if a value for the given tag is contained in the IFD.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="System.UInt32"/> value with the tag.
		/// </param>
		/// <returns>
		///    A <see cref="System.Boolean"/>, which is true, if the tag is already
		///    contained in the IFD, otherwise false.
		/// </returns>
		public virtual bool ContainsTag (uint tag)
		{
			return entries.ContainsKey (tag);
		}

		/// <summary>
		///    Removes a given tag from the IFD.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="System.UInt32"/> value with the tag to remove.
		/// </param>
		public virtual void RemoveTag (uint tag)
		{
			entries.Remove (tag);
		}

		/// <summary>
		///    Removes a given tag from the IFD.
		/// </summary>
		/// <param name="entry_tag">
		///    A <see cref="IFDEntryTag"/> value with the tag to remove.
		/// </param>
		public virtual void RemoveTag (IFDEntryTag entry_tag)
		{
			RemoveTag ((uint) entry_tag);
		}

		/// <summary>
		///    Adds an <see cref="IFDEntry"/> to the IFD, if it is not already
		///    contained in, it fails otherwise.
		/// </summary>
		/// <param name="entry">
		///    A <see cref="IFDEntry"/> to add to the IFD.
		/// </param>
		public virtual void AddEntry (IFDEntry entry)
		{
			entries.Add (entry.Tag, entry);
		}

		/// <summary>
		///    Adds an <see cref="IFDEntry"/> to the IFD. If it is already contained
		///    in the IFD, it is overwritten.
		/// </summary>
		/// <param name="entry">
		///    A <see cref="IFDEntry"/> to add to the IFD.
		/// </param>
		public virtual void SetEntry (IFDEntry entry)
		{
			if (ContainsTag (entry.Tag))
				RemoveTag (entry.Tag);

			AddEntry (entry);
		}

		/// <summary>
		///   Returns the <see cref="IFDEntry"/> belonging to the given tag.
		/// </summary>
		/// <param name="entry_tag">
		///    A <see cref="System.UInt16"/> with the tag to get.
		/// </param>
		/// <returns>
		///    A <see cref="IFDEntry"/> belonging to the given tag, or
		///    null, if no such tag is contained in the IFD.
		/// </returns>
		public IFDEntry GetEntry (ushort entry_tag)
		{
			if (!entries.ContainsKey (entry_tag))
				return null;

			return entries [entry_tag];
		}

		/// <summary>
		///    Returns the <see cref="IFDEntry"/> belonging to the given tag.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="IFDEntryTag"/> with the tag to get.
		/// </param>
		/// <returns>
		///    A <see cref="IFDEntry"/> belonging to the given tag, or
		///    null, if no such tag is contained in the IFD.
		/// </returns>
		public IFDEntry GetEntry (IFDEntryTag tag)
		{
			return GetEntry ((ushort) tag);
		}

		/// <summary>
		///    Renders the current instance to a <see cref="ByteVector"/>.
		/// </summary>
		/// <param name="ifd_offset">
		///    A <see cref="System.UInt32"/> value with the offset of the current IFD.
		///    All offsets inside the IFD must be adjusted according to this given offset.
		/// </param>
		/// <param name="is_bigendian">
		///    A <see cref="System.Boolean"/>, which must be true, if the IFD should be
		///    rendered with bigendian data, otherwise false.
		/// </param>
		/// <param name="last">
		///    A <see cref="System.Boolean"/>, which must be true, if this IFD should not
		///    refer to any next IFD, otherwise false. If there is a next IFD, it is asumed,
		///    that it starts immediately after this IFD, thus it must have the offset
		///    ifd_offset + length of returned <see cref="ByteVector"/>.
		/// </param>
		/// <returns>
		///    A <see cref="ByteVector"/> containing the rendered IFD.
		/// </returns>
		public ByteVector Render (uint ifd_offset, bool is_bigendian, bool last)
		{
			this.is_bigendian = is_bigendian;

			ByteVector ifd_data = RenderIFD (ifd_offset, last);

			return ifd_data;
		}

#endregion


#region Private Methods

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

		/// <summary>
		///    Reads an IFD from file at position <see cref="offset"/> relative to <see cref="base_offset"/>.
		/// </summary>
		/// <param name="base_offset">
		///    A <see cref="System.Int64"/> with the base offset which every offset in IFD is relative to.
		/// </param>
		/// <param name="offset">
		///    A <see cref="System.UInt32"/> with the offset of the IFD relative to <see cref="base_offset"/>
		/// </param>
		/// <param name="next_offset">
		///    A <see cref="System.UInt32"/> with the offset of the next IFD, the offset is also relative to
		///    <see cref="base_offset"/>
		/// </param>
		private void ReadIFD (long base_offset, uint offset, out uint next_offset)
		{
			if (base_offset + offset > file.Length)
				throw new Exception (String.Format ("Invalid IFD offset {0}, length: {1}", offset, file.Length));

			file.Seek (base_offset + offset, SeekOrigin.Begin);
			ushort entry_count = ReadUShort ();
			uint end_offset = offset + 2 + 12 * (uint) entry_count;

			ByteVector entry_datas = file.ReadBlock (12 * entry_count);
			next_offset = ReadUInt ();

			for (int i = 0; i < entry_count; i++) {
				ByteVector entry_data = entry_datas.Mid (i * 12, 12);

				ushort entry_tag = entry_data.Mid (0, 2).ToUShort (is_bigendian);
				ushort type = entry_data.Mid (2, 2).ToUShort (is_bigendian);
				uint value_count = entry_data.Mid (4, 4).ToUInt (is_bigendian);
				ByteVector offset_data = entry_data.Mid (8, 4);

				IFDEntry entry = CreateIFDEntry (entry_tag, type, value_count, base_offset, offset_data);
				AddEntry (entry);
			}
		}

		/// <summary>
		///    Renders the IFD to an ByteVector where the offset of the IFD itself is <see cref="ifd_offset"/>
		///    and all offsets contained in the IFD are adjusted accroding it.
		/// </summary>
		/// <param name="ifd_offset">
		///    A <see cref="System.UInt32"/> with the offset of the IFD
		/// </param>
		/// <param name="last">
		///    A <see cref="System.Boolean"/> which is true, if the IFD is the last one, i.e. the offset to
		///    the next IFD, which is stored inside the IFD, is 0. If the value is false, the offset to the
		///    next IFD is set that it starts directly after the current one.
		/// </param>
		/// <returns>
		///    A <see cref="ByteVector"/> with the rendered IFD.
		/// </returns>
		private ByteVector RenderIFD (uint ifd_offset, bool last)
		{
			if (entries.Count > (int)UInt16.MaxValue)
				throw new Exception (String.Format ("Directory has too much entries: {0}", entries.Count));

			ushort entry_count = (ushort) entries.Count;

			// ifd_offset + size of entry_count + entries + next ifd offset
			uint data_offset = ifd_offset + 2 + 12 * (uint) entry_count + 4;

			// store the entries itself
			ByteVector entry_data = new ByteVector ();

			// store the data referenced by the entries
			ByteVector offset_data = new ByteVector ();

			entry_data.Add (ByteVector.FromUShort (entry_count, is_bigendian));

			foreach (IFDEntry entry in Entries)
				RenderEntryData (entry, entry_data, offset_data, data_offset);

			if (last)
				entry_data.Add ("\0\0\0\0");
			else
				entry_data.Add (ByteVector.FromUInt ((uint) (data_offset + offset_data.Count), is_bigendian));

			if (data_offset - ifd_offset != entry_data.Count)
				throw new Exception (String.Format ("Expected IFD data size was {0} but is {1}", data_offset - ifd_offset, entry_data.Count));

			entry_data.Add (offset_data);

			return entry_data;
		}

#endregion

#region Protected Methods

		/// <summary>
		///    Creates an IFDEntry from the given values. This method is used for every entry and can be
		///    overwritten in subclasses to provide own handling of some special entries.
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
		/// <param name="offset_data">
		///    A <see cref="ByteVector"/> containing exactly 4 byte with the data of the offset
		///    of the entry. Since this field isn't interpreted as an offset if the data can be
		///    directly stored in the 4 byte, we pass the <see cref="ByteVector"/> to easier interpret it.
		/// </param>
		/// <returns>
		///    A <see cref="IFDEntry"/> with the given parameter.
		/// </returns>
		protected virtual IFDEntry CreateIFDEntry (ushort tag, ushort type, uint count, long base_offset, ByteVector offset_data)
		{
			uint offset = offset_data.ToUInt (is_bigendian);

			// Fix the type for the IPTC tag.
			// From http://www.awaresystems.be/imaging/tiff/tifftags/iptc.html
			// "Often times, the datatype is incorrectly specified as LONG. "
			if (tag == (ushort) IFDEntryTag.IPTC && type == (ushort) IFDEntryType.Long) {
				type = (ushort) IFDEntryType.Byte;
			}

			if (tag == (ushort) IFDEntryTag.ExifIFD) {
				// TODO
			}

			if (tag == (ushort) IFDEntryTag.IopIFD) {
				// TODO
			}

			// A maker note may be a Sub IFD, but it may also be in an arbitrary
			// format. We try to parse a Sub IFD, if this fails, go ahead to read
			// it as an Undefined Entry below.
			if (tag == (ushort) IFDEntryTag.MakerNoteIFD) {
				// TODO
			}

			// handle first the values stored in the offset data itself
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
		///    Adds the data of a single entry to <see cref="entry_data"/>.
		/// </summary>
		/// <param name="entry_data">
		///    A <see cref="ByteVector"/> to add the entry to.
		/// </param>
		/// <param name="tag">
		///    A <see cref="System.UInt16"/> with the tag of the entry.
		/// </param>
		/// <param name="type">
		///    A <see cref="System.UInt16"/> with the type of the entry.
		/// </param>
		/// <param name="count">
		///    A <see cref="System.UInt32"/> with the data count of the entry,
		/// </param>
		/// <param name="offset">
		///    A <see cref="System.UInt32"/> with the offset field of the entry.
		/// </param>
		protected void RenderEntry (ByteVector entry_data, ushort tag, ushort type, uint count, uint offset)
		{
			entry_data.Add (ByteVector.FromUShort (tag, is_bigendian));
			entry_data.Add (ByteVector.FromUShort (type, is_bigendian));
			entry_data.Add (ByteVector.FromUInt (count, is_bigendian));
			entry_data.Add (ByteVector.FromUInt (offset, is_bigendian));
		}

		/// <summary>
		///    Renders a complete entry together with the data. The entry itself is stored in
		///    <see cref="entry_data"/> and the data of the entry is stored in <see cref="offset_data"/>
		///    if it cannot be stored in the offset. This method is called for ever <see cref="IFDEntry"/>
		///    of this IFD and can be overwritten in subclasses to provide special behavior.
		/// </summary>
		/// <param name="entry">
		///    A <see cref="IFDEntry"/> with the entry to render.
		/// </param>
		/// <param name="entry_data">
		///    A <see cref="ByteVector"/> to add the entry to.
		/// </param>
		/// <param name="offset_data">
		///    A <see cref="ByteVector"/> to add the entry data to if it cannot be stored in the offset
		///    field.
		/// </param>
		/// <param name="data_offset">
		///    A <see cref="System.UInt32"/> with the offset, were the data of the entries starts. It is
		///    needed to adjust the offsets of the entries itself.
		/// </param>
		protected virtual void RenderEntryData (IFDEntry entry, ByteVector entry_data, ByteVector offset_data, uint data_offset)
		{
			ushort tag = (ushort) entry.Tag;
			ushort type;
			uint count;
			uint offset = (uint) (data_offset + offset_data.Count);
			ByteVector data = null;

			if (entry is ByteVectorIFDEntry) {
				data = (entry as ByteVectorIFDEntry).Data;
				type = (ushort) IFDEntryType.Byte;
				count = (uint) data.Count;

			} else if (entry is LongArrayIFDEntry) {
				LongIFDEntry [] long_entries = (entry as LongArrayIFDEntry).Values;
				count = (uint) long_entries.Length;

				data = new ByteVector ();
				foreach (LongIFDEntry long_entry in long_entries) {
					data.Add (ByteVector.FromUInt (long_entry.Value, is_bigendian));
				}

				type = (ushort) IFDEntryType.Long;

			} else if (entry is LongIFDEntry) {
				data = ByteVector.FromUInt ((entry as LongIFDEntry).Value, is_bigendian);
				type = (ushort) IFDEntryType.Long;
				count = 1;

			} else if (entry is ShortArrayIFDEntry) {
				ShortIFDEntry [] short_entries = (entry as ShortArrayIFDEntry).Values;
				count = (uint) short_entries.Length;

				data = new ByteVector ();
				foreach (ShortIFDEntry short_entry in short_entries) {
					data.Add (ByteVector.FromUShort (short_entry.Value, is_bigendian));
				}

				type = (ushort) IFDEntryType.Short;

			} else if (entry is ShortIFDEntry) {
				data = ByteVector.FromUInt ((entry as ShortIFDEntry).Value, is_bigendian);
				type = (ushort) IFDEntryType.Short;
				count = 1;

			} else if (entry is RationalIFDEntry) {
				var rational_entry = entry as RationalIFDEntry;
				data = new ByteVector ();
				data.Add (ByteVector.FromUInt (rational_entry.Numerator, is_bigendian));
				data.Add (ByteVector.FromUInt (rational_entry.Denominator, is_bigendian));
				type = (ushort) IFDEntryType.Rational;
				count = 1;

			} else if (entry is SRationalIFDEntry) {
				var rational_entry = entry as SRationalIFDEntry;
				data = new ByteVector ();
				data.Add (ByteVector.FromInt (rational_entry.Numerator, is_bigendian));
				data.Add (ByteVector.FromInt (rational_entry.Denominator, is_bigendian));
				type = (ushort) IFDEntryType.SRational;
				count = 1;

			} else if (entry is StringIFDEntry) {
				data = (entry as StringIFDEntry).Value;
				data.Add ("\0");
				type = (ushort) IFDEntryType.Ascii;
				count = (uint) data.Count;

			} else if (entry is UndefinedIFDEntry) {
				data = (entry as UndefinedIFDEntry).Data;
				type = (ushort) IFDEntryType.Undefined;
				count = (uint) data.Count;

			} else if (entry is SubIFDEntry) {
				var sub_ifd = (entry as SubIFDEntry);
				data = sub_ifd.IFDTag.Render (offset, is_bigendian, true);

				type = (ushort) sub_ifd.Type;
				count = sub_ifd.Count;

			} else {
				throw new Exception (String.Format ("Unknown EntryType: {0}", entry.GetType ()));
			}

			// store data in offset, if it is smaller than 4 byte
			if (data.Count <= 4) {

				while (data.Count < 4)
					data.Add ("\0");

				offset = data.ToUInt (is_bigendian);
				data = null;
			}

			// preserve word boundary of offsets
			if (data != null && data.Count % 2 != 0)
				data.Add ("\0");

			RenderEntry (entry_data, tag, type, count, offset);
			offset_data.Add (data);
		}

#endregion
	}
}
