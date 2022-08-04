//
// Tag.cs: This abstract class provides generic access to standard tag
// features. All tag types will extend this class.
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   tag.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
// Copyright (C) 2003 Scott Wheeler
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

namespace TagLib
{
	/// <summary>
	///    Indicates the tag types used by a file.
	/// </summary>
	[Flags]
	public enum TagTypes : uint
	{
		/// <summary>
		///    No tag types.
		/// </summary>
		None = 0x00000000,

		/// <summary>
		///    Xiph's Vorbis Comment
		/// </summary>
		Xiph = 0x00000001,

		/// <summary>
		///    ID3v1 Tag
		/// </summary>
		Id3v1 = 0x00000002,

		/// <summary>
		///    ID3v2 Tag
		/// </summary>
		Id3v2 = 0x00000004,

		/// <summary>
		///    APE Tag
		/// </summary>
		Ape = 0x00000008,

		/// <summary>
		///    Apple's ILST Tag Format
		/// </summary>
		Apple = 0x00000010,

		/// <summary>
		///    ASF Tag
		/// </summary>
		Asf = 0x00000020,

		/// <summary>
		///    Standard RIFF INFO List Tag
		/// </summary>
		RiffInfo = 0x00000040,

		/// <summary>
		///    RIFF Movie ID List Tag
		/// </summary>
		MovieId = 0x00000080,

		/// <summary>
		///    DivX Tag
		/// </summary>
		DivX = 0x00000100,

		/// <summary>
		///    FLAC Metadata Blocks Tag
		/// </summary>
		FlacMetadata = 0x00000200,

		/// <summary>
		///    TIFF IFD Tag
		/// </summary>
		TiffIFD = 0x00000400,

		/// <summary>
		///    XMP Tag
		/// </summary>
		XMP = 0x00000800,

		/// <summary>
		///    Jpeg Comment Tag
		/// </summary>
		JpegComment = 0x00001000,

		/// <summary>
		///    Gif Comment Tag
		/// </summary>
		GifComment = 0x00002000,

		/// <summary>
		///    native PNG keywords
		/// </summary>
		Png = 0x00004000,

		/// <summary>
		/// IPTC-IIM tag
		/// </summary>
		IPTCIIM = 0x00008000,

		/// <summary>
		///    Audible Metadata Blocks Tag
		/// </summary>
		AudibleMetadata = 0x00010000,

		/// <summary>
		/// Matroska native tag
		/// </summary>
		Matroska = 0x00020000,

		/// <summary>
		///    All tag types.
		/// </summary>
		AllTags = 0xFFFFFFFF
	}

	/// <summary>
	///    This abstract class provides generic access to standard tag
	///    features. All tag types will extend this class.
	/// </summary>
	/// <remarks>
	///    Because not every tag type supports the same features, it may be
	///    useful to check that the value is stored by re-reading the
	///    property after it is stored.
	/// </remarks>
	public class Tag : ITag
	{

		/// <summary>
		///    Gets whether or not the current instance is empty.
		/// </summary>
		/// <value>
		///    <see langword="true" /> if the current instance does not
		///    any values. Otherwise <see langword="false" />.
		/// </value>
		/// <remarks>
		///    In the default implementation, this checks the values
		///    supported by <see cref="Tag" />, but it may be extended
		///    by child classes to support other values.
		/// </remarks>
		public override bool IsEmpty {
			get {
				return IsNullOrLikeEmpty (Title) &&
				IsNullOrLikeEmpty (Grouping) &&
				IsNullOrLikeEmpty (AlbumArtists) &&
				IsNullOrLikeEmpty (Performers) &&
				IsNullOrLikeEmpty (Composers) &&
				IsNullOrLikeEmpty (Conductor) &&
				IsNullOrLikeEmpty (Copyright) &&
				IsNullOrLikeEmpty (Album) &&
				IsNullOrLikeEmpty (Comment) &&
				IsNullOrLikeEmpty (Genres) &&
				Year == 0 &&
				BeatsPerMinute == 0 &&
				Track == 0 &&
				TrackCount == 0 &&
				Disc == 0 &&
				DiscCount == 0;
			}
		}

		/// <summary>
		///    Set the Tags that represent the Tagger software 
		///    (TagLib#) itself.
		/// </summary>
		/// <remarks>
		///    This is typically a method to call just before 
		///    saving a tag.
		/// </remarks>
		public void SetInfoTag () { DateTagged = DateTime.Now; }

		/// <summary>
		///    Copies all standard values from one tag to another,
		///    optionally overwriting existing values.
		/// </summary>
		/// <param name="source">
		///    A <see cref="Tag" /> object containing the source tag to
		///    copy the values from.
		/// </param>
		/// <param name="target">
		///    A <see cref="Tag" /> object containing the target tag to
		///    copy values to.
		/// </param>
		/// <param name="overwrite">
		///    A <see cref="bool" /> specifying whether or not to copy
		///    values over existing one.
		/// </param>
		/// <remarks>
		///    <para>This method only copies the most basic values,
		///    those contained in this class, between tags. To copy
		///    format specific tags, or additional details, additional
		///    implementations need to be applied. For example, copying
		///    from one <see cref="TagLib.Id3v2.Tag" /> to another:
		///    <c>foreach (TagLib.Id3v2.Frame frame in old_tag)
		///    new_tag.AddFrame (frame);</c></para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="source" /> or <paramref name="target" />
		///    is <see langword="null" />.
		/// </exception>
		[Obsolete ("Use Tag.CopyTo(Tag,bool)")]
		public static void Duplicate (Tag source, Tag target, bool overwrite)
		{
			if (source == null)
				throw new ArgumentNullException (nameof (source));

			if (target == null)
				throw new ArgumentNullException (nameof (target));

			source.CopyTo (target, overwrite);
		}

