//
// Bitstream.cs:
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

namespace TagLib.Ogg
{
   public class Bitstream
   {
      private ByteVector _previous_packet;
      private int        _packet_index;
      private Codec      _codec;
      private long       _first_absolute_granular_position;
      
      public Bitstream (Page page)
      {
         if (page == null)
            throw new ArgumentNullException ("page");
         
         // Assume that the first packet is completely enclosed. This should be
         // sufficient for codec recognition.
         _codec = Codec.GetCodec (page.Packets [0]);
         
         _first_absolute_granular_position = page.Header.AbsoluteGranularPosition;
      }
      
      public bool ReadPage (Page page)
      {
         if (page == null)
            throw new ArgumentNullException ("page");
         
         ByteVector[] packets = page.Packets;
         for (int i = 0; i < packets.Length; i ++)
         {
            if ((page.Header.Flags & PageFlags.FirstPacketContinued) == 0 && _previous_packet != null)
            {
               if (ReadPacket (_previous_packet))
                  return true;
               _previous_packet = null;
            }
            
            
            ByteVector packet = packets [i];
            
            // If we're at the first packet of the page, and we're continuing an
            // old packet, combine the old with the new.
            if (i == 0 && (page.Header.Flags & PageFlags.FirstPacketContinued) != 0 && _previous_packet != null)
            {
               _previous_packet.Add (packet);
               packet = _previous_packet;
            }
            _previous_packet = null;
            
            // If we're at the last packet of the page, store it.
            if (i == packets.Length - 1)
               _previous_packet = new ByteVector (packet);
            
            // Otherwise, we need to process it.
            else if (ReadPacket (packet))
               return true;
         }
         
         return false;
      }
      
      public Codec Codec {get {return _codec;}}
      
      public TimeSpan GetDuration (long lastAbsoluteGranularPosition)
      {
         return _codec.GetDuration (_first_absolute_granular_position, lastAbsoluteGranularPosition);
      }
      
      private bool ReadPacket (ByteVector packet)
      {
         return _codec.ReadPacket (packet, _packet_index++);
      }
   }
}
