namespace TagLib.Mpeg4
{
   public class UnknownBox : Box
   {
      private ByteVector data;
      
      public UnknownBox (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, handler)
      {
         this.data = LoadData (file);
      }
      
      public override ByteVector Data {get {return data;} set {data = value;}}
   }
}
