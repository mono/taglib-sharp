namespace TagLib.Mpeg4
{
   public class IsoMetaBox : FullBox
   {
      private BoxList children;
      
      public IsoMetaBox (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         children = LoadChildren (file);
      }
      
      public IsoMetaBox (ByteVector handler_type, string handler_name) : base ("meta", 0, 0)
      {
         children = new BoxList ();
         Children.Add (new IsoHandlerBox (handler_type, handler_name));
      }
      
      public override BoxList Children {get {return children;}}
   }
}
