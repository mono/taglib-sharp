using System;
using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class FileParser
   {
      private File file;
      private BoxHeader first_header;
      
      private IsoMovieHeaderBox mvhd_box;
      private IsoUserDataBox udta_box;
      
      private BoxHeader [] moov_tree;
      private BoxHeader [] udta_tree;
      
      private BoxList stco_boxes;
      private BoxList stsd_boxes;
      
      public FileParser (File file)
      {
         this.file = file;
         first_header = new BoxHeader (file, 0);
         
         if (first_header.BoxType != "ftyp")
            throw new CorruptFileException ("File does not start with 'ftyp' box.");
         
         mvhd_box = null;
         udta_box = null;
         
         moov_tree = null;
         udta_tree = null;
         
         stco_boxes = new BoxList ();
         stsd_boxes = new BoxList ();
      }
      
      public IsoMovieHeaderBox MovieHeaderBox {get {return mvhd_box;}}
      public IsoUserDataBox UserDataBox {get {return udta_box;}}
      public IsoAudioSampleEntry AudioSampleEntry
      {
         get
         {
            foreach (IsoSampleDescriptionBox box in stsd_boxes)
               foreach (Box sample_entry in box.Children)
                  if (sample_entry is IsoAudioSampleEntry)
                     return sample_entry as IsoAudioSampleEntry;
            return null;
         }
      }
      public BoxHeader [] MoovTree {get {return moov_tree;}}
      public BoxHeader [] UdtaTree {get {return udta_tree;}}
      public Box [] ChunkOffsetBoxes {get {return stco_boxes.ToArray ();}}
      
      private List<BoxHeader> AddParent (List<BoxHeader> parents, BoxHeader current)
      {
         List<BoxHeader> boxes = new List<BoxHeader> ();
         if (parents != null)
            boxes.AddRange (parents);
         boxes.Add (current);
         return boxes;
      }
		
		public void ParseBoxHeaders ()
		{
         ParseBoxHeaders (first_header.TotalBoxSize, file.Length, null);
		}
      
      private void ParseBoxHeaders (long start, long end, List<BoxHeader> parents)
      {
         long position = start;
         for (BoxHeader header = new BoxHeader (file, position); header.TotalBoxSize != 0 && position < end; position += header.TotalBoxSize)
         {
            header = new BoxHeader (file, position);
            
            if (moov_tree == null && header.BoxType == BoxTypes.Moov)
            {
               List<BoxHeader> new_parents = AddParent (parents, header);
               moov_tree = new_parents.ToArray ();
               ParseBoxHeaders (header.DataOffset + position, header.TotalBoxSize + position, new_parents);
            }
            else if (header.BoxType == BoxTypes.Mdia || header.BoxType == BoxTypes.Minf || header.BoxType == BoxTypes.Stbl || header.BoxType == BoxTypes.Trak)
               ParseBoxHeaders (header.DataOffset + position, header.TotalBoxSize + position, AddParent (parents, header));
            else if (udta_box == null && header.BoxType == BoxTypes.Udta)
              udta_tree = AddParent (parents, header).ToArray ();
		   }
		}
		
		public void ParseTag ()
		{
         ParseTag (first_header.TotalBoxSize, file.Length, null, null);
		}
      
      private void ParseTag (long start, long end, BoxHeader parent, Box handler)
      {
         long position = start;
         for (BoxHeader header = new BoxHeader (file, position); header.TotalBoxSize != 0 && position < end; position += header.TotalBoxSize)
         {
            header = new BoxHeader (file, position);
            
            if (header.BoxType == BoxTypes.Moov || header.BoxType == BoxTypes.Mdia || header.BoxType == BoxTypes.Minf || header.BoxType == BoxTypes.Stbl || header.BoxType == BoxTypes.Trak)
               ParseTag (header.DataOffset + position, header.TotalBoxSize + position, header, handler);
            else if (udta_box == null && header.BoxType == BoxTypes.Udta)
               udta_box = BoxFactory.CreateBox (header, file, parent, handler) as IsoUserDataBox;
		   }
		}
		
		public void ParseTagAndProperties ()
		{
         ParseTagAndProperties (first_header.TotalBoxSize, file.Length, null, null);
		}
      
      private void ParseTagAndProperties (long start, long end, BoxHeader parent, Box handler)
      {
         long position = start;
         for (BoxHeader header = new BoxHeader (file, position); header.TotalBoxSize != 0 && position < end; position += header.TotalBoxSize)
         {
            header = new BoxHeader (file, position);
            
            if (header.BoxType == BoxTypes.Moov || header.BoxType == BoxTypes.Mdia || header.BoxType == BoxTypes.Minf || header.BoxType == BoxTypes.Stbl || header.BoxType == BoxTypes.Trak)
               ParseTagAndProperties (header.DataOffset + position, header.TotalBoxSize + position, header, handler);
            else if (header.BoxType == BoxTypes.Stsd)
               stsd_boxes.Add (BoxFactory.CreateBox (header, file, parent, handler));
            else if (header.BoxType == BoxTypes.Hdlr)
               handler = BoxFactory.CreateBox (header, file, parent, handler) as IsoHandlerBox;
            else if (mvhd_box == null && header.BoxType == BoxTypes.Mvhd)
               mvhd_box = BoxFactory.CreateBox (header, file, parent, handler) as IsoMovieHeaderBox;
            else if (udta_box == null && header.BoxType == BoxTypes.Udta)
               udta_box = BoxFactory.CreateBox (header, file, parent, handler) as IsoUserDataBox;
		   }
		}
		
		public void ParseChunkOffsets ()
		{
         ParseChunkOffsets (first_header.TotalBoxSize, file.Length);
		}
      
		private void ParseChunkOffsets (long start, long end)
      {
         long position = start;
         for (BoxHeader header = new BoxHeader (file, position); header.TotalBoxSize != 0 && position < end; position += header.TotalBoxSize)
         {
            header = new BoxHeader (file, position);
            
            if (header.BoxType == BoxTypes.Moov)
               ParseChunkOffsets (header.DataOffset + position, header.TotalBoxSize + position);
            else if (header.BoxType == BoxTypes.Moov || header.BoxType == BoxTypes.Mdia || header.BoxType == BoxTypes.Minf || header.BoxType == BoxTypes.Stbl || header.BoxType == BoxTypes.Trak)
               ParseChunkOffsets (header.DataOffset + position, header.TotalBoxSize + position);
            else if (header.BoxType == BoxTypes.Stco || header.BoxType == BoxTypes.Co64)
               stco_boxes.Add (BoxFactory.CreateBox (header, file, null, null));
		   }
		}
	}
}
