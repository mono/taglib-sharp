/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : oggpage.cpp from TagLib
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

namespace TagLib.Ogg
{
   public class Page
   {
      public enum ContainsPacketFlags
      {
         DoesNotContainPacket = 0x0000, // No part of the packet is contained in the page
         CompletePacket       = 0x0001, // The packet is wholly contained in the page
         BeginsWithPacket     = 0x0002, // The page starts with the given packet
         EndsWithPacket       = 0x0004  // The page ends with the given packet
      };
      
      public enum PaginationStrategy
      {
         SinglePagePerGroup,
         Repaginate
      };
      
      
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private File           file;
      private long           file_offset;
      private long           packet_offset;
      private int            data_size;
      private PageHeader     header;
      private int            first_packet_index;
      private ByteVectorList packets;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Page(File file, long page_offset)
      {
         this.file = file;
         this.file_offset = page_offset;
         packet_offset = 0;
         data_size = 0;
         header = new PageHeader (file, page_offset);
         first_packet_index = -1;
         packets = new ByteVectorList ();
         
         if (file != null)
         {
            packet_offset = file_offset + header.Size;
            data_size = header.DataSize;
         }
      }
      
      public ContainsPacketFlags ContainsPacket (int index)
      {
         ContainsPacketFlags flags = ContainsPacketFlags.DoesNotContainPacket;
         
         int last_packet_index = (int) (first_packet_index + PacketCount - 1);
         if (index < first_packet_index || index > last_packet_index)
            return flags;
         
         if (index == first_packet_index)
            flags |= ContainsPacketFlags.BeginsWithPacket;

         if (index == last_packet_index)
            flags |= ContainsPacketFlags.EndsWithPacket;

         // If there's only one page and it's complete:

         if (PacketCount == 1 && !header.FirstPacketContinued && header.LastPacketCompleted)
            flags |= ContainsPacketFlags.CompletePacket;

         // Or if the page is (a) the first page and it's complete or (b) the last page
         // and it's complete or (c) a page in the middle.

         else if (((flags & ContainsPacketFlags.BeginsWithPacket) != 0 && !header.FirstPacketContinued) ||
                  ((flags & ContainsPacketFlags.EndsWithPacket) != 0 && header.LastPacketCompleted) ||
                  ((flags & ContainsPacketFlags.BeginsWithPacket) == 0 && (flags & ContainsPacketFlags.EndsWithPacket) == 0))
            flags |= ContainsPacketFlags.CompletePacket;
         
         return flags;
      }
      
      public ByteVector Render ()
      {
         ByteVector data = header.Render ();

         if (packets.IsEmpty)
         {
            if (file != null)
            {
               file.Seek (packet_offset);
               data.Add (file.ReadBlock (data_size));
            }
            else
               Debugger.Debug ("Ogg.Page.Render() -- this page is empty!");
         }
         else
            foreach (ByteVector v in packets)
               data.Add (v);

         // Compute and set the checksum for the Ogg page.  The checksum is taken over
         // the entire page with the 4 bytes reserved for the checksum zeroed and then
         // inserted in bytes 22-25 of the page header.

         ByteVector checksum = ByteVector.FromUInt (data.CheckSum, false);
         for (int i = 0; i < 4; i++)
            data [i + 22] = checksum [i];

         return data;
      }
      
      public static Page [] Paginate (ByteVectorList packets,      PaginationStrategy strategy,
                               uint stream_serial_number,   int first_page,
                               bool first_packet_continued, bool last_packet_completed,
                               bool contains_last_packet)
      {
         ArrayList l = new ArrayList ();

         int total_size = 0;
         
         foreach (ByteVector b in packets)
            total_size += b.Count;

         if (strategy == PaginationStrategy.Repaginate || total_size + packets.Count > 255 * 256)
         {
            Debugger.Debug ("Ogg.Page.Paginate() -- Sorry!  Repagination is not yet implemented.");
            return (Page []) l.ToArray (typeof (Page));
         }

         // TODO: Handle creation of multiple pages here with appropriate pagination.

         Page p = new Page (packets, stream_serial_number, first_page, first_packet_continued,
                            last_packet_completed, contains_last_packet);
         l.Add (p);

         return (Page []) l.ToArray (typeof (Page));
      }

      public static Page [] Paginate (ByteVectorList packets,      PaginationStrategy strategy,
                               uint stream_serial_number,   int first_page,
                               bool first_packet_continued, bool last_packet_completed)
      {
         return Paginate (packets, strategy, stream_serial_number, first_page,
                          first_packet_continued, last_packet_completed, false);
      }
      
      public static Page [] Paginate (ByteVectorList packets,      PaginationStrategy strategy,
                               uint stream_serial_number,   int first_page,
                               bool first_packet_continued)
      {
         return Paginate (packets, strategy, stream_serial_number, first_page,
                          first_packet_continued, true);
      }
      
      public static Page [] Paginate (ByteVectorList packets,      PaginationStrategy strategy,
                               uint stream_serial_number,   int first_page)
      {
         return Paginate (packets, strategy, stream_serial_number, first_page,
                          false);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public long FileOffset {get {return file_offset;}}
      
      public PageHeader Header {get {return header;}}
      
      public int FirstPacketIndex
      {
         get {return first_packet_index;}
         set {first_packet_index = value;}
      }
      
      public uint PacketCount {get {return (uint) header.PacketSizes.Length;}}
      
      public ByteVectorList Packets
      {
         get
         {
            if (!packets.IsEmpty)
               return packets;

            ByteVectorList l = new ByteVectorList ();

            if (file != null && header.IsValid)
            {
               file.Seek (packet_offset);

               foreach (int packet_size in header.PacketSizes)
                  l.Add (file.ReadBlock (packet_size));
            }
            else
               Debugger.Debug ("Ogg.Page.Packets -- attempting to read packets from an invalid page.");

            return l;
         }
      }
      
      public int Size {get {return header.Size + header.DataSize;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      
      
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////

      protected Page (ByteVectorList packets,     uint stream_serial_number,
                      int page_number,            bool first_packet_continued,
                      bool last_packet_completed, bool contains_last_packet)
      {
         file = null;
         file_offset = -1;
         packet_offset = 0;
         data_size = 0;
         header = new PageHeader ();
         first_packet_index = -1;
         this.packets = packets;

         ByteVector data = new ByteVector ();
         ArrayList packet_sizes = new ArrayList ();

         header.FirstPageOfStream    = (page_number == 0 && !first_packet_continued);
         header.LastPageOfStream     = contains_last_packet;
         header.FirstPacketContinued = first_packet_continued;
         header.LastPacketCompleted  = last_packet_completed;
         header.StreamSerialNumber   = stream_serial_number;
         header.PageSequenceNumber   = page_number;

         // Build a page from the list of packets.
         foreach (ByteVector v in packets)
         {
            packet_sizes.Add (v.Count);
            data.Add (v);
         }
         
         header.PacketSizes = (int []) packet_sizes.ToArray (typeof (int));
      }
      
      protected Page (ByteVectorList packets,     uint stream_serial_number,
                        int page_number,            bool first_packet_continued,
                        bool last_packet_completed)
               : this (packets, stream_serial_number, page_number,
                        first_packet_continued, last_packet_completed, false)
      {
      }
      
      protected Page (ByteVectorList packets,     uint stream_serial_number,
                        int page_number,            bool first_packet_continued)
               : this (packets, stream_serial_number, page_number,
                        first_packet_continued, true)
      {
      }
      
      protected Page (ByteVectorList packets,     uint stream_serial_number,
                        int page_number)
               : this (packets, stream_serial_number, page_number, false)
      {
      }
   }
}
