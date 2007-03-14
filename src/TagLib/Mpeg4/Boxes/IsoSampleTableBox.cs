namespace TagLib.Mpeg4
{
   public class IsoSampleTableBox : Box
   {
      private BoxList children;
      
      public IsoSampleTableBox (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         children = LoadChildren (file);
      }
      
      public override BoxList Children {get {return children;}}
   }
}
