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

using System;

namespace TagLib.Asf
{
   public class Properties : TagLib.Properties
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private TimeSpan duration;
      private uint width;
      private uint height;
      private short codec_id;
      private short channels;
      private uint sample_rate;
      private uint bytes_per_second;
      private short bits_per_pixel;
      private MediaTypes types;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Properties (HeaderObject header, ReadStyle style) : base (style)
      {
         duration         = TimeSpan.Zero;
         width            = 0;
         height           = 0;
         codec_id         = 0;
         channels         = 0;
         sample_rate      = 0;
         bytes_per_second = 0;
         bits_per_pixel   = 0;
         types = MediaTypes.Unknown;
         
         foreach (Object obj in header.Children)
         {
            if (obj is FilePropertiesObject)
               duration = ((FilePropertiesObject) obj).PlayDuration;
            
            if (obj is StreamPropertiesObject)
            {
               StreamPropertiesObject stream = obj as StreamPropertiesObject;
               
               if (bytes_per_second == 0 && stream.StreamType.Equals (Guid.AsfAudioMedia))
               {
                  types |= MediaTypes.Audio;
                  ByteVector data = stream.TypeSpecificData;
                  codec_id         = data.Mid (0, 2).ToShort (false);
                  channels         = data.Mid (2, 2).ToShort (false);
                  sample_rate      = data.Mid (4, 4).ToUInt  (false);
                  bytes_per_second = data.Mid (8, 4).ToUInt  (false);
               }
               else if (width == 0 && height == 0 && stream.StreamType.Equals (Guid.AsfVideoMedia))
               {
                  types |= MediaTypes.Video;
                  ByteVector data = stream.TypeSpecificData;
                  width = data.Mid (0, 4).ToUInt (false);
                  height = data.Mid (4, 4).ToUInt (false);
                  bits_per_pixel = data.Mid (25, 2).ToShort (false);
               }
            }
         }
      }
      
      public Properties (HeaderObject header) : this (header, ReadStyle.Average)
      {
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TimeSpan Duration        {get {return duration;}}
      public override int      AudioBitrate    {get {return (int) (bytes_per_second * 8 / 1000);}}
      public override int      AudioSampleRate {get {return (int) sample_rate;}}
      public override int      AudioChannels   {get {return channels;}}
      public override int      VideoWidth      {get {return (int) width;}}
      public override int      VideoHeight     {get {return (int) height;}}
      public override MediaTypes MediaTypes    {get {return types;}}

      public short CodecId {get {return codec_id;}}
      public short BitsPerPixel {get {return bits_per_pixel;}}
   }
}
