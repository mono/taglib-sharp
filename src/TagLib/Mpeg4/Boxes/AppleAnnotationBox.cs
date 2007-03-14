namespace TagLib.Mpeg4
{
   public class AppleAnnotationBox : Box
   {
      private BoxList children;
      
      public AppleAnnotationBox (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         children = LoadChildren (file);
      }
      
      public AppleAnnotationBox (ByteVector type) : base (type)
      {
         children = new BoxList ();
      }
      
      public override BoxList Children {get {return children;}}
   }
}