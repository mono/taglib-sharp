/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : mpegproperties.cpp from TagLib
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
   public class Properties : TagLib.Properties
   {
      private Version version;
      private VideoHeader video_header;
      private AudioHeader audio_header;
      private TimeSpan duration;
      private long length;
      
      public Properties (Version version, VideoHeader video_header, AudioHeader audio_header, double duration, long length, ReadStyle style) : base (style)
      {
         this.version = version;
         this.video_header = video_header;
         this.audio_header = audio_header;
         this.duration = TimeSpan.FromSeconds (duration);
         this.length = length;
      }
      
      public override TimeSpan Duration
      {
         get
         {
            if (duration <= TimeSpan.Zero && audio_header != null)
            {
               if (audio_header.XingHeader != null)
               {
                  // Read the length and the bitrate from the Xing header.
      
                  int [,] block_size = new int [3,4] {
                     { 0, 384, 1152, 1152 }, // Version 1
                     { 0, 384, 1152, 576 },  // Version 2
                     { 0, 384, 1152, 576 }   // Version 2.5
                  };
                  
                  double time_per_frame = (double) block_size [(int) audio_header.Version, audio_header.Layer] / (double) audio_header.SampleRate;
                  duration = TimeSpan.FromSeconds (time_per_frame * audio_header.XingHeader.TotalFrames);
               }
               else if (audio_header.FrameLength > 0 && audio_header.Bitrate > 0)
               {
                  // Since there was no valid Xing header found, we hope that we're
                  // in a constant bitrate file.
                  int frames = (int) ((length - audio_header.Position) / audio_header.FrameLength + 1);
                  duration = TimeSpan.FromSeconds ((double) (audio_header.FrameLength * frames) / (double) (audio_header.Bitrate * 125) + 0.5);
               }
            }
            
            return duration;
         }
      }
      
      public override int AudioBitrate
      {
         get
         {
            if (audio_header == null)
               return 0;
            
            if (audio_header.XingHeader != null)
               return (int) (Duration > TimeSpan.Zero ? ((audio_header.XingHeader.TotalSize * 8L) / Duration.TotalSeconds) / 1000 : 0);
            
            
            return audio_header.Bitrate;
         }
      }
      public override int         AudioSampleRate    {get {return audio_header == null ? 0 : audio_header.SampleRate;}}
      public override int         AudioChannels      {get {return audio_header == null ? 0 : (audio_header.ChannelMode == ChannelMode.SingleChannel ? 1 : 2);}}
      public          int         AudioLayer         {get {return audio_header == null ? 0 : audio_header.Layer;}}
      public          ChannelMode AudioChannelMode   {get {return audio_header == null ? 0 : audio_header.ChannelMode;}}
      public          bool        AudioIsCopyrighted {get {return audio_header == null ? false : audio_header.IsCopyrighted;}}
      public          bool        AudioIsOriginal    {get {return audio_header == null ? false : audio_header.IsOriginal;}}
      
      public override int         VideoWidth         {get {return video_header == null ? 0 : video_header.Width;}}
      public override int         VideoHeight        {get {return video_header == null ? 0 : video_header.Height;}}
      public          int         VideoBitrate       {get {return video_header == null ? 0 : video_header.Bitrate;}}
      public          double      VideoFrameRate     {get {return video_header == null ? 0 : video_header.FrameRate;}}
      
      public          Version     Version            {get {return version;}}
      public override MediaTypes  MediaTypes
      {
         get
         {
            MediaTypes types = MediaTypes.Unknown;
            if (audio_header != null)
               types |= MediaTypes.Audio;
            if (video_header != null)
               types |= MediaTypes.Video;
            
            return types;
         }
      }
   }
}
