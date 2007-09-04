//
// Paginator.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   oggpage.cpp from TagLib
//
// Copyright (C) 2006-2007 Brian Nickel
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
	public class Paginator
	{
      private ByteVectorCollection packets;
      private PageHeader? first_page_header;
      private Codec codec;
      
		public Paginator (Codec codec)
		{
         packets = new ByteVectorCollection ();
         first_page_header = null;
         this.codec = codec;
		}
      
      public void AddPage (Page page)
      {
         if (first_page_header == null)
            first_page_header = page.Header;
         
         if (page.Packets.Length == 0)
            return;
            
         ByteVector[] page_packets = page.Packets;
            
         for (int i = 0; i < page_packets.Length; i ++)
         {
            if ((page.Header.Flags & PageFlags.FirstPacketContinued) != 0 && i == 0 && packets.Count > 0)
               packets [packets.Count - 1].Add (page_packets [0]);
            else
               packets.Add (page_packets [i]);
         }
      }
      
      public void SetComment (XiphComment comment)
      {
         codec.SetCommentPacket (packets, comment);
      }
      
      
      public Page [] Paginate ()
      {
         List<Page> pages = new List<Page> ();
         
         int total_size = packets.Count;
         
         foreach (ByteVector b in packets)
            total_size += b.Count;

         // TODO: Handle creation of multiple pages here with appropriate pagination.
         if (total_size > 0xFF00)
            throw new NotImplementedException ("Repagination is not yet implemented.");
            

         pages.Add (new Page (packets, (PageHeader) first_page_header));
         return pages.ToArray ();
      }
	}
}
