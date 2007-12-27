//
// PageHeader.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   oggpageheader.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
// Copyright (C) 2003 Scott Wheeler (Original Implementation)
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

namespace TagLib.Ogg
{
   [Flags]
   public enum PageFlags : byte
   {
      None                 = 0,
      FirstPacketContinued = 1,
      FirstPageOfStream    = 2,
      LastPageOfStream     = 4
   }
   
   public struct PageHeader
   {
      #region Private Propertis
      private List<int> _packet_sizes;
      private byte      _version;
      private PageFlags _flags;
      private ulong     _absolute_granular_position;
      private uint      _stream_serial_number;
      private uint      _page_sequence_number;
      private uint      _size;
      private uint      _data_size;
      #endregion
      
      
      
      #region Constructors
      public PageHeader (uint streamSerialNumber, uint pageNumber, PageFlags flags)
      {
         _version = 0;
         _flags = flags;
         _absolute_granular_position = 0;
         _stream_serial_number = streamSerialNumber;
         _page_sequence_number = pageNumber;
         _size = 0;
         _data_size = 0;
         _packet_sizes = new List<int> ();
         
         if (pageNumber == 0 && (flags & PageFlags.FirstPacketContinued) == 0)
            _flags |= PageFlags.FirstPageOfStream;
      }
      
      public PageHeader (File file, long position)
      {
         file.Seek (position);

         // An Ogg page header is at least 27 bytes, so we'll go ahead and read that
         // much and then get the rest when we're ready for it.

         ByteVector data = file.ReadBlock (27);
         if (data.Count < 27 || !data.StartsWith ("OggS"))
            throw new CorruptFileException ("Error reading page header");
         
         _version = data [4];
         _flags = (PageFlags) data [5];
         _absolute_granular_position = data.Mid( 6, 8).ToULong (false);
         _stream_serial_number       = data.Mid(14, 4).ToUInt (false);
         _page_sequence_number       = data.Mid(18, 4).ToUInt (false);

         // Byte number 27 is the number of page segments, which is the only variable
         // length portion of the page header.  After reading the number of page
         // segments we'll then read in the coresponding data for this count.
         int page_segment_count = data [26];
         ByteVector page_segments = file.ReadBlock (page_segment_count);
         
         // Another sanity check.
         if(page_segment_count < 1 || page_segments.Count != page_segment_count)
            throw new CorruptFileException ("Incorrect number of page segments");

         // The base size of an Ogg page 27 bytes plus the number of lacing values.
         _size = (uint)(27 + page_segment_count);
         _packet_sizes = new List<int> ();
         
         int packet_size = 0;
         _data_size = 0;
         
         for (int i = 0; i < page_segment_count; i++)
         {
            _data_size += page_segments [i];
            packet_size += page_segments [i];

            if (page_segments [i] < 255)
            {
               _packet_sizes.Add (packet_size);
               packet_size = 0;
            }
         }
         
         if (packet_size > 0)
            _packet_sizes.Add (packet_size);
      }
      
		public PageHeader (PageHeader original, uint offset, PageFlags flags)
		{
			_version = original._version;
			_flags = flags;
			_absolute_granular_position = original._absolute_granular_position;
			_stream_serial_number = original._stream_serial_number;
			_page_sequence_number = original._page_sequence_number + offset;
			_size = original._size;
			_data_size = original._data_size;
			_packet_sizes = new List<int> ();
			
			if (_page_sequence_number == 0 && (flags & PageFlags.FirstPacketContinued) == 0)
				_flags |= PageFlags.FirstPageOfStream;
		}
      
      #endregion
      
      
      
      #region Public Properties
      public int [] PacketSizes
      {
         get {return _packet_sizes.ToArray ();}
         set {_packet_sizes.Clear (); _packet_sizes.AddRange (value);}
      }
      
      public PageFlags Flags                    {get {return _flags;}}
      public long      AbsoluteGranularPosition {get {return (long) _absolute_granular_position;}}
      public uint      PageSequenceNumber       {get {return _page_sequence_number;}}
      public uint      StreamSerialNumber       {get {return _stream_serial_number;}}
      public uint      Size                     {get {return _size;}}
      public uint      DataSize                 {get {return _data_size;}}
      #endregion
      
      
      
      #region Public Methods
      public ByteVector Render ()
      {
         ByteVector data = new ByteVector ();

         data.Add ("OggS");
         data.Add (_version); // stream structure version
         data.Add ((byte) _flags);
         data.Add (ByteVector.FromULong (_absolute_granular_position, false));
         data.Add (ByteVector.FromUInt (_stream_serial_number, false));
         data.Add (ByteVector.FromUInt ((uint) _page_sequence_number, false));
         data.Add (new ByteVector (4, 0)); // checksum, to be filled in later.
         ByteVector page_segments = LacingValues;
         data.Add ((byte) page_segments.Count);
         data.Add (page_segments);

         return data;
      }
      #endregion
      
      
      
      #region Private Properties
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

               if (i < sizes.Length - 1 || (_packet_sizes [i] % 255) != 0)
                  data.Add ((byte) rem);
            }

            return data;
         }
      }
      #endregion
      
      
      
      
#region IEquatable
		
		public override int GetHashCode ()		{
			unchecked {
				return (int) (LacingValues.GetHashCode () ^
					_version ^ (int) _flags ^
					(int) _absolute_granular_position ^
					_stream_serial_number ^
					_page_sequence_number ^ _size ^
					_data_size);
		}
		}
      
      public override bool Equals (object obj)
      {
         if (!(obj is PageHeader))
            return false;
         
         return Equals ((PageHeader) obj);
      }
      
      public bool Equals (PageHeader other)
      {
         return _packet_sizes == other._packet_sizes &&
         _version == other._version && _flags == other._flags &&
         _absolute_granular_position == other._absolute_granular_position &&
         _stream_serial_number == other._stream_serial_number &&
         _page_sequence_number == other._page_sequence_number &&
         _size == other._size && _data_size == other._data_size;
      }
      
      public static bool operator == (PageHeader first, PageHeader second)
      {
         return first.Equals (second);
      }
      
      public static bool operator != (PageHeader first, PageHeader second)
      {
         return !first.Equals (second);
      }
      #endregion
   }
}
