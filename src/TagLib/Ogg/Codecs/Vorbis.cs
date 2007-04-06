using System;

namespace TagLib.Ogg.Codecs
{
   public class Vorbis : Codec
   {
      private static ByteVector id = "vorbis";
      
      private HeaderPacket header;
      private ByteVector comment_data;
      
      public Vorbis ()
      {
         comment_data = null;
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
      
      public override int        AudioBitrate    {get {return (int) ((float)header.bitrate_nominal / 1000f + 0.5);}}
      public override int        AudioSampleRate {get {return header.sample_rate;}}
      public override int        AudioChannels   {get {return header.channels;}}
      public override MediaTypes MediaTypes      {get {return MediaTypes.Audio;}}
      public override ByteVector CommentData     {get {return comment_data;}}
      public override TimeSpan GetDuration (long last_granular_position, long first_granular_position)
      {
         return TimeSpan.FromSeconds ((double) (last_granular_position - first_granular_position) / (double) header.sample_rate);
      }
      
      public override void SetCommentPacket (ByteVectorList packets, XiphComment comment)
      {
         ByteVector data = new ByteVector ((byte) 0x03);
         data.Add (id);
         data.Add (comment.Render ());
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
         public int sample_rate;
         public int channels;
         public int vorbis_version;
         public int bitrate_maximum;
         public int bitrate_nominal;
         public int bitrate_minimum;
         
         public HeaderPacket (ByteVector data)
         {
            int pos = 7;
            vorbis_version  = (int) data.Mid(pos, 4).ToUInt (false);
            
            pos += 4;
            channels        = data [pos];
            
            pos += 1;
            sample_rate     = (int) data.Mid(pos, 4).ToUInt (false);
            
            pos += 4;
            bitrate_maximum = (int) data.Mid(pos, 4).ToUInt (false);
            
            pos += 4;
            bitrate_nominal = (int) data.Mid(pos, 4).ToUInt (false);
            
            pos += 4;
            bitrate_minimum = (int) data.Mid(pos, 4).ToUInt (false);
         }
      }
   }
}
