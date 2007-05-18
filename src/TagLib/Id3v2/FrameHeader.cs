/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2frame.cpp from TagLib
 ***************************************************************************/

/***************************************************************************
 *   This library is free software; you can redistribute it and/or modify  *
 *   it  under the terms of the GNU Lesser General Public License version  *
 *   2.1 as published by the Free Software Foundation.                     *
 *                                                                         *
 *   This library is distributed in the hope that it will be useful, but   *
 *   WITHOUT ANY WARRANTY; without even the implied warranty of            *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU     *
 *   Lesser General Public License for more details.                       *
 *                                                                         *
 *   You should have received a copy of the GNU Lesser General Public      *
 *   License along with this library; if not, write to the Free Software   *
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  *
 *   USA                                                                   *
 ***************************************************************************/

using System.Collections;
using System;

namespace TagLib.Id3v2
{
   public class FrameHeader
   {
      #region Properties
      private ByteVector frame_id = null;
      private uint frame_size = 0;
      private ushort flags = 0;
      #endregion
      
      
      
      #region Constructors
      public FrameHeader (ByteVector data, uint version)
      {
         SetData (data, version);
      }
      
      public FrameHeader (ByteVector data, Header header) : this (data, header.MajorVersion)
      {
      }
      #endregion
      
      
      
      #region Public Methods
      private void SetData (ByteVector data, uint version)
      {
         switch (version)
         {
         case 2:
            if (data.Count < 3)
               throw new CorruptFileException ("Data must contain at least a frame ID.");

            // Set the frame ID -- the first three bytes
            frame_id = data.Mid (0, 3);

            // If the full header information was not passed in, do not continue
            // to the steps to parse the frame size and flags.
            frame_size = (data.Count < 6) ? 0 : data.Mid (3, 3).ToUInt ();
            
            flags = 0;
            
            return;
            
         case 3:
            if (data.Count < 4)
               throw new CorruptFileException ("Data must contain at least a frame ID.");

            // Set the frame ID -- the first four bytes
            frame_id = data.Mid (0, 4);

            // If the full header information was not passed in, do not continue
            // steps to parse the frame size and flags.
            frame_size = (data.Count < 10) ? 0 : data.Mid (4, 4).ToUInt ();
            
            if (data.Count < 10)
               return;
            
            // Store the flags internally as version 2.4.
            flags = (ushort) (((data [8] << 7) & 0x7000) |
                              ((data [9] >> 4) & 0x000C) |
                              ((data [9] << 1) & 0x0040));
            
            return;
            
         case 4:
            if (data.Count < 4)
               throw new CorruptFileException ("Data must contain at least a frame ID.");

            // Set the frame ID -- the first four bytes
            frame_id = data.Mid (0, 4);

            // If the full header information was not passed in, do not continue
            // steps to parse the frame size and flags.
            frame_size = (data.Count < 10) ? 0 : SynchData.ToUInt (data.Mid (4, 4));
            
            if (data.Count < 10)
               return;
            
            flags = data.Mid (8, 2).ToUShort ();
            
            return;
            
         default:
            throw new CorruptFileException ("Unsupported tag version.");
         }
      }
      
      public ByteVector Render (uint version)
      {
         ByteVector data = new ByteVector ();
         
         switch (version)
         {
         case 2:
            data.Add (ConvertId (frame_id, version));
            data.Add (ByteVector.FromUInt (frame_size).Mid (1, 3));
            return data;
            
         case 3:
            ushort new_flags = (ushort) (((flags << 1) & 0xE000) |
                                         ((flags << 4) & 0x00C0) |
                                         ((flags >> 1) & 0x0020));
            
            data.Add (ConvertId (frame_id, version));
            data.Add (ByteVector.FromUInt (frame_size));
            data.Add (ByteVector.FromUShort (new_flags));
            
            return data;
            
         case 4:
            data.Add (frame_id.Mid (0, 4));
            data.Add (SynchData.FromUInt (frame_size));
            data.Add (ByteVector.FromUShort (flags));
            
            return data;
            
         default:
            throw new CorruptFileException ("Unsupported tag version.");
         }
      }
      
      public ByteVector Render (Header header)
      {
         return Render (header.MajorVersion);
      }
      
      public static uint Size (uint version)
      {
         return (uint) (version < 3 ? 6 : 10);
      }
      
      public static uint Size (Header header)
      {
         return Size (header.MajorVersion);
      }
      #endregion
      
