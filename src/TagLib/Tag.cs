/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : tag.cpp from TagLib
 ***************************************************************************/

/***************************************************************************
 *   This library is free software; you can redistribute it and/or modify  *
 *   it  under the terms of the GNU Lesser General Public License version  *
 *   2.1 as published by the Free Software Foundation.                     *
 *                                                                         *
 *   This library is distributed in the hope that it will be useful, but   *
 *   WITHOUT ANY WARRANTY; without even the implied warranty of            *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU     *
 *   Lesser General Public License for more details.                       *
 *                                                                         *
 *   You should have received a copy of the GNU Lesser General Public      *
 *   License along with this library; if not, write to the Free Software   *
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  *
 *   USA                                                                   *
 ***************************************************************************/

using System;

namespace TagLib {
	
	/// <summary>
	///    Indicates the tag types used by a file.
	/// </summary>
	[Flags]
	public enum TagTypes : uint
	{
		/// <summary>
		///    No tag types.
		/// </summary>
		None         = 0x00000000,
		
		/// <summary>
		///    Xiph's Vorbis Comment
		/// </summary>
		Xiph         = 0x00000001,
		
		/// <summary>
		///    ID3v1 Tag
		/// </summary>
		Id3v1        = 0x00000002,
		
		/// <summary>
		///    ID3v2 Tag
		/// </summary>
		Id3v2        = 0x00000004,
		
		/// <summary>
		///    APE Tag
		/// </summary>
		Ape          = 0x00000008,
		
		/// <summary>
		///    Apple's ILST Tag Format
		/// </summary>
		Apple        = 0x00000010,
		
		/// <summary>
		///    ASF Tag
		/// </summary>
		Asf          = 0x00000020,
		
		/// <summary>
		///    Standard RIFF INFO List Tag
		/// </summary>
		RiffInfo     = 0x00000040,
		
		/// <summary>
		///    RIFF Movie ID List Tag
		/// </summary>
		MovieId      = 0x00000080,
		
		/// <summary>
		///    DivX Tag
		/// </summary>
		DivX         = 0x00000100,
		
		/// <summary>
		///    FLAC Metadata Blocks Tag
		/// </summary>
		FlacMetadata = 0x00000200,
		
		/// <summary>
		///    All tag types.
		/// </summary>
		AllTags      = 0xFFFFFFFF
	}
	
	/// <summary>
	///    This abstract class provides generic access to standard tag
	///    features.
	/// </summary>
	/// <remarks>
	///    Because not every tag type supports the same features, it may be
	///    useful to check that the value is stored by re-reading the
	///    property after it is stored.
	/// </remarks>
	public abstract class Tag
	{
		/// <summary>
		///    Gets the tag types contained in the current instance.
		/// </summary>
		/// <value>
		///    A bitwise combined <see cref="TagLib.TagTypes" />
		///    containing the tag types contained in the current
		///    instance.
		/// </value>
		/// <remarks>
		///    For a standard tag, the value should be intuitive. For
		///    example, <see cref="TagLib.Id3v2.Tag" /> objects have a
		///    value of <see cref="TagLib.TagTypes.Id3v2" />. However,
		///    for tags of type <see cref="TagLib.CombinedTag" /> may
		///    contain multiple or no types.
		/// </remarks>
		public abstract TagTypes TagTypes {get;}
		
		/// <summary>
		///    Gets and sets the title for the media described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the title for the
		///    media described by the current instance or <see
		///    langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    The title is most commonly the name of the song or
		///    episode or a movie title. For example, "Daydream
		///    Believer" (a song by the Monkies), "Space Seed" (an
		///    episode of Star Trek), or "Harold and Kumar Go To White
		///    Castle" (a movie).
		/// </remarks>
		public virtual string Title {
			get {return null;}
			set {}
		}
		
