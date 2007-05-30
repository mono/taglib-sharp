using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class AppleItemListBox : Box
   {
      private IEnumerable<Box> children;
      
      public AppleItemListBox (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, handler)
      {
         children = LoadChildren (file);
      }
      
      public AppleItemListBox () : base ("ilst")
      {
         children = new List<Box> ();
      }
      
      public override IEnumerable<Box> Children {get {return children;}}
   }
}