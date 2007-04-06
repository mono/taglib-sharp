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
   public class PageHeader
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private IntList packet_sizes;
      private bool first_packet_continued;
      private bool first_page_of_stream;
      private bool last_page_of_stream;
      private long absolute_granular_position;
      private uint stream_serial_number;
      private int page_sequence_number;
      private int size;
      private int data_size;      
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      
      protected PageHeader ()
      {
         packet_sizes               = new IntList ();
         first_packet_continued     = false;
         first_page_of_stream       = false;
         last_page_of_stream        = false;
         absolute_granular_position = 0;
         stream_serial_number       = 0;
         page_sequence_number       = 0;
         size                       = 0;
         data_size                  = 0;
      }
      
      public PageHeader (uint stream_serial_number, int page_number, 
                         bool first_packet_continued, bool contains_last_packet) : this ()
      {
         this.first_page_of_stream = page_number == 0 && !first_packet_continued;
         this.stream_serial_number = stream_serial_number;
         this.page_sequence_number = page_number;
         this.first_packet_continued = first_packet_continued;
         this.last_page_of_stream = contains_last_packet;
      }
      
      public PageHeader (File file, long page_offset) : this ()
      {
         Read (file, page_offset);
      }
      
      public ByteVector Render ()
      {
         ByteVector data = new ByteVector ();

         // capture patern
         data.Add ("OggS");

         // stream structure version
         data.Add ((byte) 0);

         // header type flag
         byte flags = 0;
         if (first_packet_continued)    flags |= 1;
         if (page_sequence_number == 0) flags |= 2;
         if (last_page_of_stream)       flags |= 4;

         data.Add (flags);

         // absolute granular position
         data.Add (ByteVector.FromLong (absolute_granular_position, false));

         // stream serial number
         data.Add (ByteVector.FromUInt (stream_serial_number, false));

         // page sequence number
         data.Add (ByteVector.FromUInt ((uint) page_sequence_number, false));

         // checksum -- this is left empty and should be filled in by the Ogg::Page
         // class
         data.Add (new ByteVector (4, 0));

         // page segment count and page segment table
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
            
      public bool FirstPacketContinued     {get {return first_packet_continued;}}
      public bool LastPacketCompleted      {get {return (packet_sizes [packet_sizes.Count - 1] % 255) != 0;}}
      public bool FirstPageOfStream        {get {return first_page_of_stream;}}
      public bool LastPageOfStream         {get {return last_page_of_stream;}}
      public long AbsoluteGranularPosition {get {return absolute_granular_position;}}
      public int  PageSequenceNumber       {get {return page_sequence_number;}}
      public uint StreamSerialNumber       {get {return stream_serial_number;}}
      public int  Size                     {get {return size;}}
      public int  DataSize                 {get {return data_size;}}
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Read (File file, long file_offset)
      {
         file.Seek (file_offset);

         // An Ogg page header is at least 27 bytes, so we'll go ahead and read that
         // much and then get the rest when we're ready for it.

         ByteVector data = file.ReadBlock (27);

         // Sanity check -- make sure that we were in fact able to read as much data as
         // we asked for and that the page begins with "OggS".

         if (data.Count != 27 || !data.StartsWith ("OggS"))
            throw new CorruptFileException ("Error reading page header");

         byte flags = data [5];

         first_packet_continued = (flags & 1) != 0;
         first_page_of_stream   = ((flags >> 1) & 1) != 0;
         last_page_of_stream    = ((flags >> 2) & 1) != 0;

         absolute_granular_position = data.Mid( 6, 8).ToLong (false);
         stream_serial_number       = data.Mid(14, 4).ToUInt (false);
         page_sequence_number       = (int) data.Mid(18, 4).ToUInt (false);

         // Byte number 27 is the number of page segments, which is the only variable
         // length portion of the page header.  After reading the number of page
         // segments we'll then read in the coresponding data for this count.

         int page_segment_count = data [26];

         ByteVector page_segments = file.ReadBlock (page_segment_count);

         // Another sanity check.

         if(page_segment_count < 1 || page_segments.Count != page_segment_count)
            throw new CorruptFileException ("Incorrect number of page segments");

         // The base size of an Ogg page 27 bytes plus the number of lacing values.

         size = 27 + page_segment_count;

         int packet_size = 0;

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

               if (i < sizes.Length - 1 || LastPacketCompleted)
               data.Add ((byte) rem);
            }

            return data;
         }
      }
   }
}
