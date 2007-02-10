/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : oggfile.cpp from TagLib
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
using System.Collections.Generic;
using System;

namespace TagLib.Ogg
{
   public abstract class File : TagLib.File
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private uint       stream_serial_number;
      private List<Page>  pages;
      private PageHeader first_page_header;
      private PageHeader last_page_header;
      private List<IntList>  packet_to_page_map;
      private Dictionary<uint, ByteVector> dirty_packets;
      private IntList    dirty_pages;

      // The current page for the reader -- used by nextPage()
      private Page current_page;
      //! The current page for the packet parser -- used by packet()
      private Page current_packet_page;
      //! The packets for the currentPacketPage -- used by packet()
      private ByteVectorList current_packets;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public ByteVector GetPacket (uint i)
      {
         // Check to see if we're called setPacket() for this packet since the last
         // save:

         if (dirty_packets.ContainsKey (i))
            return dirty_packets [i];

         // If we haven't indexed the page where the packet we're interested in starts,
         // begin reading pages until we have.

         while (packet_to_page_map.Count <= i)
            if (!NextPage ())
               throw new CorruptFileException ("Could not find the requested packet.");
         
         // Start reading at the first page that contains part (or all) of this packet.
         // If the last read stopped at the packet that we're interested in, don't
         // reread its packet list.  (This should make sequential packet reads fast.)
         
         int page_index = (packet_to_page_map [(int)i]) [0];
         if (current_packet_page != pages [page_index])
         {
            current_packet_page = pages [page_index];
            current_packets = current_packet_page.Packets;
         }

         // If the packet is completely contained in the first page that it's in, then
         // just return it now.

         if ((current_packet_page.ContainsPacket ((int)i) & Page.ContainsPacketFlags.CompletePacket) != 0)
            return current_packets [(int)(i - current_packet_page.FirstPacketIndex)];

         // If the packet is *not* completely contained in the first page that it's a
         // part of then that packet trails off the end of the page.  Continue appending
         // the pages' packet data until we hit a page that either does not end with the
         // packet that we're fetching or where the last packet is complete.

         ByteVector packet = current_packets [current_packets.Count - 1];
         while ((current_packet_page.ContainsPacket ((int) i) & Page.ContainsPacketFlags.EndsWithPacket) != 0
                && !current_packet_page.Header.LastPacketCompleted)
         {
            page_index ++;
            if (page_index == pages.Count && !NextPage ())
               throw new CorruptFileException ("Could not find the requested packet.");
            
            current_packet_page = pages [page_index];
            current_packets = current_packet_page.Packets;
            packet.Add (current_packets [0]);
         }

         return packet;
      }
      
      public void SetPacket (uint i, ByteVector p)
      {
         while (packet_to_page_map.Count <= i)
            if (!NextPage ())
            {
               Debugger.Debug ("Ogg.File.SetPacket() -- Could not set the requested packet.");
               return;
            }

         foreach (int page in packet_to_page_map [(int) i])
            dirty_pages.SortedInsert (page, true);
         
         if (dirty_packets.ContainsKey (i))
            dirty_packets [i] = p;
         else
            dirty_packets.Add (i, p);
      }
      
