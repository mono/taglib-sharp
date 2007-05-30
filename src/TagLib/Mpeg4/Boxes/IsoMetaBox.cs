using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class IsoMetaBox : FullBox
   {
      private IEnumerable<Box> children;
      
      public IsoMetaBox (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, file, handler)
      {
         children = LoadChildren (file);
      }
      
      public IsoMetaBox (ByteVector handler_type, string handler_name) : base ("meta", 0, 0)
      {
         children = new List<Box> ();
         AddChild (new IsoHandlerBox (handler_type, handler_name));
      }
      
      public override IEnumerable<Box> Children {get {return children;}}
   }
}
