//
// AviHeaderList.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
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

namespace TagLib.Riff
{
	/// <summary>
	///    This class provides support for reading an AVI header list to
	///    extract stream information.
	/// </summary>
	public class AviHeaderList
	{

		/// <summary>
		///    Contains the AVI codec information.
		/// </summary>
		readonly List<ICodec> codecs = new List<ICodec> ();

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AviHeaderList" /> by reading the contents of a raw
		///    RIFF list from a specified position in a <see
		///    cref="TagLib.File"/>.
		/// </summary>
		/// <param name="file">
		///    A <see cref="TagLib.File" /> object containing the file
		///    from which the contents of the new instance is to be
		///    read.
		/// </param>
		/// <param name="position">
		///    A <see cref="long" /> value specify at what position to
		///    read the list.
		/// </param>
		/// <param name="length">
		///    A <see cref="int" /> value specifying the number of bytes
		///    to read.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="file" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///    <paramref name="position" /> is less than zero or greater
		///    than the size of the file.
		/// </exception>
		/// <exception cref="CorruptFileException">
		///    The list does not contain an AVI header or the AVI header
		///    is the wrong length.
		/// </exception>
		public AviHeaderList (TagLib.File file, long position, int length)
		{
			if (file == null)
				throw new ArgumentNullException (nameof (file));

			if (length < 0)
				throw new ArgumentOutOfRangeException (nameof (length));

			if (position < 0 || position > file.Length - length)
				throw new ArgumentOutOfRangeException (nameof (position));

			List list = new List (file, position, length);

			if (!list.ContainsKey ("avih"))
				throw new CorruptFileException ("Avi header not found.");

			ByteVector header_data = list["avih"][0];
			if (header_data.Count != 0x38)
				throw new CorruptFileException ("Invalid header length.");

			Header = new AviHeader (header_data, 0);

			foreach (ByteVector list_data in list["LIST"]) {
				if (list_data.StartsWith ("strl")) {
					AviStream stream = AviStream.ParseStreamList (list_data);
					if (stream != null)
						codecs.Add (stream.Codec);
				}
			}
		}

		/// <summary>
		///    Gets the header for the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="AviHeader" /> object containing the header
		///    for the current instance.
		/// </value>
		public AviHeader Header { get; private set; }

		/// <summary>
		///    Gets the codecs contained in the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="T:ICodec[]" /> containing the codecs contained
		///    in the current instance.
		/// </value>
		public ICodec[] Codecs {
			get { return codecs.ToArray (); }
		}
	}

	/// <summary>
	///    This structure provides a representation of a Microsoft
	///    AviMainHeader structure, minus the first 8 bytes.
	/// </summary>
	public struct AviHeader
	{
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AviHeader" /> by reading the raw structure from the
		///    beginning of a <see cref="ByteVector" /> object.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object containing the raw
		///    data structure.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="data" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="CorruptFileException">
		///    <paramref name="data" /> contains less than 40 bytes.
		/// </exception>
		[Obsolete ("Use AviHeader(ByteVector,int)")]
		public AviHeader (ByteVector data) : this (data, 0)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AviStreamHeader" /> by reading the raw structure
		///    from a specified position in a <see cref="ByteVector" />
		///    object.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object containing the raw
		///    data structure.
		/// </param>
		/// <param name="offset">
		///    A <see cref="int" /> value specifying the index in
		///    <paramref name="data"/> at which the structure begins.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="data" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///    <paramref name="offset" /> is less than zero.
		/// </exception>
		/// <exception cref="CorruptFileException">
		///    <paramref name="data" /> contains less than 40 bytes at
		///    <paramref name="offset" />.
		/// </exception>
		public AviHeader (ByteVector data, int offset)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));

			if (offset < 0)
				throw new ArgumentOutOfRangeException (nameof (offset));

			if (offset + 40 > data.Count)
				throw new CorruptFileException ("Expected 40 bytes.");

			MicrosecondsPerFrame = data.Mid (offset, 4).ToUInt (false);
			MaxBytesPerSecond = data.Mid (offset + 4, 4).ToUInt (false);
			Flags = data.Mid (offset + 12, 4).ToUInt (false);
			TotalFrames = data.Mid (offset + 16, 4).ToUInt (false);
			InitialFrames = data.Mid (offset + 20, 4).ToUInt (false);
			Streams = data.Mid (offset + 24, 4).ToUInt (false);
			SuggestedBufferSize = data.Mid (offset + 28, 4).ToUInt (false);
			Width = data.Mid (offset + 32, 4).ToUInt (false);
			Height = data.Mid (offset + 36, 4).ToUInt (false);
		}

		/// <summary>
		///    Gets the number of microseconds per frame.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value specifying number of
		///    microseconds per frame.
		/// </value>
		public uint MicrosecondsPerFrame { get; private set; }

		/// <summary>
		///    Gets the maximum number of bytes per second.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value specifying maximum number of
		///    bytes per second.
		/// </value>
		public uint MaxBytesPerSecond { get; private set; }

		/// <summary>
		///    Gets the file flags.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value specifying file flags.
		/// </value>
		public uint Flags { get; private set; }

		/// <summary>
		///    Gets the number of frames in the file.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value specifying the number of
		///    frames in the file.
		/// </value>
		public uint TotalFrames { get; private set; }

		/// <summary>
		///    Gets how far ahead audio is from video.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value specifying how far ahead
		///    audio is from video.
		/// </value>
		public uint InitialFrames { get; private set; }

		/// <summary>
		///    Gets the number of streams in the file.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value specifying the number of
		///    streams in the file.
		/// </value>
		public uint Streams { get; private set; }

		/// <summary>
		///    Gets the suggested buffer size for the file.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value specifying the buffer size.
		/// </value>
		public uint SuggestedBufferSize { get; private set; }

		/// <summary>
		///    Gets the width of the video in the file.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the width of the
		///    video.
		/// </value>
		public uint Width { get; private set; }

		/// <summary>
		///    Gets the height of the video in the file.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the height of the
		///    video.
		/// </value>
		public uint Height { get; private set; }

		/// <summary>
		///    Gets the duration of the media in the file.
		/// </summary>
		/// <value>
		///    A <see cref="TimeSpan" /> value containing the duration
		///    of the file.
		/// </value>
		public TimeSpan Duration {
			get {
				return TimeSpan.FromMilliseconds (TotalFrames * (double)MicrosecondsPerFrame / 1000.0);
			}
		}
	}
}
