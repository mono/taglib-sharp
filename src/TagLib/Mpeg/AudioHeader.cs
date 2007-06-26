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
#region Enums
   public enum Version
   {
      Unknown   = -1,
      Version1  =  0, // MPEG Version 1
      Version2  =  1, // MPEG Version 2
      Version25 =  2  // MPEG Version 2.5
   }

   public enum ChannelMode
   {
      Stereo        = 0, // Stereo
      JointStereo   = 1, // Stereo
      DualChannel   = 2, // Dual Mono
      SingleChannel = 3  // Mono
   };
#endregion
   
   public struct AudioHeader : IAudioCodec
   {
      #region Private Static Value Arrays
      private static readonly int [,] sample_rates = new int [3,4] {
         { 44100, 48000, 32000, 0 }, // Version 1
         { 22050, 24000, 16000, 0 }, // Version 2
         { 11025, 12000, 8000,  0 }  // Version 2.5
      };
      
      private static readonly int [,] block_size = new int [3,4] {
         { 0, 384, 1152, 1152 }, // Version 1
         { 0, 384, 1152, 576 },  // Version 2
         { 0, 384, 1152, 576 }   // Version 2.5
      };

      private static readonly int [,,] bitrates = new int [2,3,16] {
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
      #endregion
      
      #region Private Properties
      private uint        flags;
      private long        position;
      private long        stream_length;
      private XingHeader  xing_header;
      private TimeSpan    duration;
      #endregion
      
      public static readonly AudioHeader Unknown = new AudioHeader (0,0,0,XingHeader.Unknown);
      
      #region Constructors
      private AudioHeader (uint flags, long position, long streamLength, XingHeader xing_header)
      {
         this.flags = flags;
         this.position = position;
         this.stream_length = streamLength;
         this.xing_header = xing_header;
         this.duration = TimeSpan.Zero;
      }
      
      private AudioHeader (ByteVector data, TagLib.File file, long position)
      {
         this.duration = TimeSpan.Zero;
         this.position = position;
         stream_length = 0;
         if (data.Count < 4)
            throw new CorruptFileException ("Insufficient header length.");
         
         if (data [0] != 0xFF)
            throw new CorruptFileException ("First byte did not match MPEG synch.");
         
         if (data [1] < 0xE0)
            throw new CorruptFileException ("Second byte did not match MPEG synch.");

         flags = data.ToUInt ();
         xing_header = XingHeader.Unknown;
         
         if (AudioBitrate < 0)
            throw new CorruptFileException ("Header uses invalid bitrate index.");
         
         if (AudioSampleRate == 0)
            throw new CorruptFileException ("Invalid sample rate.");

         try
         {
            // Check for a Xing header that will help us in gathering
            // information about a VBR stream.
            file.Seek (position + XingHeader.XingHeaderOffset (Version, ChannelMode));
            xing_header = new XingHeader (file.ReadBlock (16));
         }
         catch (CorruptFileException)
         {
         }
      }
#endregion

#region Public Properties
      public Version Version
      {
         get
         {
            switch ((flags >> 19) & 0x03)
            {
            case 0:  return Version.Version25;
            case 2:  return Version.Version2;
            default: return Version.Version1;
            }
         }
      }
      
      public int AudioLayer
      {
         get
         {
            switch ((flags >> 17) & 0x03)
            {
            case 1:  return 3;
            case 2:  return 2;
            default: return 1;
            }
         }
      }
      
      public int AudioBitrate
      {
         get
         {
            if (xing_header.TotalSize > 0 && duration > TimeSpan.Zero)
               return (int) (((XingHeader.TotalSize * 8L) / duration.TotalSeconds) / 1000);
            
            int version_index = Version == Version.Version1 ? 0 : 1;
            int layer_index = AudioLayer > 0 ? AudioLayer - 1 : 0;
            int bitrate_index = (int) (flags >> 12) & 0x0F;
            return bitrates [version_index, layer_index, bitrate_index];
         }
      }
      
      public int AudioSampleRate
      {
         get
         {
            return sample_rates [(int) Version, (int) (flags >> 10) & 0x03];
         }
      }
      
      public int AudioFrameLength
      {
         get
         {
            // Calculate the frame length
            switch (AudioLayer)
            {
            case 1:  return (((12000  * AudioBitrate) / AudioSampleRate) + (IsPadded ? 1 : 0))*4;
            case 2:  return   (144000 * AudioBitrate) / AudioSampleRate + (IsPadded ? 1 : 0);
            case 3:  return (((144000 * AudioBitrate) / AudioSampleRate) / (Version == Version.Version1 ? 1 : 2)) + (IsPadded ? 1 : 0);
            default: return 0;
            }
         }
      }
      
      public TimeSpan Duration
      {
         get
         {
            if (duration > TimeSpan.Zero)
               return duration;
            
            if (xing_header.TotalFrames > 0)
            {
               // Read the length and the bitrate from the Xing header.
               double time_per_frame = (double) block_size [(int) Version, AudioLayer] / (double) AudioSampleRate;
               duration = TimeSpan.FromSeconds (time_per_frame * XingHeader.TotalFrames);
            }
            else if (AudioFrameLength > 0 && AudioBitrate > 0)
            {
               // Since there was no valid Xing header found, we hope that we're
               // in a constant bitrate file.
               int frames = (int) ((stream_length - position) / AudioFrameLength + 1);
               duration = TimeSpan.FromSeconds ((double) (AudioFrameLength * frames) / (double) (AudioBitrate * 125) + 0.5);
            }
            
            return duration;
         }
      }
      
      public string Description
      {
         get
         {
            System.Text.StringBuilder builder = new System.Text.StringBuilder ();
            builder.Append ("MPEG Version ");
            switch (Version)
            {
            case Version.Version1:  builder.Append ("1");   break;
            case Version.Version2:  builder.Append ("2");   break;
            case Version.Version25: builder.Append ("2.5"); break;
            }
            builder.Append (" Audio, Layer ");
            builder.Append (AudioLayer);
            
            if (xing_header.Present)
               builder.Append (" VBR");
            
            return builder.ToString ();
         }
      }
      
      public bool        IsProtected   {get {return ((flags >>16) & 1) == 0;}}
      public bool        IsPadded      {get {return ((flags >> 9) & 1) == 1;}}
      public bool        IsCopyrighted {get {return ((flags >> 3) & 1) == 1;}}
      public bool        IsOriginal    {get {return ((flags >> 2) & 1) == 1;}}
      public int         AudioChannels {get {return ChannelMode == ChannelMode.SingleChannel ? 1 : 2;}}
      public ChannelMode ChannelMode   {get {return (ChannelMode) ((flags >> 14) & 0x03);}}
      public MediaTypes  MediaTypes    {get {return MediaTypes.Audio;}}
      public XingHeader  XingHeader    {get {return xing_header;}}
#endregion
      
#region Public Methods
      public void SetStreamLength (long streamLength)
      {
         this.stream_length = streamLength;
      }
#endregion
      
#region Public Static Methods
      public static bool Find (out AudioHeader header, TagLib.File file, long position, int length)
      {
         if (file == null)
            throw new ArgumentNullException ("file");
         
         long end = position + length;
         
         header = AudioHeader.Unknown;
         
         file.Seek (position);
         ByteVector buffer = file.ReadBlock (3);
         
         if (buffer.Count < 3)
            return false;
         
         do
         {
            file.Seek (position + 3);
            buffer = buffer.Mid (buffer.Count - 3);
            buffer.Add (file.ReadBlock ((int) File.BufferSize));
            
            for (int i = 0; i < buffer.Count - 3 && (length < 0 || position + i < end); i++)
               if (buffer [i] == 0xFF && buffer [i + 1] > 0xE0)
                  try
                  {
                     header = new AudioHeader (buffer.Mid (i, 4), file, position + i);
                     return true;
                  } catch (CorruptFileException) {}
            
            position += File.BufferSize;
         }
         while (buffer.Count > 3 && (length < 0 || position < end));
         
         return false;
      }
      
      public static bool Find (out AudioHeader header, TagLib.File file, long position)
      {
         return Find (out header, file, position, -1);
      }
#endregion
   }
}
