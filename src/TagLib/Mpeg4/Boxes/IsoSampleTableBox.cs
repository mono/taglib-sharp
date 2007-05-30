using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class IsoSampleTableBox : Box
   {
      private IEnumerable<Box> children;
      
      public IsoSampleTableBox (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, handler)
      {
         children = LoadChildren (file);
      }
      
      public override IEnumerable<Box> Children {get {return children;}}
   }
}
