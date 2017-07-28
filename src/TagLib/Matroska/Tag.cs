//
// Tag.cs:
//
// Author:
//   Julien Moutte <julien@fluendo.com>
//   Sebastien Mouy <starwer@laposte.net>
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
using System.Globalization;
using System.Linq;
using System.Text;

namespace TagLib.Matroska
{
    /// <summary>
    /// Describes a Matroska Tag, containing SimpleTag 
    /// </summary>
    public class Tag : TagLib.Tag
    {
        #region Private fields/Properties

        /// <summary>
        /// Define if this represent a video content (true), or an audio content (false)
        /// </summary>
        private bool IsVideo
        {
            get { return Tags == null || Tags.IsVideo; }
        }


        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tags">the Tags this Tag should be added to.</param>
        /// <param name="targetTypeValue">the Target Type ValueTags this Tag represents.</param>
        public Tag(Tags tags = null, ushort targetTypeValue = 0)
        {
            if (targetTypeValue != 0) TargetTypeValue = targetTypeValue;
            Tags = tags;
            if(tags != null) tags.Add(this);
        }


        /// <summary>
        /// Shallo Copy Constructor
        /// </summary>
        /// <param name="tag">the Tag object to duplicate.</param>
        /// <param name="tags">the Tags this Tag should be added to.</param>
        public Tag(Tag tag, Tags tags = null) : this(tags)
        {
            TargetTypeValue = tag.TargetTypeValue;

            // Duplicate SimpleTag (shallow copy)
            foreach (var stag in tag.SimpleTag)
            {
                // Make sure tag ID is in upper-case
                SimpleTag[stag.Key.ToUpper()] = new List<string>(stag.Value);
            }
        }


        #endregion


        #region Methods

        /// <summary>
        /// Return a Tag of a certain Target type.  
        /// </summary>
        /// <param name="create">Create one if it doesn't exist yet.</param>
        /// <param name="targetTypeValue">Target Type Value of the .</param>
        /// <returns>the Tag representing the collection</returns>
        private Tag TagsGet(bool create, ushort targetTypeValue)
        {
            Tag ret = Tags?.Get(targetTypeValue, true);
            if (ret == null && create)
            {
                ret = new Tag(Tags, targetTypeValue);
            }
            return ret;
        }


        /// <summary>
        /// Return the Tag representing the Album the medium belongs to.  
        /// </summary>
        /// <param name="create">Create one if it doesn't exist yet.</param>
        /// <returns>the Tag representing the collection</returns>
        private Tag TagsAlbum(bool create)
        {
            Tag ret = null;
            if (Tags != null)
            {
                ret = Tags.Album;
                if (ret == null && create)
                {
                    var targetTypeValue = Tags.IsVideo ? (ushort)70 : (ushort)50;
                    ret = new Tag(Tags, targetTypeValue);
                }
            }
            return ret;
        }


        /// <summary>
        /// Set a Tag value
        /// </summary>
        /// <param name="key">Tag Name</param>
        /// <param name="value">value to be set</param>
        /// <param name="index">index of the value index to set (if several tags of same name)</param>
        private void TagSet(string key, string value, int index = 0)
        {
            List<string> list = null;

            if (!SimpleTag.TryGetValue(key, out list))
            {
                SimpleTag[key] = list = new List<string>(1);
            }

            if (index >= list.Count)
                list.Add(value);
            else
                list[index] = value;
        }

        /// <summary>
        /// Set a Tag list
        /// </summary>
        /// <param name="key">Tag Name</param>
        /// <param name="values">Array of values</param>
        private void TagSet(string key, string[] values)
        {
            SimpleTag[key] = values.ToList();
        }

        /// <summary>
        /// Set a Tag value as unsigned integer
        /// </summary>
        /// <param name="key">Tag Name</param>
        /// <param name="value">unsigned integer value to be set</param>
        /// <param name="format">Format for string convertion to be used (default: null)</param>
        /// <param name="index">index of the value index to set (default: 0)</param>
        private void TagSetUint(string key, uint value, string format = null, int index = 0)
        {
            TagSet(key, value.ToString(format, CultureInfo.InvariantCulture));
        }





        /// <summary>
        /// Retrieve a Tag list
        /// </summary>
        /// <param name="key">Tag name</param>
        /// <param name="recu">Also search in parent Tag if true (default: true)</param>
        /// <returns>Array of values</returns>
        private string[] Get(string key, bool recu = true)
        {
            string[] ret = null;
            List<string> list;

            if (!SimpleTag.TryGetValue(key, out list) && recu)
            {
                Tag tag = this;
                while ((tag = tag.Parent) != null && !tag.SimpleTag.TryGetValue(key, out list)) ;
            }

            if (list != null)
            {
                ret = list.ToArray();
            }

            return ret;
        }

