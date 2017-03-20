//
// File.cs: Provides tagging and properties support for Apple's AIFF 
// files.
//
// Author:
//   Helmut Wahrmann
//
// Copyright (C) 2009 Helmut Wahrmann
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
using TagLib.Id3v2;

namespace TagLib.Aiff
{
	/// <summary>
	///    This class extends <see cref="TagLib.File" /> to provide
	///    support for reading and writing tags and properties for files
	///    using the AIFF file format.
	/// </summary>
	[SupportedMimeType("taglib/aif", "aif")]
	[SupportedMimeType("taglib/aiff", "aiff")]
	[SupportedMimeType("audio/x-aiff")]
	[SupportedMimeType("audio/aiff")]
	[SupportedMimeType("sound/aiff")]
	[SupportedMimeType("application/x-aiff")]
	public class File : TagLib.File
	{
		#region Private Fields

		/// <summary>
		///    Contains the address of the AIFF header block.
		/// </summary>
		private ByteVector header_block = null;

		/// <summary>
		///  Contains the Id3v2 tag.
		/// </summary>
		private Id3v2.Tag tag = null;

		/// <summary>
		///  Contains the media properties.
		/// </summary>
		private Properties properties = null;

		#endregion

		#region Public Static Fields

		/// <summary>
		///    The identifier used to recognize a AIFF files.
		/// </summary>
		/// <value>
		///    "FORM"
		/// </value>
		public static readonly ReadOnlyByteVector FileIdentifier = "FORM";

		/// <summary>
		///    The identifier used to recognize a AIFF Common chunk.
		/// </summary>
		/// <value>
		///    "COMM"
		/// </value>
		public static readonly ReadOnlyByteVector CommIdentifier = "COMM";

		/// <summary>
		///    The identifier used to recognize a AIFF Sound Data Chunk.
		/// </summary>
		/// <value>
		///    "SSND"
		/// </value>
		public static readonly ReadOnlyByteVector SoundIdentifier = "SSND";

		/// <summary>
		///    The identifier used to recognize a AIFF ID3 chunk.
		/// </summary>
		/// <value>
		///    "ID3 "
		/// </value>
		public static readonly ReadOnlyByteVector ID3Identifier = "ID3 ";

        /// <summary>
        ///    The identifier used to confirm that data within an AIFF ID3 chunk is really an ID3 tag.
        /// </summary>
        public static readonly ReadOnlyByteVector ID3TagHeader = new ReadOnlyByteVector(new byte[] {0x49, 0x44, 0x33});
		#endregion

