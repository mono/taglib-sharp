/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : wvproperties.cpp from libtunepimp
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

namespace TagLib.WavPack
{
   public class StreamHeader : IAudioCodec
   {
      #region Constants
      private static readonly uint [] sample_rates = {6000, 8000, 9600, 11025, 12000,
         16000, 22050, 24000, 32000, 44100, 48000, 64000, 88200, 96000, 192000};
      
      private static readonly int  BYTES_STORED = 3;
      private static readonly int  MONO_FLAG    = 4;
      private static readonly int  SHIFT_LSB   = 13;
      private static readonly long SHIFT_MASK  = (0x1fL << SHIFT_LSB);
      private static readonly int  SRATE_LSB   = 23;
      private static readonly long SRATE_MASK  = (0xfL << SRATE_LSB);
      #endregion
      
      
      
      #region Private Properties
      private long stream_length;
      private ushort version;
      private uint flags;
      private uint samples;
      #endregion
      
      
      
      #region Public Static Properties
      public static readonly uint Size = 32;
      #endregion
      
      
      
      #region Constructors
      public StreamHeader (ByteVector data, long stream_length)
      {
         if (!data.StartsWith ("wvpk"))
            throw new CorruptFileException ();
         
         this.stream_length = stream_length;
         version = data.Mid (8, 2).ToUShort (false);
         flags   = data.Mid (24, 4).ToUInt (false);
         samples = data.Mid (12, 4).ToUInt (false);
      }
      #endregion
      
      
      
      #region Public Properties
      public TimeSpan Duration
      {
         get
         {
            return AudioSampleRate > 0 ? TimeSpan.FromSeconds ((double) samples / (double) AudioSampleRate + 0.5) : TimeSpan.Zero;
         }
      }
      
      public int AudioBitrate
      {
         get
         {
            return (int) (Duration > TimeSpan.Zero ? ((stream_length * 8L) / Duration.TotalSeconds) / 1000 : 0);
         }
      }
      
      public int AudioSampleRate
      {
         get
         {
            return (int) (sample_rates [(flags & SRATE_MASK) >> SRATE_LSB]);
         }
      }
      
      public int        AudioChannels {get {return ((flags & MONO_FLAG) != 0) ? 1 : 2;}}
      
      public MediaTypes MediaTypes    {get {return MediaTypes.Audio;}}
      
      public int        Version       {get {return version;}}
      
      public int BitsPerSample
      {
         get
         {
            return (int) (((flags & BYTES_STORED) + 1) * 8 - ((flags & SHIFT_MASK) >> SHIFT_LSB));
         }
      }
      
      public string Description {get {return "WavPack Version " + Version + " Audio";}}
      #endregion
   }
}
