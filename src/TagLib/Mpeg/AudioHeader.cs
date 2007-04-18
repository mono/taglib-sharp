/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : mpegheader.cpp from TagLib
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

namespace TagLib.Mpeg
{
   public enum Version
   {
      Version1   = 0, // MPEG Version 1
      Version2   = 1, // MPEG Version 2
      Version2_5 = 2  // MPEG Version 2.5
   }

   public enum ChannelMode
   {
      Stereo        = 0, // Stereo
      JointStereo   = 1, // Stereo
      DualChannel   = 2, // Dual Mono
      SingleChannel = 3  // Mono
   };

   public class AudioHeader
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Version     version;
      private int         layer;
      private bool        protection_enabled;
      private int         bitrate;
      private int         sample_rate;
      private bool        is_padded;
      private ChannelMode channel_mode;
      private bool        is_copyrighted;
      private bool        is_original;
      private int         frame_length;
      private long        position;
      private XingHeader  xing_header;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      private AudioHeader (long offset)
      {
          version            = Version.Version1;
          layer              = 0;
          protection_enabled = false;
          bitrate            = 0;
          sample_rate        = 0;
          is_padded          = false;
          channel_mode       = ChannelMode.Stereo;
          is_copyrighted     = false;
          is_original        = false;
          frame_length       = 0;
          position           = offset;
          xing_header        = null;
      }
      
      private AudioHeader (TagLib.File file, ByteVector data, long offset) : this (offset)
      {
         Parse (file, data);
      }
      
      public AudioHeader (TagLib.File file, long offset) : this (offset)
      {
         file.Seek (offset);
         Parse (file, file.ReadBlock (4));
      }
      
      public static AudioHeader Find (TagLib.File file, long position, int length)
      {
         long end = position + length;
         
         file.Seek (position);
         ByteVector buffer = file.ReadBlock (3);
         
         if (buffer.Count < 3)
            return null;
         
         do
         {
            file.Seek (position + 3);
            buffer = buffer.Mid (buffer.Count - 3);
            buffer.Add (file.ReadBlock ((int) File.BufferSize));
            
            for (int i = 0; i < buffer.Count - 3 && (length < 0 || position + i < end); i++)
               if (buffer [i] == 0xFF && SecondSynchByte (buffer [i + 1]))
                  try {return new AudioHeader (file, buffer.Mid (i, 4), position + i);} catch {}
            
            position += File.BufferSize;
         }
         while (buffer.Count > 3 && (length < 0 || position < end));
         
         return null;
      }
      
      public static AudioHeader Find (TagLib.File file, long position)
      {
         return Find (file, position, -1);
      }
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public Version     Version           {get {return version;}}
      public int         Layer             {get {return layer;}}
      public bool        ProtectionEnabled {get {return protection_enabled;}}
      public int         Bitrate           {get {return bitrate;}}
      public int         SampleRate        {get {return sample_rate;}}
      public bool        IsPadded          {get {return is_padded;}}
      public ChannelMode ChannelMode       {get {return channel_mode;}}
      public bool        IsCopyrighted     {get {return is_copyrighted;}}
      public bool        IsOriginal        {get {return is_original;}}
      public int         FrameLength       {get {return frame_length;}}
      public long        Position          {get {return position;}}
      public XingHeader  XingHeader        {get {return xing_header;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Parse (TagLib.File file, ByteVector data)
      {
         if(data.Count < 4 || data [0] != 0xff)
            throw new CorruptFileException ("First byte did not match MPEG synch.");

         uint flags = data.ToUInt();

         // Check for the second byte's part of the MPEG synch

         if ((flags & 0xFFE00000) != 0xFFE00000)
            throw new CorruptFileException ("Second byte did not match MPEG synch.");

         // Set the MPEG version
         switch ((flags >> 19) & 0x03)
         {
            case 0: version = Version.Version2_5; break;
            case 2: version = Version.Version2; break;
            case 3: version = Version.Version1; break;
         }

         // Set the MPEG layer
         switch ((flags >> 17) & 0x03)
         {
            case 1: layer = 3; break;
            case 2: layer = 2; break;
            case 3: layer = 1; break;
         }

         protection_enabled = ((flags >>16) & 1) == 0;

         // Set the bitrate
         int [,,] bitrates = new int [2,3,16] {
            { // Version 1
               { 0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448, -1 }, // layer 1
               { 0, 32, 48, 56, 64,  80,  96,  112, 128, 160, 192, 224, 256, 320, 384, -1 }, // layer 2
               { 0, 32, 40, 48, 56,  64,  80,  96,  112, 128, 160, 192, 224, 256, 320, -1 }  // layer 3
            },
            { // Version 2 or 2.5
               { 0, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256, -1 }, // layer 1
               { 0, 8,  16, 24, 32, 40, 48, 56,  64,  80,  96,  112, 128, 144, 160, -1 }, // layer 2
               { 0, 8,  16, 24, 32, 40, 48, 56,  64,  80,  96,  112, 128, 144, 160, -1 }  // layer 3
            }
         };

         int version_index = version == Version.Version1 ? 0 : 1;
         int layer_index = layer > 0 ? layer - 1 : 0;

         // The bitrate index is encoded as the first 4 bits of the 3rd byte,
         // i.e. 1111xxxx

         int i = (int) (flags >> 12) & 0x0F;

         bitrate = bitrates [version_index,layer_index,i];

         if (bitrate < 0)
            throw new CorruptFileException ("Header uses invalid bitrate index.");

         // Set the sample rate

         int [,] sample_rates = new int [3,4] {
            { 44100, 48000, 32000, 0 }, // Version 1
            { 22050, 24000, 16000, 0 }, // Version 2
            { 11025, 12000, 8000,  0 }  // Version 2.5
         };

         // The sample rate index is encoded as two bits in the 3nd byte,
         // i.e. xxxx11xx
         i = (int) (flags >> 10) & 0x03;

         sample_rate = sample_rates [(int) version,i];

         if(sample_rate == 0)
            throw new CorruptFileException ("Invalid sample rate.");

         // The channel mode is encoded as a 2 bit value at the end of the 3nd
         // byte, i.e. xxxxxx11
         channel_mode = (ChannelMode)((data[3] & 0xC0) >> 6);

         // TODO: Add mode extension for completeness

         is_original    = ((flags >> 2) & 1) == 1;
         is_copyrighted = ((flags >> 3) & 1) == 1;
         is_padded      = ((flags >> 9) & 1) == 1;

         // Calculate the frame length
         if(layer == 1)
            frame_length = (((12000 * bitrate) / sample_rate) + (IsPadded ? 1 : 0))*4;
         else if (layer == 2) 
            frame_length = (144000 * bitrate) / sample_rate + (IsPadded ? 1 : 0);
         else if (layer == 3)
            frame_length = (((144000 * bitrate) / sample_rate) / (version == Version.Version1 ? 1 : 2)) + (IsPadded ? 1 : 0);
         
         
         try
         {
            // Check for a Xing header that will help us in gathering
            // information about a VBR stream.
            file.Seek (Position + XingHeader.XingHeaderOffset (Version, ChannelMode));
            xing_header = new XingHeader (file.ReadBlock (16));
         }
         catch {}
      }
      
      private static bool SecondSynchByte (byte b)
      {
         return b >= 0xE0;
      }
   }
}
