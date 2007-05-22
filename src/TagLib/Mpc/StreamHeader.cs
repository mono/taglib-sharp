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

namespace TagLib.MusePack
{
   public struct StreamHeader : IAudioCodec
   {
#region Private properties
      private int  version;
      private long stream_length;
      private int  sample_rate;
      private uint frames;
      private uint header_data;
      private static ushort [] sftable = {44100, 48000, 37800, 32000};
#endregion
      
#region Constructors
      public StreamHeader (ByteVector data, long stream_length)
      {
         if (!data.StartsWith ("MP+"))
            throw new CorruptFileException ();
         
         this.stream_length = stream_length;
         
         version = data [3] & 15;
         
         if (version >= 7)
         {
            frames      = data.Mid (4, 4).ToUInt (false);
            uint flags  = data.Mid (8, 4).ToUInt (false);
            sample_rate = sftable [(int) (((flags >> 17) & 1) * 2 + ((flags >> 16) & 1))];
            header_data = 0;
         }
         else
         {
            header_data = data.Mid (0, 4).ToUInt (false);
            version = (int) ((header_data >> 11) & 0x03ff);
            sample_rate = 44100;
            frames = data.Mid (4, version >= 5 ? 4 : 2).ToUInt (false);
         }
      }
#endregion
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public static readonly uint Size = 8 * 7;
      
      public TimeSpan Duration
      {
         get
         {
            if (sample_rate > 0 && stream_length > 0)
               return TimeSpan.FromSeconds ((double) (frames * 1152 - 576) / (double) sample_rate + 0.5);
            
            return TimeSpan.Zero;
         }
      }
      
      public int AudioBitrate
      {
         get
         {
            if (header_data != 0)
               return (int) ((header_data >> 23) & 0x01ff);
            
            return (int) (Duration > TimeSpan.Zero ? ((stream_length * 8L) / Duration.TotalSeconds) / 1000 : 0);
         }
      }
      
      public int AudioSampleRate {get {return sample_rate;}}
      
      public int        AudioChannels {get {return 2;}}
      public MediaTypes MediaTypes    {get {return MediaTypes.Audio;}}
      public int        Version       {get {return version;}}
      public string     Description   {get {return "MusePack Version "  + Version + " Audio";}}
   }
}
