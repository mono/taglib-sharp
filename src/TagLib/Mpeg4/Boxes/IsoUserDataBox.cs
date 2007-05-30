using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class IsoUserDataBox : Box
   {
      private IEnumerable<Box> children;
      
      public IsoUserDataBox (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, handler)
      {
         children = LoadChildren (file);
      }
      
      public IsoUserDataBox () : base ("udta")
      {
         children = new List<Box> ();
      }
      
      public override IEnumerable<Box> Children {get {return children;}}
   }
}
