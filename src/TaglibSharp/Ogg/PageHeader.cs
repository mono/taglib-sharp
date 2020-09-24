//
// PageHeader.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   oggpageheader.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
// Copyright (C) 2003 Scott Wheeler (Original Implementation)
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

namespace TagLib.Ogg
{
	/// <summary>
	///    Indicates the special properties of a <see cref="Page" />.
	/// </summary>
	[Flags]
	public enum PageFlags : byte
	{
		/// <summary>
		///    The page is a normal page.
		/// </summary>
		None = 0,

		/// <summary>
		///    The first packet of the page is continued from the
		///    previous page.
		/// </summary>
		FirstPacketContinued = 1,

		/// <summary>
		///    The page is the first page of the stream.
		/// </summary>
		FirstPageOfStream = 2,

		/// <summary>
		///    The page is the last page of the stream.
		/// </summary>
		LastPageOfStream = 4
	}

	/// <summary>
	///    This structure provides a representation of an Ogg page header.
	/// </summary>
	public struct PageHeader
	{
		#region Private Propertis

		/// <summary>
		///    Contains the sizes of the packets contained in the
		///    current instance.
		/// </summary>
		readonly List<int> packet_sizes;

		/// <summary>
		///    Contains the OGG version.
		/// </summary>
		readonly byte version;

		/// <summary>
		///    Contains the page absolute granular postion.
		/// </summary>
		readonly ulong absolute_granular_position;