		#region Public Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="File" /> for a specified path in the local file
		///    system and specified read style.
		/// </summary>
		/// <param name="path">
		///    A <see cref="string" /> object containing the path of the
		///    file to use in the new instance.
		/// </param>
		/// <param name="propertiesStyle">
		///    A <see cref="ReadStyle" /> value specifying at what level
		///    of accuracy to read the media properties, or <see
		///    cref="ReadStyle.None" /> to ignore the properties.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		public File(string path, ReadStyle propertiesStyle)
			: this(new File.LocalFileAbstraction(path),
			       propertiesStyle)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="File" /> for a specified path in the local file
		///    system with an average read style.
		/// </summary>
		/// <param name="path">
		///    A <see cref="string" /> object containing the path of the
		///    file to use in the new instance.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		public File(string path)
			: this(path, ReadStyle.Average)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="File" /> for a specified file abstraction and
		///    specified read style.
		/// </summary>
		/// <param name="abstraction">
		///    A <see cref="IFileAbstraction" /> object to use when
		///    reading from and writing to the file.
		/// </param>
		/// <param name="propertiesStyle">
		///    A <see cref="ReadStyle" /> value specifying at what level
		///    of accuracy to read the media properties, or <see
		///    cref="ReadStyle.None" /> to ignore the properties.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="abstraction" /> is <see langword="null"
		///    />.
		/// </exception>
		public File(File.IFileAbstraction abstraction,
		            ReadStyle propertiesStyle)
			: base(abstraction)
		{
			Mode = AccessMode.Read;
			try
			{
				uint aiff_size;
				long tag_start, tag_end;
				Read(true, propertiesStyle, out aiff_size,
				     out tag_start, out tag_end);
			}
			finally
			{
				Mode = AccessMode.Closed;
			}

			TagTypesOnDisk = TagTypes;

			GetTag(TagTypes.Id3v2, true);
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="File" /> for a specified file abstraction with an
		///    average read style.
		/// </summary>
		/// <param name="abstraction">
		///    A <see cref="IFileAbstraction" /> object to use when
		///    reading from and writing to the file.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="abstraction" /> is <see langword="null"
		///    />.
		/// </exception>
		public File(File.IFileAbstraction abstraction)
			: this(abstraction, ReadStyle.Average)
		{
		}

		#endregion

		#region Public Properties

		/// <summary>
		///    Gets a abstract representation of all tags stored in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="TagLib.Tag" /> object representing all tags
		///    stored in the current instance.
		/// </value>
		public override Tag Tag
		{
			get { return tag; }
		}

		/// <summary>
		///    Gets the media properties of the file represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="TagLib.Properties" /> object containing the
		///    media properties of the file represented by the current
		///    instance.
		/// </value>
		public override TagLib.Properties Properties
		{
			get { return properties; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		///    Saves the changes made in the current instance to the
		///    file it represents.
		/// </summary>
		public override void Save()
		{
			Mode = AccessMode.Write;
			try
			{
				ByteVector data = new ByteVector();

				// Add the ID3 chunk and ID32 tag to the vector
				if (tag != null)
				{
					ByteVector tag_data = tag.Render();
					if (tag_data.Count > 10)
					{
						if (tag_data.Count%2 == 1)
							tag_data.Add(0);

						data.Add("ID3 ");
						data.Add(ByteVector.FromUInt(
						         	(uint) tag_data.Count,
						         	true));
						data.Add(tag_data);
					}
				}

				// Read the file to determine the current AIFF
				// size and the area tagging is in.
				uint aiff_size;
				long tag_start, tag_end;
				Read(false, ReadStyle.None, out aiff_size,
				     out tag_start, out tag_end);

				// If tagging info cannot be found, place it at
				// the end of the file.
				if (tag_start < 12 || tag_end < tag_start)
					tag_start = tag_end = Length;

				int length = (int) (tag_end - tag_start + 8);

				// Insert the tagging data.
				Insert(data, tag_start, length);

				// If the data size changed update the aiff size.
				if (data.Count - length != 0 &&
				    tag_start <= aiff_size)
				{
					// Depending, if a Tag has been added or removed, 
					// the length needs to be adjusted
					if (tag == null)
					{
						length -= 16;
					}
					else
					{
						length -= 8;
					}

					Insert(ByteVector.FromUInt((uint)
					                           (aiff_size + data.Count - length),
					                           true), 4, 4);
				}
				// Update the tag types.
				TagTypesOnDisk = TagTypes;
			}
			finally
			{
				Mode = AccessMode.Closed;
			}
		}

		/// <summary>
		///    Removes a set of tag types from the current instance.
		/// </summary>
		/// <param name="types">
		///    A bitwise combined <see cref="TagLib.TagTypes" /> value
		///    containing tag types to be removed from the file.
		/// </param>
		/// <remarks>
		///    In order to remove all tags from a file, pass <see
		///    cref="TagTypes.AllTags" /> as <paramref name="types" />.
		/// </remarks>
		public override void RemoveTags(TagTypes types)
		{
			if (types == TagLib.TagTypes.Id3v2 ||
			    types == TagLib.TagTypes.AllTags)
			{
				tag = null;
			}
		}

		/// <summary>
		///    Gets a tag of a specified type from the current instance,
		///    optionally creating a new tag if possible.
		/// </summary>
		/// <param name="type">
		///    A <see cref="TagLib.TagTypes" /> value indicating the
		///    type of tag to read.
		/// </param>
		/// <param name="create">
		///    A <see cref="bool" /> value specifying whether or not to
		///    try and create the tag if one is not found.
		/// </param>
		/// <returns>
		///    A <see cref="Tag" /> object containing the tag that was
		///    found in or added to the current instance. If no
		///    matching tag was found and none was created, <see
		///    langword="null" /> is returned.
		/// </returns>
		public override TagLib.Tag GetTag(TagTypes type, bool create)
		{
			TagLib.Tag id32_tag = null;

			switch (type)
			{
				case TagTypes.Id3v2:
					if (tag == null && create)
					{
						tag = new Id3v2.Tag();
						tag.Version = 2;
					}

					id32_tag = tag;
					break;
			}

			return id32_tag;
		}

		#endregion

		#region Private Methods

		/// <summary>
		///    Reads the contents of the current instance determining
		///    the size of the riff data, the area the tagging is in,
		///    and optionally reading in the tags and media properties.
		/// </summary>
		/// <param name="read_tags">
		///    If <see langword="true" />, any tags found will be read
		///    into the current instance.
		/// </param>
		/// <param name="style">
		///    A <see cref="ReadStyle"/> value specifying how the media
		///    data is to be read into the current instance.
		/// </param>
		/// <param name="aiff_size">
		///    A <see cref="uint"/> value reference to be filled with
		///    the size of the RIFF data as read from the file.
		/// </param>
		/// <param name="tag_start">
		///    A <see cref="long" /> value reference to be filled with
		///    the absolute seek position at which the tagging data
		///    starts.
		/// </param>
		/// <param name="tag_end">
		///    A <see cref="long" /> value reference to be filled with
		///    the absolute seek position at which the tagging data
		///    ends.
		/// </param>
		/// <exception cref="CorruptFileException">
		///    The file does not begin with <see cref="FileIdentifier"
		///    />.
		/// </exception>
		private void Read(bool read_tags, ReadStyle style,
		                  out uint aiff_size, out long tag_start,
		                  out long tag_end)
		{
			Seek(0);
			if (ReadBlock(4) != FileIdentifier)
				throw new CorruptFileException(
					"File does not begin with AIFF identifier");

			aiff_size = ReadBlock(4).ToUInt(true);
			tag_start = -1;
			tag_end = -1;

			// Get the properties of the file
			if (header_block == null &&
			    style != ReadStyle.None)
			{
				long common_chunk_pos = Find(CommIdentifier, 0);

				if (common_chunk_pos == -1)
				{
					throw new CorruptFileException(
						"No Common chunk available in AIFF file.");
				}

				Seek(common_chunk_pos);
				header_block = ReadBlock((int) StreamHeader.Size);

				StreamHeader header = new StreamHeader(header_block, aiff_size);
				properties = new Properties(TimeSpan.Zero, header);
			}

			// Now we search for the ID3 chunk.
			// Normally it appears after the Sound data chunk. But as the order of
			// chunks is free, it might be the case that the ID3 chunk appears before 
			// the sound data chunk.
			// So we search first for the Sound data chunk and see, if an ID3 chunk appears before
			long id3_chunk_pos = -1;
			long sound_chunk_pos = Find(SoundIdentifier, 0, ID3Identifier);
			if (sound_chunk_pos == -1)
			{
				// The ID3 chunk appears before the Sound chunk
                                id3_chunk_pos = FindId3Chunk(0);
			}

			// Now let's look for the Sound chunk again
			// Since a previous return value of -1 does mean, that the ID3 chunk was found first
			sound_chunk_pos = Find(SoundIdentifier, 0);
			if (sound_chunk_pos == -1)
			{
				throw new CorruptFileException(
					"No Sound chunk available in AIFF file.");
			}

			// Get the length of the Sound chunk and use this as a start value to look for the ID3 chunk
			Seek(sound_chunk_pos + 4);
			ulong sound_chunk_length = ReadBlock(4).ToULong(true);
			long start_search_pos = (long) sound_chunk_length + sound_chunk_pos + 4;

			if (id3_chunk_pos == -1)
			{
				id3_chunk_pos = FindId3Chunk(start_search_pos);
			}

			if (id3_chunk_pos > -1)
			{
				if (read_tags && tag == null)
				{
					tag = new Id3v2.Tag(this,
					                    id3_chunk_pos + 8);
				}

				// Get the length of the tag out of the ID3 chunk
				Seek(id3_chunk_pos + 4);
				uint tag_size = ReadBlock(4).ToUInt(true) + 8;

				tag_start = InvariantStartPosition = id3_chunk_pos;
				tag_end = InvariantEndPosition = tag_start + tag_size;
			}
		}

		#endregion

	    private long FindId3Chunk(long searchStartPos)
	    {
	        var searchPosition = searchStartPos;

	        while (true)
	        {
                var chunkHeaderPosition = Find(ID3Identifier, searchPosition);
	            if (chunkHeaderPosition < 0)
	            {
	                return chunkHeaderPosition;
	            }

                // a true ID3 block will have the ID3 header repeated 8 bytes after the chunk header
                var id3HeaderPosition = Find(ID3TagHeader, chunkHeaderPosition + 8);
	            if (id3HeaderPosition < 0)
	            {
	                return -1;
	            }

	            if (id3HeaderPosition == chunkHeaderPosition + 8)
	            {
                    return chunkHeaderPosition;
	            }

                // continue the search starting with the last position that matched
                searchPosition = chunkHeaderPosition + 4;

	            // if no true ID3 chunk is found, Find will return -1, and the loop will exit
	        }
	    }
	}


}
