//
// BoxHeader.cs: Provides support for reading and writing headers for ISO/IEC
// 14496-12 boxes.
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2006-2007 Brian Nickel
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

namespace TagLib.Mpeg4
{
	/// <summary>
	///    This structure provides support for reading and writing headers
	///    for ISO/IEC 14496-12 boxes.
	/// </summary>
	public struct BoxHeader
	{
		#region Private Fields

		/// <summary>
		///    Contains the box size.
		/// </summary>
		ulong box_size;

		/// <summary>
		///    Contains the header size.
		/// </summary>
		uint header_size;

		/// <summary>
		///    Contains the position of the header.
		/// </summary>
		readonly long position;

		/// <summary>
		///    Indicated that the header was read from a file.
		/// </summary>
		readonly bool from_disk;

		#endregion



		#region Public Fields

		/// <summary>
		///    An empty box header.
		/// </summary>
		public static readonly BoxHeader Empty = new BoxHeader ("xxxx");

		#endregion



		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="BoxHeader" /> by reading it from a specified seek
		///    position in a specified file.
		/// </summary>
		/// <param name="file">
		///    A <see cref="TagLib.File" /> object to read the new
		///    instance from.
		/// </param>
		/// <param name="position">
		///    A <see cref="long" /> value specifiying the seek position
		///    in <paramref name="file" /> at which to start reading.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="file" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="CorruptFileException">
		///    There isn't enough data in the file to read the complete
		///    header.
		/// </exception>
		public BoxHeader (TagLib.File file, long position)
		{
			if (file == null)
				throw new ArgumentNullException (nameof (file));

			Box = null;
			from_disk = true;
			this.position = position;
			file.Seek (position);

			ByteVector data = file.ReadBlock (32);
			int offset = 0;

			if (data.Count < 8 + offset)
				throw new CorruptFileException ("Not enough data in box header.");

			header_size = 8;
			box_size = data.Mid (offset, 4).ToUInt ();
			BoxType = data.Mid (offset + 4, 4);

			// If the size is 1, that just tells us we have a
			// massive ULONG size waiting for us in the next 8
			// bytes.
			if (box_size == 1) {
				if (data.Count < 8 + offset)
					throw new CorruptFileException ("Not enough data in box header.");

				header_size += 8;
				offset += 8;
				box_size = data.Mid (offset, 8).ToULong ();
			}

			// UUID has a special header with 16 extra bytes.
			if (BoxType == Mpeg4.BoxType.Uuid) {
				if (data.Count < 16 + offset)
					throw new CorruptFileException ("Not enough data in box header.");

				header_size += 16;
				ExtendedType = data.Mid (offset, 16);
			} else
				ExtendedType = null;

			if (box_size > (ulong)(file.Length - position)) {
				throw new CorruptFileException (
					$"Box header specified a size of {box_size} bytes but only {file.Length - position} bytes left in the file");
			}
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="BoxHeader" /> with a specified box type.
		/// </summary>
		/// <param name="type">
		///    A <see cref="ByteVector" /> object containing the four
		///    byte box type.
		/// </param>
		/// <remarks>
		///    <see cref="BoxHeader(ByteVector,ByteVector)" /> must be
		///    used to create a header of type "<c>uuid</c>".
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="type" /> is <see langword="null" /> or is
		///    equal to "<c>uuid</c>".
		/// </exception>
		/// <exception cref="ArgumentException">
		///    <paramref name="type" /> isn't exactly 4 bytes long.
		/// </exception>
		public BoxHeader (ByteVector type) : this (type, null)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="BoxHeader" /> with a specified box type and
		///    optionally extended type.
		/// </summary>
		/// <param name="type">
		///    A <see cref="ByteVector" /> object containing the four
		///    byte box type.
		/// </param>
		/// <param name="extendedType">
		///    A <see cref="ByteVector" /> object containing the four
		///    byte box type.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="type" /> is <see langword="null" /> - or -
		///    <paramref name="type" /> is equal to "<c>uuid</c>" and
		///    <paramref name="extendedType" /> is <see langword="null"
		///    />.
		/// </exception>
		/// <exception cref="ArgumentException">
		///    <paramref name="type" /> isn't exactly 4 bytes long - or
		///    - <paramref name="type" /> isn't "<c>uuid</c>" but
		///    <paramref name="extendedType" /> isn't <see
		///    langword="null" /> - or - paramref name="type" /> is
		///    "<c>uuid</c>" but <paramref name="extendedType" /> isn't
		///    exactly 16 bytes long.
		/// </exception>
		public BoxHeader (ByteVector type, ByteVector extendedType)
		{
			position = -1;
			Box = null;
			from_disk = false;
			BoxType = type;

			if (type == null)
				throw new ArgumentNullException (nameof (type));

			if (type.Count != 4)
				throw new ArgumentException ("Box type must be 4 bytes in length.", nameof (type));

			box_size = header_size = 8;

			if (type != "uuid") {
				if (extendedType != null)
					throw new ArgumentException ("Extended type only permitted for 'uuid'.", nameof (extendedType));

				ExtendedType = extendedType;
				return;
			}

			if (extendedType == null)
				throw new ArgumentNullException (nameof (extendedType));

			if (extendedType.Count != 16)
				throw new ArgumentException (
					"Extended type must be 16 bytes in length.",
					nameof (extendedType));

			box_size = header_size = 24;
			ExtendedType = extendedType;
		}

		#endregion



		#region Public Properties

		/// <summary>
		///    Gets the type of box represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="ByteVector" /> object containing the 4 byte
		///    box type.
		/// </value>
		public ByteVector BoxType { get; private set; }

		/// <summary>
		///    Gets the extended type of the box represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="ByteVector" /> object containing the 16 byte
		///    extended type, or <see langword="null" /> if <see
		///    cref="BoxType" /> is not "<c>uuid</c>".
		/// </value>
		public ByteVector ExtendedType { get; private set; }

		/// <summary>
		///    Gets the size of the header represented by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="long" /> value containing the size of the
		///    header represented by the current instance.
		/// </value>
		public long HeaderSize {
			get { return header_size; }
		}

		/// <summary>
		///    Gets and sets the size of the data in the box described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="long" /> value containing the size of the
		///    data in the box described by the current instance.
		/// </value>
		public long DataSize {
			get { return (long)(box_size - header_size); }
			set { box_size = (ulong)value + header_size; }
		}

		/// <summary>
		///    Gets the offset of the box data from the position of the
		///    header.
		/// </summary>
		/// <value>
		///    A <see cref="long" /> value containing the offset of the
		///    box data from the position of the header.
		/// </value>
		[Obsolete ("Use HeaderSize")]
		public long DataOffset {
			get { return header_size; }
		}

		/// <summary>
		///    Gets the total size of the box described by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="long" /> value containing the total size of
		///    the box described by the current instance.
		/// </value>
		public long TotalBoxSize {
			get { return (long)box_size; }
		}

		/// <summary>
		///    Gets the position box represented by the current instance
		///    in the file it comes from.
		/// </summary>
		/// <value>
		///    A <see cref="long" /> value containing the position box
		///    represented by the current instance in the file it comes
		///    from.
		/// </value>
		public long Position {
			get { return from_disk ? position : -1; }
		}

		#endregion



		#region Public Methods

		/// <summary>
		///    Overwrites the header on disk, updating it to include a
		///    change in the size of the box.
		/// </summary>
		/// <param name="file">
		///    A <see cref="TagLib.File" /> object containing the file
		///    from which the box originates.
		/// </param>
		/// <param name="sizeChange">
		///    A <see cref="long" /> value indicating the change in the
		///    size of the box described by the current instance.
		/// </param>
		/// <returns>
		///    The size change encountered by the box that parents the
		///    box described the the current instance, equal to the
		///    size change of the box plus any size change that should
		///    happen in the header.
		/// </returns>
		public long Overwrite (TagLib.File file, long sizeChange)
		{
			if (file == null)
				throw new ArgumentNullException (nameof (file));

			if (!from_disk)
				throw new InvalidOperationException ("Cannot overwrite headers not on disk.");

			long old_header_size = HeaderSize;
			DataSize += sizeChange;
			file.Insert (Render (), position, old_header_size);
			return sizeChange + HeaderSize - old_header_size;
		}

		/// <summary>
		///    Renders the header represented by the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="ByteVector" /> object containing the
		///    rendered version of the current instance.
		/// </returns>
		public ByteVector Render ()
		{
			// Enlarge for size if necessary.
			if ((header_size == 8 || header_size == 24) && box_size > uint.MaxValue) {
				header_size += 8;
				box_size += 8;
			}

			// Add the box size and type to the output.
			ByteVector output = ByteVector.FromUInt (
				(header_size == 8 || header_size == 24) ?
				(uint)box_size : 1);
			output.Add (BoxType);

			// If the box size is 16 or 32, we must have more a
			// large header to append.
			if (header_size == 16 || header_size == 32)
				output.Add (ByteVector.FromULong (box_size));

			// The only reason for such a big size is an extended
			// type. Extend!!!
			if (header_size >= 24)
				output.Add (ExtendedType);

			return output;
		}

		#endregion



		#region Internal Properties

		/// <summary>
		///    Gets and sets the box represented by the current instance
		///    as a means of temporary storage for internal uses.
		/// </summary>
		internal Box Box { get; set; }

		#endregion
	}
}