        /// <summary>
        /// Retrieve a Tag value as string
        /// </summary>
        /// <param name="key">Tag name</param>
        /// <param name="index">value index to retrieve  (default: 0)</param>
        /// <param name="recu">Also search in parent Tag if true (default: true)</param>
        /// <returns>Tag value</returns>
        private string GetString(string key, int index = 0, bool recu = true)
        {
            string ret = null;
            List<string> list;

            if (!SimpleTag.TryGetValue(key, out list) && recu)
            {
                Tag tag = this;
                while ((tag = tag.Parent) != null && !tag.SimpleTag.TryGetValue(key, out list)) ;
            }

            if (list != null)
            {
                if (index < list.Count)
                {
                    ret = list[index];
                }
            }

            return ret;
        }


        /// <summary>
        /// Retrieve a Tag value as unsigned integer
        /// </summary>
        /// <param name="key">Tag name</param>
        /// <param name="index">value index to retrieve (default: 0)</param>
        /// <param name="recu">Also search in parent Tag if true (default: false)</param>
        /// <returns>Tag value as unsigned integer</returns>
        private uint GetUint(string key, int index = 0, bool recu = false)
        {
            uint ret = 0;
            string val = GetString(key, index, recu);

            if (val != null)
            {
                uint.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out ret);
            }

            return ret;
        }


        #endregion


        #region Taglib.Tag

        /// <summary>
        /// Retrieve a list of Matroska Tags 
        /// </summary>
        public Tags Tags { private set; get; }


        /// <summary>
        /// Retrieve the parent Tag, of higher TargetTypeValue (if any, null if none)
        /// </summary>
        public Tag Parent
        {
            get
            {
                Tag ret = null;
                if (Tags != null)
                {
                    int i = Tags.IndexOf(this);
                    if (i > 0) ret = Tags[i - 1];
                }
                return ret;
            }
        }

        /// <summary>
        ///    Gets the Matroska Target Type Value of this Tag.
        /// </summary>
        public ushort TargetTypeValue
        {
            get
            {
                return _TargetTypeValue;
            }
            set
            {
                // Coerce: Valid values are: 10 20 30 40 50 60 70
                _TargetTypeValue = (ushort)
                    ( value > 70 ? 70
                    : value < 10 ? 10
                    : ((value + 5) / 10) * 10
                    );

                // Make sure the List keeps ordered
                if (Tags != null)
                {
                    if(TargetType == null)  Tags.MakeTargetType(_TargetTypeValue);
                    Tags.Add(this);
                }
            }
        }
        private ushort _TargetTypeValue = 0;

        /// <summary>
        ///    Gets the Matroska Target Type (informational name) of this Tag.
        /// </summary>
        public string TargetType = null;

        /// <summary>
        /// Array of unique IDs to identify the Track(s) the tags belong to. If the value is 0 at this level, the tags apply to all tracks in the Segment.
        /// </summary>
        public ulong[] TrackUID = null;

        /// <summary>
        /// Array ofunique IDs to identify the EditionEntry(s) the tags belong to. If the value is 0 at this level, the tags apply to all editions in the Segment.
        /// </summary>
        public ulong[] EditionUID = null;

        /// <summary>
        ///  Array ofunique IDs to identify the Chapter(s) the tags belong to. If the value is 0 at this level, the tags apply to all chapters in the Segment. 
        /// </summary>
        public ulong[] ChapterUID = null;

        /// <summary>
        /// Array of unique IDs to identify the Attachment(s) the tags belong to. If the value is 0 at this level, the tags apply to all the attachments in the Segment.
        /// </summary>
        public ulong[] AttachmentUID = null;