      #region Private Methods
      private ByteVector ConvertId (ByteVector frame_id, uint version)
      {
         switch (version)
         {
            case 2:
               switch (frame_id.ToString ())
               {
                  case "RBUF": return "BUF";
                  case "PCNT": return "CNT";
                  case "COMM": return "COM";
                  case "AENC": return "CRA";
                  case "ETCO": return "ETC";
                  case "GEOB": return "GEO";
                  case "TIPL": return "IPL";
                  case "MCDI": return "MCI";
                  case "MLLT": return "MLL";
                  case "APIC": return "PIC";
                  case "POPM": return "POP";
                  case "RVRB": return "REV";
                  case "SYLT": return "SLT";
                  case "SYTC": return "STC";
                  case "TALB": return "TAL";
                  case "TBPM": return "TBP";
                  case "TCOM": return "TCM";
                  case "TCON": return "TCO";
                  case "TCOP": return "TCR";
                  case "TDRC": return "TDA";
                  case "TDLY": return "TDY";
                  case "TENC": return "TEN";
                  case "TFLT": return "TFT";
                  case "TKEY": return "TKE";
                  case "TLAN": return "TLA";
                  case "TLEN": return "TLE";
                  case "TMED": return "TMT";
                  case "TOAL": return "TOA";
                  case "TOFN": return "TOF";
                  case "TOLY": return "TOL";
                  case "TDOR": return "TOR";
                  case "TPE1": return "TP1";
                  case "TPE2": return "TP2";
                  case "TPE3": return "TP3";
                  case "TPE4": return "TP4";
                  case "TPOS": return "TBA";
                  case "TPUB": return "TPB";
                  case "TSRC": return "TRC";
                  case "TRCK": return "TRK";
                  case "TSSE": return "TSS";
                  case "TIT1": return "TT1";
                  case "TIT2": return "TT2";
                  case "TIT3": return "TT3";
                  case "TXXX": return "TXX";
                  case "UFID": return "UFI";
                  case "USLT": return "ULT";
                  case "WOAF": return "WAF";
                  case "WOAR": return "WAR";
                  case "WOAS": return "WAS";
                  case "WCOM": return "WCM";
                  case "WCOP": return "WCP";
                  case "WPUB": return "WPB";
                  case "WXXX": return "WXX";
                  default:     return frame_id;
               }

            case 3:
               switch (frame_id.ToString ())
               {
                  case "TDOR": return "TORY";
                  case "TDRC": return "TYER";
                  default:     return frame_id;
               }

            default:
               return frame_id;
         }
      }
      #endregion
      
      #region Public Properties
      public ByteVector FrameId
      {
         get {return frame_id;}
         set {if (value != null) frame_id = value.Mid (0, 4);}
      }
      
      public uint FrameSize
      {
         get {return frame_size;}
         set {frame_size = value;}
      }
      
      public bool TagAlterPreservation
      {
         get {return (flags & 0x4000) == 0x4000;}
         set {if (value) flags |= 0x4000; else flags &= 0xBFFF;}
      }
      
      public bool FileAlterPreservation
      {
         get {return (flags & 0x2000) == 0x2000;}
         set {if (value) flags |= 0x2000; else flags &= 0xDFFF;}
      }
      
      public bool ReadOnly
      {
         get {return (flags & 0x1000) == 0x1000;}
         set {if (value) flags |= 0x1000; else flags &= 0xEFFF;}
      }
      
      public bool GroupingIdentity
      {
         get {return (flags & 0x0040) == 0x0040;}
         set {if (value) flags |= 0x0040; else flags &= 0xFFBF;}
      }
      
      public bool Compression
      {
         get {return (flags & 0x0008) == 0x0008;}
         set {if (value) throw new UnsupportedFormatException ("Compression not supported.");}
         /*set {if (value) flags |= 0x0008; else flags &= 0xFFF7;}*/
      }
      
      public bool Encryption
      {
         get {return (flags & 0x0004) == 0x0004;}
         set {if (value) throw new UnsupportedFormatException ("Encryption not supported.");}
         /*set {if (value) flags |= 0x0004; else flags &= 0xFFFB;}*/
      }
      
      public bool Unsychronisation
      {
         get {return (flags & 0x0002) == 0x0002;}
         set {if (value) flags |= 0x0002; else flags &= 0xFFFD;}
      }
      
      public bool DataLengthIndicator
      {
         get {return (flags & 0x0001) == 0x0001;}
         set {if (value) flags |= 0x0001; else flags &= 0xFFFE;}
      }
      #endregion
   }
}
