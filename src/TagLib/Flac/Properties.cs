/***************************************************************************
    copyright            : (C) 2006 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : audioproperties.cpp from TagLib
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

namespace TagLib.Flac
{
   public class Properties : TagLib.Properties
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private TimeSpan duration;
      private int bitrate;
      private int sample_rate;
      private int sample_width;
      private int channels;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Properties (ByteVector data, long stream_length, ReadStyle style) : base (style)
      {
         duration     = TimeSpan.Zero;
         bitrate      = 0;
         sample_rate  = 0;
         sample_width = 0;
         channels     = 0;
         
         Read (data, stream_length, style);
      }

      public Properties (ByteVector data, long stream_length) : this (data, stream_length, ReadStyle.Average)
      {
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TimeSpan   Duration    {get {return duration;}}
      public override int        AudioBitrate     {get {return bitrate;}}
      public override int        AudioSampleRate  {get {return sample_rate;}}
      public override int        AudioChannels    {get {return channels;}}
      public override MediaTypes MediaTypes  {get {return MediaTypes.Audio;}}
      public          int        SampleWidth {get {return sample_width;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////

      private void Read (ByteVector data, long stream_length, ReadStyle style)
      {
         if (data.Count < 18)
            throw new CorruptFileException ("Not enough data in FLAC header.");

         int pos = 0;

         // Minimum block size (in samples)
         pos += 2;

         // Maximum block size (in samples)
         pos += 2;

         // Minimum frame size (in bytes)
         pos += 3;

         // Maximum frame size (in bytes)
         pos += 3;

         uint flags = data.Mid (pos, 4).ToUInt (true);
         sample_rate = (int) (flags >> 12);
         channels = (int) (((flags >> 9) & 7) + 1);
         sample_width = (int) (((flags >> 4) & 31) + 1);

         // The last 4 bits are the most significant 4 bits for the 36 bit
         // stream length in samples. (Audio files measured in days)

         double high_length = (double) (sample_rate > 0 ? (((flags & 0xf) << 28) / sample_rate) << 4 : 0);
         pos += 4;

         duration = sample_rate > 0 ? TimeSpan.FromSeconds ((double) data.Mid (pos, 4).ToUInt (true) / (double) sample_rate + high_length) : TimeSpan.Zero;
         pos += 4;

         // Uncompressed bitrate:
         
         //bitrate = ((sample_rate * channels) / 1000) * sample_width;
         
         // Real bitrate:
         bitrate = (int) (duration > TimeSpan.Zero ? ((stream_length * 8L) / duration.TotalSeconds) / 1000 : 0);
      }
   }
}