      public override void Save ()
      {
         Mode = AccessMode.Write;
         
         IntList page_group = new IntList ();

         foreach (int page in dirty_pages)
            if (!page_group.IsEmpty && page_group [page_group.Count - 1] + 1 != page)
            {
               WritePageGroup (page_group);
               page_group.Clear ();
            }
            else
               page_group.Add (page);
         
         WritePageGroup (page_group);
         dirty_pages.Clear ();
         dirty_packets.Clear ();
         
         Mode = AccessMode.Closed;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public PageHeader FirstPageHeader
      {
         get
         {
            if (first_page_header == null)
            {
               long first_page_header_offset = Find ("OggS");

               if (first_page_header_offset < 0)
                  return null;

               first_page_header = new PageHeader (this, first_page_header_offset);
            }
            
            return first_page_header.IsValid ? first_page_header : null;
         }
      }
      
      public PageHeader LastPageHeader
      {
         get
         {
            if (last_page_header == null)
            {
               long last_page_header_offset = RFind ("OggS");

               if(last_page_header_offset < 0)
                  return null;

               last_page_header = new PageHeader (this, last_page_header_offset);
            }
            return last_page_header.IsValid ? last_page_header : null;
         }
      }



      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected File (string file) : base (file)
      {
         ClearPageData ();
      }
      
      protected void ClearPageData ()
      {
         stream_serial_number = 0;
         pages                = new List<Page> ();
         first_page_header    = null;
         last_page_header     = null;
         packet_to_page_map   = new List<IntList> ();
         dirty_packets        = new Dictionary<uint, ByteVector> ();
         dirty_pages          = new IntList ();

         current_page         = null;
         current_packet_page  = null;
         current_packets      = null;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private bool NextPage ()
      {
         long next_page_offset;
         int current_packet;

         if (pages.Count == 0)
         {
            current_packet = 0;
            next_page_offset = Find ("OggS");
            if (next_page_offset < 0)
            return false;
         }
         else
         {
            if (current_page.Header.LastPageOfStream)
               return false;

            if (current_page.Header.LastPacketCompleted)
               current_packet = (int)(current_page.FirstPacketIndex + current_page.PacketCount);
            else
               current_packet = (int)(current_page.FirstPacketIndex + current_page.PacketCount - 1);

            next_page_offset = current_page.FileOffset + current_page.Size;
         }

         // Read the next page and add it to the page list.

         current_page = new Page (this, next_page_offset);

         if(!current_page.Header.IsValid)
         {
            current_page = null;
            return false;
         }

         current_page.FirstPacketIndex = current_packet;

         if (pages.Count == 0)
            stream_serial_number = current_page.Header.StreamSerialNumber;

         pages.Add (current_page);

         // Loop through the packets in the page that we just read appending the
         // current page number to the packet to page map for each packet.

         for (int i = 0; i < current_page.PacketCount; i++)
         {
            current_packet = current_page.FirstPacketIndex + i;
            if (packet_to_page_map.Count <= current_packet)
               packet_to_page_map.Add (new IntList ());
            (packet_to_page_map [current_packet]).Add (pages.Count - 1);
         }

         return true;

      }
      
      private void WritePageGroup (IntList page_group)
      {
         if(page_group.IsEmpty)
            return;
         
         ByteVectorList packets = new ByteVectorList ();

         // If the first page of the group isn't dirty, append its partial content here.

         if(!dirty_pages.Contains (this.pages [page_group [0]].FirstPacketIndex))
            packets.Add (this.pages [page_group [0]].Packets [0]);

         int previous_packet = -1;
         int original_size = 0;

         for (int i = 0; i < page_group.Count; i ++)
         {
            int page = page_group [i];
            
            uint first_packet = (uint) this.pages [page].FirstPacketIndex;
            uint last_packet  = first_packet + this.pages [page].PacketCount - 1;

            for (uint j = first_packet; j <= last_packet; j ++)
            {

               if (i == page_group.Count - 1 && j == last_packet && !dirty_pages.Contains ((int)j))
                  packets.Add (this.pages [page].Packets [this.pages [page].Packets.Count - 1]);
               else if((int)j != previous_packet)
               {
                  previous_packet = (int) j;
                  packets.Add (GetPacket (j));
               }
            }
            original_size += this.pages [page].Size;
         }

         bool continued = this.pages [page_group [0]].Header.FirstPacketContinued;
         bool completed = this.pages [page_group [page_group.Count - 1]].Header.LastPacketCompleted;

         // TODO: This pagination method isn't accurate for what's being done here.
         // This should account for real possibilities like non-aligned packets and such.

         Page [] pages = Page.Paginate (packets, Page.PaginationStrategy.SinglePagePerGroup,
                                        stream_serial_number, page_group [0],
                                        continued, completed);

         ByteVector data = new ByteVector ();
         
         foreach (Page p in pages)
            data.Add (p.Render ());

         // The insertion algorithms could also be improve to queue and prioritize data
         // on the way out.  Currently it requires rewriting the file for every page
         // group rather than just once; however, for tagging applications there will
         // generally only be one page group, so it's not worth the time for the
         // optimization at the moment.

         Insert (data, this.pages [page_group [0]].FileOffset, original_size);

         // Update the page index to include the pages we just created and to delete the
         // old pages.

         foreach (Page p in pages)
         {
            int index = p.Header.PageSequenceNumber;
            this.pages [index] = p;
         }
      }
   }
}
