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
   public class Properties : TagLib.Properties
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private int version;
      private TimeSpan duration;
      private int bitrate;
      private int sample_rate;
      private int channels;
      private int bits_per_sample;
      
      private static uint [] sample_rates = {6000, 8000, 9600, 11025, 12000,
         16000, 22050, 24000, 32000, 44100, 48000, 64000, 88200, 96000, 192000};
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Properties (ByteVector data, long stream_length, ReadStyle style) : base (style)
      {
         version         = 0;
         duration        = TimeSpan.Zero;
         bitrate         = 0;
         sample_rate     = 0;
         channels        = 0;
         bits_per_sample = 0;
         
         Read (data, stream_length, style);
      }
      
      public Properties (ByteVector data, long stream_length) : this (data, stream_length, ReadStyle.Average)
      {
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public static readonly uint HeaderSize = 32;
      
      public override TimeSpan Duration      {get {return duration;}}
      public override int      AudioBitrate       {get {return bitrate;}}
      public override int      AudioSampleRate    {get {return sample_rate;}}
      public override int      AudioChannels      {get {return channels;}}
      public override MediaTypes MediaTypes  {get {return MediaTypes.Audio;}}
      public          int      Version       {get {return version;}}
      public          int      BitsPerSample {get {return bits_per_sample;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Read (ByteVector data, long stream_length, ReadStyle style)
      {
         int  BYTES_STORED = 3;
         int  MONO_FLAG    = 4;

         int  SHIFT_LSB   = 13;
         long SHIFT_MASK  = (0x1fL << SHIFT_LSB);

         int  SRATE_LSB   = 23;
         long SRATE_MASK  = (0xfL << SRATE_LSB);
         
         if (!data.StartsWith ("wvpk"))
            throw new CorruptFileException ();

         version = data.Mid (8, 2).ToShort (false);
         
         uint flags = data.Mid (24, 4).ToUInt (false);
         bits_per_sample = (int) (((flags & BYTES_STORED) + 1) * 8 - ((flags & SHIFT_MASK) >> SHIFT_LSB));
         sample_rate = (int) (sample_rates [(flags & SRATE_MASK) >> SRATE_LSB]);
         channels = ((flags & MONO_FLAG) != 0) ? 1 : 2;
  
         uint samples = data.Mid (12, 4).ToUInt (false);
         duration = sample_rate > 0 ? TimeSpan.FromSeconds ((double) samples / (double) sample_rate + 0.5) : TimeSpan.Zero;
         bitrate = (int) (duration > TimeSpan.Zero ? ((stream_length * 8L) / duration.TotalSeconds) / 1000 : 0);
      }
   }
}
