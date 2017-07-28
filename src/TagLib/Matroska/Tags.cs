//
// Tags.cs:
//
// Author:
//   Sebastien Mouy <starwer@laposte.net>
//
// Copyright (C) 2017 Starwer
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
using System.Collections.ObjectModel;

namespace TagLib.Matroska
{
    /// <summary>
    /// Describes all the Matroska Tags in a file as a list, ordered from higher TargetTypeValue to lower. 
    /// A <see cref="Tags"/> object contains several <see cref="Tag"/>
    /// </summary>
    public class Tags : Collection<Tag>
    {
        #region Private fields/Properties

        // Cross reference to file is required to retrieve whether this is a video/audio content
        private File _file;

        // Store the pictures
        private IPicture[] pictures = new Picture[0];

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">the file it belongs to.</param>
        public Tags(File file)
        {
            _file = file;
        }


        /// <summary>
        /// Shallow Copy Constructor
        /// </summary>
        /// <param name="tags">the Tags object to duplicate.</param>
        public Tags(Tags tags) : this(tags._file)
        {
            Title = tags.Title;

            // Duplicate Tags
            foreach (Tag tag in tags)
            {
                var ntag = new Tag(tag, this);
            }

            // Duplicate picture (shallow copy)
            pictures = new Picture[tags.Pictures.Length];
            for (int i = 0; i < pictures.Length; i++)
            {
                pictures[i] = tags.Pictures[i];
            }
        }



        #endregion


        #region Override Collection, to keep the items ordered

        /// <summary>
        /// Try to Insert an element to the Tag list at a given index, but can insert it at another index if the 
        /// index doesn't keep this list sorted by descending TargetTypeValue
        /// </summary>
        /// <param name="index">index at which the Tag element should be preferably inserted</param>
        /// <param name="tag">Tag element to be inserted t the Tag list</param>
        protected override void InsertItem(int index, Tag tag)
        {
            if (tag == null) throw new ArgumentNullException("Can't add a null Matroska.Tag to a Matroska.Tags object");

            // Remove duplicate
            for (int j = 0; j < this.Count; j++)
            {
                if (this[j] == tag)
                {
                    RemoveAt(j);
                    break;
                }
            }

            if (index < 0 || index >= this.Count || this[index].TargetTypeValue < tag.TargetTypeValue || (index + 1 < this.Count && this[index + 1].TargetTypeValue > tag.TargetTypeValue))
            {
                for (index = this.Count - 1; index >= 0; index--)
                {
                    if (this[index].TargetTypeValue >= tag.TargetTypeValue)
                        break;
                }

                index++;
            }

            base.InsertItem(index, tag);

        }

        /// <summary>
        /// Replace a tag in the list.
        /// </summary>
        /// <param name="index">Index of the lement to be replaced</param>
        /// <param name="tag">tag to replace the older one</param>
        protected override void SetItem(int index, Tag tag)
        {
            RemoveItem(index);
            InsertItem(index, tag);
        }

        /// <summary>
        /// Remove a Tag from the Tags list
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }

        /// <summary>
        /// Clears the values stored in the current Tags and Children.
        /// </summary>
        protected override void ClearItems()
        {
            Title = null;
            var medium = Medium;

            foreach (var tag in this)
            {
                tag.Clear();
            }

            base.ClearItems();

            // Keep Medium Tag reference unchanged
            Add(medium);
        }


        #endregion



        #region Methods

        /// <summary>
        /// Create a TargetType from a given TargetTypeValue, depending on the media-type
        /// </summary>
        /// <param name="targetTypeValue">TargetTypeValue to be converted to text</param>
        /// <returns>Textual description of the TargetTypeValue</returns>
        public string MakeTargetType(ushort targetTypeValue)
        {
            string ret = null;

            switch (targetTypeValue)
            {
                case 70: ret = "COLLECTION"; break;
                case 60: ret = IsVideo ? "SEASON" : "VOLUME"; break;
                case 50: ret = IsVideo ? "MOVIE" : "ALBUM"; break;
                case 40: ret = "PART"; break;
                case 30: ret = IsVideo ? "CHAPTER" : "TRACK"; break;
                case 20: ret = IsVideo ? "SCENE" : "MOVEMENT"; break;
                case 10: ret = IsVideo ? "SHOT" : null; break;
            }
            return ret;
        }


        /// <summary>
        /// Find first Tag of a given given TargetTypeValue
        /// </summary>
        /// <param name="targetTypeValue">TargetTypeValue to find</param>
        /// <param name="medium">null: any kind, true: represent the current medium, false: represent a sub-element</param>
        /// <returns>the Tag if match found, null otherwise</returns>
        public Tag Get(ushort targetTypeValue, bool? medium = true)
        {
            Tag ret = null;
            int i;

            // Find first match of the given targetValue
            // List is sorted in descending TargetTypeValue
            for (i = this.Count - 1; i >= 0; i--)
            {
                if (targetTypeValue == this[i].TargetTypeValue)
                {
                    ret = this[i];
                    if (medium != null)
                    {
                        bool isMedium = (ret.TrackUID == null && ret.EditionUID == null && ret.ChapterUID == null && ret.AttachmentUID == null);
                        if (medium == isMedium) break;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return i >= 0 ? ret : null;
        }


        #endregion


        #region Properties


        /// <summary>
        /// Define if this represent a video content (true), or an audio content (false)
        /// </summary>
        public bool IsVideo
        {
            get { return _file == null || _file.MimeType != "taglib/mka"; }
        }


        /// <summary>
        /// Title of the medium, from the Segment
        /// </summary>
        public string Title { get; set; }


        
        /// <summary>
        /// Get/set the Tag that represents the current medium (file)
        /// </summary>
        public Tag Medium
        {
            get
            {
                Tag ret = null;
                bool vid = IsVideo;

                // Lower level without UID is the Tag representing the file
                // List is sorted in descending TargetTypeValue
                for (int i = this.Count - 1; i >= 0; i--)
                {
                    ret = this[i];
                    if (ret.TargetTypeValue != 40 || !vid) // Avoid CD/DVD
                    {
                        if (ret.TrackUID == null && ret.EditionUID == null && ret.ChapterUID == null && ret.AttachmentUID == null) break;
                    }
                }
                

                return ret;
            }
        }

        /// <summary>
        /// Get/set the Tag that represents the Collection the current medium (file) belongs to.
        /// For Audio, this should be an Album, type 50 (itself if the mka file represents an album).
        /// For Video, this should be a Collection, type 70.
        /// </summary>
        public Tag Album
        {
            get
            {
                ushort targetValue = IsVideo ? (ushort)70 : (ushort)50;
                return Get(targetValue, true);
            }
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
        public IPicture [] Pictures
        {
            get
            {
                return pictures;
            }
            set
            {
                if (value==null && pictures.Length>0)
                {
                    pictures = new Picture[0];
                }
                else
                {
                    pictures = value;
                }
            }
        }

        #endregion
    }
}
