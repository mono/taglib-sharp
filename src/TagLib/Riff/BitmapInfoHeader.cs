//
// BitmapInfoHeader.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
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

using System.Globalization;

namespace TagLib.Riff
{
   public struct BitmapInfoHeader : IVideoCodec
   {
      uint size; 
      uint width; 
      uint height; 
      ushort planes; 
      ushort bit_count; 
      ByteVector compression_id; 
      uint size_of_image; 
      uint x_pixels_per_meter; 
      uint y_pixels_per_meter; 
      uint colors_used; 
      uint colors_important;
      
      public BitmapInfoHeader (ByteVector data) : this (data, 0) {}
      
      public BitmapInfoHeader (ByteVector data, int offset)
      {
         if (data == null)
            throw new System.ArgumentNullException ("data");
         
         if (offset + 40 > data.Count)
            throw new CorruptFileException ("Expected 40 bytes.");
         
         if (offset > int.MaxValue - 40)
            throw new System.ArgumentOutOfRangeException ("offset");
         
         size               = data.Mid (offset +  0, 4).ToUInt (false);
         width              = data.Mid (offset +  4, 4).ToUInt (false);
         height             = data.Mid (offset +  8, 4).ToUInt (false);
         planes             = data.Mid (offset + 12, 2).ToUShort (false);
         bit_count          = data.Mid (offset + 14, 2).ToUShort (false);
         compression_id     = data.Mid (offset + 16, 4);
         size_of_image      = data.Mid (offset + 20, 4).ToUInt (false);
         x_pixels_per_meter = data.Mid (offset + 24, 4).ToUInt (false);
         y_pixels_per_meter = data.Mid (offset + 28, 4).ToUInt (false);
         colors_used        = data.Mid (offset + 32, 4).ToUInt (false);
         colors_important   = data.Mid (offset + 36, 4).ToUInt (false);
      }
      
      public uint       HeaderSize      {get {return size;}}
      public ushort     Planes          {get {return planes;}}
      public ushort     BitCount        {get {return bit_count;}}
      public ByteVector CompressionId   {get {return compression_id;}}
      public uint       ImageSize       {get {return size_of_image;}}
      public uint       XPixelsPerMeter {get {return x_pixels_per_meter;}}
      public uint       YPixelsPerMeter {get {return y_pixels_per_meter;}}
      public uint       ColorsUsed      {get {return colors_used;}}
      public uint       ImportantColors {get {return colors_important;}}
      
      public int VideoWidth  {get {return (int)width;}}
      public int VideoHeight {get {return (int)height;}}
      public MediaTypes MediaTypes {get {return MediaTypes.Video;}}
      public System.TimeSpan Duration {get {return System.TimeSpan.Zero;}}
      
