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
   public class StreamHeader : IAudioCodec
   {
#region Private Properties
      private uint     flags         = 0;
      private int      sample_rate   = 0;
      private int      sample_width  = 0;
      private int      channels      = 0;
      private uint     low_length    = 0;
      private long     stream_length = 0;
#endregion
      
#region Constructors
      public StreamHeader (ByteVector data, long stream_length)
      {
         if (data.Count < 18)
            throw new CorruptFileException ("Not enough data in FLAC header.");
         
         this.stream_length = stream_length;
         flags = data.Mid (10, 4).ToUInt (true);
         sample_rate = (int) (flags >> 12);
         channels = (int) (((flags >> 9) & 7) + 1);
         sample_width = (int) (((flags >> 4) & 31) + 1);
         low_length = data.Mid (14, 4).ToUInt (true);
         
         // Real bitrate:
      }
#endregion

#region IAudioCodec Properties
      public TimeSpan Duration
      {
         get
         {
            if (sample_rate > 0 && stream_length > 0)
               return TimeSpan.FromSeconds ((double) low_length / (double) sample_rate + (double) HighLength);
            else
               return TimeSpan.Zero;
         }
      }
      
      public int AudioBitrate
      {
         get
         {
            return  (int) (Duration > TimeSpan.Zero ? ((stream_length * 8L) / Duration.TotalSeconds) / 1000 : 0);
         }
      }
      public int        AudioSampleRate  {get {return sample_rate;}}
      public int        AudioChannels    {get {return channels;}}
      public MediaTypes MediaTypes       {get {return MediaTypes.Audio;}}
      public int        SampleWidth      {get {return sample_width;}}
      public string     Description      {get {return "Flac Audio";}}
      
      private uint HighLength
      {
         get
         {
            // The last 4 bits are the most significant 4 bits for the 36 bit
            // stream length in samples. (Audio files measured in days)
            return (uint)(sample_rate > 0 ? (((flags & 0xf) << 28) / sample_rate) << 4 : 0);
         }
      }
#endregion
   }
}
