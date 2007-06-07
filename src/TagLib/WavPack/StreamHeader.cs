/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : wvproperties.cpp from libtunepimp
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

namespace TagLib.WavPack
{
   public struct StreamHeader : IAudioCodec, IEquatable<StreamHeader>
   {
      #region Constants
      private static readonly uint [] sample_rates = new uint [] {6000, 8000, 9600, 11025, 12000,
         16000, 22050, 24000, 32000, 44100, 48000, 64000, 88200, 96000, 192000};
      
      private const int  BYTES_STORED = 3;
      private const int  MONO_FLAG    = 4;
      private const int  SHIFT_LSB   = 13;
      private const long SHIFT_MASK  = (0x1fL << SHIFT_LSB);
      private const int  SRATE_LSB   = 23;
      private const long SRATE_MASK  = (0xfL << SRATE_LSB);
      #endregion
      
      
      
      #region Private Properties
      private long   _stream_length;
      private ushort _version;
      private uint   _flags;
      private uint   _samples;
      #endregion
      
      
      
      #region Public Static Properties
      public const uint Size = 32;
      public static readonly ReadOnlyByteVector FileIdentifier = "wvpk";
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
         _version = data.Mid ( 8, 2).ToUShort (false);
         _flags   = data.Mid (24, 4).ToUInt (false);
         _samples = data.Mid (12, 4).ToUInt (false);
      }
      #endregion
      
      
      
      #region Public Properties
      public TimeSpan Duration
      {
         get
         {
            return AudioSampleRate > 0 ? TimeSpan.FromSeconds ((double) _samples / (double) AudioSampleRate + 0.5) : TimeSpan.Zero;
         }
      }
      
      public int AudioBitrate
      {
         get
         {
            return (int) (Duration > TimeSpan.Zero ? ((_stream_length * 8L) / Duration.TotalSeconds) / 1000 : 0);
         }
      }
      
      public int AudioSampleRate
      {
         get
         {
            return (int) (sample_rates [(_flags & SRATE_MASK) >> SRATE_LSB]);
         }
      }
      
      public int        AudioChannels {get {return ((_flags & MONO_FLAG) != 0) ? 1 : 2;}}
      
      public MediaTypes MediaTypes    {get {return MediaTypes.Audio;}}
      
      public int        Version       {get {return _version;}}
      
      public int BitsPerSample
      {
         get
         {
            return (int) (((_flags & BYTES_STORED) + 1) * 8 - ((_flags & SHIFT_MASK) >> SHIFT_LSB));
         }
      }
      
      public string Description {get {return "WavPack Version " + Version + " Audio";}}
      #endregion
      
      
      
      #region IEquatable
      public override int GetHashCode ()
      {
         unchecked
         {
            return (int) (_flags ^ _samples ^ _version);
         }
      }
      
      public override bool Equals (object obj)
      {
         if (!(obj is StreamHeader))
            return false;
         
         return Equals ((StreamHeader) obj);
      }
      
      public bool Equals (StreamHeader other)
      {
         return _flags == other._flags && _samples == other._samples && _version == other._version;
      }
      
      public static bool operator == (StreamHeader first, StreamHeader second)
      {
         return first.Equals (second);
      }
      
      public static bool operator != (StreamHeader first, StreamHeader second)
      {
         return !first.Equals (second);
      }
      #endregion
   }
}
