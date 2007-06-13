namespace TagLib.Riff
{
   public struct WaveFormatEx : IAudioCodec
   {
      ushort format_tag;
      ushort channels;
      uint   samples_per_second;
      uint   average_bytes_per_second;
      ushort bits_per_sample;
      
      public WaveFormatEx (ByteVector data) : this (data, 0) {}
      
      public WaveFormatEx (ByteVector data, int offset)
      {
         if (data == null)
            throw new System.ArgumentNullException ("data");
         
         if (offset + 16 > data.Count)
            throw new CorruptFileException ("Expected 16 bytes.");
         
         if (offset > int.MaxValue - 16)
            throw new System.ArgumentOutOfRangeException ("offset");
         
         format_tag               = data.Mid (offset,      2).ToUShort (false);
         channels                 = data.Mid (offset +  2, 2).ToUShort (false);
         samples_per_second       = data.Mid (offset +  4, 4).ToUInt   (false);
         average_bytes_per_second = data.Mid (offset +  8, 4).ToUInt   (false);
         bits_per_sample          = data.Mid (offset + 14, 2).ToUShort (false);
      }
      
      public ushort FormatTag             {get {return format_tag;}}
      public uint   AverageBytesPerSecond {get {return average_bytes_per_second;}}
      public ushort BitsPerSample         {get {return bits_per_sample;}}
      
      
      public int AudioSampleRate {get {return (int) samples_per_second;}}
      public int AudioChannels {get {return channels;}}
      public int AudioBitrate {get {return (int) System.Math.Round (average_bytes_per_second * 8d / 1000d);}}
      public MediaTypes MediaTypes {get {return MediaTypes.Audio;}}
      public System.TimeSpan Duration {get {return System.TimeSpan.Zero;}}
      
      public string Description
      {
         get
         {
            switch (FormatTag)
            {
            case 0x0000: return "Unknown Wave Format";
            case 0x0001: return "PCM Audio";
            case 0x0002: return "Microsoft Adaptive PCM Audio";
            case 0x0003: return "PCM Audio in IEEE floating-point format";
            case 0x0004: return "Compaq VSELP Audio";
            case 0x0005: return "IBM CVSD Audio";
            case 0x0006: return "Microsoft ALAW Audio";
            case 0x0007: return "Microsoft MULAW Audio";
            case 0x0008: return "Microsoft DTS Audio";
            case 0x0009: return "Microsoft DRM Encrypted Audio";
            case 0x000A: return "Microsoft Speech Audio";
            case 0x000B: return "Microsoft Windows Media RT Voice Audio";
            case 0x0010: return "OKI ADPCM Audio";
            case 0x0011: return "Intel ADPCM Audio";
            case 0x0012: return "VideoLogic ADPCM Audio";
            case 0x0013: return "Sierra ADPCM Audio";
            case 0x0014: return "Antex ADPCM Audio";
            case 0x0015: return "DSP DIGISTD Audio";
            case 0x0016: return "DSP DIGIFIX Audio";
            case 0x0017: return "Dialogic OKI ADPCM Audio";
            case 0x0018: return "Media Vision ADPCM Audio for Jazz 16";
            case 0x0019: return "Hewlett-Packard CU Audio";
            case 0x001A: return "Hewlett-Packard Dynamic Voice Audio";
            case 0x0020: return "Yamaha ADPCM Audio";
            case 0x0021: return "Speech Compression Audio";
            case 0x0022: return "DSP Group True Speech Audio";
            case 0x0023: return "Echo Speech Audio";
            case 0x0024: return "Ahead AF36 Audio";
            case 0x0025: return "Audio Processing Technology Audio";
            case 0x0026: return "Ahead AF10 Audio";
            case 0x0027: return "Aculab Prosody CTI Speech Card Audio";
            case 0x0028: return "Merging Technologies LRC Audio";
            case 0x0030: return "Dolby AC2 Audio";
            case 0x0031: return "Microsoft GSM6.10 Audio";
            case 0x0032: return "Microsoft MSN Audio";
            case 0x0033: return "Antex ADPCME Audio";
            case 0x0034: return "Control Resources VQLPC";
            case 0x0035: return "DSP REAL Audio";
            case 0x0036: return "DSP ADPCM Audio";
            case 0x0037: return "Control Resources CR10 Audio";
            case 0x0038: return "Natural MicroSystems VBXADPCM Audio";
            case 0x0039: return "Roland RDAC Proprietary Audio Format";
            case 0x003A: return "Echo Speech Proprietary Audio Compression Format";
            case 0x003B: return "Rockwell ADPCM Audio";
            case 0x003C: return "Rockwell DIGITALK Audio";
            case 0x003D: return "Xebec Proprietary Audio Compression Format";
            case 0x0040: return "Antex G721 ADPCM Audio";
            case 0x0041: return "Antex G728 CELP Audio";
            case 0x0042: return "Microsoft MSG723 Audio";
            case 0x0043: return "Microsoft MSG723.1 Audio";
            case 0x0044: return "Microsoft MSG729 Audio";
            case 0x0045: return "Microsoft SPG726 Audio";
            case 0x0050: return "Microsoft MPEG Audio";
            case 0x0052: return "InSoft RT24 Audio";
            case 0x0053: return "InSoft PAC Audio";
            case 0x0055: return "ISO/MPEG Layer 3 Audio";
            case 0x0059: return "Lucent G723 Audio";
            case 0x0060: return "Cirrus Logic Audio";
            case 0x0061: return "ESS Technology PCM Audio";
            case 0x0062: return "Voxware Audio";
            case 0x0063: return "Canopus ATRAC Audio";
            case 0x0064: return "APICOM G726 ADPCM Audio";
            case 0x0065: return "APICOM G722 ADPCM Audio";
            case 0x0067: return "Microsoft DSAT Display Audio";
            case 0x0069: return "Voxware Byte Aligned Audio";
            case 0x0070: return "Voxware AC8 Audio";
            case 0x0071: return "Voxware AC10 Audio";
            case 0x0072: return "Voxware AC16 Audio";
            case 0x0073: return "Voxware AC20 Audio";
            case 0x0074: return "Voxware RT24 Audio";
            case 0x0075: return "Voxware RT29 Audio";
            case 0x0076: return "Voxware RT29HW Audio";
            case 0x0077: return "Voxware VR12 Audio";
            case 0x0078: return "Voxware VR18 Audio";
            case 0x0079: return "Voxware TQ40 Audio";
            case 0x007A: return "Voxware SC3 Audio";
            case 0x007B: return "Voxware SC3 Audio";
            case 0x0080: return "SoftSound Audio";
            case 0x0081: return "Voxware TQ60 Audio";
            case 0x0082: return "Microsoft RT24 Audio";
            case 0x0083: return "AT&T G729A Audio";
            case 0x0084: return "Motion Pixels MVI2 Audio";
            case 0x0085: return "Datafusion Systems G726 Audio";
            case 0x0086: return "Datafusion Systems G610 Audio";
            case 0x0088: return "Iterated Systems Audio";
            case 0x0089: return "OnLive! Audio";
            case 0x008A: return "Multitude FT SX20 Audio";
            case 0x008B: return "InfoCom ITS ACM G721 Audio";
            case 0x008C: return "Convedia G729 Audio";
            case 0x008D: return "Congruency Audio";
            case 0x0091: return "Siemens Business Communications 24 Audio";
            case 0x0092: return "Sonic Foundary Dolby AC3 Audio";
            case 0x0093: return "MediaSonic G723 Audio";
            case 0x0094: return "Aculab Prosody CTI Speech Card Audio";
            case 0x0097: return "ZyXEL ADPCM";
            case 0x0098: return "Philips Speech Processing LPCBB Audio";
            case 0x0099: return "Studer Professional PACKED Audio";
            case 0x00A0: return "Malden Electronics Phony Talk Audio";
            case 0x00A1: return "Racal Recorder GSM Audio";
            case 0x00A2: return "Racal Recorder G720.a Audio";
            case 0x00A3: return "Racal G723.1 Audio";
            case 0x00A4: return "Racal Tetra ACELP Audio";
            case 0x00B0: return "NEC AAC Audio";
            case 0x0100: return "Rhetorex ADPCM Audio";
            case 0x0101: return "BeCubed IRAT Audio";
            case 0x0111: return "Vivo G723 Audio";
            case 0x0112: return "Vivo Siren Audio";
            case 0x0120: return "Philips Speach Processing CELP Audio";
            case 0x0121: return "Philips Speach Processing GRUNDIG Audio";
            case 0x0123: return "Digital Equipment Corporation G723 Audio";
            case 0x0125: return "Sanyo LD-ADPCM Audio";
            case 0x0130: return "Sipro Lab ACELPNET Audio";
            case 0x0131: return "Sipro Lab ACELP4800 Audio";
            case 0x0132: return "Sipro Lab ACELP8v3 Audio";
            case 0x0133: return "Sipro Lab G729 Audio";
            case 0x0134: return "Sipro Lab G729A Audio";
            case 0x0135: return "Sipro Lab KELVIN Audio";
            case 0x0136: return "VoiceAge AMR Audio";
            case 0x0140: return "Dictaphone G726 ADPCM Audio";
            case 0x0141: return "Dictaphone CELP68 Audio";
            case 0x0142: return "Dictaphone CELP54 Audio";
            case 0x0150: return "QUALCOMM Pure Voice Audio";
            case 0x0151: return "QUALCOMM Half Rate Audio";
            case 0x0155: return "Ring Zero TUBGSM Audio";
            case 0x0160: return "Microsoft WMA1 Audio";
            case 0x0161: return "Microsoft WMA2 Audio";
            case 0x0162: return "Microsoft Multichannel WMA Audio";
            case 0x0163: return "Microsoft Lossless WMA Audio";
/*
            case 0x0170 	WAVE_FORMAT_UNISYS_NAP_ADPCM 	Unisys Corporation 	Not specified
            case 0x0171 	WAVE_FORMAT_UNISYS_NAP_ULAW 	Unisys Corporation 	Not specified
            case 0x0172 	WAVE_FORMAT_UNISYS_NAP_ALAW 	Unisys Corporation 	Not specified
            case 0x0173 	WAVE_FORMAT_UNISYS_NAP_16K 	Unisys Corporation 	Not specified
            case 0X0174 	WAVE_FORMAT_MM_SYCOM_ACM_SYC008 	SyCom Technologies 	Not specified
            case 0x0175 	WAVE_FORMAT_MM_SYCOM_ACM_SYC701_G726L 	SyCom Technologies 	Not specified
            case 0x0176 	WAVE_FORMAT_MM_SYCOM_ACM_SYC701_CELP54 	SyCom Technologies 	Not specified
            case 0x0177 	WAVE_FORMAT_MM_SYCOM_ACM_SYC701_CELP68 	SyCom Technologies 	Not specified
            case 0x0178 	WAVE_FORMAT_KNOWLEDGE_ADVENTURE_ADPCM 	Knowledge Adventure, Inc. 	Not specified
            case 0x0180 	WAVE_FORMAT_MPEG2AAC 	Fraunhofer IIS 	Not specified
            case 0x0190 	WAVE_FORMAT_DTS_DS 	Digital Theater Systems, Inc. 	Not specified
            case 0x1979 	WAVE_FORMAT_INNINGS_ADPCM 	Innings Telecom Inc. 	Not specified
            case 0x0200 	WAVE_FORMAT_CREATIVE_ADPCM 	Creative Labs, Inc. 	Not specified
            case 0x0202 	WAVE_FORMAT_CREATIVE_FASTSPEECH8 	Creative Labs, Inc. 	Fast Speech 8
            case 0x0203 	WAVE_FORMAT_CREATIVE_FASTSPEECH10 	Creative Labs, Inc. 	Fast Speech 10
            case 0x0210 	WAVE_FORMAT_UHER_ADPCM 	UHER informatik GmbH 	Not specified
            case 0x0220 	WAVE_FORMAT_QUARTERDECK 	Quarterdeck Corporation 	Not specified
            case 0x0230 	WAVE_FORMAT_ILINK_VC 	I-Link Worldwide 	Not specified
            case 0x0240 	WAVE_FORMAT_RAW_SPORT 	Aureal Semiconductor Inc. 	Not specified
            case 0x0250 	WAVE_FORMAT_IPI_HSX 	Interactive Products, Inc. 	Not specified
            case 0x0251 	WAVE_FORMAT_IPI_RPELP 	Interactive Products, Inc. 	Not specified
            case 0x0260 	WAVE_FORMAT_CS2 	Consistent Software 	Cs2
            case 0x0270 	WAVE_FORMAT_SONY_SCX 	Sony Corporation 	Not specified
            case 0x0271 	WAVE_FORMAT_SONY_SCY 	Sony Corporation 	Not specified
            case 0x0272 	WAVE_FORMAT_SONY_ATRAC3 	Sony Corporation 	Not specified
            case 0x0273 	WAVE_FORMAT_SONY_SPC 	Sony Corporation 	Not specified
            case 0x0280 	WAVE_FORMAT_TELUM 	Telum Inc. 	Not specified
            case 0x0281 	WAVE_FORMAT_TELUMIA 	Telum Inc. 	Not specified
            case 0x0285 	WAVE_FORMAT_NCVS_ADPCM 	Norcom Electronics Corporation 	Norcom Voice Systems ADPCM
            case 0x0300 	WAVE_FORMAT_FM_TOWNS_SND 	Fujitsu Corporation 	Not specified
            case 0x0301 	Reserved 	Fujitsu Corporation 	Not specified
            case 0x0302 	Reserved 	Fujitsu Corporation 	Not specified
            case 0x0303 	Reserved 	Fujitsu Corporation 	Not specified
            case 0x0304 	Reserved 	Fujitsu Corporation 	Not specified
            case 0x0305 	Reserved 	Fujitsu Corporation 	Not specified
            case 0x0306 	Reserved 	Fujitsu Corporation 	Not specified
            case 0x0307 	Reserved 	Fujitsu Corporation 	Not specified
            case 0x0308 	Reserved 	Fujitsu Corporation 	Not specified
            case 0x0350 	WAVE_FORMAT_DEVELOPMENT 	Micronas Semiconductors, Inc. 	Not specified
            case 0x0351 	WAVE_FORMAT_CELP833 	Micronas Semiconductors, Inc. 	Not specified
            case 0x0400 	WAVE_FORMAT_BTV_DIGITAL 	Brooktree Corporation 	Brooktree digital audio format
            case 0x0450 	WAVE_FORMAT_QDESIGN_MUSIC 	QDesign Corporation 	Not specified
            case 0x0680 	WAVE_FORMAT_VME_VMPCM 	AT&T 	Not specified
            case 0x0681 	WAVE_FORMAT_TPC 	AT&T 	Not specified
            case 0x1000 	WAVE_FORMAT_OLIGSM 	Ing. C. Olivetti & C., S.p.A. 	Not specified
            case 0x1001 	WAVE_FORMAT_OLIADPCM 	Ing. C. Olivetti & C., S.p.A. 	Not specified
            case 0x1002 	WAVE_FORMAT_OLICELP 	Ing. C. Olivetti & C., S.p.A. 	Not specified
            case 0x1003 	WAVE_FORMAT_OLISBC 	Ing. C. Olivetti & C., S.p.A. 	Not specified
            case 0x1004 	WAVE_FORMAT_OLIOPR 	Ing. C. Olivetti & C., S.p.A. 	Not specified
            case 0x1100 	WAVE_FORMAT_LH_CODEC 	Lernout & Hauspie 	Not specified
            case 0X1101 	WAVE_FORMAT_LH_CELP 	Lernout & Hauspie 	Not specified
            case 0X1102 	WAVE_FORMAT_LH_SB8 	Lernout & Hauspie 	Not specified
            case 0X1103 	WAVE_FORMAT_LH_SB12 	Lernout & Hauspie 	Not specified
            case 0X1104 	WAVE_FORMAT_LH_SB16 	Lernout & Hauspie 	Not specified
            case 0x1400 	WAVE_FORMAT_NORRIS 	Norris Communications, Inc. 	Not specified
            case 0x1500 	WAVE_FORMAT_SOUNDSPACE_MUSICOMPRESS 	AT&T 	Not specified
            case 0x163 	WAVE_FORMAT_WMAUDIO_LOSSLESS 	Microsoft Corporation 	WMA lossless
            case 0x164 	WAVE_FORMAT_WMASPDIF 	Microsoft Corporation 	WMA Pro over S/PDIF
            case 0x1971 	WAVE_FORMAT_SONICFOUNDRY_LOSSLESS 	Sonic Foundry 	Not specified
            case 0x2000 	WAVE_FORMAT_DVM 	FAST Multimedia AG 	Not specified
            case 0x2500 	WAVE_FORMAT_MSCE 	Microsoft Corporation 	Reserved rangle to 0x2600
            case 0x4143 	WAVE_FORMAT_DIVIO_AAC 	Divio, Inc. 	Divio's AAC
            case 0x4201 	WAVE_FORMAT_NOKIA_AMR 	Nokia Mobile Phones 	Nokia adaptive multirate
            case 0x4243 	WAVE_FORMAT_DIVIO_G726 	Divio, Inc. 	Divio's G726
            case 0x7000 	WAVE_FORMAT_3COM_NBX 	3Com Corporation 	Not specified
            case 0x7A21 	WAVE_FORMAT_NTT_DOCOMO_AMR_NO_SID 	Microsoft Corporation 	Adaptive multirate
            case 0x7A22 	WAVE_FORMAT_NTT_DOCOMO_AMR_WITH_SID 	Microsoft Corporation 	AMR with silence detection
            case 0xA100 	WAVE_FORMAT_COMVERSEINFOSYS_G723_1 	Comverse Infosys Ltd. 	Not specified
            case 0xA101 	WAVE_FORMAT_COMVERSEINFOSYS_AVQSBC 	Comverse Infosys Ltd. 	Not specified
            case 0xA102 	WAVE_FORMAT_COMVERSEINFOSYS_OLDSBC 	Comverse Infosys Ltd. 	Not specified
            case 0xA103 	WAVE_FORMAT_SYMBOLTECH_G729A 	Symbol Technologies Canada 	Symbol Technology's G729A
            case 0xA104 	WAVE_FORMAT_VOICEAGE_AMR_WB 	VoiceAge Corporation 	Not specified
            case 0xA105 	WAVE_FORMAT_ITI_G726 	Ingenient Technologies, Inc. 	Ingenient's G726
            case 0xA106 	WAVE_FORMAT_AAC 	Not specified. 	ISO/MPEG-4 advanced audio Coding
            case 0xA107 	WAVE_FORMAT_ESLG726 	Encore Software Ltd. 	Encore Software Ltd's G726
*/
            default: return "Unknown Audio (" + FormatTag + ")";
            }
         }
      }
      
      
      
      #region IEquatable
      public override int GetHashCode ()
      {
         unchecked
         {            return (int) (format_tag ^ channels ^ samples_per_second ^
                                      average_bytes_per_second ^ bits_per_sample);
         }
      }
      
      public override bool Equals (object obj)
      {
         if (!(obj is WaveFormatEx))
            return false;
         
         return Equals ((WaveFormatEx) obj);
      }
      
      public bool Equals (WaveFormatEx other)
      {
         return format_tag == other.format_tag && channels == other.channels &&
         samples_per_second == other.samples_per_second &&
         average_bytes_per_second == other.average_bytes_per_second &&
         bits_per_sample == other.bits_per_sample;
      }
      
      public static bool operator == (WaveFormatEx first, WaveFormatEx second)
      {
         return first.Equals (second);
      }
      
      public static bool operator != (WaveFormatEx first, WaveFormatEx second)
      {
         return !first.Equals (second);
      }
      #endregion
   }
}