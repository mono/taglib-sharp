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
   [Flags]
   public enum FrameFlags : ushort
   {
      TagAlterPreservation  = 0x4000,
      FileAlterPreservation = 0x2000,
      ReadOnly              = 0x1000,
      GroupingIdentity      = 0x0040,
      Compression           = 0x0008,
      Encryption            = 0x0004,
      Unsychronisation      = 0x0002,
      DataLengthIndicator   = 0x0001
   }
   
   public struct FrameHeader
   {
      #region Properties
      private ReadOnlyByteVector frame_id;
      private uint frame_size;
      private FrameFlags flags;
      #endregion
      
      
      
      #region Constructors
      public FrameHeader (ByteVector data, byte version)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         flags = 0;
         frame_size = 0;
         
         switch (version)
         {
         case 2:
            if (data.Count < 3)
               throw new CorruptFileException ("Data must contain at least a frame ID.");

            // Set the frame ID -- the first three bytes
            frame_id = ConvertId (data.Mid (0, 3), version, false);
            
            // If the full header information was not passed in, do not continue
            // to the steps to parse the frame size and flags.
            if (data.Count < 6)
               return;
            
            frame_size = data.Mid (3, 3).ToUInt ();
            return;
            
         case 3:
            if (data.Count < 4)
               throw new CorruptFileException ("Data must contain at least a frame ID.");

            // Set the frame ID -- the first four bytes
            frame_id = ConvertId (data.Mid (0, 4), version, false);

            // If the full header information was not passed in, do not continue
            // steps to parse the frame size and flags.
            if (data.Count < 10)
               return;
            
            // Store the flags internally as version 2.4.
            frame_size = data.Mid (4, 4).ToUInt ();
            flags      = (FrameFlags) (((data [8] << 7) & 0x7000) |
                                       ((data [9] >> 4) & 0x000C) |
                                       ((data [9] << 1) & 0x0040));
            
            return;
            
         case 4:
            if (data.Count < 4)
               throw new CorruptFileException ("Data must contain at least a frame ID.");

            // Set the frame ID -- the first four bytes
            frame_id = new ReadOnlyByteVector (data.Mid (0, 4));

            // If the full header information was not passed in, do not continue
            // steps to parse the frame size and flags.
            if (data.Count < 10)
               return;
            
            frame_size = SynchData.ToUInt (data.Mid (4, 4));
            flags      = (FrameFlags) data.Mid (8, 2).ToUShort ();
            
            return;
            
         default:
            throw new CorruptFileException ("Unsupported tag version.");
         }
      }
      #endregion
      
      
      
      #region Public Methods
      public ByteVector Render (byte version)
      {
         ByteVector data = new ByteVector ();
         ByteVector id = ConvertId (frame_id, version, true);
         if (id == null)
            throw new NotImplementedException ();
         
         switch (version)
         {
         case 2:
            data.Add (id);
            data.Add (ByteVector.FromUInt (frame_size).Mid (1, 3));
            return data;
            
         case 3:
            ushort new_flags = (ushort) ((((ushort)flags << 1) & 0xE000) |
                                         (((ushort)flags << 4) & 0x00C0) |
                                         (((ushort)flags >> 1) & 0x0020));
            
            data.Add (id);
            data.Add (ByteVector.FromUInt (frame_size));
            data.Add (ByteVector.FromUShort (new_flags));
            
            return data;
            
         case 4:
            data.Add (id);
            data.Add (SynchData.FromUInt (frame_size));
            data.Add (ByteVector.FromUShort ((ushort) flags));
            
            return data;
            
         default:
            throw new NotImplementedException ("Unsupported tag version.");
         }
      }
      
      public static uint Size (byte version)
      {
         return (uint) (version < 3 ? 6 : 10);
      }
      #endregion
      
      #region Public Properties
      public ReadOnlyByteVector FrameId
      {
         get {return frame_id;}
         set {if (value != null) frame_id = value.Count == 4 ? value : new ReadOnlyByteVector (value.Mid (0, 4));}
      }
      
      public uint FrameSize
      {
         get {return frame_size;}
         set {frame_size = value;}
      }
      
      public FrameFlags Flags
      {
         get {return flags;}
         set
         {
            if ((value & (FrameFlags.Compression | FrameFlags.Encryption)) != 0)
               throw new UnsupportedFormatException ("Encryption not supported.");
            flags = value;
         }
      }
      #endregion
      
      
      
      #region Private Methods
      private static ReadOnlyByteVector ConvertId (ByteVector id, byte version, bool to_version)
      {
         if (version == 4)
            return id is ReadOnlyByteVector ? id as ReadOnlyByteVector : new ReadOnlyByteVector (id);
         
         if (id == null || version < 2)
            return null;
         
         if (!to_version && (id == FrameType.EQUA || id == FrameType.RVAD || id == FrameType.TRDA || id == FrameType.TSIZ))
            return null;
         
         
         if (version == 2)
         {
            for (int i = 0; i < 57; i ++)
            {
               if (to_version  && version2_frames [i,1] == id)
                  return version2_frames [i,0];
               
               if (!to_version && version2_frames [i,0] == id)
                  return version2_frames [i,1];
            }
         }
         
         if (version == 3)
         {
            for (int i = 0; i < 2; i ++)
            {
               if (to_version  && version3_frames [i,1] == id)
                  return version3_frames [i,0];
               
               if (!to_version && version3_frames [i,0] == id)
                  return version3_frames [i,1];
            }
         }
         
         if ((id.Count != 4 && version > 2) || (id.Count != 3 && version == 2))
            return null;
         
         return id is ReadOnlyByteVector ? id as ReadOnlyByteVector : new ReadOnlyByteVector (id);
      }
      
      private static readonly ReadOnlyByteVector [,] version2_frames = new ReadOnlyByteVector [57,2] {
         { "BUF", "RBUF" },
         { "CNT", "PCNT" },
         { "COM", "COMM" },
         { "CRA", "AENC" },
         { "ETC", "ETCO" },
         { "GEO", "GEOB" },
         { "IPL", "TIPL" },
         { "MCI", "MCDI" },
         { "MLL", "MLLT" },
         { "PIC", "APIC" },
         { "POP", "POPM" },
         { "REV", "RVRB" },
         { "SLT", "SYLT" },
         { "STC", "SYTC" },
         { "TAL", "TALB" },
         { "TBP", "TBPM" },
         { "TCM", "TCOM" },
         { "TCO", "TCON" },
         { "TCR", "TCOP" },
         { "TDA", "TDRC" },
         { "TDY", "TDLY" },
         { "TEN", "TENC" },
         { "TFT", "TFLT" },
         { "TKE", "TKEY" },
         { "TLA", "TLAN" },
         { "TLE", "TLEN" },
         { "TMT", "TMED" },
         { "TOA", "TOAL" },
         { "TOF", "TOFN" },
         { "TOL", "TOLY" },
         { "TOR", "TDOR" },
         { "TOT", "TOAL" },
         { "TP1", "TPE1" },
         { "TP2", "TPE2" },
         { "TP3", "TPE3" },
         { "TP4", "TPE4" },
         { "TPA", "TPOS" },
         { "TPB", "TPUB" },
         { "TRC", "TSRC" },
         { "TRD", "TDRC" },
         { "TRK", "TRCK" },
         { "TSS", "TSSE" },
         { "TT1", "TIT1" },
         { "TT2", "TIT2" },
         { "TT3", "TIT3" },
         { "TXT", "TOLY" },
         { "TXX", "TXXX" },
         { "TYE", "TDRC" },
         { "UFI", "UFID" },
         { "ULT", "USLT" },
         { "WAF", "WOAF" },
         { "WAR", "WOAR" },
         { "WAS", "WOAS" },
         { "WCM", "WCOM" },
         { "WCP", "WCOP" },
         { "WPB", "WPUB" },
         { "WXX", "WXXX" }
      };
      
      private static readonly ReadOnlyByteVector [,] version3_frames = new ReadOnlyByteVector [2,2] {
         { "TORY", "TDOR" },
         { "TYER", "TDRC" }
      };
      #endregion
   }
}
