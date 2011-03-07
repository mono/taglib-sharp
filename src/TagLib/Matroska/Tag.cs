//
// Tag.cs:
//
// Author:
//   Julien Moutte <julien@fluendo.com>
//
// Copyright (C) 2011 FLUENDO S.A.
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
using System.Text;

namespace TagLib.Matroska
{
    /// <summary>
    /// Describes a Matroska Tag.
    /// </summary>
    public class Tag : TagLib.Tag
    {
        #region Private fields

        private string title;
        private string author;
        private string album;
        private string comments;
        private string genres;
        private string copyright;

        #endregion

        #region Constructors

        #endregion

        #region Taglib.Tag


        /// <summary>
        ///    Gets the tag types contained in the current instance.
        /// </summary>
        /// <value>
        ///    Always <see cref="TagTypes.Id3v2" />.
        /// </value>
        public override TagTypes TagTypes
        {
            get { return TagTypes.Id3v2; }
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
        ///    This property is implemented using the title stored in
        ///    the ASF Content Description Object.
        /// </remarks>
        public override string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        ///    Gets and sets the sort names for the Track Title of
        ///    the media described by the current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the sort name of 
        ///    the Track Title of the media described by the current
        ///    instance or null if no value is present.
        /// </value>
        /// <remarks>
        ///    This property is implemented using the "WM/TitleSortOrder"
        ///    field.
        ///    http://msdn.microsoft.com/en-us/library/aa386866(VS.85).aspx
        /// </remarks>
        public override string TitleSort
        {
            get { return null; }
            set { }
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
        ///    This property is implemented using the author stored in
        ///    the MKV Tag element.
        /// </remarks>
        public override string [] Performers
        {
            get { return new string [] { author }; }
            set { author = string.Join ("; ", value); }
        }

        /// <summary>
        ///    Gets and sets the sort names of the performers or artists
        ///    who performed in the media described by the current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string[]" /> containing the sort names for
        ///    the performers or artists who performed in the media
        ///    described by the current instance, or an empty array if
        ///    no value is present. 
        /// </value>
        public override string [] PerformersSort
        {
            get
            {
                return new string [] {};
            }
            set { }
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
        public override string [] AlbumArtists
        {
            get
            {
                return new string [] {};
            }
            set {}
        }

        /// <summary>
        ///    Gets and sets the sort names for the band or artist who
        ///    is credited in the creation of the entire album or
        ///    collection containing the media described by the
        ///    current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string[]" /> containing the sort names
        ///    for the band or artist who is credited in the creation
        ///    of the entire album or collection containing the media
        ///    described by the current instance or an empty array if
        ///    no value is present.
        /// </value>
        public override string [] AlbumArtistsSort
        {
            get
            {
                return new string [] { };
            }
            set { }
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
        public override string [] Composers
        {
            get
            {
                return new string [] { };
            }
            set { }
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
        ///    This property is implemented using the "ALBUM" Tag.
        /// </remarks>
        public override string Album
        {
            get
            {
                return album;
            }
            set
            {
                album = value;
            }
        }

        /// <summary>
        ///    Gets and sets the sort names for the Album Title of
        ///    the media described by the current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the sort name of 
        ///    the Album Title of the media described by the current
        ///    instance or null if no value is present.
        /// </value>
        public override string AlbumSort
        {
            get
            {
                return null;
            }
            set { }
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
        ///    This property is implemented using the "COMMENTS" Tag.
        /// </remarks>
        public override string Comment
        {
            get { return comments; }
            set { comments = value; }
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
        ///    This property is implemented using the "GENRE" Tag.
        /// </remarks>
        public override string [] Genres
        {
            get
            {
                string value = genres;

                if (value == null || value.Trim ().Length == 0)
                    return new string [] { };

                string [] result = value.Split (';');

                for (int i = 0; i < result.Length; i++) {
                    string genre = result [i].Trim ();

                    byte genre_id;
                    int closing = genre.IndexOf (')');
                    if (closing > 0 && genre [0] == '(' &&
                        byte.TryParse (genre.Substring (
                        1, closing - 1), out genre_id))
                        genre = TagLib.Genres
                            .IndexToAudio (genre_id);

                    result [i] = genre;
                }

                return result;
            }
            set
            {
                genres = String.Join ("; ", value);
            }
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
        public override uint Year
        {
            get
            {
                return 0;
            }
            set { }
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
        public override uint Track
        {
            get
            {
                return 0;
            }
            set { }
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
        public override uint TrackCount
        {
            get
            {
                return 0;
            }
            set { }
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
        public override uint Disc
        {
            get
            {
                return 0;
            }
            set { }
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
        public override uint DiscCount
        {
            get
            {
                return 0;
            }
            set { }
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
        public override string Lyrics
        {
            get { return null; }
            set { }
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
        public override string Grouping
        {
            get
            {
                return null;
            }
            set {}
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
        public override uint BeatsPerMinute
        {
            get
            {
                return 0;
            }
            set { }
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
        public override string Conductor
        {
            get { return null; }
            set { }
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
        ///    This property is implemented using the "COPYRIGHT" Tag.
        /// </remarks>
        public override string Copyright
        {
            get { return copyright; }
            set { copyright = value; }
        }

        /// <summary>
        ///    Gets and sets the MusicBrainz Artist ID of
        ///    the media described by the current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the MusicBrainz 
        ///    ArtistID for the media described by the current
        ///    instance or null if no value is present.
        /// </value>
        public override string MusicBrainzArtistId
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///    Gets and sets the MusicBrainz Release ID of
        ///    the media described by the current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the MusicBrainz 
        ///    ReleaseID for the media described by the current
        ///    instance or null if no value is present.
        /// </value>
        public override string MusicBrainzReleaseId
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///    Gets and sets the MusicBrainz Release Artist ID of
        ///    the media described by the current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the MusicBrainz 
        ///    ReleaseArtistID for the media described by the current
        ///    instance or null if no value is present.
        /// </value>
        public override string MusicBrainzReleaseArtistId
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///    Gets and sets the MusicBrainz Track ID of
        ///    the media described by the current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the MusicBrainz 
        ///    TrackID for the media described by the current
        ///    instance or null if no value is present.
        /// </value>
        public override string MusicBrainzTrackId
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///    Gets and sets the MusicBrainz Disc ID of
        ///    the media described by the current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the MusicBrainz 
        ///    DiscID for the media described by the current
        ///    instance or null if no value is present.
        /// </value>
        public override string MusicBrainzDiscId
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///    Gets and sets the MusicIP PUID of
        ///    the media described by the current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the MusicIPPUID 
        ///    for the media described by the current instance or
        ///    null if no value is present.
        /// </value>
        public override string MusicIpId
        {
            get { return null; }
            set { }
        }

        // <summary>
        //    Gets and sets the AmazonID of
        //    the media described by the current instance.
        // </summary>
        // <value>
        //    A <see cref="string" /> containing the AmazonID 
        //    for the media described by the current instance or
        //    null if no value is present.  
        // </value>
        // <remarks>
        //    A definition on where to store the ASIN for
        //    Windows Media is not currently defined
        // </remarks>
        //public override string AmazonId {
        //    get { return null; }
        //    set {}
        //}

        /// <summary>
        ///    Gets and sets the MusicBrainz Release Status of
        ///    the media described by the current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the MusicBrainz 
        ///    ReleaseStatus for the media described by the current
        ///    instance or null if no value is present.
        /// </value>
        public override string MusicBrainzReleaseStatus
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///    Gets and sets the MusicBrainz Release Type of
        ///    the media described by the current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the MusicBrainz 
        ///    ReleaseType for the media described by the current
        ///    instance or null if no value is present.
        /// </value>
        public override string MusicBrainzReleaseType
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///    Gets and sets the MusicBrainz Release Country of
        ///    the media described by the current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the MusicBrainz 
        ///    ReleaseCountry for the media described by the current
        ///    instance or null if no value is present.
        /// </value>
        public override string MusicBrainzReleaseCountry
        {
            get { return null; }
            set { }
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
        public override IPicture [] Pictures
        {
            get
            {
                List<IPicture> l = new List<IPicture> ();


                return l.ToArray ();
            }
            set { }
        }

        /// <summary>
        ///    Gets whether or not the current instance is empty.
        /// </summary>
        /// <value>
        ///    <see langword="true" /> if the current instance does not
        ///    any values. Otherwise <see langword="false" />.
        /// </value>
        public override bool IsEmpty
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///    Clears the values stored in the current instance.
        /// </summary>
        public override void Clear ()
        {
            
        }

        #endregion
    }
}
