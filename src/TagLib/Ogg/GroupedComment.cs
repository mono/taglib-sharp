//
// GroupedComment.cs:
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

using System.Collections.Generic;

namespace TagLib.Ogg
{
	/// <summary>
	///    This class combines a collection of <see cref="XiphComment"/>
	///    objects so that properties can be read from each but are only set
	///    to the first comment of the file.
	/// </summary>
	public class GroupedComment : Tag
	{
#region Private Fields
		
		/// <summary>
		///    Contains a mapping between stream serial numbers and
		///    comments.
		/// </summary>
		private Dictionary<uint, XiphComment> comment_hash;
		
		/// <summary>
		///    Contains comments in the order they are added.
		/// </summary>
		private List<XiphComment> tags;
		
#endregion
		
		
		
#region Constructors
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="GroupedComment" /> with now contents.
		/// </summary>
		public GroupedComment () : base ()
		{
			comment_hash = new Dictionary <uint, XiphComment> ();
			tags = new List<XiphComment> ();
		}
		
		/// <summary>
		///    Gets an enumeration of the comments in the current
		///    instance, in the order they were added.
		/// </summary>
		/// <value>
		///    A <see cref="T:System.Collections.Generic.IEnumerable`1"
		///    /> object enumerating through the <see cref="XiphComment"
		///    /> objects contained in the current instance.
		/// </value>
		public IEnumerable<XiphComment> Comments {
			get {return tags;}
		}
		
		/// <summary>
		///    Gets a comment in the current instance for a specified
		///    stream.
		/// </summary>
		/// <param name="streamSerialNumber">
		///    A <see cref="uint" /> value containing the serial number
		///    of the stream of the comment to get.
		/// </param>
		/// <returns>
		///    A <see cref="XiphComment"/> with the matching serial
		///    number.
		/// </returns>
		public XiphComment GetComment (uint streamSerialNumber)
		{
			return comment_hash [streamSerialNumber];
		}
		
		/// <summary>
		///    Adds a Xiph comment to the current instance.
		/// </summary>
		/// <param name="streamSerialNumber">
		///    A <see cref="uint" /> value containing the serial number
		///    of the stream containing the comment.
		/// </param>
		/// <param name="comment">
		///    A <see cref="XiphComment" /> object to add to the current
		///    instance.
		/// </param>
		public void AddComment (uint streamSerialNumber,
		                        XiphComment comment)
		{
			comment_hash.Add (streamSerialNumber, comment);
			tags.Add (comment);
		}
		
		/// <summary>
		///    Adds a Xiph comment to the current instance.
		/// </summary>
		/// <param name="streamSerialNumber">
		///    A <see cref="uint" /> value containing the serial number
		///    of the stream containing the comment.
		/// </param>
		/// <param name="data">
		///    A <see cref="ByteVector"/> object containing the raw Xiph
		///    comment to add to the current instance.
		/// </param>
		public void AddComment (uint streamSerialNumber,
		                        ByteVector data)
		{
			AddComment (streamSerialNumber, new XiphComment (data));
		}
		
#endregion
		
		
		
#region TagLib.Tag
		
		/// <summary>
		///    Gets the tag types contained in the current instance.
		/// </summary>
		/// <value>
		///    A bitwise combined <see cref="TagLib.TagTypes" />
		///    containing the tag types contained in the current
		///    instance.
		/// </value>
		/// <remarks>
		///    This value contains a bitwise combined value from all the
		///    child tags.
		/// </remarks>
		/// <seealso cref="Tag.TagTypes" />
		public override TagTypes TagTypes {
			get {
				TagTypes types = TagTypes.None;
				foreach (XiphComment tag in tags)
					if (tag != null)
						types |= tag.TagTypes;
				
				return types;
			}
		}

		/// <summary>
		///    Gets and sets the title for the media described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the title for
		///    the media described by the current instance or <see
		///    langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-<see
		///    langword="null" /> value is returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Title" />
		public override string Title {
			get {
				string output = null;
				foreach (XiphComment tag in tags)
					if (tag != null && output == null)
						output = tag.Title;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Title = value;}
		}
		
