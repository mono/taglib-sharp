namespace TagLib.Mpeg4
{
   public class UnknownBox : Box
   {
      private ByteVector data;
      
      public UnknownBox (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         this.data = LoadData (file);
      }
      
      public override ByteVector Data {get {return data;} set {data = value;}}
   }
}
