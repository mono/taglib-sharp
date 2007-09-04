//
// StreamHeader.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   flagproperties.cpp from TagLib
//
// Copyright (C) 2006-2007 Brian Nickel
// Copyright (C) 2003 Allan Sandfeld Jensen (Original Implementation)
// 
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

using System.Collections;
using System;

namespace TagLib.Flac
{
   public struct StreamHeader : IAudioCodec
   {
      #region Private Properties
      private uint flags;
      private uint low_length;
      private long stream_length;
      #endregion
      
      
      
      #region Constructors
      public StreamHeader (ByteVector data, long streamLength)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         if (data.Count < 18)
            throw new CorruptFileException ("Not enough data in FLAC header.");
         
         this.stream_length = streamLength;
         this.flags = data.Mid (10, 4).ToUInt (true);
         low_length = data.Mid (14, 4).ToUInt (true);
      }
      #endregion

      #region Public Properties
      public TimeSpan Duration
      {
         get
         {
            return (AudioSampleRate > 0 && stream_length > 0) ?
               TimeSpan.FromSeconds ((double) low_length / (double) AudioSampleRate + (double) HighLength) :
               TimeSpan.Zero;
         }
      }
      
      public int AudioBitrate
      {
         get
         {
            return  (int) (Duration > TimeSpan.Zero ? ((stream_length * 8L) / Duration.TotalSeconds) / 1000 : 0);
         }
      }
      
      public int        AudioSampleRate  {get {return (int) (flags >> 12);}}
      public int        AudioChannels    {get {return (int) (((flags >> 9) & 7) + 1);}}
      public MediaTypes MediaTypes       {get {return MediaTypes.Audio;}}
      public int        AudioSampleWidth {get {return (int) (((flags >> 4) & 31) + 1);}}
      public string     Description      {get {return "Flac Audio";}}
      
      private uint HighLength
      {
         get
         {
            // The last 4 bits are the most significant 4 bits for the 36 bit
            // stream length in samples. (Audio files measured in days)
            return (uint) (AudioSampleRate > 0 ? (((flags & 0xf) << 28) / AudioSampleRate) << 4 : 0);
         }
      }
      #endregion
   }
}
