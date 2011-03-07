//
// VideoTrack.cs:
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
    public enum VideoAspectRatioType
    {
        AspectRatioModeFree = 0x0,
        AspectRatioModeKeep = 0x1,
        AspectRatioModeFixed = 0x2
    }

    public class VideoTrack : Track, IVideoCodec
    {
        #region Private fields

        private uint width;
        private uint height;
        private uint disp_width;
        private uint disp_height;
        private double framerate;
        private bool interlaced;
        private VideoAspectRatioType ratio_type;
        private ByteVector fourcc;

        private List<EBMLElement> unknown_elems = new List<EBMLElement> ();

        #endregion

        #region Constructors

        public VideoTrack (File _file, EBMLElement element)
            : base (_file, element)
        {
            MatroskaID matroska_id;

            // Here we handle the unknown elements we know, and store the rest
            foreach (EBMLElement elem in base.UnknownElements) {
                matroska_id = (MatroskaID) elem.ID;

                if (matroska_id == MatroskaID.MatroskaTrackVideo) {
                    ulong i = 0;

                    while (i < elem.DataSize) {
                        EBMLElement child = new EBMLElement (_file, elem.DataOffset + i);

                        matroska_id = (MatroskaID) child.ID;

                        switch (matroska_id) {
                            case MatroskaID.MatroskaVideoDisplayWidth:
                                disp_width = child.ReadUInt ();
                                break;
                            case MatroskaID.MatroskaVideoDisplayHeight:
                                disp_height = child.ReadUInt ();
                                break;
                            case MatroskaID.MatroskaVideoPixelWidth:
                                width = child.ReadUInt ();
                                break;
                            case MatroskaID.MatroskaVideoPixelHeight:
                                height = child.ReadUInt ();
                                break;
                            case MatroskaID.MatroskaVideoFrameRate:
                                framerate = child.ReadDouble ();
                                break;
                            case MatroskaID.MatroskaVideoFlagInterlaced:
                                interlaced = child.ReadBool ();
                                break;
                            case MatroskaID.MatroskaVideoAspectRatioType:
                                ratio_type = (VideoAspectRatioType) child.ReadUInt ();
                                break;
                            case MatroskaID.MatroskaVideoColourSpace:
                                fourcc = child.ReadBytes ();
                                break;
                            default:
                                unknown_elems.Add (child);
                                break;
                        }

                        i += child.Size;
                    }
                }
                else if (matroska_id == MatroskaID.MatroskaTrackDefaultDuration) {
                    uint tmp = elem.ReadUInt ();
                    framerate = 1000000000.0 / (double) tmp;
                }
                else {
                    unknown_elems.Add (elem);
                }
            }
        }

        #endregion

        #region Public fields

        public new List<EBMLElement> UnknownElements
        {
            get { return unknown_elems; }
        }

        #endregion

        #region Public methods

        #endregion

        #region ICodec

        public override MediaTypes MediaTypes
        {
            get { return MediaTypes.Video; }
        }

        #endregion

        #region IVideoCodec

        public int VideoWidth
        {
            get { return (int) width; }
        }

        public int VideoHeight
        {
            get { return (int) height; }
        }

        #endregion
    }
}
