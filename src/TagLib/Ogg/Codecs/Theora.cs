//
// Theora.cs:
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
   public class Theora : Codec, IVideoCodec
   {
      private static ByteVector id = "theora";
      
      private HeaderPacket header;
      private ByteVector comment_data;
      
      private Theora ()
      {
      }
      
      public static Codec FromPacket (ByteVector packet)
      {
         return (PacketType (packet) == 0x80) ? new Theora () : null;
      }
      
      public override bool ReadPacket (ByteVector packet, int index)
      {
         int type = PacketType (packet);
         if (type != 0x80 && index == 0)
            throw new CorruptFileException ("Stream does not begin with theora identifier");
         
         if (comment_data == null)
         {
            if (type == 0x80)
               header = new HeaderPacket (packet);
            else if (type == 0x81)
               comment_data = packet.Mid (7);
            else
               return true;
         }
         
         return comment_data != null;
      }
      
      public int        VideoWidth  {get {return header.width;}}
      public int        VideoHeight {get {return header.height;}}
      public override MediaTypes MediaTypes  {get {return MediaTypes.Video;}}
      public override ByteVector CommentData {get {return comment_data;}}
      public override string Description  {get {return "Vorbis Version " + header.major_version + "." + header.minor_version + " Video";}}
      public override TimeSpan GetDuration (long firstGranularPosition, long lastGranularPosition)
      {
         return TimeSpan.FromSeconds (header.GranuleTime (lastGranularPosition) - header.GranuleTime (firstGranularPosition));
      }
      
      public override void SetCommentPacket (ByteVectorCollection packets, XiphComment comment)
      {
         if (packets == null)
            throw new ArgumentNullException ("packets");
         
         if (comment == null)
            throw new ArgumentNullException ("comment");
         
         ByteVector data = new ByteVector ((byte) 0x81);
         data.Add (id);
         data.Add (comment.Render (true));
         if (packets.Count > 1 && PacketType (packets [1]) == 0x81)
            packets [1] = data;
         else
            packets.Insert (1, data);
      }
      
      private static int PacketType (ByteVector packet)
      {
         if (packet.Count <= id.Count || packet [0] < 0x80)
            return 0;
         
         for (int i = 0; i < id.Count; i ++)
            if (packet [i + 1] != id [i])
               return 0;
         
         return packet [0];
      }
      
      private struct HeaderPacket
      {
         public byte major_version;
         public byte minor_version;
         public byte revision_version;
         public int width;
         public int height;
         public int fps_numerator;
         public int fps_denominator;
         public int keyframe_granule_shift;
         
         public HeaderPacket (ByteVector data)
         {
            
            int pos = 7;
            
            major_version = data [pos];
            pos += 1;
            
            minor_version = data [pos];
            pos += 1;
            
            revision_version = data [pos];
            pos += 1;
            
            // width = data.Mid (pos, 2).ToShort () << 4;
            pos += 2;
            
            // height = data.Mid (pos, 2).ToShort () << 4;
            pos += 2;
            
            // Frame Width.
            width = (int) data.Mid (pos, 3).ToUInt ();
            pos += 3;
            
            // Frame Height.
            height = (int) data.Mid (pos, 3).ToUInt ();
            pos += 3;
            
            // Offset X.
            pos += 1;
            
            // Offset Y.
            pos += 1;
            
            fps_numerator = (int) data.Mid (pos, 4).ToUInt ();
            pos += 4;
            
            fps_denominator = (int) data.Mid (pos, 4).ToUInt ();
            pos += 4;
            
            // Aspect Numerator.
            pos += 3;
            
            // Aspect Denominator.
            pos += 3;
            
            // Colorspace.
            pos += 1;
            
            // Target bitrate.
            pos += 3;
            
            ushort last_bits = data.Mid (pos, 2).ToUShort ();
            keyframe_granule_shift = (last_bits >> 5) & 0x1F;
         }
         
         // Many thanks to the good people at irc://irc.freenode.net#theora for
         // making this code a reality.
         public double GranuleTime (long granular_position)
         {
            long iframe = granular_position >> keyframe_granule_shift;
            long pframe = granular_position - (iframe << keyframe_granule_shift);
            return (iframe + pframe) * ((double)fps_denominator/(double)fps_numerator);
         }
      }
   }
}