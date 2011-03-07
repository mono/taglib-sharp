//
// File.cs:
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

using System.Collections.Generic;
using System;

namespace TagLib.Matroska
{
    /// <summary>
    /// Enumeration listing supported Matroska track types.
    /// </summary>
    public enum TrackType
    {
        /// <summary>
        /// Video track type.
        /// </summary>
        Video = 0x1,
        /// <summary>
        /// Audio track type.
        /// </summary>
        Audio = 0x2,
        /// <summary>
        /// Complex track type.
        /// </summary>
        Complex = 0x3,
        /// <summary>
        /// Logo track type.
        /// </summary>
        Logo = 0x10,
        /// <summary>
        /// Subtitle track type.
        /// </summary>
        Subtitle = 0x11,
        /// <summary>
        /// Buttons track type.
        /// </summary>
        Buttons = 0x12,
        /// <summary>
        /// Control track type.
        /// </summary>
        Control = 0x20
    }

    /// <summary>
    ///    This class extends <see cref="TagLib.File" /> to provide tagging
    ///    and properties support for Matroska files.
    /// </summary>
    [SupportedMimeType ("taglib/mkv", "mkv")]
    [SupportedMimeType ("taglib/mka", "mka")]
    [SupportedMimeType ("taglib/mks", "mks")]
    [SupportedMimeType ("video/webm")]
    [SupportedMimeType ("video/x-matroska")]
    public class File : TagLib.File
    {
        #region Private Fields

        /// <summary>
        ///   Contains the tags for the file.
        /// </summary>
        private Matroska.Tag tag = new Matroska.Tag ();

        /// <summary>
        ///    Contains the media properties.
        /// </summary>
        private Properties properties;

        private UInt64 duration_unscaled;
        private uint time_scale;

        private TimeSpan duration;

#pragma warning disable 414 // Assigned, never used
        private string title;
#pragma warning restore 414

        private List<Track> tracks = new List<Track> ();

        #endregion



