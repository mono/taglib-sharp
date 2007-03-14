using System;

namespace TagLib.Mpeg4
{
	public static class BoxFactory
	{
      // Create a box by reading the file and add it to "parent".
      public static Box CreateBox (BoxHeader header, File file, BoxHeader parent, Box handler)
      {
         // The first few children of an "stsd" are sample entries.
         if (parent != null && parent.BoxType == "stsd")
         {
            if (handler != null && (handler as IsoHandlerBox).HandlerType == "soun")
               return new IsoAudioSampleEntry (header, file, handler);
            else
               return new IsoSampleEntry (header, file, handler);
         }
         
         // Standard items...
         ByteVector type = header.BoxType;
         
         if (type == BoxTypes.Mvhd)
            return new IsoMovieHeaderBox (header, file, handler);
         else if (type == BoxTypes.Stbl)
            return new IsoSampleTableBox (header, file, handler);
         else if (type == BoxTypes.Stsd)
            return new IsoSampleDescriptionBox (header, file, handler);
         else if (type == BoxTypes.Stco)
            return new IsoChunkOffsetBox (header, file, handler);
         else if (type == BoxTypes.Co64)
            return new IsoChunkLargeOffsetBox (header, file, handler);
         else if (type == BoxTypes.Hdlr)
            return new IsoHandlerBox (header, file, handler);
         else if (type == BoxTypes.Udta)
            return new IsoUserDataBox (header, file, handler);
         else if (type == BoxTypes.Meta)
            return new IsoMetaBox (header, file, handler);
         else if (type == BoxTypes.Ilst)
            return new AppleItemListBox (header, file, handler);
         else if (type == BoxTypes.Data)
            return new AppleDataBox (header, file, handler);
         else if (type == BoxTypes.Esds)
            return new AppleElementaryStreamDescriptor (header, file, handler);
         else if (type == BoxTypes.Free || type == BoxTypes.Skip)
            return new IsoFreeSpaceBox (header, file, handler);
         else if (type == BoxTypes.Mean || type == BoxTypes.Name)
            return new AppleAdditionalInfoBox (header, file, handler);
         
         // If we still don't have a tag, and we're inside an ItemListBox, load
         // lthe box as an AnnotationBox (Apple tag item).
         if (parent.BoxType == BoxTypes.Ilst)
            return new AppleAnnotationBox (header, file, handler);
         
         // Nothing good. Go generic.
         return new UnknownBox (header, file, handler);
      }
      
      public static Box CreateBox (File file, long position, BoxHeader parent, Box handler)
      {
         BoxHeader header = new BoxHeader (file, position);
         return CreateBox (header, file, parent, handler);
      }
	}
}
