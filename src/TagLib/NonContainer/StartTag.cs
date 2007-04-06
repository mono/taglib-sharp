using System;

namespace TagLib.NonContainer
{
   public class StartTag : CombinedTag
   {
      private TagLib.File file;
      
      public StartTag (TagLib.File file) : base ()
      {
         this.file = file;
      }
      
      public long Read ()
      {
         TagLib.Tag tag;
         ClearTags ();
         long end = 0;
         while ((tag = ReadTag (ref end)) != null)
         {
            AddTag (tag);
         }
         
         return end;
      }
      
      public ByteVector Render ()
      {
         ByteVector data = new ByteVector ();
         foreach (TagLib.Tag t in Tags)
         {
            if (t is TagLib.Ape.Tag)
               data.Add ((t as TagLib.Ape.Tag).Render ());
            else if (t is TagLib.Id3v2.Tag)
               data.Add ((t as TagLib.Id3v2.Tag).Render (false));
         }
         
         return data;
      }
      
      public long Write ()
      {
         ByteVector data = Render ();
         file.Insert (data, 0, TotalSize);
         return data.Count;
      }
      
      public long TotalSize
      {
         get
         {
            long size = 0;
            while (ReadTagInfo (size, out size) != TagTypes.NoTags)
               ;
            
            return size;
         }
      }
      
      private TagLib.Tag ReadTag (ref long index)
      {
         long end;
         TagTypes type = ReadTagInfo (index, out end);
         TagLib.Tag tag = null;
         try
         {
            switch (type)
            {
            case TagTypes.Ape:
               tag = new TagLib.Ape.Tag (file, index);
               index = end;
               break;
            case TagTypes.Id3v2:
               tag = new TagLib.Id3v2.Tag (file, index);
               index = end;
               break;
            }
         }
         catch (CorruptFileException) {}
         
         return tag;
      }
      
      private TagTypes ReadTagInfo (long start, out long end)
      {
         end = start;
         int read_size = (int) Math.Max (TagLib.Ape.Footer.Size, TagLib.Id3v2.Header.Size);
         file.Seek (start);
         ByteVector data = file.ReadBlock (read_size);
         
         try
         {
            if (data.StartsWith (TagLib.Ape.Footer.FileIdentifier))
            {
               TagLib.Ape.Footer footer = new TagLib.Ape.Footer (data);
               end = start + footer.CompleteTagSize;
               return TagTypes.Ape;
            }
            
            if (data.StartsWith (TagLib.Id3v2.Header.FileIdentifier))
            {
               TagLib.Id3v2.Header header = new TagLib.Id3v2.Header (data);
               end = start + header.CompleteTagSize;
               return TagTypes.Id3v2;
            }
         }
         catch (CorruptFileException) {}
         
         return TagTypes.NoTags;
      }
      
      public void RemoveTags (TagTypes types)
      {
         for (int i = Tags.Length - 1; i >= 0; i --)
         {
            TagLib.Tag t = Tags [i];
            
            if (
                ((types & TagTypes.Id3v1) == TagTypes.Id3v1 && t is TagLib.Id3v1.Tag) ||
                ((types & TagTypes.Id3v2) == TagTypes.Id3v2 && t is TagLib.Id3v2.Tag) ||
                ((types & TagTypes.Ape  ) == TagTypes.Ape   && t is TagLib.Ape.Tag  ) 
               )
               RemoveTag (t);
         }
      }
      
      public TagLib.Tag AddTag (TagTypes type, TagLib.Tag copy)
      {
         TagLib.Tag tag = null;
         if (type == TagTypes.Id3v2)
            tag = new TagLib.Id3v2.Tag ();
         else if (type == TagTypes.Ape)
            tag = new TagLib.Ape.Tag ();
         
         if (tag != null)
         {
            if (copy != null)
               Tag.Duplicate (copy, tag, true);
            AddTag (tag);
         }
         
         return tag;
      }
   }
}