		/// <summary>
		///    Gets and sets the performers or artists who performed in
		///    the media described by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the performers or
		///    artists who performed in the media described by the
		///    current instance or an empty array if no value is
		///    present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-empty value is
		///    returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Performers" />
		public override string [] Performers {
			get {
				string [] output = new string [0];
				foreach (XiphComment tag in tags)
					if (tag != null && output.Length == 0)
						output = tag.Performers;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Performers = value;}
		}

		/// <summary>
		///    Gets and sets the band or artist who is credited in the
		///    creation of the entire album or collection containing the
		///    media described by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the band or artist
		///    who is credited in the creation of the entire album or
		///    collection containing the media described by the current
		///    instance or an empty array if no value is present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-empty value is
		///    returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.AlbumArtists" />
		public override string [] AlbumArtists {
			get {
				string [] output = new string [0];
				foreach (XiphComment tag in tags)
					if (tag != null && output.Length == 0)
						output = tag.AlbumArtists;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].AlbumArtists = value;}
		}

		/// <summary>
		///    Gets and sets the composers of the media represented by
		///    the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the composers of the
		///    media represented by the current instance or an empty
		///    array if no value is present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-empty value is
		///    returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Composers" />
		public override string [] Composers {
			get {
				string [] output = new string [0];
				foreach (XiphComment tag in tags)
					if (tag != null && output.Length == 0)
						output = tag.Composers;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Composers = value;}
		}
		
		/// <summary>
		///    Gets and sets the album of the media represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the album of
		///    the media represented by the current instance or <see
		///    langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-<see
		///    langword="null" /> value is returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Album" />
		public override string Album {
			get {
				string output = null;
				foreach (XiphComment tag in tags)
					if (tag != null && output == null)
						output = tag.Album;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Album = value;}
		}
		
		/// <summary>
		///    Gets and sets a user comment on the media represented by
		///    the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing user comments
		///    on the media represented by the current instance or <see
		///    langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-<see
		///    langword="null" /> value is returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Comment" />
		public override string Comment {
			get {
				string output = null;
				foreach (XiphComment tag in tags)
					if (tag != null && output == null)
						output = tag.Comment;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Comment = value;}
		}
		
		/// <summary>
		///    Gets and sets the genres of the media represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the genres of the
		///    media represented by the current instance or an empty
		///    array if no value is present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-empty value is
		///    returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Genres" />
		public override string [] Genres {
			get {
				string [] output = new string [0];
				foreach (XiphComment tag in tags)
					if (tag != null && output.Length == 0)
						output = tag.Genres;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Genres = value;}
		}
		
		/// <summary>
		///    Gets and sets the year that the media represented by the
		///    current instance was recorded.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the year that the media
		///    represented by the current instance was created or zero
		///    if no value is present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-zero value is
		///    returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Year" />
		public override uint Year {
			get {
				uint output = 0;
				foreach (XiphComment tag in tags)
					if (tag != null && output == 0)
						output = tag.Year;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Year = value;}
		}
		
		/// <summary>
		///    Gets and sets the position of the media represented by
		///    the current instance in its containing album.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the position of the
		///    media represented by the current instance in its
		///    containing album or zero if not specified.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-zero value is
		///    returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Track" />
		public override uint Track {
			get {
				uint output = 0;
				foreach (XiphComment tag in tags)
					if (tag != null && output == 0)
						output = tag.Track;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Track = value;}
		}
		
