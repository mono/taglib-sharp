//
// PageHeader.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   oggpage.cpp from TagLib
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

using System.Collections.Generic;
using System;

namespace TagLib.Ogg
{
   public class Page
   {
#region Private Properties
      private PageHeader     header;
      private ByteVectorCollection packets;
#endregion
      
#region Constructors
      protected Page (PageHeader header)
      {
         this.header = header;
         packets = new ByteVectorCollection ();
      }
      
      public Page (File file, long position) : this (new PageHeader (file, position))
      {
         if (file == null)
            throw new ArgumentNullException ("file");
         
         file.Seek (position + header.Size);
         
         foreach (int packet_size in header.PacketSizes)
            packets.Add (file.ReadBlock (packet_size));
      }
      
      public Page (ByteVectorCollection packets, PageHeader header) : this (header)
      {
         if (packets == null)
            throw new ArgumentNullException ("packets");
         
         this.packets = packets;

         List<int> packet_sizes = new List<int> ();

         // Build a page from the list of packets.
         foreach (ByteVector v in packets)
            packet_sizes.Add (v.Count);
         
         header.PacketSizes = packet_sizes.ToArray ();
      }
#endregion
      
      public ByteVector Render ()
      {
         ByteVector data = header.Render ();

         foreach (ByteVector v in packets)
            data.Add (v);

         // Compute and set the checksum for the Ogg page.  The checksum is taken over
         // the entire page with the 4 bytes reserved for the checksum zeroed and then
         // inserted in bytes 22-25 of the page header.

         ByteVector checksum = ByteVector.FromUInt (data.Checksum, false);
         for (int i = 0; i < 4; i++)
            data [i + 22] = checksum [i];

         return data;
      }
		
		public static void OverwriteSequenceNumbers (File file,
		                                             long position,
		                                             IDictionary<uint, int> shiftTable)
		{
			bool done = true;
			foreach (KeyValuePair<uint, int> pair in shiftTable)
				if (pair.Value != 0) {
					done = false;
					break;
				}
			
			if (done)
				return;
			
			while (position < file.Length) {
				PageHeader header = new PageHeader (file, position);
				int size = (int) (header.Size + header.DataSize);
				
				if (shiftTable.ContainsKey (header.StreamSerialNumber)
					&& shiftTable [header.StreamSerialNumber] != 0) {
					file.Seek (position);
					ByteVector page_data = file.ReadBlock (size);
					
					ByteVector new_data = ByteVector.FromUInt (
						(uint)(header.PageSequenceNumber +
						shiftTable [header.StreamSerialNumber]),
						false);
					
					for (int i = 18; i < 22; i ++)
						page_data [i] = new_data [i - 18];
					for (int i = 22; i < 26; i++)
						page_data [i] = 0;
					
					new_data.Add (ByteVector.FromUInt (
						page_data.Checksum, false));
					file.Seek (position + 18);
					file.WriteBlock (new_data);
				}
				position += size;
			}
		}
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public PageHeader Header {get {return header;}}
      
      public ByteVector[] Packets {get {return packets.ToArray ();}}
      
      public uint Size {get {return header.Size + header.DataSize;}}
   }
}
