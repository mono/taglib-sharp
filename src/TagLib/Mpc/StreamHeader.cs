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
      #region Constants
      private static ushort [] sftable = {44100, 48000, 37800, 32000};
      #endregion
      
      
      
      #region Private properties
      private int  _version;
      private long _stream_length;
      private int  _sample_rate;
      private uint _frames;
      private uint _header_data;
      #endregion
      
      
      
      #region Public Static Properties
      public const uint Size = 56;
      public static readonly ReadOnlyByteVector FileIdentifier = "MP+";
      #endregion
      
      
      
      #region Constructors
      public StreamHeader (ByteVector data, long streamLength)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         if (!data.StartsWith (FileIdentifier))
            throw new CorruptFileException ("Data does not begin with identifier.");
         
         if (data.Count < Size)
            throw new CorruptFileException ("Insufficient data in stream header");
         
         _stream_length = streamLength;
         
         _version = data [3] & 15;
         
         if (_version >= 7)
         {
            _frames      = data.Mid (4, 4).ToUInt (false);
            uint flags   = data.Mid (8, 4).ToUInt (false);
            _sample_rate = sftable [(int) (((flags >> 17) & 1) * 2 + ((flags >> 16) & 1))];
            _header_data = 0;
         }
         else
         {
            _header_data = data.Mid (0, 4).ToUInt (false);
            _version     = (int) ((_header_data >> 11) & 0x03ff);
            _sample_rate = 44100;
            _frames      = data.Mid (4, _version >= 5 ? 4 : 2).ToUInt (false);
         }
      }
      #endregion
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      
      public TimeSpan Duration
      {
         get
         {
            if (_sample_rate > 0 && _stream_length > 0)
               return TimeSpan.FromSeconds ((double) (_frames * 1152 - 576) / (double) _sample_rate + 0.5);
            
            return TimeSpan.Zero;
         }
      }
      
      public int AudioBitrate
      {
         get
         {
            if (_header_data != 0)
               return (int) ((_header_data >> 23) & 0x01ff);
            
            return (int) (Duration > TimeSpan.Zero ? ((_stream_length * 8L) / Duration.TotalSeconds) / 1000 : 0);
         }
      }
      
      public int AudioSampleRate {get {return _sample_rate;}}
      
      public int        AudioChannels {get {return 2;}}
      public MediaTypes MediaTypes    {get {return MediaTypes.Audio;}}
      public int        Version       {get {return _version;}}
      public string     Description   {get {return "MusePack Version "  + Version + " Audio";}}
   }
}
