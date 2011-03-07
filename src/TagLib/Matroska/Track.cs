//
// Track.cs:
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
    /// Describes a Matroska Track.
    /// </summary>
    public class Track : ICodec
    {
        #region Private fields

#pragma warning disable 414 // Assigned, never used
        private uint track_number;
        private uint track_uid;
        private string track_codec_id;
        private string track_codec_name;
        private string track_name;
        private string track_language;
        private bool track_enabled;
        private bool track_default;
        private ByteVector codec_data;
#pragma warning restore 414

        private List<EBMLElement> unknown_elems = new List<EBMLElement> ();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a <see cref="Track" /> parsing from provided 
        /// file data.
        /// Parsing will be done reading from _file at position references by 
        /// parent element's data section.
        /// </summary>
        /// <param name="_file"><see cref="File" /> instance to read from.</param>
        /// <param name="element">Parent <see cref="EBMLElement" />.</param>
        public Track (File _file, EBMLElement element)
        {
            ulong i = 0;

            while (i < element.DataSize) {
                EBMLElement child = new EBMLElement (_file, element.DataOffset + i);

                MatroskaID matroska_id = (MatroskaID) child.ID;

                switch (matroska_id) {
                    case MatroskaID.MatroskaTrackNumber:
                        track_number = child.ReadUInt ();
                        break;
                    case MatroskaID.MatroskaTrackUID:
                        track_uid = child.ReadUInt ();
                        break;
                    case MatroskaID.MatroskaCodecID:
                        track_codec_id = child.ReadString ();
                        break;
                    case MatroskaID.MatroskaCodecName:
                        track_codec_name = child.ReadString ();
                        break;
                    case MatroskaID.MatroskaTrackName:
                        track_name = child.ReadString ();
                        break;
                    case MatroskaID.MatroskaTrackLanguage:
                        track_language = child.ReadString ();
                        break;
                    case MatroskaID.MatroskaTrackFlagEnabled:
                        track_enabled = child.ReadBool ();
                        break;
                    case MatroskaID.MatroskaTrackFlagDefault:
                        track_default = child.ReadBool ();
                        break;
                    case MatroskaID.MatroskaCodecPrivate:
                        codec_data = child.ReadBytes ();
                        break;
                    default:
                        unknown_elems.Add (child);
                        break;
                }

                i += child.Size;
            }
        }

        #endregion

        #region Public fields

        /// <summary>
        /// List of unknown elements encountered while parsing.
        /// </summary>
        public List<EBMLElement> UnknownElements
        {
            get { return unknown_elems; }
        }

        #endregion

        #region Public methods

        #endregion

        #region ICodec

        /// <summary>
        /// Describes track duration.
        /// </summary>
        public virtual TimeSpan Duration
        {
            get { return TimeSpan.Zero; }
        }

        /// <summary>
        /// Describes track media types.
        /// </summary>
        public virtual MediaTypes MediaTypes
        {
            get { return MediaTypes.None; }
        }

        /// <summary>
        /// Track description.
        /// </summary>
        public virtual string Description
        {
            get { return String.Format ("{0} {1}", track_codec_name, track_language); }
        }

        #endregion
    }
}
