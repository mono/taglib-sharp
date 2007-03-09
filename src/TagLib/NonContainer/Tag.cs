using System;
using System.Collections.Generic;

namespace TagLib.NonContainer
{
   public class Tag : CombinedTag
   {
      private StartTag start_tag;
      private EndTag end_tag;
      
      public Tag (File file) : base ()
      {
         start_tag = new StartTag (file);
         end_tag = new EndTag (file);
         AddTag (start_tag);
         AddTag (end_tag);
      }
      
      public StartTag StartTag {get {return start_tag;}}
      public EndTag EndTag {get {return end_tag;}}
      
      public override TagLib.Tag [] Tags
      {
         get
         {
            List<TagLib.Tag> tags = new List<TagLib.Tag> ();
            tags.AddRange (start_tag.Tags);
            tags.AddRange (end_tag.Tags);
            return tags.ToArray ();
         }
      }
      
      public TagLib.Tag GetTag (TagTypes type)
      {
         foreach (TagLib.Tag t in Tags)
         {
            if (type == TagTypes.Id3v1 && t is TagLib.Id3v1.Tag)
               return t;
            
            if (type == TagTypes.Id3v2 && t is TagLib.Id3v2.Tag)
               return t;
            
            if (type == TagTypes.Ape && t is TagLib.Ape.Tag)
               return t;
         }
         
         return null;
      }
      
      public void RemoveTags (TagTypes types)
      {
         start_tag.RemoveTags (types);
         end_tag.RemoveTags (types);
      }
      
      public void Read (out long start, out long end)
      {
         start = ReadStart ();
         end = ReadEnd ();
      }
      
      public long ReadStart ()
      {
         return start_tag.Read ();
      }
      
      public long ReadEnd ()
      {
         return end_tag.Read ();
      }
      
      public void Write (out long start, out long end)
      {
         start = start_tag.Write ();
         end = end_tag.Write ();
      }
   }
}