		/// <summary>
		///    Gets and sets the number of tracks in the album
		///    containing the media represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the number of tracks in
		///    the album containing the media represented by the current
		///    instance or zero if not specified.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-zero value is
		///    returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.TrackCount" />
		public override uint TrackCount {
			get {
				uint output = 0;
				foreach (XiphComment tag in tags)
					if (tag != null && output == 0)
						output = tag.TrackCount;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].TrackCount = value;}
		}
		
		/// <summary>
		///    Gets and sets the number of the disc containing the media
		///    represented by the current instance in the boxed set.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the number of the disc
		///    containing the media represented by the current instance
		///    in the boxed set.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-zero value is
		///    returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Disc" />
		public override uint Disc {
			get {
				uint output = 0;
				foreach (XiphComment tag in tags)
					if (tag != null && output == 0)
						output = tag.Disc;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Disc = value;}
		}
		
		/// <summary>
		///    Gets and sets the number of discs in the boxed set
		///    containing the media represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the number of discs in
		///    the boxed set containing the media represented by the
		///    current instance or zero if not specified.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-zero value is
		///    returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.DiscCount" />
		public override uint DiscCount {
			get {
				uint output = 0;
				foreach (XiphComment tag in tags)
					if (tag != null && output == 0)
						output = tag.DiscCount;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].DiscCount = value;}
		}
		
		/// <summary>
		///    Gets and sets the lyrics or script of the media
		///    represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the lyrics or
		///    script of the media represented by the current instance
		///    or <see langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-<see
		///    langword="null" /> value is returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Lyrics" />
		public override string Lyrics {
			get {
				string output = null;
				foreach (XiphComment tag in tags)
					if (tag != null && output == null)
						output = tag.Lyrics;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Lyrics = value;}
		}
		
		/// <summary>
		///    Gets and sets the grouping on the album which the media
		///    in the current instance belongs to.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the grouping on
		///    the album which the media in the current instance belongs
		///    to or <see langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-<see
		///    langword="null" /> value is returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Grouping" />
		public override string Grouping {
			get {
				string output = null;
				foreach (XiphComment tag in tags)
					if (tag != null && output == null)
						output = tag.Grouping;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Grouping = value;}
		}
		
		/// <summary>
		///    Gets and sets the number of beats per minute in the audio
		///    of the media represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the number of beats per
		///    minute in the audio of the media represented by the
		///    current instance, or zero if not specified.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-zero value is
		///    returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.BeatsPerMinute" />
		public override uint BeatsPerMinute {
			get {
				uint output = 0;
				foreach (XiphComment tag in tags)
					if (tag != null && output == 0)
						output = tag.BeatsPerMinute;
				
				return output;
			}
			set {
				if (tags.Count > 0)
					tags [0].BeatsPerMinute = value;
			}
		}

		/// <summary>
		///    Gets and sets the conductor or director of the media
		///    represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the conductor
		///    or director of the media represented by the current
		///    instance or <see langword="null" /> if no value present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-<see
		///    langword="null" /> value is returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Conductor" />
		public override string Conductor {
			get {
				string output = null;
				foreach (XiphComment tag in tags)
					if (tag != null && output == null)
						output = tag.Conductor;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Conductor = value;}
		}
		
		/// <summary>
		///    Gets and sets the copyright information for the media
		///    represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the copyright
		///    information for the media represented by the current
		///    instance or <see langword="null" /> if no value present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-<see
		///    langword="null" /> value is returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Copyright" />
		public override string Copyright {
			get {
				string output = null;
				foreach (XiphComment tag in tags)
					if (tag != null && output == null)
						output = tag.Copyright;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Copyright = value;}
		}
		
		/// <summary>
		///    Gets and sets a collection of pictures associated with
		///    the media represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="IPicture[]" /> containing a collection of
		///    pictures associated with the media represented by the
		///    current instance or an empty array if none are present.
		/// </value>
		/// <remarks>
		///    <para>When getting the value, the child comments are
		///    looped through in order and the first non-empty value is
		///    returned.</para>
		///    <para>When setting the value, it is stored in the first
		///    comment.</para>
		/// </remarks>
		/// <seealso cref="Tag.Pictures" />
		public override IPicture [] Pictures {
			get {
				IPicture [] output = new IPicture [0];
				foreach (XiphComment tag in tags)
					if (tag != null && output.Length == 0)
						output = tag.Pictures;
				
				return output;
			}
			set {if (tags.Count > 0) tags [0].Pictures = value;}
		}
		
		/// <summary>
		///    Gets whether or not the current instance is empty.
		/// </summary>
		/// <value>
		///    <see langword="true" /> if all the comments tags are
		///     empty; otherwise <see langword="false" />.
		/// </value>
		/// <seealso cref="Tag.IsEmpty" />
		public override bool IsEmpty {
			get {
				foreach (XiphComment tag in tags)
					if (!tag.IsEmpty)
						return false;
				
				return true;
			}
		}
		
		/// <summary>
		///    Clears all of the child tags.
		/// </summary>
		public override void Clear ()
		{
			foreach (XiphComment tag in tags)
				tag.Clear ();
		}
		
#endregion
	}
}
