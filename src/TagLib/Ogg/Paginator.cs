using System;
using System.Collections.Generic;

namespace TagLib.Ogg
{
	public class Paginator
	{
      private ByteVectorList packets;
      private PageHeader first_page_header;
      private Codec codec;
      
		public Paginator (Codec codec)
		{
         packets = new ByteVectorList ();
         first_page_header = null;
         this.codec = codec;
		}
      
      public void AddPage (Page page)
      {
         if (first_page_header == null)
            first_page_header = page.Header;
         
         if (page.Packets.Count == 0)
            return;
            
         ByteVectorList page_packets = page.Packets;
            
         for (int i = 0; i < page_packets.Count; i ++)
         {
            if (page.Header.FirstPacketContinued && i == 0)
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
            

         pages.Add (new Page (packets, first_page_header));
         return pages.ToArray ();
      }
	}
}
