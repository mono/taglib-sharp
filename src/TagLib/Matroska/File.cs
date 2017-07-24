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
        private Matroska.Tag tag;

        /// <summary>
        ///    Contains the media properties.
        /// </summary>
        private Properties properties;

        private ulong duration_unscaled;
        private ulong time_scale;
        private TimeSpan duration;

        private List<Track> tracks = new List<Track> ();

        /// <summary>
        /// Contains the tags to be written (still)
        /// </summary>
        private Dictionary<string, List<string>> writeTags;

        private bool removeTags;

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
            tag = new Matroska.Tag(this);

            removeTags = false;
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
                // Create a list of tags to create by cloning Dictionary / list
                writeTags = new Dictionary<string, List<string>>(tag.Custom.Count);
                foreach (var tag in tag.Custom)
                {
                    // Make sure tag ID is in upper-case
                    writeTags[tag.Key.ToUpper()] = new List<string>(tag.Value);
                }
                ReadWrite(ReadStyle.None);
                removeTags = false;
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
                tag.Clear();
                removeTags = true; // enable erasing all tags
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
            if (type == TagTypes.Matroska) ret = tag;
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
        /// Add a Tag
        /// </summary>
        /// <param name="key">Tag name</param>
        /// <param name="value">Tag value</param>
        private void TagPush(string key, string value)
        {
            List<string> list = null;

            if(tag.Custom.ContainsKey(key))
                list = tag.Custom[key];

            if (list == null)
                tag.Custom[key] = list = new List<string>(1);

            list.Add(value);
        }

        /// <summary>
        /// Get and remove a Tag from the list of tags to be written.
        /// </summary>
        /// <param name="key">Tag name,  or take first element if null (default)</param>
        /// <returns>Tag value</returns>
        private string TagPop(string key)
        {
            string ret = null;
            List<string> list;

            if (writeTags.TryGetValue(key, out list))
            {
                if (list != null)
                {
                    if (list.Count > 0)
                    {
                        ret = list[0];
                        list.RemoveAt(0);
                    }

                    if (list.Count == 0)
                    {
                        writeTags.Remove(key);
                    }
                }
            }

            return ret;
        }

        private string TagPop(out string key)
        {
            string ret;
            if (writeTags.Count == 0)
            {
                key = null;
                ret = null;
            }
            else
            {
                key = writeTags.First().Key;
                ret = TagPop(key);
            }
            return ret;
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
                    case MatroskaID.MatroskaSegment:
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
                    case MatroskaID.MatroskaSeekHead:
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
                    case MatroskaID.MatroskaTracks:
                        if(propertiesStyle != ReadStyle.None) ReadTracks (child);
                        break;
                    case MatroskaID.MatroskaSegmentInfo:
                        ret += ReadWriteSegmentInfo(child);
                        break;
                    case MatroskaID.MatroskaTags:
                        if (Mode == AccessMode.Write && removeTags)
                        {
                            ret += child.Remove(); // Remove all Tags
                            continue;
                        }
                        ret += ReadWriteTags(child);
                        break;
                    case MatroskaID.MatroskaAttachments:
                        ret += ReadWriteAttachments(child);
                        break;
                    case MatroskaID.MatroskaCluster:
                        break;
                    case MatroskaID.MatroskaVoid:
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

                // Create EBML Tags
                if (writeTags != null && writeTags.Count > 0)
                {
                    var ebml_tags = new EBMLElement(this, i, MatroskaID.MatroskaTags);
                    ReadWriteTags(ebml_tags);
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
            var element = new EBMLElement(this, position, MatroskaID.MatroskaSeekHead);
            position = element.DataOffset;


            // Create the Seek Entries
            foreach (var segm in segm_list)
            {
                ulong i = position;

                var seekEntry = new EBMLElement(this, i, MatroskaID.MatroskaSeek);
                i += seekEntry.Size;

                var seekId = new EBMLElement(this, i, MatroskaID.MatroskaSeekID);
                seekId.WriteData(segm.ID);
                i += seekId.Size;

                var seekPosition = new EBMLElement(this, i, MatroskaID.MatroskaSeekPosition);
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

            var element = new EBMLElement(this, position, MatroskaID.MatroskaVoid);
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
                    case MatroskaID.MatroskaSeekID:
                        ebml_id = child;
                        break;
                    case MatroskaID.MatroskaSeekPosition:
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




        private long ReadWriteTags (EBMLElement element)
        {
            long ret = 0;
            ulong i = 0;

            while (i < (ulong)((long)element.DataSize + ret)) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;

                switch (matroska_id) {
                    case MatroskaID.MatroskaTag:
                        ret += ReadWriteTag(child);
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            if (Mode == AccessMode.Write)
            {
                // Create new EBML Tag
                if(writeTags != null && writeTags.Count>0)
                {
                    i += element.DataOffset;

                    // Create new EBML Tag
                    var ebml_tag = new EBMLElement(this, i, MatroskaID.MatroskaTag);
                    i += ebml_tag.Size;

                    // Insert mandatory EBML Targets (empty: default, i.e. 50)
                    var ebml_targets = new EBMLElement(this, i, MatroskaID.MatroskaTargets, "");
                    i += ebml_targets.Size;

                    ebml_tag.DataSize = ebml_targets.Size;
                    ReadWriteTag(ebml_tag);
                    ret += (long)ebml_tag.Size;
                }

                // Update Element EBML data-size
                ret = element.ResizeData(ret);
            }

            return ret;
        }


        private long ReadWriteTag (EBMLElement element)
        {
            long ret = 0;
            ulong i = 0;
            bool thisMedium = true;

            // TODO: Handle all TargetTypes properly

            while (i < (ulong)((long)element.DataSize + ret)) {
                EBMLElement child = new EBMLElement (this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;

                switch (matroska_id) {
                    case MatroskaID.MatroskaTargets:
                        thisMedium = ReadTargets(child);
                        // Read-Write global tags only. We assume that the Targets *always* preeceeds the SimpleTag
                        if (!thisMedium) return ret; // Skip this Tag
                        break;
                    case MatroskaID.MatroskaSimpleTag:
                        ret += ReadWriteSimpleTag(child);
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            if (Mode == AccessMode.Write && thisMedium)
            {
                // Add remaining Simple tags to write
                if (writeTags != null && writeTags.Count > 0)
                {
                    i += element.DataOffset;

                    string value, key;
                    while ((value = TagPop(out key)) != null) {

                        var ebml_Simpletag = new EBMLElement(this, i, MatroskaID.MatroskaSimpleTag);
                        i += ebml_Simpletag.Size;

                        var ebml_tagName = new EBMLElement(this, i, MatroskaID.MatroskaTagName, key);
                        i += ebml_tagName.Size;
                        var ebml_tagLanguage = new EBMLElement(this, i, MatroskaID.MatroskaTagLanguage, "und");
                        i += ebml_tagLanguage.Size;
                        var ebml_tagDefault = new EBMLElement(this, i, MatroskaID.MatroskaTagDefault, (ulong)1);
                        i += ebml_tagDefault.Size;
                        var ebml_tagString = new EBMLElement(this, i, MatroskaID.MatroskaTagString, value);
                        i += ebml_tagString.Size;

                        ebml_Simpletag.DataSize = ebml_tagName.Size + ebml_tagString.Size + ebml_tagLanguage.Size + ebml_tagDefault.Size;
                        i = ebml_Simpletag.Offset + ebml_Simpletag.Size;
                        ret += (long)ebml_Simpletag.Size;
                    }

                    writeTags = null;
                }

                // Update Element EBML data-size
                ret = element.ResizeData(ret);
            }

            return ret;
        }

        private long ReadWriteSimpleTag (EBMLElement element)
        {
            long ret = 0;
            ulong i = 0;
#pragma warning disable 219 // Assigned, never read
            string tag_name = null, tag_language = null, tag_string = null;
#pragma warning restore 219
            bool isBinary = false;

            // For overwritting EBML string/binary (mutex)
            EBMLElement ebml_string = null;

            while (i < (ulong)((long)element.DataSize + ret)) {
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
                        ebml_string = child;
                        tag_string = child.ReadString ();
                        break;
                    case MatroskaID.MatroskaTagBinary:
                        if(ebml_string==null) ebml_string = child;
                        isBinary = true;
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            if (Mode == AccessMode.Write)
            {
                string val;
                if ((val = TagPop(tag_name)) != null) {

                    if (ebml_string == null)
                    {
                        // Create the missing EBML string at the end for the current element
                        ebml_string = new EBMLElement(this, element.Offset + element.Size, MatroskaID.MatroskaTagString, val);
                        ret += (long)ebml_string.Size;
                    }
                    else if (tag_string != val)
                    {
                        // Replace existing string inside the EBML string
                        if(isBinary) ebml_string.ID = (uint)MatroskaID.MatroskaTagString;
                        ret += ebml_string.WriteData(val);
                    }

                    // Update Element EBML data-size
                    ret = element.ResizeData(ret);
                }
                else if (!isBinary) // Preserve Binary Tag
                {
                    // Remove EBML Tag
                    ret += element.Remove();
                }
            }
            else if (!isBinary) // Binary Tag not handled
            {
                TagPush(tag_name, tag_string);
            }

            return ret;
        }

        private bool ReadTargets(EBMLElement element)
        {
            bool subselection = false;
            bool ret = false;
            uint target = 0;
            ulong i = 0;

            while (i < element.DataSize)
            {
                EBMLElement child = new EBMLElement(this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID)child.ID;

                switch (matroska_id)
                {
                    case MatroskaID.MatroskaTargetTypeValue:
                        target = (uint)child.ReadULong();
                        break;
                    case MatroskaID.MatroskaTagTrackUID:
                    case MatroskaID.MatroskaTagEditionUID:
                    case MatroskaID.MatroskaTagChapterUID:
                    case MatroskaID.MatroskaTagAttachmentUID:
                        subselection = true;
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            // Lower level not targeting subselection is the Tag representing the file
            if (!subselection)
            {
                if (Mode == AccessMode.Write)
                {
                    ret = target == tag.TargetType;
                }
                else if(target < tag.TargetType || tag.TargetType == 0)
                {
                    tag.TargetType = target;
                    ret = true;
                }
            }

            return ret;
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
                    case MatroskaID.MatroskaAttachedFile:
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
                    case MatroskaID.MatroskaFileName:
                        file_name = child.ReadString();
                        break;
                    case MatroskaID.MatroskaFileMimeType:
                        file_mime = child.ReadString();
                        break;
                    case MatroskaID.MatroskaFileDescription:
                        file_desc = child.ReadString();
                        break;
                    case MatroskaID.MatroskaFileData:
                        file_data = child.ReadBytes();
                        break;
                    default:
                        break;
                }

                i += child.Size;
            }

            if (file_mime != null && file_name!=null && file_data!=null && file_mime.StartsWith("image/"))
            {
                List<IPicture> pictures = tag.Pictures.ToList();

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
                tag.Pictures = pictures.ToArray();
            }

        }

        private long ReadWriteSegmentInfo(EBMLElement element)
        {
            long ret = 0;
            ulong i = 0;

            const string tag_name = "TITLE";
            string tag_string = null;
            EBMLElement ebml_string = null;


            while (i < (ulong)((long)element.DataSize + ret))
            {
                EBMLElement child = new EBMLElement(this, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID)child.ID;

                switch (matroska_id)
                {
                    case MatroskaID.MatroskaDuration:
                        duration_unscaled = (UInt64)child.ReadDouble();
                        if (time_scale > 0)
                        {
                            duration = TimeSpan.FromSeconds(duration_unscaled * time_scale / 1000000000);
                        }
                        break;
                    case MatroskaID.MatroskaTimeCodeScale:
                        time_scale = child.ReadULong();
                        if (duration_unscaled > 0)
                        {
                            duration = TimeSpan.FromSeconds(duration_unscaled * time_scale / 1000000000);
                        }
                        break;
                    case MatroskaID.MatroskaTitle:
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
                string val;
                if ((val = TagPop(tag_name)) != null)
                {
                    if (ebml_string == null)
                    {
                        // Create the missing EBML string at the end for the current element
                        ebml_string = new EBMLElement(this, element.Offset + element.Size, MatroskaID.MatroskaTitle, val);
                        ret += (long)ebml_string.Size;
                    }
                    else if (tag_string != val)
                    {
                        // Replace existing string inside the EBML string
                        ret += ebml_string.WriteData(val);
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
                if (tag_string != null) TagPush(tag_name, tag_string);
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