		#endregion



		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="PageHeader" /> with a given serial number, page
		///    number, and flags.
		/// </summary>
		/// <param name="streamSerialNumber">
		///    A <see cref="uint" /> value containing the serial number
		///    for the stream containing the page described by the new
		///    instance.
		/// </param>
		/// <param name="pageNumber">
		///    A <see cref="uint" /> value containing the index of the
		///    page described by the new instance in the stream.
		/// </param>
		/// <param name="flags">
		///    A <see cref="PageFlags" /> object containing the flags
		///    that apply to the page described by the new instance.
		/// </param>
		public PageHeader (uint streamSerialNumber, uint pageNumber, PageFlags flags)
		{
			version = 0;
			Flags = flags;
			absolute_granular_position = 0;
			StreamSerialNumber = streamSerialNumber;
			PageSequenceNumber = pageNumber;
			Size = 0;
			DataSize = 0;
			packet_sizes = new List<int> ();
			LastPacketComplete = false;

			if (pageNumber == 0 && (flags & PageFlags.FirstPacketContinued) == 0)
				Flags |= PageFlags.FirstPageOfStream;
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="PageHeader" /> by reading a raw Ogg page header
		///    from a specified position in a specified file.
		/// </summary>
		/// <param name="file">
		///    A <see cref="File" /> object containing the file from
		///    which the contents of the new instance are to be read.
		/// </param>
		/// <param name="position">
		///    A <see cref="long" /> value specify at what position to
		///    read.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="file" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///    <paramref name="position" /> is less than zero or greater
		///    than the size of the file.
		/// </exception>
		/// <exception cref="CorruptFileException">
		///    The Ogg identifier could not be found at the correct
		///    location.
		/// </exception>
		public PageHeader (File file, long position)
		{
			if (file == null)
				throw new ArgumentNullException (nameof (file));

			if (position < 0 || position > file.Length - 27)
				throw new ArgumentOutOfRangeException (nameof (position));

			file.Seek (position);

			// An Ogg page header is at least 27 bytes, so we'll go
			// ahead and read that much and then get the rest when
			// we're ready for it.

			ByteVector data = file.ReadBlock (27);
			if (data.Count < 27 || !data.StartsWith ("OggS"))
				throw new CorruptFileException ("Error reading page header");

			version = data[4];
			Flags = (PageFlags)data[5];
			absolute_granular_position = data.Mid (6, 8).ToULong (false);
			StreamSerialNumber = data.Mid (14, 4).ToUInt (false);
			PageSequenceNumber = data.Mid (18, 4).ToUInt (false);

			// Byte number 27 is the number of page segments, which
			// is the only variable length portion of the page
			// header. After reading the number of page segments
			// we'll then read in the coresponding data for this
			// count.
			int page_segment_count = data[26];
			ByteVector page_segments = file.ReadBlock (page_segment_count);

			// Another sanity check.
			if (page_segment_count < 1 || page_segments.Count != page_segment_count)
				throw new CorruptFileException ("Incorrect number of page segments");

			// The base size of an Ogg page 27 bytes plus the number
			// of lacing values.
			Size = (uint)(27 + page_segment_count);
			packet_sizes = new List<int> ();

			int packet_size = 0;
			DataSize = 0;

			for (int i = 0; i < page_segment_count; i++) {
				DataSize += page_segments[i];
				packet_size += page_segments[i];

				if (page_segments[i] < 255) {
					packet_sizes.Add (packet_size);
					packet_size = 0;
				}
			}

			if (packet_size > 0)
				packet_sizes.Add (packet_size);

			LastPacketComplete = page_segments[page_segment_count - 1] < 255;
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="PageHeader" /> by copying the values from another
		///    instance, offsetting the page number and applying new
		///    flags.
		/// </summary>
		/// <param name="original">
		///    A <see cref="PageHeader"/> object to copy the values
		///    from.
		/// </param>
		/// <param name="offset">
		///    A <see cref="uint"/> value specifying how much to offset
		///    the page sequence number in the new instance.
		/// </param>
		/// <param name="flags">
		///    A <see cref="PageFlags"/> value specifying the flags to
		///    use in the new instance.
		/// </param>
		public PageHeader (PageHeader original, uint offset, PageFlags flags)
		{
			version = original.version;
			Flags = flags;
			absolute_granular_position = original.absolute_granular_position;
			StreamSerialNumber = original.StreamSerialNumber;
			PageSequenceNumber = original.PageSequenceNumber + offset;
			Size = original.Size;
			DataSize = original.DataSize;
			packet_sizes = new List<int> ();
			LastPacketComplete = false;

			if (PageSequenceNumber == 0 && (flags & PageFlags.FirstPacketContinued) == 0)
				Flags |= PageFlags.FirstPageOfStream;
		}

		#endregion



		#region Public Properties

		/// <summary>
		///    Gets and sets the sizes for the packets in the page
		///    described by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="T:int[]" /> containing the packet sizes.
		/// </value>
		public int[] PacketSizes {
			get { return packet_sizes.ToArray (); }
			set {
				packet_sizes.Clear ();
				packet_sizes.AddRange (value);
			}
		}

		/// <summary>
		///	   Indicates whether the final packet is continued on the next page
		/// </summary>
		/// <value>
		///	   true if the final packet is complete and not continued on the next page
		/// </value>
		public bool LastPacketComplete { get; private set; }

		/// <summary>
		///	   Gets the flags for the page described by the current
		///	   instance.
		/// </summary>
		/// <value>
		///	   A <see cref="PageFlags" /> value containing the page
		///	   flags.
		/// </value>
		public PageFlags Flags { get; private set; }

		/// <summary>
		///    Gets the absolute granular position of the page described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="long" /> value containing the absolute
		///    granular position of the page.
		/// </value>
		public long AbsoluteGranularPosition {
			get { return (long)absolute_granular_position; }
		}

		/// <summary>
		///    Gets the sequence number of the page described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the sequence
		///    number of the page.
		/// </value>
		public uint PageSequenceNumber { get; private set; }

		/// <summary>
		///    Gets the serial number of stream that the page described
		///    by the current instance belongs to.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the stream serial
		///    number.
		/// </value>
		public uint StreamSerialNumber { get; private set; }

		/// <summary>
		///    Gets the size of the header as it appeared on disk.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the header size.
		/// </value>
		public uint Size { get; private set; }

		/// <summary>
		///    Gets the size of the data portion of the page described
		///    by the current instance as it appeared on disk.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the data size.
		/// </value>
		public uint DataSize { get; private set; }

		#endregion



		#region Public Methods

		/// <summary>
		///    Renders the current instance as a raw Ogg page header.
		/// </summary>
		/// <returns>
		///    A <see cref="ByteVector" /> object containing the
		///    rendered version of the current instance.
		/// </returns>
		public ByteVector Render ()
		{
			ByteVector data = new ByteVector {
				"OggS",
				version, // stream structure version
				(byte)Flags,
				ByteVector.FromULong (
				absolute_granular_position, false),
				ByteVector.FromUInt (
				StreamSerialNumber, false),
				ByteVector.FromUInt (
				PageSequenceNumber, false),
				new ByteVector (4, 0) // checksum, to be filled in later.
			};
			ByteVector page_segments = LacingValues;
			data.Add ((byte)page_segments.Count);
			data.Add (page_segments);

			return data;
		}

		#endregion



		#region Private Properties

		/// <summary>
		///    Gets the rendered lacing values for the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="ByteVector" /> object containing the
		///    rendered lacing values.
		/// </value>
		ByteVector LacingValues {
			get {
				ByteVector data = new ByteVector ();

				int[] sizes = PacketSizes;

				for (int i = 0; i < sizes.Length; i++) {
					// The size of a packet in an Ogg page
					// is indicated by a series of "lacing
					// values" where the sum of the values
					// is the packet size in bytes. Each of
					// these values is a byte. A value of
					// less than 255 (0xff) indicates the
					// end of the packet.

					int quot = sizes[i] / 255;
					int rem = sizes[i] % 255;

					for (int j = 0; j < quot; j++)
						data.Add (255);

					if (i < sizes.Length - 1 ||
						(packet_sizes[i] % 255) != 0)
						data.Add ((byte)rem);
				}

				return data;
			}
		}

		#endregion



		#region IEquatable

		/// <summary>
		///    Generates a hash code for the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="int" /> value containing the hash code for
		///    the current instance.
		/// </returns>
		public override int GetHashCode ()		{
			unchecked {
				return (int)(LacingValues.GetHashCode () ^
					version ^ (int)Flags ^
					(int)absolute_granular_position ^
					StreamSerialNumber ^
					PageSequenceNumber ^ Size ^
					DataSize);
			}
		}

		/// <summary>
		///    Checks whether or not the current instance is equal to
		///    another object.
		/// </summary>
		/// <param name="other">
		///    A <see cref="object" /> to compare to the current
		///    instance.
		/// </param>
		/// <returns>
		///    A <see cref="bool" /> value indicating whether or not the
		///    current instance is equal to <paramref name="other" />.
		/// </returns>
		/// <seealso cref="M:System.IEquatable`1.Equals" />
		public override bool Equals (object other)
		{
			if (!(other is PageHeader))
				return false;

			return Equals ((PageHeader)other);
		}

		/// <summary>
		///    Checks whether or not the current instance is equal to
		///    another instance of <see cref="PageHeader" />.
		/// </summary>
		/// <param name="other">
		///    A <see cref="PageHeader" /> object to compare to the
		///    current instance.
		/// </param>
		/// <returns>
		///    A <see cref="bool" /> value indicating whether or not the
		///    current instance is equal to <paramref name="other" />.
		/// </returns>
		/// <seealso cref="M:System.IEquatable`1.Equals" />
		public bool Equals (PageHeader other)
		{
			return packet_sizes == other.packet_sizes &&
				version == other.version &&
				Flags == other.Flags &&
				absolute_granular_position ==
					other.absolute_granular_position &&
				StreamSerialNumber ==
					other.StreamSerialNumber &&
				PageSequenceNumber ==
					other.PageSequenceNumber &&
				Size == other.Size &&
				DataSize == other.DataSize;
		}

		/// <summary>
		///    Gets whether or not two instances of <see
		///    cref="PageHeader" /> are equal to eachother.
		/// </summary>
		/// <param name="first">
		///    A <see cref="PageHeader" /> object to compare.
		/// </param>
		/// <param name="second">
		///    A <see cref="PageHeader" /> object to compare.
		/// </param>
		/// <returns>
		///    <see langword="true" /> if <paramref name="first" /> is
		///    equal to <paramref name="second" />. Otherwise, <see
		///    langword="false" />.
		/// </returns>
		public static bool operator == (PageHeader first, PageHeader second)
		{
			return first.Equals (second);
		}

		/// <summary>
		///    Gets whether or not two instances of <see
		///    cref="PageHeader" /> differ.
		/// </summary>
		/// <param name="first">
		///    A <see cref="PageHeader" /> object to compare.
		/// </param>
		/// <param name="second">
		///    A <see cref="PageHeader" /> object to compare.
		/// </param>
		/// <returns>
		///    <see langword="true" /> if <paramref name="first" /> is
		///    unequal to <paramref name="second" />. Otherwise, <see
		///    langword="false" />.
		/// </returns>
		public static bool operator != (PageHeader first, PageHeader second)
		{
			return !first.Equals (second);
		}

		#endregion
	}
}