		/// <summary>
		///    Copies the values from the current instance to another
		///    <see cref="TagLib.Tag" />, optionally overwriting
		///    existing values.
		/// </summary>
		/// <param name="target">
		///    A <see cref="Tag" /> object containing the target tag to
		///    copy values to.
		/// </param>
		/// <param name="overwrite">
		///    A <see cref="bool" /> specifying whether or not to copy
		///    values over existing one.
		/// </param>
		/// <remarks>
		///    <para>This method only copies the most basic values when
		///    copying between different tag formats, however, if
		///    <paramref name="target" /> is of the same type as the
		///    current instance, more advanced copying may be done.
		///    For example, <see cref="TagLib.Id3v2.Tag" /> will copy
		///    all of its frames to another tag.</para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="target" /> is <see langword="null" />.
		/// </exception>
		public override void CopyTo (Tag target, bool overwrite)
		{
			if (target == null)
				throw new ArgumentNullException (nameof (target));

			if (overwrite || IsNullOrLikeEmpty (target.Title))
				target.Title = Title;

			if (overwrite || IsNullOrLikeEmpty (target.Subtitle))
				target.Subtitle = Subtitle;

			if (overwrite || IsNullOrLikeEmpty (target.Description))
				target.Description = Description;

			if (overwrite || IsNullOrLikeEmpty (target.AlbumArtists))
				target.AlbumArtists = AlbumArtists;

			if (overwrite || IsNullOrLikeEmpty (target.Performers))
				target.Performers = Performers;

			if (overwrite || IsNullOrLikeEmpty (target.PerformersRole))
				target.PerformersRole = PerformersRole;

			if (overwrite || IsNullOrLikeEmpty (target.Composers))
				target.Composers = Composers;

			if (overwrite || IsNullOrLikeEmpty (target.Album))
				target.Album = Album;

			if (overwrite || IsNullOrLikeEmpty (target.Comment))
				target.Comment = Comment;

			if (overwrite || IsNullOrLikeEmpty (target.Genres))
				target.Genres = Genres;

			if (overwrite || target.Year == 0)
				target.Year = Year;

			if (overwrite || target.Track == 0)
				target.Track = Track;

			if (overwrite || target.TrackCount == 0)
				target.TrackCount = TrackCount;

			if (overwrite || target.Disc == 0)
				target.Disc = Disc;

			if (overwrite || target.DiscCount == 0)
				target.DiscCount = DiscCount;

			if (overwrite || target.BeatsPerMinute == 0)
				target.BeatsPerMinute = BeatsPerMinute;

			if (overwrite || IsNullOrLikeEmpty (target.InitialKey))
				target.InitialKey = InitialKey;

			if (overwrite || IsNullOrLikeEmpty (target.Publisher))
				target.Publisher = Publisher;

			if (overwrite || IsNullOrLikeEmpty (target.ISRC))
				target.ISRC = ISRC;

			if (overwrite || IsNullOrLikeEmpty (target.RemixedBy))
				target.RemixedBy = RemixedBy;

			if (overwrite || IsNullOrLikeEmpty (target.Grouping))
				target.Grouping = Grouping;

			if (overwrite || IsNullOrLikeEmpty (target.Conductor))
				target.Conductor = Conductor;

			if (overwrite || IsNullOrLikeEmpty (target.Copyright))
				target.Copyright = Copyright;

			if (overwrite || target.DateTagged == null)
				target.DateTagged = DateTagged;

			if (overwrite || target.Pictures == null || target.Pictures.Length == 0)
				target.Pictures = Pictures;
		}

		/// <summary>
		///    Checks if a <see cref="string" /> is <see langword="null"
		///    /> or contains only whitespace characters.
		/// </summary>
		/// <param name="value">
		///    A <see cref="string" /> object to check.
		/// </param>
		/// <returns>
		///    <see langword="true" /> if the string is <see
		///    langword="null" /> or contains only whitespace
		///    characters. Otherwise <see langword="false" />.
		/// </returns>
		static bool IsNullOrLikeEmpty (string value)
		{
			return value == null || value.Trim ().Length == 0;
		}

		/// <summary>
		///    Checks if all the strings in the array return <see
		///    langword="true" /> with <see
		///    cref="IsNullOrLikeEmpty(string)" /> or if the array is
		///    <see langword="null" /> or is empty.
		/// </summary>
		/// <param name="value">
		///    A <see cref="T:string[]" /> to check the contents of.
		/// </param>
		/// <returns>
		///    <see langword="true" /> if the array is <see
		///    langword="null" /> or empty, or all elements return <see
		///    langword="true" /> for <see
		///    cref="IsNullOrLikeEmpty(string)" />. Otherwise <see
		///    langword="false" />.
		/// </returns>
		static bool IsNullOrLikeEmpty (string[] value)
		{
			if (value == null)
				return true;

			foreach (string s in value)
				if (!IsNullOrLikeEmpty (s))
					return false;

			return true;
		}
	}
}
