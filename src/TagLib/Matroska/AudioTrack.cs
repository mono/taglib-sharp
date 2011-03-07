//
// AudioTrack.cs:
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
    /// Describes a Matroska Audio track.
    /// </summary>
    public class AudioTrack : Track, IAudioCodec
    {
        #region Private fields

#pragma warning disable 414 // Assigned, never used
        private double rate;
        private uint channels;
        private uint depth;
#pragma warning restore 414

        private List<EBMLElement> unknown_elems = new List<EBMLElement> ();

        #endregion

        #region Constructors

        /// <summary>
        ///  Construct a <see cref="AudioTrack" /> reading information from 
        ///  provided file data.
        /// Parsing will be done reading from _file at position references by 
        /// parent element's data section.
        /// </summary>
        /// <param name="_file"><see cref="File" /> instance to read from.</param>
        /// <param name="element">Parent <see cref="EBMLElement" />.</param>
        public AudioTrack (File _file, EBMLElement element)
            : base (_file, element)
        {
            MatroskaID matroska_id;

            // Here we handle the unknown elements we know, and store the rest
            foreach (EBMLElement elem in base.UnknownElements) {

                matroska_id = (MatroskaID) elem.ID;

                if (matroska_id == MatroskaID.MatroskaTrackAudio) {
                    ulong i = 0;

                    while (i < elem.DataSize) {
                        EBMLElement child = new EBMLElement (_file, elem.DataOffset + i);

                        matroska_id = (MatroskaID) child.ID;

                        switch (matroska_id) {
                            case MatroskaID.MatroskaAudioChannels:
                                channels = child.ReadUInt ();
                                break;
                            case MatroskaID.MatroskaAudioBitDepth:
                                depth = child.ReadUInt ();
                                break;
                            case MatroskaID.MatroskaAudioSamplingFreq:
                                rate = child.ReadDouble ();
                                break;
                            default:
                                unknown_elems.Add (child);
                                break;
                        }

                        i += child.Size;
                    }
                }
                else {
                    unknown_elems.Add (elem);
                }
            }
        }

        #endregion

        #region Public fields

        /// <summary>
        /// List of unknown elements encountered while parsing.
        /// </summary>
        public new List<EBMLElement> UnknownElements
        {
            get { return unknown_elems; }
        }

        #endregion

        #region Public methods

        #endregion

        #region ICodec

        /// <summary>
        /// This type of track only has audio media type.
        /// </summary>
        public override MediaTypes MediaTypes
        {
            get { return MediaTypes.Audio; }
        }

        #endregion

        #region IAudioCodec

        /// <summary>
        /// Audio track bitrate.
        /// </summary>
        public int AudioBitrate
        {
            get { return 0; }
        }

        /// <summary>
        /// Audio track sampling rate.
        /// </summary>
        public int AudioSampleRate
        {
            get { return (int) rate; }
        }

        /// <summary>
        /// Number of audio channels in this track.
        /// </summary>
        public int AudioChannels
        {
            get { return (int) channels; }
        }

        #endregion
    }
}
