/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : mpcproperties.cpp from TagLib
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

namespace TagLib.Mpc
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
      
      private static ushort [] sftable = {44100, 48000, 37800, 32000};
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Properties (ByteVector data, long stream_length, ReadStyle style) : base (style)
      {
         version     = 0;
         duration    = TimeSpan.Zero;
         bitrate     = 0;
         sample_rate = 0;
         channels    = 0;
         
         Read (data, stream_length, style);
      }
      
      public Properties (ByteVector data, long stream_length) : this (data, stream_length, ReadStyle.Average)
      {
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public static readonly uint HeaderSize = 8 * 7;
      
      public override TimeSpan Duration   {get {return duration;}}
      public override int      AudioBitrate    {get {return bitrate;}}
      public override int      AudioSampleRate {get {return sample_rate;}}
      public override int      AudioChannels   {get {return channels;}}
      public override MediaTypes MediaTypes  {get {return MediaTypes.Audio;}}
      public          int      MpcVersion {get {return version;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Read (ByteVector data, long stream_length, ReadStyle style)
      {
         if (!data.StartsWith ("MP+"))
            throw new CorruptFileException ();

         version = data [3] & 15;

         uint frames;

         if (version >= 7)
         {
            frames = data.Mid (4, 4).ToUInt (false);
            uint flags = data.Mid (8, 4).ToUInt (false);
            sample_rate = sftable [(int) (((flags >> 17) & 1) * 2 + ((flags >> 16) & 1))];
            channels = 2;
         }
         else
         {
            uint header_data = data.Mid (0, 4).ToUInt (false);
            bitrate = (int) ((header_data >> 23) & 0x01ff);
            version = (int) ((header_data >> 11) & 0x03ff);
            sample_rate = 44100;
            channels = 2;
            if (version >= 5)
               frames = data.Mid (4, 4).ToUInt (false);
            else
               frames = data.Mid (4, 2).ToUInt (false);
         }

         uint samples = frames * 1152 - 576;
         duration = sample_rate > 0 ? TimeSpan.FromSeconds ((double) samples / (double) sample_rate + 0.5) : TimeSpan.Zero;

         if (bitrate == 0)
            bitrate = (int) (duration > TimeSpan.Zero ? ((stream_length * 8L) / duration.TotalSeconds) / 1000 : 0);
      }
   }
}