		/// <summary>
		///    Gets and sets the performers or artists who performed in
		///    the media described by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the performers or
		///    artists who performed in the media described by the
		///    current instance or an empty array if not value is
		///    present.
		/// </value>
		/// <remarks>
		///    <para>This field is most commonly called "Artists" in
		///    media applications and should be used to represent each
		///    of the artists appearing in the media. It can be simple
		///    in theform of "The Beatles", or more complicated in the
		///    form of "John Lennon, Paul McCartney, George Harrison,
		///    Pete Best", depending on the preferences of the listener
		///    and the degree to which they organize their media
		///    collection.</para>
		///    <para>As the preference of the user may vary,
		///    applications should not try to limit the user in what
		///    choice they may make.</para>
		/// </remarks>
		public virtual string [] Performers {
			get {return new string [] {};}
			set {}
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
		///    instance or an empty array if not value is present.
		/// </value>
		/// <remarks>
		///    <para>This field is typically optional but aids in the
		///    sorting of compilations or albums with multiple artists.
		///    For example, if an album has several artists, sorting by
		///    artist will split up the album and sorting by album will
		///    split up albums by the same artist. Having a single album
		///    artist for an entire album will solve this
		///    problem.</para>
		///    <para>As this value is to be used as a sorting key, it
		///    should be used with less variation than <see
		///    cref="Performers" />. Where performers can be broken into
		///    muliple artist it is best to stick with a single band
		///    name. For example, "The Beatles".</para>
		/// </remarks>
		public virtual string [] AlbumArtists {
			get {return new string [] {};}
			set {}
		}
		
		/// <summary>
		///    Gets and sets the composers of the media represented by
		///    the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the composers of the
		///    media represented by the current instance or an empty
		///    array if not value is present.
		/// </value>
		/// <remarks>
		///    <para>This field represents the composers, song writers,
		///    script writers, or persons who claim authorship of the
		///    media.</para>
		/// </remarks>
		public virtual string [] Composers {
			get {return new string [] {};}
			set {}
		}
		
		/// <summary>
		///    Gets and sets the album of the media represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the album of the media
		///    represented by the current instance or null if no value
		///    is present.
		/// </value>
		/// <remarks>
		///    <para>This field represents the name of the album the
		///    media belongs to. In the case of a boxed set, it should
		///    be the name of the entire set rather than the individual
		///    disc.</para>
		///    <para>For example, "Rubber Soul" (an album by the
		///    Beatles), "The Sopranos: Complete First Season" (a boxed
		///    set of TV episodes), or "Back To The Future Trilogy" (a 
		///    boxed set of movies).</para>
		/// </remarks>
		public virtual string Album {
			get {return null;}
			set {}
		}
		
		/// <summary>
		///    Gets and sets a user comment on the media.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing user comments on the
		///    media.
		/// </value>
		/// <remarks>
		///    <para>This field should be used to store user notes and
		///    comments. There is no constraint on what text can be
		///    stored here, but it should not contain program
		///    information.</para>
		///    <para>Because this field contains notes that the user
		///    might think of while listening to the media, it may be
		///    useful for an application to make this field easily
		///    accessible, perhaps even including it in the main
		///    interface.</para>
		/// </remarks>
		public virtual string Comment {
			get {return null;}
			set {}
		}
		
		public virtual string [] Genres {
			get {return new string [] {};}
			set {}
		}
		
		public virtual uint Year {
			get {return 0;}
			set {}
		}
		
		public virtual uint Track {
			get {return 0;}
			set {}
		}
		
		public virtual uint TrackCount {
			get {return 0;}
			set {}
		}
		
		public virtual uint Disc {
			get {return 0;}
			set {}
		}
		
		public virtual uint DiscCount {
			get {return 0;}
			set {}
		}
		
		public virtual string Lyrics {
			get {return null;}
			set {}
		}
		
		public virtual string Grouping {
			get {return null;}
			set {}
		}
		
		public virtual uint BeatsPerMinute {
			get {return 0;}
			set {}
		}
		
		public virtual string Conductor {
			get {return null;}
			set {}
		}
		
		public virtual string Copyright {
			get {return null;}
			set {}
		}
		
		public virtual IPicture [] Pictures {
			get {return new Picture [] {};}
			set {}
		}
		
		[Obsolete("For album artists use AlbumArtists. For track artists, use Performers")]
		public virtual string [] Artists {
			get {return Performers;}
			set {Performers = value;}
		}
		
		[Obsolete("For album artists use FirstAlbumArtist. For track artists, use FirstPerformer")]
		public string FirstArtist {
			get {return FirstPerformer;}
		}
		
		public string FirstAlbumArtist {
			get {return FirstInGroup(AlbumArtists);}
		}
		
		public string FirstPerformer {
			get {return FirstInGroup(Performers);}
		}
		
		public string FirstComposer {
			get {return FirstInGroup(Composers);}
		}
		
		public string FirstGenre {
			get {return FirstInGroup(Genres);}
		}
		
		[Obsolete("For album artists use JoinedAlbumArtists. For track artists, use JoinedPerformers")]
		public string JoinedArtists {
			get {return JoinedPerformers;}
		}
		
		public string JoinedAlbumArtists {
			get {return JoinGroup(AlbumArtists);}
		}
		
		public string JoinedPerformers {
			get {return JoinGroup(Performers);}
		}
		
		public string JoinedComposers {
			get {return JoinGroup(Composers);}
		}
		
		public string JoinedGenres {
			get {return JoinGroup(Genres);}
		}
		
		private static string FirstInGroup(string [] group)
		{
			return group == null || group.Length == 0 ? null : group[0];
		}
		
		private static string JoinGroup(string [] group)
		{
			return new StringCollection(group).ToString(", ");
		}

		public virtual bool IsEmpty {
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
		
		public abstract void Clear ();
		
		public static void Duplicate (Tag source, Tag target, bool overwrite)
		{
			if (source == null)
				throw new ArgumentNullException ("source");
			
			if (target == null)
				throw new ArgumentNullException ("target");
			
			if (overwrite || IsNullOrLikeEmpty (target.Title))
				target.Title = source.Title;
			
			if (overwrite || IsNullOrLikeEmpty (target.AlbumArtists))
				target.AlbumArtists = source.AlbumArtists;
			
			if (overwrite || IsNullOrLikeEmpty (target.Performers))
				target.Performers = source.Performers;
			
			if (overwrite || IsNullOrLikeEmpty (target.Composers))
				target.Composers = source.Composers;
			
			if (overwrite || IsNullOrLikeEmpty (target.Album))
				target.Album = source.Album;
			
			if (overwrite || IsNullOrLikeEmpty (target.Comment))
				target.Comment = source.Comment;
			
			if (overwrite || IsNullOrLikeEmpty (target.Genres))
				target.Genres = source.Genres;
			
			if (overwrite || target.Year == 0)
				target.Year = source.Year;
			
			if (overwrite || target.Track == 0)
				target.Track = source.Track;
			
			if (overwrite || target.TrackCount == 0)
				target.TrackCount = source.TrackCount;
			
			if (overwrite || target.Disc == 0)
				target.Disc = source.Disc;
			
			if (overwrite || target.DiscCount == 0)
				target.DiscCount = source.DiscCount;
			
			if (overwrite || target.BeatsPerMinute == 0)
				target.BeatsPerMinute = source.BeatsPerMinute;
			
			if (overwrite || IsNullOrLikeEmpty (target.Grouping))
				target.Grouping = source.Grouping;
			
			if (overwrite || IsNullOrLikeEmpty (target.Conductor))
				target.Conductor = source.Conductor;
			
			if (overwrite || IsNullOrLikeEmpty (target.Copyright))
				target.Conductor = source.Copyright;
		}
		
		private static bool IsNullOrLikeEmpty (string value)
		{
			return value == null || value.Trim ().Length == 0;
		}
		
		private static bool IsNullOrLikeEmpty (string [] value)
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
