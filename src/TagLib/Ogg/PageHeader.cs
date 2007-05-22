/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : mpegheader.cpp from TagLib
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

using System;

namespace TagLib.Ogg
{
   [Flags ()]
   public enum PageFlags : byte
   {
      None                 = 0,
      FirstPacketContinued = 1,
      FirstPageOfStream    = 2,
      LastPageOfStream     = 4
   }
   
   public struct PageHeader
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private IntList   packet_sizes;
      private byte      version;
      private PageFlags flags;
      private ulong     absolute_granular_position;
      private uint      stream_serial_number;
      private uint      page_sequence_number;
      private uint      size;
      private uint      data_size;      
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      
      public PageHeader (uint stream_serial_number, uint page_number, PageFlags flags)
      {
         this.version = 0;
         this.flags = flags;
         if (page_number == 0 && (this.flags & PageFlags.FirstPacketContinued) == 0)
            this.flags |= PageFlags.FirstPageOfStream;
         this.absolute_granular_position = 0;
         this.stream_serial_number = stream_serial_number;
         this.page_sequence_number = page_number;
         this.size = 0;
         this.data_size = 0;
         this.packet_sizes = new IntList ();
      }
      
      public PageHeader (File file, long page_offset)
      {
         file.Seek (page_offset);

         // An Ogg page header is at least 27 bytes, so we'll go ahead and read that
         // much and then get the rest when we're ready for it.

         ByteVector data = file.ReadBlock (27);
         if (data.Count < 27 || !data.StartsWith ("OggS"))
            throw new CorruptFileException ("Error reading page header");
         
         version = data [4];
         flags = (PageFlags) data [5];
         absolute_granular_position = data.Mid( 6, 8).ToULong (false);
         stream_serial_number       = data.Mid(14, 4).ToUInt (false);
         page_sequence_number       = data.Mid(18, 4).ToUInt (false);

         // Byte number 27 is the number of page segments, which is the only variable
         // length portion of the page header.  After reading the number of page
         // segments we'll then read in the coresponding data for this count.
         uint page_segment_count = data [26];
         ByteVector page_segments = file.ReadBlock (page_segment_count);
         
         // Another sanity check.
         if(page_segment_count < 1 || page_segments.Count != page_segment_count)
            throw new CorruptFileException ("Incorrect number of page segments");

         // The base size of an Ogg page 27 bytes plus the number of lacing values.
         size = 27 + page_segment_count;
         packet_sizes = new IntList ();
         
         int packet_size = 0;
         data_size = 0;
         
         for (int i = 0; i < page_segment_count; i++)
         {
            data_size += page_segments [i];
            packet_size += page_segments [i];

            if (page_segments [i] < 255)
            {
               packet_sizes.Add (packet_size);
               packet_size = 0;
            }
         }
         
         if (packet_size > 0)
            packet_sizes.Add (packet_size);
      }
      
      public ByteVector Render ()
      {
         ByteVector data = new ByteVector ();

         data.Add ("OggS");
         data.Add (version); // stream structure version
         data.Add ((byte) flags);
         data.Add (ByteVector.FromULong (absolute_granular_position, false));
         data.Add (ByteVector.FromUInt (stream_serial_number, false));
         data.Add (ByteVector.FromUInt ((uint) page_sequence_number, false));
         data.Add (new ByteVector (4, 0)); // checksum, to be filled in later.
         ByteVector page_segments = LacingValues;
         data.Add ((byte) page_segments.Count);
         data.Add (page_segments);

         return data;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public int [] PacketSizes
      {
         get {return packet_sizes.ToArray ();}
         set {packet_sizes.Clear (); packet_sizes.Add (value);}
      }
      
      public PageFlags Flags {get {return flags;}}
      public long AbsoluteGranularPosition {get {return (long) absolute_granular_position;}}
      public uint  PageSequenceNumber       {get {return page_sequence_number;}}
      public uint StreamSerialNumber       {get {return stream_serial_number;}}
      public uint  Size                     {get {return size;}}
      public uint  DataSize                 {get {return data_size;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private ByteVector LacingValues
      {
         get
         {
            ByteVector data = new ByteVector ();
            
            int [] sizes = PacketSizes;
            
            for (int i = 0; i < sizes.Length; i ++)
            {
               // The size of a packet in an Ogg page is indicated by a series of "lacing
               // values" where the sum of the values is the packet size in bytes.  Each of
               // these values is a byte.  A value of less than 255 (0xff) indicates the end
               // of the packet.
               
               int quot = sizes [i] / 255;
               int rem  = sizes [i] % 255;
               
               for (int j = 0; j < quot; j++)
                  data.Add ((byte) 255);

               if (i < sizes.Length - 1 || (packet_sizes [i] % 255) != 0)
                  data.Add ((byte) rem);
            }

            return data;
         }
      }
   }
}
