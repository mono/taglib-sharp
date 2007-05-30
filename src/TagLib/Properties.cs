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

using System;
using System.Collections.Generic;

namespace TagLib
{
   public class Properties : IAudioCodec, IVideoCodec
   {
      private List<ICodec> codecs = new List<ICodec> ();
      private TimeSpan duration = TimeSpan.Zero;
      
      public TimeSpan Duration
      {
         get
         {
            TimeSpan duration = this.duration;
            
            if (duration != TimeSpan.Zero)
               return duration;
            
            foreach (ICodec codec in codecs)
               if (codec.Duration > duration)
                  duration = codec.Duration;
            
            return duration;
         }
      }
      
      public int AudioBitrate
      {
         get
         {
            foreach (ICodec codec in codecs)
               if (codec is IAudioCodec && (codec.MediaTypes | MediaTypes.Audio) == MediaTypes.Audio && (codec as IAudioCodec).AudioBitrate != 0)
                  return (codec as IAudioCodec).AudioBitrate;
            
            return 0;
         }
      }
      
      public int AudioSampleRate
      {
         get
         {
            foreach (ICodec codec in codecs)
               if (codec is IAudioCodec && (codec.MediaTypes | MediaTypes.Audio) == MediaTypes.Audio && (codec as IAudioCodec).AudioSampleRate != 0)
                  return (codec as IAudioCodec).AudioSampleRate;
            
            return 0;
         }
      }
      
      public int AudioChannels
      {
         get
         {
            foreach (ICodec codec in codecs)
               if (codec is IAudioCodec && (codec.MediaTypes | MediaTypes.Audio) == MediaTypes.Audio && (codec as IAudioCodec).AudioChannels != 0)
                  return (codec as IAudioCodec).AudioChannels;
            
            return 0;
         }
      }
      
      public int VideoWidth
      {
         get
         {
            foreach (ICodec codec in codecs)
               if (codec is IVideoCodec && (codec.MediaTypes | MediaTypes.Video) == MediaTypes.Video && (codec as IVideoCodec).VideoWidth != 0)
                  return (codec as IVideoCodec).VideoWidth;
            
            return 0;
         }
      }
      
      public int VideoHeight
      {
         get
         {
            foreach (ICodec codec in codecs)
               if (codec is IVideoCodec && (codec.MediaTypes | MediaTypes.Video) == MediaTypes.Video && (codec as IVideoCodec).VideoHeight != 0)
                  return (codec as IVideoCodec).VideoHeight;
            
            return 0;
         }
      }
      
      public MediaTypes MediaTypes
      {
         get
         {
            MediaTypes types = MediaTypes.None;
            
            foreach (ICodec codec in codecs)
               if (codec != null)
                  types |= codec.MediaTypes;
            
            return types;
         }
      }
      
      public string Description
      {
         get
         {
            StringCollection l = new StringCollection ();
            foreach (ICodec codec in codecs)
               if (codec != null)
                  l.Add (codec.Description);
            
            return string.Join ("; ", l.ToArray ());
         }
      }
      
      public IEnumerable<ICodec> Codecs {get {return codecs;}}
      
      public Properties ()
      {
      }
      
      public Properties (TimeSpan duration, params ICodec[] codecs)
      {
         this.duration = duration;
         this.codecs.AddRange (codecs);
      }
      
      public Properties (TimeSpan duration, IEnumerable<ICodec> codecs)
      {
         this.duration = duration;
         this.codecs.AddRange (codecs);
      }
   }
}
