namespace TagLib.Mpeg4
{
   public class IsoUserDataBox : Box
   {
      private BoxList children;
      
      public IsoUserDataBox (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         children = LoadChildren (file);
      }
      
      public IsoUserDataBox () : base ("udta")
      {
         children = new BoxList ();
      }
      
      public override BoxList Children {get {return children;}}
   }
}
