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
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public PageHeader Header {get {return header;}}
      
      public ByteVector[] Packets {get {return packets.ToArray ();}}
      
      public uint Size {get {return header.Size + header.DataSize;}}
   }
}