        #region Constructors

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
        public File (string path, ReadStyle propertiesStyle)
            : this (new File.LocalFileAbstraction (path),
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
        public File (string path)
            : this (path, ReadStyle.Average)
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
        public File (File.IFileAbstraction abstraction,
                     ReadStyle propertiesStyle)
            : base (abstraction)
        {
            Mode = AccessMode.Read;
            try {
                Read (propertiesStyle);
                TagTypesOnDisk = TagTypes;
            }
            finally {
                Mode = AccessMode.Closed;
            }

            List<ICodec> codecs = new List<ICodec> ();

            foreach (Track track in tracks) {
                codecs.Add (track);
            }

            properties = new Properties (duration, codecs);
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
        public File (File.IFileAbstraction abstraction)
            : this (abstraction, ReadStyle.Average)
        {
        }

        #endregion



        #region Public Methods

        /// <summary>
        ///    Saves the changes made in the current instance to the
        ///    file it represents.
        /// </summary>
        public override void Save ()
        {
            Mode = AccessMode.Write;
            try {

            }
            finally {
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
        public override void RemoveTags (TagLib.TagTypes types)
        {

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
        public override TagLib.Tag GetTag (TagLib.TagTypes type,
                                           bool create)
        {

            return null;
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
        public override TagLib.Tag Tag
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
            get
            {
                return properties;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///    Reads the file with a specified read style.
        /// </summary>
        /// <param name="propertiesStyle">
        ///    A <see cref="ReadStyle" /> value specifying at what level
        ///    of accuracy to read the media properties, or <see
        ///    cref="ReadStyle.None" /> to ignore the properties.
        /// </param>
        private void Read (ReadStyle propertiesStyle)
        {
            ulong offset = 0;

            while (offset < (ulong) Length) {
                EBMLElement element = new EBMLElement (this, offset);

                EBMLID ebml_id = (EBMLID) element.ID;
                MatroskaID matroska_id = (MatroskaID) element.ID;

                switch (ebml_id) {
                    case EBMLID.EBMLHeader:
                        ReadHeader (element);
                        break;
                    default:
                        break;
                }
                switch (matroska_id) {
                    case MatroskaID.MatroskaSegment:
                        ReadSegment (element);
                        break;
                    default:
                        break;
                }

                offset += element.Size;
            }
        }

        private void ReadHeader (EBMLElement element)
        {
            string doctype = null;
            ulong i = 0;

            while (i < element.DataSize) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                EBMLID ebml_id = (EBMLID) child.ID;

                switch (ebml_id) {
                    case EBMLID.EBMLDocType:
                        doctype = child.ReadString ();
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            // Check DocType
            if (String.IsNullOrEmpty (doctype) || (doctype != "matroska" && doctype != "webm")) {
                throw new UnsupportedFormatException ("DocType is not matroska or webm");
            }
        }

        private void ReadSegment (EBMLElement element)
        {
            ulong i = 0;

            while (i < element.DataSize) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;

                switch (matroska_id) {
                    case MatroskaID.MatroskaTracks:
                        ReadTracks (child);
                        break;
                    case MatroskaID.MatroskaSegmentInfo:
                        ReadSegmentInfo (child);
                        break;
                    case MatroskaID.MatroskaTags:
                        ReadTags (child);
                        break;
                    case MatroskaID.MatroskaCluster:
                        // Get out of here when we reach the clusters for now.
                        return;
                    default:
                        break;
                }

                i += child.Size;
            }
        }

        private void ReadTags (EBMLElement element)
        {
            ulong i = 0;

            while (i < element.DataSize) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;

                switch (matroska_id) {
                    case MatroskaID.MatroskaTag:
                        ReadTag (child);
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }
        }

        private void ReadTag (EBMLElement element)
        {
            ulong i = 0;

            while (i < element.DataSize) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;

                switch (matroska_id) {
                    case MatroskaID.MatroskaSimpleTag:
                        ReadSimpleTag (child);
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }
        }

        private void ReadSimpleTag (EBMLElement element)
        {
            ulong i = 0;
#pragma warning disable 219 // Assigned, never read
            string tag_name = null, tag_language = null, tag_string = null;
#pragma warning restore 219

            while (i < element.DataSize) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;

                switch (matroska_id) {
                    case MatroskaID.MatroskaTagName:
                        tag_name = child.ReadString ();
                        break;
                    case MatroskaID.MatroskaTagLanguage:
                        tag_language = child.ReadString ();
                        break;
                    case MatroskaID.MatroskaTagString:
                        tag_string = child.ReadString ();
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            if (tag_name == "AUTHOR") {
                tag.Performers = new string [] { tag_string };
            }
            else if (tag_name == "TITLE") {
                tag.Title = tag_string;
            }
            else if (tag_name == "ALBUM") {
                tag.Album = tag_string;
            }
            else if (tag_name == "COMMENTS") {
                tag.Comment = tag_string;
            }
        }

        private void ReadSegmentInfo (EBMLElement element)
        {
            ulong i = 0;

            while (i < element.DataSize) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;

                switch (matroska_id) {
                    case MatroskaID.MatroskaDuration:
                        duration_unscaled = (UInt64) child.ReadDouble ();
                        if (time_scale > 0) {
                            duration = TimeSpan.FromSeconds (duration_unscaled * time_scale / 1000000000);
                        }
                        break;
                    case MatroskaID.MatroskaTimeCodeScale:
                        time_scale = child.ReadUInt ();
                        if (duration_unscaled > 0) {
                            duration = TimeSpan.FromSeconds (duration_unscaled * time_scale / 1000000000);
                        }
                        break;
                    case MatroskaID.MatroskaTitle:
                        title = child.ReadString ();
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }
        }

        private void ReadTracks (EBMLElement element)
        {
            ulong i = 0;

            while (i < element.DataSize) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;

                switch (matroska_id) {
                    case MatroskaID.MatroskaTrackEntry:
                        ReadTrackEntry (child);
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }
        }

        private void ReadTrackEntry (EBMLElement element)
        {
            ulong i = 0;

            while (i < element.DataSize) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;

                switch (matroska_id) {
                    case MatroskaID.MatroskaTrackType: {
                            TrackType track_type = (TrackType) child.ReadUInt ();

                            switch (track_type) {
                                case TrackType.Video: {
                                        VideoTrack track = new VideoTrack (this, element);

                                        tracks.Add (track);
                                        break;
                                    }
                                case TrackType.Audio: {
                                        AudioTrack track = new AudioTrack (this, element);

                                        tracks.Add (track);
                                        break;
                                    }
                                case TrackType.Subtitle: {
                                        SubtitleTrack track = new SubtitleTrack (this, element);

                                        tracks.Add (track);
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    default:
                        break;
                }

                i += child.Size;
            }
        }

        #endregion

        #region Private Properties

        #endregion
    }
}