        /// <summary>
        /// List SimpleTag contained in the current Tag (must never be null)
        /// </summary>
        public Dictionary<string, List<string>> SimpleTag = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);



        /// <summary>
        ///    Gets the tag types contained in the current instance.
        /// </summary>
        /// <value>
        ///    Always <see cref="TagTypes.Matroska" />.
        /// </value>
        public override TagTypes TagTypes
        {
            get { return TagTypes.Matroska; }
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
        ///    This property is implemented using the TITLE tag and the Segment Title.
        /// </remarks>
        public override string Title
        {
            get
            {
                var ret = GetString("TITLE");
                if (ret == null && Tags?.Medium == this) ret = Tags.Title;
                return ret;
            }
            set
            {
                TagSet("TITLE", value);
                if (Tags?.Medium == this) Tags.Title = value;
            }
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
        ///    This property is implemented using the ACTOR/PERFORMER stored in
        ///    the MKV Tag element.
        /// </remarks>
        public override string [] Performers
        {
            get { return Get(IsVideo ? "ACTOR" : "PERFORMER"); }
            set { TagSet(IsVideo ? "ACTOR" : "PERFORMER", value); }
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
        /// <remarks>
        ///    This property is implemented using the "ARTIST" Tag.
        /// </remarks>
        public override string [] AlbumArtists
        {
            get { return TagsAlbum(false)?.Get("ARTIST"); }
            set { TagsAlbum(true)?.TagSet("ARTIST", value); }
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
        /// <remarks>
        ///    This property is implemented using the "COMPOSER" Tag.
        /// </remarks>
        public override string [] Composers
        {
            get { return Get("COMPOSER"); }
            set { TagSet("COMPOSER", value); }
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
        ///    This property is implemented using the "TITLE" Tag in the Collection Tags.
        /// </remarks>
        public override string Album
        {
            get { return TagsAlbum(false)?.GetString("TITLE"); }
            set { TagsAlbum(true)?.TagSet("TITLE", value); }
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
        ///    This property is implemented using the "COMMENT" Tag.
        /// </remarks>
        public override string Comment
        {
            get { return GetString("COMMENT"); }
            set { TagSet("COMMENT", value); }
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
                string value = GetString("GENRE");

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
                TagSet("GENRE", String.Join ("; ", value));
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
                string val = GetString("DATE_RECORDED");
                uint ret = 0;

                // Parse Date to retrieve year
                // Expected format: YYYY-MM-DD HH:MM:SS.MSS 
                //   with: YYYY = Year, -MM = Month, -DD = Days, 
                //         HH = Hours, :MM = Minutes, :SS = Seconds, :MSS = Milliseconds
                if (val != null)
                {
                    int off = val.IndexOf('-');
                    if (off > 0) val = val.Substring(0, off);
                    uint.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out ret);
                }

                return ret;
            }
            set { TagSetUint("DATE_RECORDED", value); }
        }

        /// <summary>
        ///    Gets and sets the position of the media represented by
        ///    the current instance in its containing item (album, disc, episode, collection...).
        /// </summary>
        /// <value>
        ///    A <see cref="uint" /> containing the position of the
        ///    media represented by the current instance in its
        ///    containing album or zero if not specified.
        /// </value>
        public override uint Track
        {
            get { return GetUint("PART_NUMBER"); }
            set { TagSetUint("PART_NUMBER", value, "00"); }
        }

        /// <summary>
        ///    Gets and sets the number of items contained in the parent Tag (album, disc, episode, collection...)
        ///    the media represented by the current instance belongs to.
        /// </summary>
        /// <value>
        ///    A <see cref="uint" /> containing the number of tracks in
        ///    the album containing the media represented by the current
        ///    instance or zero if not specified.
        /// </value>
        public override uint TrackCount
        {
            get { return TagsGet(false, (ushort)(TargetTypeValue + 10))?.GetUint("TOTAL_PARTS") ?? 0; }
            set { TagsGet(true, (ushort)(TargetTypeValue + 10))?.TagSetUint("TOTAL_PARTS", value); }
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
            get { return TagsGet(false, 40)?.GetUint("PART_NUMBER") ?? 0; }
            set { TagsGet(true, 40)?.TagSetUint("PART_NUMBER", value); }
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
            get { return TagsGet(false, 50)?.GetUint("TOTAL_PARTS") ?? 0; }
            set { TagsGet(true, 50)?.TagSetUint("TOTAL_PARTS", value); }
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
            get { return GetString("LYRICS"); }
            set { TagSet("LYRICS", value); }
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
            get { return TagsAlbum(false)?.GetString("GROUPING"); }
            set { TagsAlbum(true)?.TagSet("GROUPING", value); }
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
            get { return GetUint("BPM", 0, true); }
            set { TagSetUint("BPM", value); }
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
            get { return GetString(IsVideo ? "DIRECTOR" : "CONDUCTOR"); }
            set { TagSet(IsVideo ? "DIRECTOR" : "CONDUCTOR", value); }
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
            get { return GetString("COPYRIGHT"); }
            set { TagSet("COPYRIGHT", value); }
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
                return Tags?.Pictures;
            }
            set
            {
                Tags.Pictures = value;
            }
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
                return SimpleTag.Count == 0;
            }
        }

        /// <summary>
        ///    Clears the values stored in the current instance.
        /// </summary>
        public override void Clear ()
        {
            SimpleTag.Clear();
        }

        #endregion
    }
}
