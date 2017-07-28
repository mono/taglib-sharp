//
// File.cs:
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

using System.Collections.Generic;
using System;
using System.Linq;

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
        private Matroska.Tags tags;

        /// <summary>
        ///    Contains the media properties.
        /// </summary>
        private Properties properties;

        private ulong duration_unscaled;
        private ulong time_scale;
        private TimeSpan duration;

        private List<Track> tracks = new List<Track> ();

        private bool updateTags = false;

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
            tags = new Matroska.Tags(this);

            Mode = AccessMode.Read;

            try {
                ReadWrite (propertiesStyle);
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

            updateTags = true;
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
                ReadWrite(ReadStyle.None);
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
            if((types & TagTypes.Matroska) !=0)
            {
                tags.Clear();
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
        public override TagLib.Tag GetTag (TagLib.TagTypes type,
                                           bool create)
        {
            TagLib.Tag ret = null;
            if (type == TagTypes.Matroska) ret = Tag;
            return ret;
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
            get
            {
                if(updateTags)
                {
                    var retag = tags.ToArray();
                    foreach (var tag in retag)
                    {
                        // This will force the default TagetTypeValue to get a proper value according to the medium type (audio/video)
                        if (tag.TargetTypeValue == 0) tags.Add(tag);
                    }
                    updateTags = false;
                }

                // Add Empty Tag representing the Medium to avoid null object
                if (tags.Medium == null)
                {
                    new Matroska.Tag(tags); 
                }

                return tags.Medium;
            }
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
        /// Add a Tag
        /// </summary>
        /// <param name="tag">Tag Object</param>
        /// <param name="key">TagName value</param>
        /// <param name="value">TagString value</param>
        private void TagPush(Tag tag, string key, string value)
        {
            List<string> list = null;

            if(tag.SimpleTag.ContainsKey(key))
                list = tag.SimpleTag[key];

            if (list == null)
                tag.SimpleTag[key] = list = new List<string>(1);

            list.Add(value);
        }
        

        /// <summary>
        ///    Reads (and Write, if file Mode is Write) the file with a specified read style.
        /// </summary>
        /// <param name="propertiesStyle">
        ///    A <see cref="ReadStyle" /> value specifying at what level
        ///    of accuracy to read the media properties, or <see
        ///    cref="ReadStyle.None" /> to ignore the properties.
        /// </param>
        private void ReadWrite (ReadStyle propertiesStyle)
        {
            long ret = 0;
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
                    case MatroskaID.Segment:
                        ret += ReadWriteSegment(element, propertiesStyle);
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

        private long ReadWriteSegment (EBMLElement element, ReadStyle propertiesStyle)
        {
            long ret = 0;
            ulong i = 0;
            var segm_list = new List<EBMLElement>(8);
            EBMLElement ebml_seek = null; // find first SeekHead
            ulong ebml_seek_end = 0;

            while (i < (ulong)((long)element.DataSize + ret)) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;
                bool seek = true;

                switch (matroska_id) {
                    case MatroskaID.SeekHead:
                        if (ebml_seek == null && i == 0)
                        {
                            ebml_seek = child;
                            ebml_seek_end = i + child.Size;
                        }
                        else if (Mode == AccessMode.Write)
                        {
                            ret += child.Remove(); // Remove all other SeekHeads
                            continue;
                        }
                        seek = false;
                        break;
                    case MatroskaID.Tracks:
                        if(propertiesStyle != ReadStyle.None) ReadTracks (child);
                        break;
                    case MatroskaID.SegmentInfo:
                        ret += ReadWriteSegmentInfo(child);
                        break;
                    case MatroskaID.Tags:
                        if (Mode == AccessMode.Write)
                        {
                            ret += child.Remove(); // Remove all Tags
                            continue;
                        }
                        ReadTags(child);
                        break;
                    case MatroskaID.Attachments:
                        ret += ReadWriteAttachments(child);
                        break;
                    case MatroskaID.Cluster:
                        break;
                    case MatroskaID.Void:
                        if (ebml_seek_end == i) ebml_seek_end += child.Size; // take on void
                        seek = false;
                        break;
                    default:
                        break;
                }

                if(seek) segm_list.Add(child);

                i += child.Size;
            }

            if (Mode == AccessMode.Write)
            {
                i += element.DataOffset;

                // Create EBML Tags if Tag available
                if (tags.Count > 0 && (tags.Count>1 || !tags[0].IsEmpty))
                {
                    var ebml_tags = new EBMLElement(this, i, MatroskaID.Tags);
                    WriteTags(ebml_tags);
                    ret += (long)ebml_tags.Size;
                    i += ebml_tags.Size;
                    segm_list.Add(ebml_tags);
                }

                // TODO: Create EBML Attachments

                // Generate the SeekHead
                ret += WriteSeekHead(element.DataOffset, (long)ebml_seek_end, segm_list);

                // Update Element EBML data-size
                ret = element.ResizeData(ret);
            }

            return ret;
        }


        private long  WriteSeekHead(ulong position, long oldsize = 0, List<EBMLElement> segm_list = null)
        {
            // Reserve size upfront for the SeekHead to know the offset to apply to each position
            long seek_size = 4 + 8 + (segm_list.Count * (4 + 8 + 2 * (2 + 8))) + 10;
            if (oldsize > seek_size) seek_size = oldsize;
            long ret = seek_size - oldsize;
            long offset = ret - (long)position;

            // Replace old SeekHead by new one
            if(oldsize>0) RemoveBlock((long)position, oldsize);
            var element = new EBMLElement(this, position, MatroskaID.SeekHead);
            position = element.DataOffset;


            // Create the Seek Entries
            foreach (var segm in segm_list)
            {
                ulong i = position;

                var seekEntry = new EBMLElement(this, i, MatroskaID.Seek);
                i += seekEntry.Size;

                var seekId = new EBMLElement(this, i, MatroskaID.SeekID);
                seekId.WriteData(segm.ID);
                i += seekId.Size;

                var seekPosition = new EBMLElement(this, i, MatroskaID.SeekPosition);
                seekPosition.WriteData((ulong)((long)segm.Offset + offset));
                i += seekPosition.Size;

                seekEntry.DataSize = seekId.Size + seekPosition.Size;
                position += seekEntry.Size;
            }


            // Update Element EBML data-size
            element.DataSize = position - element.DataOffset;

            // Complete with reserved size with void
            long voidSize = seek_size - (long)element.Size;
            WriteVoid(element.Offset + element.Size, (int)voidSize);

            return ret;
        }


        private long WriteVoid(ulong position, int size)
        {
            if (size<2) throw new UnsupportedFormatException("Invalid size for WriteVoid");

            var element = new EBMLElement(this, position, MatroskaID.Void);
            element.DataSize = (ulong)size;
            element.DataSize = 0;
            element.WriteData(new ByteVector(size - (int)element.Size));

            return size;
        }


        private long ReadWriteSeek(EBMLElement element, long offset = 0, List<EBMLElement> segm_list = null)
        {
            long ret = 0;
            ulong i = 0;
            EBMLElement ebml_id = null;
            EBMLElement ebml_position = null;

            while (i < (ulong)((long)element.DataSize + ret))
            {
                EBMLElement child = new EBMLElement(this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID)child.ID;

                switch (matroska_id)
                {
                    case MatroskaID.SeekID:
                        ebml_id = child;
                        break;
                    case MatroskaID.SeekPosition:
                        ebml_position = child;
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            if (Mode == AccessMode.Write)
            {
                i += element.DataOffset;

                if(ebml_id != null && ebml_position != null)
                {
                    // Valid
                    ulong target_id = ebml_id.ReadULong();
                    for(int idx=0; idx< segm_list.Count; idx++)
                    {
                        var ebml = segm_list[idx];
                        if (target_id == ebml.ID)
                        {
                            long position = (long) ebml.Offset + offset;
                            segm_list.RemoveAt(idx);
                            idx--;
                        }
                    }


                    // Update Element EBML data-size
                    ret = element.ResizeData(ret);
                }
                else
                {
                    // Invalid
                    RemoveBlock((long)i, (long)element.Size);
                }
            }

            return ret;
        }


        private void ReadTags (EBMLElement element)
        {
            ulong i = 0;

            while (i < (ulong)((long)element.DataSize)) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;

                switch (matroska_id) {
                    case MatroskaID.Tag:
                        ReadTag(child);
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }
        }


        private void ReadTag(EBMLElement element)
        {
            ulong i = 0;

            // Create new Tag
            var tag = new Matroska.Tag(tags);

            while (i < (ulong)((long)element.DataSize))
            {
                EBMLElement child = new EBMLElement(this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID)child.ID;

                switch (matroska_id)
                {
                    case MatroskaID.Targets:
                        ReadTargets(child, tag);
                        break;
                    case MatroskaID.SimpleTag:
                        ReadSimpleTag(child, tag);
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

        }


        private void ReadTargets(EBMLElement element, Tag tag)
        {
            ulong i = 0;

            string targetType = null;
            var TrackUID = new List<ulong>();
            var EditionUID = new List<ulong>();
            var ChapterUID = new List<ulong>();
            var AttachmentUID = new List<ulong>();

            while (i < element.DataSize)
            {
                EBMLElement child = new EBMLElement(this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID)child.ID;

                switch (matroska_id)
                {
                    case MatroskaID.TargetTypeValue:
                        tag.TargetTypeValue = (ushort)child.ReadULong();
                        break;
                    case MatroskaID.TargetType:
                        targetType = child.ReadString();
                        break;
                    case MatroskaID.TagTrackUID:
                        TrackUID.Add(child.ReadULong());
                        break;
                    case MatroskaID.TagEditionUID:
                        EditionUID.Add(child.ReadULong());
                        break;
                    case MatroskaID.TagChapterUID:
                        ChapterUID.Add(child.ReadULong());
                        break;
                    case MatroskaID.TagAttachmentUID:
                        AttachmentUID.Add(child.ReadULong());
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            tag.TargetType = targetType;
            if (TrackUID.Count > 0) tag.TrackUID = TrackUID.ToArray();
            if (EditionUID.Count > 0) tag.EditionUID = EditionUID.ToArray();
            if (ChapterUID.Count > 0) tag.ChapterUID = ChapterUID.ToArray();
            if (AttachmentUID.Count > 0) tag.AttachmentUID = AttachmentUID.ToArray();

        }


        private void ReadSimpleTag(EBMLElement element, Tag tag)
        {
            ulong i = 0;
#pragma warning disable 219 // Assigned, never read
            string tag_name = null, tag_language = null, tag_string = null;
#pragma warning restore 219
            bool isBinary = false;

            // For overwritting EBML string/binary (mutex)
            EBMLElement ebml_string = null;

            while (i < (ulong)((long)element.DataSize))
            {
                EBMLElement child = new EBMLElement(this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID)child.ID;

                switch (matroska_id)
                {
                    case MatroskaID.TagName:
                        tag_name = child.ReadString();
                        break;
                    case MatroskaID.TagLanguage:
                        tag_language = child.ReadString();
                        break;
                    case MatroskaID.TagString:
                        ebml_string = child;
                        tag_string = child.ReadString();
                        break;
                    case MatroskaID.TagBinary:
                        if (ebml_string == null) ebml_string = child;
                        isBinary = true;
                        break;
                    case MatroskaID.SimpleTag:
                        // TODO: SimpleTag recursion
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            if (!isBinary) // Binary Tag not handled
            {
                TagPush(tag, tag_name, tag_string);
            }

        }

        private void WriteTags(EBMLElement element)
        {
            ulong i = element.DataOffset;

            foreach (var tag in tags)
            {
                // Create new EBML Tag
                var ebml_tag = new EBMLElement(this, i, MatroskaID.Tag);

                // Write Tag content
                WriteTag(ebml_tag, tag);

                i += ebml_tag.Size;
            }

            // Update Tags EBML data-size
            element.DataSize = i - element.DataOffset;

        }


        private void WriteTag(EBMLElement element, Tag tag)
        {
            ulong i = element.DataOffset;

            // Write Targets
            var ebml_targets = new EBMLElement(this, i, MatroskaID.Targets);
            WriteTargets(ebml_targets, tag);
            i += ebml_targets.Size;

            // Extract the SimpleTag from the Tag object
            foreach( var stagList in tag.SimpleTag)
            {
                string key = stagList.Key;
                foreach (var stag in stagList.Value)
                {
                    var ebml_Simpletag = new EBMLElement(this, i, MatroskaID.SimpleTag);
                    WriteSimpleTag(ebml_Simpletag, key, stag);
                    i += ebml_Simpletag.Size;
                }
            }

            // Update Tag EBML data-size
            element.DataSize = i - element.DataOffset;
        }


        private void WriteSimpleTag (EBMLElement element, string key, string value)
        {
            ulong i = element.DataOffset;

            // Write SimpleTag content
            var ebml_tagName = new EBMLElement(this, i, MatroskaID.TagName, key);
            i += ebml_tagName.Size;

            var ebml_tagLanguage = new EBMLElement(this, i, MatroskaID.TagLanguage, "und");
            i += ebml_tagLanguage.Size;

            var ebml_tagDefault = new EBMLElement(this, i, MatroskaID.TagDefault, (ulong)1);
            i += ebml_tagDefault.Size;

            var ebml_tagString = new EBMLElement(this, i, MatroskaID.TagString, value);
            i += ebml_tagString.Size;

            // TODO: SimpleTag Recursion


            // Update Tag EBML data-size
            element.DataSize = i - element.DataOffset;
        }


        private void WriteTargets(EBMLElement element, Tag tag)
        {
            ulong i = element.DataOffset;

            // Write Targets content

            if (tag.TargetTypeValue > 0) 
            {
                var ebml_targetTypeValue = new EBMLElement(this, i, MatroskaID.TargetTypeValue, tag.TargetTypeValue);
                i += ebml_targetTypeValue.Size;
            }

            if (tag.TargetType != null)
            {
                var ebml_targetType = new EBMLElement(this, i, MatroskaID.TargetType, tag.TargetType);
                i += ebml_targetType.Size;
            }

            if (tag.TrackUID != null)
            {
                foreach (var value in tag.TrackUID)
                {
                    var ebml_targetUID = new EBMLElement(this, i, MatroskaID.TagTrackUID, value);
                    i += ebml_targetUID.Size;
                }
            }

            if (tag.EditionUID != null)
            {
                foreach (var value in tag.EditionUID)
                {
                    var ebml_targetUID = new EBMLElement(this, i, MatroskaID.TagEditionUID, value);
                    i += ebml_targetUID.Size;
                }
            }

            if (tag.ChapterUID != null)
            {
                foreach (var value in tag.ChapterUID)
                {
                    var ebml_targetUID = new EBMLElement(this, i, MatroskaID.TagChapterUID, value);
                    i += ebml_targetUID.Size;
                }
            }

            if (tag.AttachmentUID != null)
            {
                foreach (var value in tag.AttachmentUID)
                {
                    var ebml_targetUID = new EBMLElement(this, i, MatroskaID.TagAttachmentUID, value);
                    i += ebml_targetUID.Size;
                }
            }

            // Update Tag EBML data-size
            element.DataSize = i - element.DataOffset;
        }



        private long ReadWriteAttachments(EBMLElement element)
        {
            long ret = 0;
            ulong i = 0;

            while (i < (ulong)((long)element.DataSize + ret))
            {
                EBMLElement child = new EBMLElement(this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID)child.ID;

                switch (matroska_id)
                {
                    case MatroskaID.AttachedFile:
                        ReadAttachedFile(child);
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            return ret;
        }

        private void ReadAttachedFile(EBMLElement element)
        {
            ulong i = 0;
#pragma warning disable 219 // Assigned, never read
            string file_name = null, file_mime = null, file_desc = null;
            ByteVector file_data = null;
#pragma warning restore 219

            while (i < element.DataSize)
            {
                EBMLElement child = new EBMLElement(this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID)child.ID;

                switch (matroska_id)
                {
                    case MatroskaID.FileName:
                        file_name = child.ReadString();
                        break;
                    case MatroskaID.FileMimeType:
                        file_mime = child.ReadString();
                        break;
                    case MatroskaID.FileDescription:
                        file_desc = child.ReadString();
                        break;
                    case MatroskaID.FileData:
                        file_data = child.ReadBytes();
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            if (file_mime != null && file_name!=null && file_data!=null && file_mime.StartsWith("image/"))
            {
                List<IPicture> pictures = tags.Pictures.ToList();

                var pic = new Picture(file_data);
                pic.Description = file_name;
                pic.MimeType = file_mime;
                //pic.Description = file_desc;

                // Set picture type from its name
                string fname = file_name.ToLower(); 
                pic.Type = PictureType.Other;
                foreach (var ptype in Enum.GetNames(typeof(PictureType)))
                {
                    if (fname.Contains(ptype.ToLower()))
                    {
                        pic.Type = (PictureType) Enum.Parse(typeof(PictureType), ptype);
                        break;
                    }
                }
                if(pic.Type == PictureType.Other && (fname.Contains("cover") || fname.Contains("poster")))
                {
                    pic.Type = PictureType.FrontCover;
                }

                pictures.Add(pic);
                tags.Pictures = pictures.ToArray();
            }

        }

        private long ReadWriteSegmentInfo(EBMLElement element)
        {
            long ret = 0;
            ulong i = 0;

            string tag_string = null;
            EBMLElement ebml_string = null;


            while (i < (ulong)((long)element.DataSize + ret))
            {
                EBMLElement child = new EBMLElement(this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID)child.ID;

                switch (matroska_id)
                {
                    case MatroskaID.Duration:
                        duration_unscaled = (UInt64)child.ReadDouble();
                        if (time_scale > 0)
                        {
                            duration = TimeSpan.FromSeconds(duration_unscaled * time_scale / 1000000000);
                        }
                        break;
                    case MatroskaID.TimeCodeScale:
                        time_scale = child.ReadULong();
                        if (duration_unscaled > 0)
                        {
                            duration = TimeSpan.FromSeconds(duration_unscaled * time_scale / 1000000000);
                        }
                        break;
                    case MatroskaID.Title:
                        ebml_string = child;
                        tag_string = child.ReadString();
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            if (Mode == AccessMode.Write)
            {
                // Write SegmentInfo Title
                string title = tags.Title;
                if (title != null)
                {
                    if (ebml_string == null)
                    {
                        // Create the missing EBML string at the end for the current element
                        ebml_string = new EBMLElement(this, element.Offset + element.Size, MatroskaID.Title, title);
                        ret += (long)ebml_string.Size;
                    }
                    else if (tag_string != title)
                    {
                        // Replace existing string inside the EBML string
                        ret += ebml_string.WriteData(title);
                    }
                }
                else if (ebml_string != null)
                {
                    ret += ebml_string.Remove();
                }

                // Update Element EBML data-size
                ret = element.ResizeData(ret);
            }
            else
            {
                tags.Title = tag_string;
            }

            return ret;
        }

        private void ReadTracks (EBMLElement element)
        {
            ulong i = 0;

            while (i < element.DataSize) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;

                switch (matroska_id) {
                    case MatroskaID.TrackEntry:
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
                    case MatroskaID.TrackType: {
                            TrackType track_type = (TrackType) child.ReadULong ();

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
