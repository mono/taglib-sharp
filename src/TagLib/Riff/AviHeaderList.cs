//
// AviHeaderList.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TagLib.Riff
{
   public class AviHeaderList
   {
      AviHeader header;
      List<ICodec> codecs = new List<ICodec> ();
      
      public AviHeaderList (TagLib.File file, long position, int length)
      {
         if (file == null)
            throw new ArgumentNullException ("file");
         
         List list = new List (file, position, length);
         
         if (!list.ContainsKey ("avih"))
            throw new CorruptFileException ("Avi header not found.");
         
         ByteVector header_data = list ["avih"][0];
         if (header_data.Count != 0x38)
            throw new CorruptFileException ("Invalid header length.");
            
         header = new AviHeader (header_data);
         
         foreach (ByteVector list_data in list ["LIST"])
         {
            if (list_data.StartsWith ("strl"))
               codecs.Add (AviStream.ParseStreamList (list_data).Codec);
         }
      }
      
      public AviHeader Header {get {return header;}}
      public ICodec [] Codecs {get {return codecs.ToArray ();}}
   }
   
   public struct AviHeader
   {
      uint microseconds_per_frame;
      uint max_bytes_per_second;
      uint flags;
      uint total_frames;
      uint initial_frames;
      uint streams;
      uint suggested_buffer_size;
      uint width;
      uint height;
      
      public AviHeader (ByteVector data) : this (data, 0) {}
      
      public AviHeader (ByteVector data, int offset)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         microseconds_per_frame = data.Mid (offset,      4).ToUInt (false);
         max_bytes_per_second   = data.Mid (offset +  4, 4).ToUInt (false);
         flags                  = data.Mid (offset + 12, 4).ToUInt (false);
         total_frames           = data.Mid (offset + 16, 4).ToUInt (false);
         initial_frames         = data.Mid (offset + 20, 4).ToUInt (false);
         streams                = data.Mid (offset + 24, 4).ToUInt (false);
         suggested_buffer_size  = data.Mid (offset + 28, 4).ToUInt (false);
         width                  = data.Mid (offset + 32, 4).ToUInt (false);
         height                 = data.Mid (offset + 36, 4).ToUInt (false);
      }
      
      public uint MicrosecondsPerFrame {get {return microseconds_per_frame;}}
      public uint MaxBytesPerSecond {get {return max_bytes_per_second;}}
      public uint Flags {get {return flags;}}
      public uint TotalFrames {get {return total_frames;}}
      public uint InitialFrames {get {return initial_frames;}}
      public uint Streams {get {return streams;}}
      public uint SuggestedBufferSize {get {return suggested_buffer_size;}}
      public uint Width {get {return width;}}
      public uint Height {get {return height;}}
      
      public TimeSpan Duration {
         get
         {
            return TimeSpan.FromMilliseconds ((double) TotalFrames
                                            * (double) MicrosecondsPerFrame
                                            / 1000.0);
         }
      }
   }
}