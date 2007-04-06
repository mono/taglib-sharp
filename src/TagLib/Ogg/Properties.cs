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

using System.Collections.Generic;
using System;

namespace TagLib.Ogg
{
   public class Properties : TagLib.Properties
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private TimeSpan  duration;
      private int bitrate, sample_rate, channels, width, height;
      private MediaTypes types;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Properties (Dictionary<uint,Bitstream> streams, PageHeader last_header, ReadStyle style) : base (style)
      {
         bitrate = sample_rate = channels = width = height = 0;
         types = MediaTypes.Unknown;
         
         foreach (Bitstream stream in streams.Values)
         {
            types |= stream.Codec.MediaTypes;
            
            if ((stream.Codec.MediaTypes & MediaTypes.Audio) != MediaTypes.Unknown)
            {
               bitrate     = stream.Codec.AudioBitrate;
               sample_rate = stream.Codec.AudioSampleRate;
               channels    = stream.Codec.AudioChannels;
            }
            
            if ((stream.Codec.MediaTypes & MediaTypes.Video) != MediaTypes.Unknown)
            {
               width  = stream.Codec.VideoWidth;
               height = stream.Codec.VideoHeight;
            }
         }
         
         this.duration = streams [last_header.StreamSerialNumber].GetDuration (last_header.AbsoluteGranularPosition);
      }

      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TimeSpan Duration       {get {return duration;}}
      public override int        AudioBitrate    {get {return bitrate;}}
      public override int        AudioSampleRate {get {return sample_rate;}}
      public override int        AudioChannels   {get {return channels;}}
      public override int        VideoWidth      {get {return width;}}
      public override int        VideoHeight     {get {return height;}}
      public override MediaTypes MediaTypes      {get {return types;}}

   }
}
