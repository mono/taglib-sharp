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
    /// <summary>
    /// Public enumeration listing Matroska specific EBML Identifiers.
    /// </summary>
    public enum MatroskaID
    {
        /// <summary>
        /// Indicates a Matroska Segment EBML element.
        /// </summary>
        MatroskaSegment = 0x18538067,

        /// <summary>
        /// Indicates a Matroska Segment Info EBML element.
        /// </summary>
        MatroskaSegmentInfo = 0x1549A966,

        /// <summary>
        /// Indicates a Matroska Tracks EBML Element.
        /// </summary>
        MatroskaTracks = 0x1654AE6B,

        /// <summary>
        /// Indicates a Matroska Cues EBML element.
        /// </summary>
        MatroskaCues = 0x1C53BB6B,

        /// <summary>
        /// Indicates a Matroska Tags EBML element.
        /// </summary>
        MatroskaTags = 0x1254C367,

        /// <summary>
        /// Indicates a Matroska Seek Head EBML element.
        /// </summary>
        MatroskaSeekHead = 0x114D9B74,

        /// <summary>
        /// Indicates a Matroska Cluster EBML element.
        /// </summary>
        MatroskaCluster = 0x1F43B675,

        /// <summary>
        /// Indicates a Matroska Attachments EBML element.
        /// </summary>
        MatroskaAttachments = 0x1941A469,

        /// <summary>
        /// Indicates a Matroska Chapters EBML element.
        /// </summary>
        MatroskaChapters = 0x1043A770,

        /* IDs in the SegmentInfo master */

        /// <summary>
        /// Indicate a Matroska Code Scale EBML element.
        /// </summary>
        MatroskaTimeCodeScale = 0x2AD7B1,

        /// <summary>
        /// Indicates a Matroska Duration EBML element.
        /// </summary>
        MatroskaDuration = 0x4489,

        /// <summary>
        /// Indicates a Matroska Writing App EBML element.
        /// </summary>
        MatroskaWrittingApp = 0x5741,

        /// <summary>
        /// Indicates a Matroska Muxing App EBML element.
        /// </summary>
        MatroskaMuxingApp = 0x4D80,

        /// <summary>
        /// Indicate a Matroska Date UTC EBML element.
        /// </summary>
        MatroskaDateUTC = 0x4461,

        /// <summary>
        /// Indicate a Matroska Segment UID EBML element.
        /// </summary>
        MatroskaSegmentUID = 0x73A4,

        /// <summary>
        /// Indicate a Matroska Segment File Name EBML element.
        /// </summary>
        MatroskaSegmentFileName = 0x7384,

        /// <summary>
        /// Indicate a Matroska Prev UID EBML element.
        /// </summary>
        MatroskaPrevUID = 0x3CB923,

        /// <summary>
        /// Indicate a Matroska Prev File Name EBML element.
        /// </summary>
        MatroskaPrevFileName = 0x3C83AB,

        /// <summary>
        /// Indicate a Matroska Nex UID EBML element.
        /// </summary>
        MatroskaNexUID = 0x3EB923,

        /// <summary>
        /// Indicate a Matroska Nex File Name EBML element.
        /// </summary>
        MatroskaNexFileName = 0x3E83BB,

        /// <summary>
        /// Indicate a Matroska Title EBML element.
        /// </summary>
        MatroskaTitle = 0x7BA9,

        /// <summary>
        /// Indicate a Matroska Segment Family EBML element.
        /// </summary>
        MatroskaSegmentFamily = 0x4444,

        /// <summary>
        /// Indicate a Matroska Chapter Translate EBML element.
        /// </summary>
        MatroskaChapterTranslate = 0x6924,

        /* ID in the Tracks master */

        /// <summary>
        /// Indicate a Matroska Track Entry EBML element.
        /// </summary>
        MatroskaTrackEntry = 0xAE,

        /* IDs in the TrackEntry master */

        /// <summary>
        /// Indicate a Matroska Track Number EBML element.
        /// </summary>
        MatroskaTrackNumber = 0xD7,

        /// <summary>
        /// Indicate a Matroska Track UID EBML element.
        /// </summary>
        MatroskaTrackUID = 0x73C5,

        /// <summary>
        /// Indicate a Matroska Track Type EBML element.
        /// </summary>
        MatroskaTrackType = 0x83,

        /// <summary>
        /// Indicate a Matroska Track Audio EBML element.
        /// </summary>
        MatroskaTrackAudio = 0xE1,

        /// <summary>
        /// Indicate a Matroska Track Video EBML element.
        /// </summary>
        MatroskaTrackVideo = 0xE0,

        /// <summary>
        /// Indicate a Matroska Track Encoding EBML element.
        /// </summary>
        MatroskaContentEncodings = 0x6D80,

        /// <summary>
        /// Indicate a Matroska Codec ID EBML element.
        /// </summary>
        MatroskaCodecID = 0x86,

        /// <summary>
        /// Indicate a Matroska Codec Private EBML element.
        /// </summary>
        MatroskaCodecPrivate = 0x63A2,

        /// <summary>
        /// Indicate a Matroska Codec Name EBML element.
        /// </summary>
        MatroskaCodecName = 0x258688,

        /// <summary>
        /// Indicate a Matroska Track Name EBML element.
        /// </summary>
        MatroskaTrackName = 0x536E,

        /// <summary>
        /// Indicate a Matroska Track Language EBML element.
        /// </summary>
        MatroskaTrackLanguage = 0x22B59C,

        /// <summary>
        /// Indicate a Matroska Track Enabled EBML element.
        /// </summary>
        MatroskaTrackFlagEnabled = 0xB9,

        /// <summary>
        /// Indicate a Matroska Track Flag Default EBML element.
        /// </summary>
        MatroskaTrackFlagDefault = 0x88,

        /// <summary>
        /// Indicate a Matroska Track Flag Forced EBML element.
        /// </summary>
        MatroskaTrackFlagForced = 0x55AA,

        /// <summary>
        /// Indicate a Matroska Track Flag Lacing EBML element.
        /// </summary>
        MatroskaTrackFlagLacing = 0x9C,

        /// <summary>
        /// Indicate a Matroska Track Min Cache EBML element.
        /// </summary>
        MatroskaTrackMinCache = 0x6DE7,

        /// <summary>
        /// Indicate a Matroska Track Max Cache EBML element.
        /// </summary>
        MatroskaTrackMaxCache = 0x6DF8,

        /// <summary>
        /// Indicate a Matroska Track Default Duration EBML element.
        /// </summary>
        MatroskaTrackDefaultDuration = 0x23E383,

        /// <summary>
        /// Indicate a Matroska Track Time Code Scale EBML element.
        /// </summary>
        MatroskaTrackTimeCodeScale = 0x23314F,

        /// <summary>
        /// Indicate a Matroska Track Max Block Addition EBML element.
        /// </summary>
        MatroskaMaxBlockAdditionID = 0x55EE,

        /// <summary>
        /// Indicate a Matroska Track Attachment Link EBML element.
        /// </summary>
        MatroskaTrackAttachmentLink = 0x7446,

        /// <summary>
        /// Indicate a Matroska Track Overlay EBML element.
        /// </summary>
        MatroskaTrackOverlay = 0x6FAB,

        /// <summary>
        /// Indicate a Matroska Track Translate EBML element.
        /// </summary>
        MatroskaTrackTranslate = 0x6624,

        /// <summary>
        /// Indicate a Matroska Track Offset element.
        /// </summary>
        MatroskaTrackOffset = 0x537F,

        /// <summary>
        /// Indicate a Matroska Codec Settings EBML element.
        /// </summary>
        MatroskaCodecSettings = 0x3A9697,

        /// <summary>
        /// Indicate a Matroska Codec Info URL EBML element.
        /// </summary>
        MatroskaCodecInfoUrl = 0x3B4040,

        /// <summary>
        /// Indicate a Matroska Codec Download URL EBML element.
        /// </summary>
        MatroskaCodecDownloadUrl = 0x26B240,

        /// <summary>
        /// Indicate a Matroska Codec Decode All EBML element.
        /// </summary>
        MatroskaCodecDecodeAll = 0xAA,

        /* IDs in the TrackVideo master */
        /* NOTE: This one is here only for backward compatibility.
        * Use _TRACKDEFAULDURATION */

        /// <summary>
        /// Indicate a Matroska Video Frame Rate EBML element.
        /// </summary>
        MatroskaVideoFrameRate = 0x2383E3,

        /// <summary>
        /// Indicate a Matroska Video Display Width EBML element.
        /// </summary>
        MatroskaVideoDisplayWidth = 0x54B0,

        /// <summary>
        /// Indicate a Matroska Video Display Height EBML element.
        /// </summary>
        MatroskaVideoDisplayHeight = 0x54BA,

        /// <summary>
        /// Indicate a Matroska Video Display Unit EBML element.
        /// </summary>
        MatroskaVideoDisplayUnit = 0x54B2,

        /// <summary>
        /// Indicate a Matroska Video Pixel Width EBML element.
        /// </summary>
        MatroskaVideoPixelWidth = 0xB0,

        /// <summary>
        /// Indicate a Matroska Video Pixel Height EBML element.
        /// </summary>
        MatroskaVideoPixelHeight = 0xBA,

        /// <summary>
        /// Indicate a Matroska Video Pixel Crop Bottom EBML element.
        /// </summary>
        MatroskaVideoPixelCropBottom = 0x54AA,

        /// <summary>
        /// Indicate a Matroska Video Pixel Crop Top EBML element.
        /// </summary>
        MatroskaVideoPixelCropTop = 0x54BB,

        /// <summary>
        /// Indicate a Matroska Video Pixel Crop Left EBML element.
        /// </summary>
        MatroskaVideoPixelCropLeft = 0x54CC,

        /// <summary>
        /// Indicate a Matroska Video Pixel Crop Right EBML element.
        /// </summary>
        MatroskaVideoPixelCropRight = 0x54DD,

        /// <summary>
        /// Indicate a Matroska Video Flag Interlaced EBML element.
        /// </summary>
        MatroskaVideoFlagInterlaced = 0x9A,

        /// <summary>
        /// Indicate a Matroska Video Stereo Mode EBML element.
        /// </summary>
        MatroskaVideoStereoMode = 0x53B8,

        /// <summary>
        /// Indicate a Matroska Video Aspect Ratio Type EBML element.
        /// </summary>
        MatroskaVideoAspectRatioType = 0x54B3,

        /// <summary>
        /// Indicate a Matroska Video Colour Space EBML element.
        /// </summary>
        MatroskaVideoColourSpace = 0x2EB524,

        /// <summary>
        /// Indicate a Matroska Video Gamma Value EBML element.
        /// </summary>
        MatroskaVideoGammaValue = 0x2FB523,

        /* IDs in the TrackAudio master */

        /// <summary>
        /// Indicate a Matroska Audio Sampling Freq EBML element.
        /// </summary>
        MatroskaAudioSamplingFreq = 0xB5,

        /// <summary>
        /// Indicate a Matroska Audio Bit Depth EBML element.
        /// </summary>
        MatroskaAudioBitDepth = 0x6264,

        /// <summary>
        /// Indicate a Matroska Audio Channels EBML element.
        /// </summary>
        MatroskaAudioChannels = 0x9F,

        /// <summary>
        /// Indicate a Matroska Audio Channels Position EBML element.
        /// </summary>
        MatroskaAudioChannelsPositions = 0x7D7B,

        /// <summary>
        /// Indicate a Matroska Audio Output Sampling Freq EBML element.
        /// </summary>
        MatroskaAudioOutputSamplingFreq = 0x78B5,

        /* IDs in the Tags master */

        /// <summary>
        /// Indicate a Matroska Tag EBML element.
        /// </summary>
        MatroskaTag = 0x7373,

        /* in the Tag master */

        /// <summary>
        /// Indicate a Matroska Simple Tag EBML element.
        /// </summary>
        MatroskaSimpleTag = 0x67C8,

        /// <summary>
        /// Indicate a Matroska Targets EBML element.
        /// </summary>
        MatroskaTargets = 0x63C0,

        /* in the SimpleTag master */

        /// <summary>
        /// Indicate a Matroska Tag Name EBML element.
        /// </summary>
        MatroskaTagName = 0x45A3,

        /// <summary>
        /// Indicate a Matroska Tag String EBML element.
        /// </summary>
        MatroskaTagString = 0x4487,

        /// <summary>
        /// Indicate a Matroska Tag Language EBML element.
        /// </summary>
        MatroskaTagLanguage = 0x447A,

        /// <summary>
        /// Indicate a Matroska Tag Default EBML element.
        /// </summary>
        MatroskaTagDefault = 0x4484,

        /// <summary>
        /// Indicate a Matroska Tag Binary EBML element.
        /// </summary>
        MatroskaTagBinary = 0x4485

    }
}
