using System;
using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class FileParser
   {
      private TagLib.File file;
      private BoxHeader first_header;
      
      private IsoMovieHeaderBox mvhd_box;
      private IsoUserDataBox udta_box;
      
      private BoxHeader [] moov_tree;
      private BoxHeader [] udta_tree;
      
      private List<Box> stco_boxes;
      private List<Box> stsd_boxes;
      
      public FileParser (TagLib.File file)
      {
         this.file = file;
         first_header = new BoxHeader (file, 0);
         
         if (first_header.BoxType != "ftyp")
            throw new CorruptFileException ("File does not start with 'ftyp' box.");
         
         stco_boxes = new List<Box> ();
         stsd_boxes = new List<Box> ();
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
      public IsoVisualSampleEntry VisualSampleEntry
      {
         get
         {
            foreach (IsoSampleDescriptionBox box in stsd_boxes)
            {
               foreach (Box sample_entry in box.Children)
               {
                  if (sample_entry is IsoVisualSampleEntry)
                     return sample_entry as IsoVisualSampleEntry;
               }
            }
            return null;
         }
      }
      public BoxHeader [] MoovTree {get {return moov_tree;}}
      public BoxHeader [] UdtaTree {get {return udta_tree;}}
      public Box []       ChunkOffsetBoxes {get {return stco_boxes.ToArray ();}}
      
      private static List<BoxHeader> AddParent (List<BoxHeader> parents, BoxHeader current)
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
            
            if (moov_tree == null && header.BoxType == BoxType.Moov)
            {
               List<BoxHeader> new_parents = AddParent (parents, header);
               moov_tree = new_parents.ToArray ();
               ParseBoxHeaders (header.DataOffset + position, header.TotalBoxSize + position, new_parents);
            }
            else if (header.BoxType == BoxType.Mdia || header.BoxType == BoxType.Minf || header.BoxType == BoxType.Stbl || header.BoxType == BoxType.Trak)
               ParseBoxHeaders (header.DataOffset + position, header.TotalBoxSize + position, AddParent (parents, header));
            else if (udta_box == null && header.BoxType == BoxType.Udta)
              udta_tree = AddParent (parents, header).ToArray ();
         }
      }
		
      public void ParseTag ()
      {
         ParseTag (first_header.TotalBoxSize, file.Length);
      }
      
      private void ParseTag (long start, long end)
      {
         long position = start;
         BoxHeader header;
         do
         {
            header = new BoxHeader (file, position);
            
            if (header.BoxType == BoxType.Moov || header.BoxType == BoxType.Mdia || header.BoxType == BoxType.Minf || header.BoxType == BoxType.Stbl || header.BoxType == BoxType.Trak)
               ParseTag (header.DataOffset + position, header.TotalBoxSize + position);
            else if (udta_box == null && header.BoxType == BoxType.Udta)
               udta_box = BoxFactory.CreateBox (file, header) as IsoUserDataBox;
            
            position += header.TotalBoxSize;
         }
         while (header.TotalBoxSize != 0 && position < end);
      }
		
      public void ParseTagAndProperties ()
      {
         ParseTagAndProperties (first_header.TotalBoxSize, file.Length, null);
      }
      
      private void ParseTagAndProperties (long start, long end, IsoHandlerBox handler)
      {
         long position = start;
         BoxHeader header;
         do
         {
            header = new BoxHeader (file, position);
            ByteVector type = header.BoxType;
            
            if (type == BoxType.Moov || type == BoxType.Mdia || type == BoxType.Minf || type == BoxType.Stbl || type == BoxType.Trak)
               ParseTagAndProperties (header.DataOffset + position, header.TotalBoxSize + position, handler);
            else if (type == BoxType.Stsd)
               stsd_boxes.Add (BoxFactory.CreateBox (file, header, handler));
            else if (type == BoxType.Hdlr)
               handler = BoxFactory.CreateBox (file, header, handler) as IsoHandlerBox;
            else if (mvhd_box == null && type == BoxType.Mvhd)
               mvhd_box = BoxFactory.CreateBox (file, header, handler) as IsoMovieHeaderBox;
            else if (udta_box == null && type == BoxType.Udta)
               udta_box = BoxFactory.CreateBox (file, header, handler) as IsoUserDataBox;
            
            position += header.TotalBoxSize;
         }
         while (header.TotalBoxSize != 0 && position < end);
      }
		
      public void ParseChunkOffsets ()
      {
         ParseChunkOffsets (first_header.TotalBoxSize, file.Length);
      }
      
      private void ParseChunkOffsets (long start, long end)
      {
         long position = start;
         BoxHeader header;
         do
         {
            header = new BoxHeader (file, position);
            
            if (header.BoxType == BoxType.Moov)
               ParseChunkOffsets (header.DataOffset + position, header.TotalBoxSize + position);
            else if (header.BoxType == BoxType.Moov || header.BoxType == BoxType.Mdia || header.BoxType == BoxType.Minf || header.BoxType == BoxType.Stbl || header.BoxType == BoxType.Trak)
               ParseChunkOffsets (header.DataOffset + position, header.TotalBoxSize + position);
            else if (header.BoxType == BoxType.Stco || header.BoxType == BoxType.Co64)
               stco_boxes.Add (BoxFactory.CreateBox (file, header));
            
            position += header.TotalBoxSize;
         }
         while (header.TotalBoxSize != 0 && position < end);
      }
   }
}
