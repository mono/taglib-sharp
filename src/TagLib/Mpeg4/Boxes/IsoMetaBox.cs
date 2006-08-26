namespace TagLib.Mpeg4
{
   public class IsoMetaBox : FullBox
   {
      public IsoMetaBox (BoxHeader header, Box parent) : base (header, parent)
      {
      }
      
      public IsoMetaBox (ByteVector handler_type, string handler_name, Box parent) : base ("meta", 0, parent)
      {
         AddChild (new IsoHandlerBox (handler_type, handler_name, this));
      }
      
      public IsoMetaBox (ByteVector handler_type, string handler_name) : this (handler_type, handler_name, null)
      {
      }
      
      public override bool  HasChildren       {get {return true;}}
      
      public override ByteVector Data
      {
         get {return null;}
         set {}
      }
   }
}
