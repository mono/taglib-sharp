using System;

namespace TagLib.NonContainer
{
   public class EndTag : CombinedTag
   {
      private TagLib.File file;
      
      public EndTag (TagLib.File file) : base ()
      {
         this.file = file;
      }
      
      public long Read ()
      {
         TagLib.Tag tag;
         ClearTags ();
         long start = file.Length;
         
         while ((tag = ReadTag (ref start)) != null)
         {
            AddTag (tag);
         }
         
         return start;
      }
      
      public ByteVector Render ()
      {
         ByteVector data = new ByteVector ();
         foreach (TagLib.Tag t in Tags)
         {
            if (t is TagLib.Ape.Tag)
               data.Add ((t as TagLib.Ape.Tag).Render ());
            else if (t is TagLib.Id3v2.Tag)
               data.Add ((t as TagLib.Id3v2.Tag).Render (true));
         }
         
         return data;
      }
      
      public long Write ()
      {
         long total_size = TotalSize;
         ByteVector data = Render ();
         file.Insert (data, file.Length - total_size, total_size);
         return file.Length - data.Count;
      }
      
      public long TotalSize
      {
         get
         {
            long start = file.Length;
            while (ReadTagInfo (ref start) != TagTypes.NoTags)
               ;
            
            return file.Length - start;
         }
      }
      
      private TagLib.Tag ReadTag (ref long end)
      {
         long start = end;
         TagTypes type = ReadTagInfo (ref start);
         TagLib.Tag tag = null;
         try
         {
            switch (type)
            {
            case TagTypes.Ape:
               tag = new TagLib.Ape.Tag (file, end - TagLib.Ape.Footer.Size);
               end = start;
               break;
            case TagTypes.Id3v2:
               tag = new TagLib.Id3v2.Tag (file, start);
               end = start;
               break;
            case TagTypes.Id3v1:
               tag = new TagLib.Id3v1.Tag (file, start);
               end = start;
               break;
            }
         }
         catch (CorruptFileException) {}
         
         return tag;
      }
      
      private TagTypes ReadTagInfo (ref long position)
      {
         int read_size = (int) Math.Max (Math.Max (TagLib.Ape.Footer.Size, TagLib.Id3v2.Footer.Size), TagLib.Id3v1.Tag.Size);
         file.Seek (position - read_size);
         ByteVector data = file.ReadBlock (read_size);
         
         try
         {
            ByteVector tag_data = data.Mid ((int)(data.Count - TagLib.Ape.Footer.Size));
            if (tag_data.StartsWith (TagLib.Ape.Footer.FileIdentifier))
            {
               TagLib.Ape.Footer footer = new TagLib.Ape.Footer (tag_data);
               position -= footer.CompleteTagSize;
               return TagTypes.Ape;
            }
         
            tag_data = data.Mid ((int)(data.Count - TagLib.Id3v2.Footer.Size));
            if (tag_data.StartsWith (TagLib.Id3v2.Footer.FileIdentifier))
            {
               TagLib.Id3v2.Footer footer = new TagLib.Id3v2.Footer (tag_data);
               position -= footer.CompleteTagSize;
               return TagTypes.Id3v2;
            }
            
            if (data.StartsWith (TagLib.Id3v1.Tag.FileIdentifier))
            {
               position -= TagLib.Id3v1.Tag.Size;
               return TagTypes.Id3v1;
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
         if (type == TagTypes.Id3v1)
            tag = new TagLib.Id3v1.Tag ();
         else if (type == TagTypes.Id3v2)
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