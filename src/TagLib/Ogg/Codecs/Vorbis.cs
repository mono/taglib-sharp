//
// Vorbis.cs:
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

namespace TagLib.Ogg.Codecs
{
   public class Vorbis : Codec, IAudioCodec
   {
      private static ByteVector id = "vorbis";
      
      private HeaderPacket header;
      private ByteVector comment_data;
      
      private Vorbis ()
      {
      }
      
      public static Codec FromPacket (ByteVector packet)
      {
         return (PacketType (packet) == 1) ? new Vorbis () : null;
      }
      
      public override bool ReadPacket (ByteVector packet, int index)
      {
         int type = PacketType (packet);
         if (type != 1 && index == 0)
            throw new CorruptFileException ("Stream does not begin with vorbis identifier");
         
         if (comment_data == null)
         {
            if (type == 1)
               header = new HeaderPacket (packet);
            else if (type == 3)
               comment_data = packet.Mid (7);
            else
               return true;
         }
         
         return comment_data != null;
      }
      
      public          int        AudioBitrate    {get {return (int) ((float)header.bitrate_nominal / 1000f + 0.5);}}
      public          int        AudioSampleRate {get {return (int) header.sample_rate;}}
      public          int        AudioChannels   {get {return (int) header.channels;}}
      public override MediaTypes MediaTypes      {get {return MediaTypes.Audio;}}
      public override ByteVector CommentData     {get {return comment_data;}}
      public override string     Description     {get {return "Vorbis Version " + header.vorbis_version + " Audio";}}

      public override TimeSpan GetDuration (long firstGranularPosition, long lastGranularPosition)
      {
         return TimeSpan.FromSeconds ((double) (lastGranularPosition - firstGranularPosition) / (double) header.sample_rate);
      }
      
      public override void SetCommentPacket (ByteVectorCollection packets, XiphComment comment)
      {
         if (packets == null)
            throw new ArgumentNullException ("packets");
         
         if (comment == null)
            throw new ArgumentNullException ("comment");
         
         ByteVector data = new ByteVector ((byte) 0x03);
         data.Add (id);
         data.Add (comment.Render (true));
         if (packets.Count > 1 && PacketType (packets [1]) == 0x03)
            packets [1] = data;
         else
            packets.Insert (1, data);
      }
      
      private static int PacketType (ByteVector packet)
      {
         if (packet.Count <= id.Count)
            return 0;
         
         for (int i = 0; i < id.Count; i ++)
            if (packet [i + 1] != id [i])
               return 0;
         
         return packet [0];
      }
      
      private struct HeaderPacket
      {
         public uint sample_rate;
         public uint channels;
         public uint vorbis_version;
         public uint bitrate_maximum;
         public uint bitrate_nominal;
         public uint bitrate_minimum;
         
         public HeaderPacket (ByteVector data)
         {
            int pos = 7;
            vorbis_version  = data.Mid(pos, 4).ToUInt (false);
            
            pos += 4;
            channels        = data [pos];
            
            pos += 1;
            sample_rate     = data.Mid(pos, 4).ToUInt (false);
            
            pos += 4;
            bitrate_maximum = data.Mid(pos, 4).ToUInt (false);
            
            pos += 4;
            bitrate_nominal = data.Mid(pos, 4).ToUInt (false);
            
            pos += 4;
            bitrate_minimum = data.Mid(pos, 4).ToUInt (false);
         }
      }
   }
}
