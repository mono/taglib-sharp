namespace TagLib.Mpeg4
{
   public class AppleItemListBox : Box
   {
      private BoxList children;
      
      public AppleItemListBox (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         children = LoadChildren (file);
      }
      
      public AppleItemListBox () : base ("ilst")
      {
         children = new BoxList ();
      }
      
      public override BoxList Children {get {return children;}}
   }
}