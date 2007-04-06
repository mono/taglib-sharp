using System;

namespace TagLib.Ogg
{
   public class Bitstream
   {
      private ByteVector previous_packet;
      int packet_index;
      private Codec codec;
      private long first_absolute_granular_position;
      
      public Bitstream (Page page)
      {
         packet_index = 0;
         previous_packet = null;
         
         // Assume that the first packet is completely enclosed. This should be
         // sufficient for codec recognition.
         codec = Codec.GetCodec (page.Packets [0]);
         
         first_absolute_granular_position = page.Header.AbsoluteGranularPosition;
      }
      
      public bool ReadPage (Page page)
      {
         ByteVectorList packets = page.Packets;
         for (int i = 0; i < packets.Count; i ++)
         {
            if (!page.Header.FirstPacketContinued && previous_packet != null)
            {
               if (ReadPacket (previous_packet))
                  return true;
               previous_packet = null;
            }
            
            
            ByteVector packet = packets [i];
            
            // If we're at the first packet of the page, and we're continuing an
            // old packet, combine the old with the new.
            if (i == 0 && page.Header.FirstPacketContinued && previous_packet != null)
            {
               previous_packet.Add (packet);
               packet = previous_packet;
            }
            previous_packet = null;
            
            // If we're at the last packet of the page, and we're not completed,
            // store the packet.
            if (i == packets.Count - 1 && !page.Header.LastPacketCompleted)
               previous_packet = new ByteVector (packet);
            
            // Otherwise, we need to process it.
            else if (ReadPacket (packet))
               return true;
         }
         
         return false;
      }
      
      public Codec Codec {get {return codec;}}
      
      public TimeSpan GetDuration (long last_absolute_granular_position)
      {
         return codec.GetDuration (last_absolute_granular_position, first_absolute_granular_position);
      }
      
      private bool ReadPacket (ByteVector packet)
      {
         return codec.ReadPacket (packet, packet_index++);
      }
   }
}
