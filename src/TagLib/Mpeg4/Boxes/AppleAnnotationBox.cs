using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class AppleAnnotationBox : Box
   {
      private IEnumerable<Box> children;
      
      public AppleAnnotationBox (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, handler)
      {
         children = LoadChildren (file);
      }
      
      public AppleAnnotationBox (ByteVector type) : base (type)
      {
         children = new List<Box> ();
      }
      
      public override IEnumerable<Box> Children {get {return children;}}
   }
}