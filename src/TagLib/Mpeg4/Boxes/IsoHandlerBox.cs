namespace TagLib.Mpeg4
{
   public class IsoHandlerBox : FullBox
   {
      private ByteVector handler_type;
      private string name;
      
      public IsoHandlerBox (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         file.Seek (DataOffset + 4);
         ByteVector box_data = file.ReadBlock (DataSize - 4);
         handler_type = box_data.Mid (0, 4);
         name = box_data.Mid (16, box_data.Find ((byte) 0, 16) - 16).ToString ();
      }
      
      public IsoHandlerBox (ByteVector handler_type, string name) : base ("hdlr", 0, 0)
      {
         this.handler_type = handler_type.Mid (0,4);
         this.name = name;
      }
      
      public override ByteVector Render ()
      {
         ByteVector output = new ByteVector (4);
         output.Add (handler_type);
         output.Add (new ByteVector (12));
         output.Add (ByteVector.FromString (name));
         output.Add (new ByteVector (2));
         return Render (output);
      }
      
      public ByteVector HandlerType {get {return handler_type;}}
      public string     Name        {get {return name;}}
   }
}
