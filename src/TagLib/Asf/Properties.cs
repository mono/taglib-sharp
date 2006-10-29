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
   public class Properties : TagLib.AudioProperties
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private TimeSpan duration;
      //private short codec_id;
      private short channels;
      private uint sample_rate;
      private uint bytes_per_second;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Properties (HeaderObject header, ReadStyle style) : base (style)
      {
         duration         = TimeSpan.Zero;
         //codec_id         = 0;
         channels         = 0;
         sample_rate      = 0;
         bytes_per_second = 0;
         
         foreach (Object obj in header.Children)
         {
            if (obj is FilePropertiesObject)
               duration = ((FilePropertiesObject) obj).PlayDuration;
            
            if (obj is StreamPropertiesObject && bytes_per_second == 0)
            {
               StreamPropertiesObject stream = (StreamPropertiesObject) obj;
               
               if (!stream.StreamType.Equals (Guid.AsfAudioMedia))
                  continue;
               
               ByteVector data = stream.TypeSpecificData;
               
               //codec_id         = data.Mid (0, 2).ToShort (false);
               channels         = data.Mid (2, 2).ToShort (false);
               sample_rate      = data.Mid (4, 4).ToUInt  (false);
               bytes_per_second = data.Mid (8, 4).ToUInt  (false);
            }
         }
      }
      
      public Properties (HeaderObject header) : this (header, ReadStyle.Average)
      {
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TimeSpan Duration   {get {return duration;}}
      public override int      Bitrate    {get {return (int) (bytes_per_second * 8 / 1000);}}
      public override int      SampleRate {get {return (int) sample_rate;}}
      public override int      Channels   {get {return channels;}}
   }
}
