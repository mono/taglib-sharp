//
// MatroskaIDs.cs:
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

namespace TagLib.Matroska
{
    public enum MatroskaID
    {
        MatroskaSegment = 0x18538067,

        MatroskaSegmentInfo = 0x1549A966,

        MatroskaTracks = 0x1654AE6B,

        MatroskaCues = 0x1C53BB6B,

        MatroskaTags = 0x1254C367,

        MatroskaSeekHead = 0x114D9B74,

        MatroskaCluster = 0x1F43B675,

        MatroskaAttachments = 0x1941A469,

        MatroskaChapters = 0x1043A770,

        /* IDs in the SegmentInfo master */
        MatroskaTimeCodeScale = 0x2AD7B1,

        MatroskaDuration = 0x4489,

        MatroskaWrittingApp = 0x5741,

        MatroskaMuxingApp = 0x4D80,

        MatroskaDateUTC = 0x4461,

        MatroskaSegmentUID = 0x73A4,

        MatroskaSegmentFileName = 0x7384,

        MatroskaPrevUID = 0x3CB923,

        MatroskaPrevFileName = 0x3C83AB,

        MatroskaNexUID = 0x3EB923,

        MatroskaNexFileName = 0x3E83BB,

        MatroskaTitle = 0x7BA9,

        MatroskaSegmentFamily = 0x4444,

        MatroskaChapterTranslate = 0x6924,

        /* ID in the Tracks master */
        MatroskaTrackEntry = 0xAE,

        /* IDs in the TrackEntry master */
        MatroskaTrackNumber = 0xD7,

        MatroskaTrackUID = 0x73C5,

        MatroskaTrackType = 0x83,

        MatroskaTrackAudio = 0xE1,

        MatroskaTrackVideo = 0xE0,

        MatroskaContentEncodings = 0x6D80,

        MatroskaCodecID = 0x86,

        MatroskaCodecPrivate = 0x63A2,

        MatroskaCodecName = 0x258688,

        MatroskaTrackName = 0x536E,

        MatroskaTrackLanguage = 0x22B59C,

        MatroskaTrackFlagEnabled = 0xB9,

        MatroskaTrackFlagDefault = 0x88,

        MatroskaTrackFlagForced = 0x55AA,

        MatroskaTrackFlagLacing = 0x9C,

        MatroskaTrackMinCache = 0x6DE7,

        MatroskaTrackMaxCache = 0x6DF8,

        MatroskaTrackDefaultDuration = 0x23E383,

        MatroskaTrackTimeCodeScale = 0x23314F,

        MatroskaMaxBlockAdditionID = 0x55EE,

        MatroskaTrackAttachmentLink = 0x7446,

        MatroskaTrackOverlay = 0x6FAB,

        MatroskaTrackTranslate = 0x6624,

        /* semi-draft */
        MatroskaTrackOffset = 0x537F,

        /* semi-draft */
        MatroskaCodecSettings = 0x3A9697,

        /* semi-draft */
        MatroskaCodecInfoUrl = 0x3B4040,

        /* semi-draft */
        MatroskaCodecDownloadUrl = 0x26B240,

        /* semi-draft */
        MatroskaCodecDecodeAll = 0xAA,

        /* IDs in the TrackVideo master */
        /* NOTE: This one is here only for backward compatibility.
        * Use _TRACKDEFAULDURATION */
        MatroskaVideoFrameRate = 0x2383E3,

        MatroskaVideoDisplayWidth = 0x54B0,

        MatroskaVideoDisplayHeight = 0x54BA,

        MatroskaVideoDisplayUnit = 0x54B2,

        MatroskaVideoPixelWidth = 0xB0,

        MatroskaVideoPixelHeight = 0xBA,

        MatroskaVideoPixelCropBottom = 0x54AA,

        MatroskaVideoPixelCropTop = 0x54BB,

        MatroskaVideoPixelCropLeft = 0x54CC,

        MatroskaVideoPixelCropRight = 0x54DD,

        MatroskaVideoFlagInterlaced = 0x9A,

        /* semi-draft */
        MatroskaVideoStereoMode = 0x53B8,

        MatroskaVideoAspectRatioType = 0x54B3,

        MatroskaVideoColourSpace = 0x2EB524,

        /* semi-draft */
        MatroskaVideoGammaValue = 0x2FB523,

        /* IDs in the TrackAudio master */
        MatroskaAudioSamplingFreq = 0xB5,

        MatroskaAudioBitDepth = 0x6264,

        MatroskaAudioChannels = 0x9F,

        /* semi-draft */
        MatroskaAudioChannelsPositions = 0x7D7B,

        MatroskaAudioOutputSamplingFreq = 0x78B5,

        /* IDs in the Tags master */
        MatroskaTag = 0x7373,

        /* in the Tag master */
        MatroskaSimpleTag = 0x67C8,

        MatroskaTargets = 0x63C0,

        /* in the SimpleTag master */
        MatroskaTagName = 0x45A3,

        MatroskaTagString = 0x4487,

        MatroskaTagLanguage = 0x447A,

        MatroskaTagDefault = 0x4484,

        MatroskaTagBinary = 0x4485

    }
}