		public string Description {
			get {
				string id = CompressionId.ToString (StringType.UTF8)
					.ToUpper (CultureInfo.InvariantCulture);
				switch (id)
				{
				case "AEMI":
					return "Array VideoONE MPEG1-I capture";
				case "ALPH":
					return "Ziracom Video";
				case "AMPG":
					return "Array VideoONE capture/compression";
				case "ANIM":
					return "Intel RDX";
/*
AP41 	Microsoft Corporation 	Reserved. 	02-Apr-01
AUR2 	AuraVision Corporation 	AuraVision Aura 2 codec. 	04-Jan-94
AURA 	AuraVision Corporation 	AuraVision Aura 1 codec. 	04-Jan-94
AUVX 	USH GmbH 	AUVX video codec. 	15-Mar-02
BT20 	Brooktree Corporation 	Brooktree MediaStream codec. 	05-Jun-95
BTCV 	Brooktree Corporation 	Brooktree composite video codec. 	05-Jun-95
CC12 	Intel Corporation 	Intel YUV12 codec. 	12-Jun-96
CDVC 	Canopus, Co., Ltd. 	Canopus DV codec. 	21-Nov-97
CGDI 	Microsoft Corporation 	Microsoft CamCorder in Office 97 (screen capture codec). 	10-Sep-01
CHAM 	Winnov, Inc. 	MM_WINNOV_CAVIARA_CHAMPAGNE. 	05-Sep-93
CM10 	CyberLink Corporation 	MediaShow 1.0. 	21-Aug-00
CPLA 	Weitek 	Weitek 4:2:0 YUV planar. 	18-Jul-94
CT10 	CyberLink Corporation 	TalkingShow 1.0. 	21-Aug-00
CVID 	SuperMac 	Cinepak by SuperMac. 	01-Aug-93
CWLT 	Microsoft Corporation 	Reserved. 	24-Jul-94
CYUV 	Creative Labs, Inc. 	Creative Labs YUV. 	Not specified.
DIV3 	Microsoft Corporation 	Reserved. 	02-Apr-01
*/
				case "DIV3":
				case "MP43":
					return "Microsoft MPEG-4 Version 3 Video";
/*
DIV4 	Microsoft Corporation 	Reserved. 	02-Apr-01
*/
				case "DIVX":
					return "DivX Video";
/*
DJPG 	Data Translation, Inc. 	Broadway 101 Motion JPEG codec. 	27-Jul-98
DP16 	Matsushita Electric Industrial Co., Ltd. 	YUV411 with DPCM 6-bit compression. 	19-May-98
DP18 	Matsushita Electric Industrial Co., Ltd. 	YUV411 with DPCM 8-bit compression. 	19-May-98
DP26 	Matsushita Electric Industrial Co., Ltd. 	YUV422 with DPCM 6-bit compression. 	19-May-98
DP28 	Matsushita Electric Industrial Co., Ltd. 	YUV422 with DPCM 8-bit compression. 	19-May-98
DP96 	Matsushita Electric Industrial Co., Ltd. 	YVU9 with DPCM 6-bit compression. 	19-May-98
DP98 	Matsushita Electric Industrial Co., Ltd. 	YVU9 with DPCM 8-bit compression. 	19-May-98
DP9L 	Matsushita Electric Industrial Co., Ltd. 	YVU9 with DPCM 6-bit compression and thinned-out. 	19-May-98
DUCK 	The Duck Corporation 	TrueMotion 1.0. 	16-Sep-97
dv25 	Matrox Electronic Systems, Ltd. 	SMPTE 314M 25Mb/s compressed. 	12-May-99
dv50 	Matrox Electronic Systems, Ltd. 	SMPTE 314M 50Mb/s compressed. 	12-May-99
DVE2 	InSoft, Inc. 	DVE-2 videoconferencing codec. 	16-May-94
DVH1 	Matsushita Electric Industrial Co., Ltd. 	DVC Pro HD. 	13-Nov-02
dvhd 	Microsoft Corporation 	DV data as defined in Part 3 of the Specification of Consumer-use Digital VCRs. Video is in the format of 1125 lines at 30.00 Hz (1125-60) or 1250 lines at 25.00 Hz (1250-50). 	Not specified.
DVNM 	Matsushita Electric Industrial Co., Ltd. 	Not specified. 	14-Feb-01
dvsd 	Microsoft Corporation 	DV data as defined in Part 2 of the Specification of Consumer-use Digital VCRs. Video is in the format of 525 lines at 29.97 Hz (525-60) or 625 lines at 25.00 Hz (625-50). 	Not specified.
dvsl 	Microsoft Corporation 	DV data as defined in Part 6 of Specification of Consumer-use Digital VCRs. Video is in the format of high-compression SD (SDL). 	Not specified.
DVX1 	Lucent Technologies 	Lucent DVX1000SP video decoder. 	17-Feb-99
DVX2 	Lucent Technologies 	Lucent DVX2000S video decoder. 	17-Feb-99
DVX3 	Lucent Technologies 	Lucent DVX3000S video decoder. 	17-Feb-99
DXTC 	Microsoft Corporation 	DirectX texture compression. 	27-Jan-98
*/
				case "DX50":
					return "DivX Version 5 Video";
/*
EMWC 	EverAd, Inc. 	EverAd Marquee WMA codec. 	12-Jan-01
ETV1 	eTreppid Technologies, LLC 	eTreppid video codec. 	11-Oct-01
ETV2 	eTreppid Technologies, LLC 	eTreppid video codec. 	11-Oct-01
ETVC 	eTreppid Technologies, LLC 	eTreppid video codec. 	11-Oct-01
FLJP 	D-Vision Systems, Inc. 	Field-encoded motion JPEG with LSI bitstream format. 	21-Oct-96
FRWA 	Softlab-Nsk Ltd. 	Forward alpha. 	25-Jun-98
FRWD 	Softlab-Nsk Ltd. 	Forward JPEG. 	25-Jun-98
FRWT 	Softlab-Nsk Ltd. 	Forward JPEG+alpha. 	25-Jun-98
FVF1 	Iterated Systems, Inc. 	Fractal video frame. 	24-Aug-93
FXT1 	3dfx Interactive, Inc. 	Not specified. 	27-Jul-99
GWLT 	Microsoft Corporation 	Reserved. 	24-Jul-94
H260 	Intel Corporation 	Conferencing codec. 	12-Jun-96
H261 	Intel Corporation 	Conferencing codec. 	12-Jun-96
H262 	Intel Corporation 	Conferencing codec. 	12-Jun-96
H263 	Intel Corporation 	Conferencing codec. 	12-Jun-96
H264 	Intel Corporation 	Conferencing codec. 	12-Jun-96
H265 	Intel Corporation 	Conferencing codec. 	12-Jun-96
H266 	Intel Corporation 	Conferencing codec. 	12-Jun-96
H267 	Intel Corporation 	Conferencing codec. 	12-Jun-96
H268 	Intel Corporation 	Conferencing codec. 	12-Jun-96
H269 	Intel Corporation 	Conferencing codec. 	12-Jun-96
I263 	Intel Corporation 	Intel I263. 	16-Sep-97
I420 	Intel Corporation 	Intel Indeo 4 codec. 	15-Jan-96
IAN 	Intel Corporation 	Intel RDX. 	12-Jun-96
ICLB 	InSoft, Inc. 	CellB videoconferencing codec. 	16-May-94
IFO9 	Intel Corporation 	Intel intermediate YUV9. 	Not specified.
ILVC 	Intel Corporation 	Intel layered Video. 	16-Sep-97
ILVR 	Intel Corporation 	ITU-T's H.263+ compression standard. 	29-Oct-97
IMAC 	Intel Corporation 	Intel hardware motion compensation. 	20-Jul-98
IPDV 	I-O Data Device, Inc. 	IEEE 1394 digital video control and capture board format. 	25-Jun-99
IRAW 	Intel Corporation 	Intel YUV uncompressed. 	12-Jun-96
ISME 	Intel Corporation 	Intel's next-generation video codec. 	01-Jul-98
IUYV 	LEAD Technologies, Inc. 	UYVY interlaced (even, then odd lines). 	09-Aug-01
*/
				case "IV30":
				case "IV31":
				case "IV32":
				case "IV33":
				case "IV34":
				case "IV35":
				case "IV36":
				case "IV37":
				case "IV38":
				case "IV39":
					return "Intel Indeo Video Version 3";
				case "IV40":
				case "IV41":
				case "IV42":
				case "IV43":
				case "IV44":
				case "IV45":
				case "IV46":
				case "IV47":
				case "IV48":
				case "IV49":
					return "Intel Indeo Video Version 4";
				case "IV50":
					return "Intel Indeo Video Version 5";
/*
IY41 	LEAD Technologies, Inc. 	Y41P interlaced (even, then odd lines). 	09-Aug-01
IYU1 	Microsoft Corporation 	IEEE 1394 Digital Camera 1.04 Specification: mode 2, 12-bit YUV (4:1:1). 	17-Aug-99
IYU2 	Microsoft Corporation 	IEEE 1394 Digital Camera 1.04 Specification: mode 2, 24 bit YUV (4:4:4). 	17-Aug-99
JPEG 	Microsoft Corporation 	Still image JPEG DIB. 	 
LEAD 	LEAD Technologies, Inc. 	Proprietary MCMP compression. 	09-Aug-01
LIA1 	Liafail, Inc. 	Liafail. 	30-Jun-00
LJPG 	LEAD Technologies, Inc. 	Lossless JPEG compression. 	09-Aug-01
LSV0 	Infinop Inc. 	Reserved. 	30-Aug-99
LSVC 	Infinop Inc. 	Infinop Lightning Strike constant bit rate video codec. 	30-Aug-99
LSVW 	Infinop Inc. 	Infinop Lightning Strike multiple bit rate video codec. 	30-Aug-99
M101 	Matrox Electronic Systems, Ltd. 	Uncompressed field-based YUY2. 	31-Jan-02
M4S2 	Microsoft Corporation 	Microsoft ISO MPEG-4 video V1.1. 	02-Apr-01
MJPG 	Microsoft Corporation 	Motion JPEG. 	Not specified.
MMES 	Matrox Electronic Systems, Ltd. 	Matrox MPEG-2 elementary video stream. 	06-May-99
MMIF 	Matrox Electronic Systems, Ltd. 	Matrox MPEG-2 elementary I-frame-only video stream. 	11-May-99
MP2A 	Media Excel Inc. 	MPEG-2 audio. 	20-Sep-01
MP2T 	Media Excel Inc. 	MPEG-2 transport. 	20-Sep-01
MP2V 	Media Excel Inc. 	MPEG-2 video. 	20-Sep-01
MP42 	Microsoft Corporation 	Microsoft MPEG-4 video codec V2. 	28-Oct-97
MP4A 	Media Excel Inc. 	MPEG-4 audio. 	20-Sep-01
MP4S 	Microsoft Corporation 	Microsoft ISO MPEG-4 video V1.0. 	02-Apr-01
MP4T 	Media Excel Inc. 	MPEG-4 transport. 	20-Sep-01
MP4V 	Media Excel Inc. 	MPEG-4 video. 	20-Sep-01
MPEG 	Chromatic Research, Inc. 	MPEG-1 video, I frame. 	16-Sep-97
MPG4 	Microsoft Corporation 	Reserved. 	02-Apr-01
*/
				case "MPG4":
					return "Microsoft MPEG-4 Version 1 Video";
/*
MRCA 	FAST Multimedia AG 	Mrcodec. 	16-Sep-97
MRLE 	Microsoft Corporation 	Run length encoding. 	01-Nov-92
MSxx 	Microsoft Corporation 	All FOURCC codes starting with 'MS' are reserved. 	22-Jun-01
MSS1 	Microsoft Corporation 	Microsoft screen codec V1. 	02-Apr-99
MSV1 	Microsoft Corporation 	Microsoft video codec V1. 	05-Jun-98
MSVC 	Microsoft Corporation 	Video 1. 	01-Nov-92
MV10 	Nokia Mobile Phones 	Nokia MVC video codec. 	06-Jan-99
MV11 	Nokia Mobile Phones 	Nokia MVC video codec. 	06-Jan-99
MV12 	Nokia Mobile Phones 	Nokia MVC video codec. 	06-Jan-99
MV99 	Nokia Mobile Phones 	Nokia MVC video codec. 	06-Jan-99
MVC1 	Nokia Mobile Phones 	Nokia MVC video codec. 	06-Jan-99
MVC2 	Nokia Mobile Phones 	Nokia MVC video codec. 	06-Jan-99
MVC9 	Nokia Mobile Phones 	Nokia MVC video codec. 	06-Jan-99
NTN1 	Nogatech Ltd. 	Nogatech video compression 1. 	06-Jan-98
NY12 	Nogatech Ltd. 	Nogatech YUV 12 format. 	28-Jun-99
NYUV 	Nogatech Ltd. 	Nogatech YUV 422 format. 	28-Jun-99
pcl2 	Pinnacle Systems, Inc. 	Pinnacle RL video codec. 	08-Dec-98
PCLE 	Pinnacle Systems, Inc. 	Studio 400 video codec. 	30-Mar-98
PHMO 	IBM Corporation 	Photomotion. 	Not specified.
QPEG 	Q-Team 	QPEG 1.1 format video codec. 	15-Nov-96
RGBT 	Computer Concepts Ltd. 	32-bit support. 	21-Oct-94
RIVA 	NVIDIA Corporation 	Swizzled texture format. 	22-Mar-99
RLND 	Roland Corporation 	Not specified. 	15-Mar-02
RT21 	Intel Corporation 	Intel Indeo 2.1. 	01-Aug-93
RVX 	Intel Corporation 	Intel RDX. 	12-Jun-96
S263 	Sorenson Vision, Inc. 	Sorenson Vision H.263. 	25-Jun-98
SCCD 	Luminositi, Inc. 	Luminositi SoftCam codec. 	11-May-98
SDCC 	Sun Communications, Inc. 	Sun Digital Camera codec. 	15-Sep-97
SFMC 	Crystal Net Corporation 	Crystal Net SFM codec. 	25-Jul-96
SMSC 	Radius 	Proprietary. 	15-Nov-94
SMSD 	Radius 	Proprietary. 	15-Nov-94
SPLC 	Splash Studios 	Splash Studios ACM audio codec. 	08-Jan-96
SQZ2 	Microsoft Corporation 	Microsoft VXtreme video codec V2. 	16-Sep-97
STVA 	STMicroelectronics 	ST CMOS Imager Data (Bayer). 	09-Aug-99
STVB 	STMicroelectronics 	ST CMOS Imager Data (Nudged Bayer). 	09-Aug-99
STVC 	STMicroelectronics 	ST CMOS Imager Data (Bunched). 	09-Aug-99
SV10 	Sorenson Vision, Inc. 	Sorenson Video R1. 	15-Sep-97
SV3M 	Sorenson Vision, Inc. 	Sorenson SV3 module decoder. 	01-Jun-99
TLMS 	TeraLogic, Inc. 	TeraLogic motion intraframe codec. 	16-Sep-97
TLST 	TeraLogic, Inc. 	TeraLogic motion intraframe codec. 	16-Sep-97
TM20 	The Duck Corporation 	TrueMotion 2.0. 	16-Sep-97
TMIC 	TeraLogic, Inc. 	TeraLogic motion intraframe codec. 	16-Sep-97
TMOT 	Horizons Technology, Inc. 	TrueMotion video compression algorithm. 	21-Oct-94
TR20 	The Duck Corporation 	TrueMotion RT 2.0. 	16-Sep-97
ULTI 	IBM Corporation 	Ultimotion. 	Not specified.
UYVP 	Evans & Sutherland 	YCbCr 4:2:2 extended precision, 10 bits per component (U0Y0V0Y1). 	15-Nov-00
V261 	Lucent Technologies 	Lucent VX3000S video codec. 	17-Feb-99
V422 	VITEC Multimedia 	24-bit YUV 4:2:2 format (CCIR 601). For this format, 2 consecutive pixels are represented by a 32-bit (4-byte) Y1UY2V color value. 	13-Dec-93
V655 	VITEC Multimedia 	16-bit YUV 4:2:2 format. 	13-Dec-93
VCR1 	ATI Technologies Inc. 	ATI VCR 1.0. 	23-Oct-97
VCWV 	VideoCon 	VideoCon wavelet. 	06-Jun-00
VDCT 	VITEC Multimedia 	Video Maker Pro DIB. 	Not specified.
VIDS 	VITEC Multimedia 	YUV 4:2:2 CCIR 601 for v422. 	Not specified.
VGPX 	Alaris, Inc. 	Alaris VGPixel video. 	30-Mar-98
VIVO 	Vivo Software, Inc. 	Vivo H.263 video codec. 	16-Sep-97
VIXL 	miro Computer Products AG 	For use with the miro line of capture cards. 	24-Jul-94
VJPG 	Video Communication Systems 	A JPEG-based compression scheme for RGB bitmaps. 	05-Feb-01
VLV1 	VideoLogic Systems 	VLCAP.DRV. 	08-Jun-93
VQC1 	ViewQuest Technologies Inc. 	0x31435156. 	13-Nov-98
VQC2 	ViewQuest Technologies Inc. 	0x32435156. 	13-Nov-98
VQJP 	ViewQuest Technologies Inc. 	VQ630 dual-mode digital camera. 	28-Nov-00
VQS4 	ViewQuest Technologies Inc. 	VQ110 digital video camera. 	28-Nov-00
VX1K 	Lucent Technologies 	Lucent  VX1000S video codec. 	17-Feb-99
VX2K 	Lucent Technologies 	Lucent VX2000S video codec. 	17-Feb-99
VXSP 	Lucent Technologies 	Lucent VX1000SP video codec. 	17-Feb-99
WBVC 	Winbond Electronics Corporation 	W9960. 	17-Sep-97
WINX 	Winnov, Inc. 	Not specified. 	11-May-00
WJPG 	Winbond Electronics Corporation 	Winbond motion JPEG bitstream format. 	04-Mar-99
WMxx 	Microsoft Corporation 	All FOURCC codes starting with 'WS' are reserved. 	22-Jun-01
WMS2 	Microsoft Corporation 	Reserved. 	02-Apr-01
*/
				case "WMV1":
					return "Microsoft Windows Media Video Version 7";
				case "WMV2":
					return "Microsoft Windows Media Video Version 8";
				case "WMV3":
					return "Microsoft Windows Media Video Version 9";
/*
WNV1 	Winnov, Inc. 	Not specified. 	11-May-00
WPY2 	Winnov, Inc. 	Not specified. 	11-May-00
WZCD 	CORE Co. Ltd. 	iScan. 	20-Jun-00
WZDC 	CORE Co. Ltd. 	iSnap. 	20-Jun-00
XJPG 	Xirlink, Inc. 	Xirlink JPEG-like compressor. 	14-Feb-01
XLV0 	NetXL, Inc. 	XL video decoder. 	18-Sep-97
*/
				case "XVID":
					return "XviD Video";
				case "YC12":
					return "Intel YUV12 Video";
				case "YCCK":
					return "Uncompressed YCbCr Video with key data";
				case "YU92":
					return "Intel YUV Video";
				case "YUV8":
					return "Winnov Caviar YUV8 Video";
				case "YUV9":
					return "Intel YUV Video";
				case "YUYP":
					return "Evans & Sutherland YCbCr 4:2:2 extended precision, 10 bits per component Video";
				case "YUYV":
					return "Canopus YUYV Compressor Video";
				case "ZPEG":
					return "Metheus Corporation Video Zipper";
				case "ZPG1":
				case "ZPG2":
				case "ZPG3":
				case "ZPG4":
					return "VoDeo Solutions Video";
				default:
					return string.Format (
						CultureInfo.InvariantCulture,
						"Unknown Image ({0})",
						CompressionId);
				}
			}
		}
		
		
		
#region IEquatable
      public override int GetHashCode ()
      {
         unchecked
         {            return (int) (size ^ width ^ height ^ planes ^ bit_count ^
                                      compression_id.ToUInt () ^ size_of_image ^
                                      x_pixels_per_meter ^ y_pixels_per_meter ^
                                      colors_used ^ colors_important);
         }
      }
      
      public override bool Equals (object obj)
      {
         if (!(obj is BitmapInfoHeader))
            return false;
         
         return Equals ((BitmapInfoHeader) obj);
      }
      
      public bool Equals (BitmapInfoHeader other)
      {
         return size == other.size && width == other.width &&
         height == other.height && planes == other.planes &&
         bit_count == other.bit_count &&
         compression_id == other.compression_id &&
         size_of_image == other.size_of_image &&
         x_pixels_per_meter == other.x_pixels_per_meter &&
         y_pixels_per_meter == other.y_pixels_per_meter &&
         colors_used == other.colors_used &&
         colors_important == other.colors_important;
      }
      
		public static bool operator == (BitmapInfoHeader first, BitmapInfoHeader second)
		{
			return first.Equals (second);
		}
		
		public static bool operator != (BitmapInfoHeader first, BitmapInfoHeader second)
		{
			return !first.Equals (second);
		}
#endregion
	}
}