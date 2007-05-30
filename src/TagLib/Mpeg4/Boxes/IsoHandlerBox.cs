namespace TagLib.Mpeg4
{
   public class IsoHandlerBox : FullBox
   {
      private ByteVector handler_type;
      private string name;
      
      public IsoHandlerBox (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, file, handler)
      {
         if (file == null)
            throw new System.ArgumentNullException ("file");
         
         file.Seek (DataPosition + 4);
         ByteVector box_data = file.ReadBlock (DataSize - 4);
         handler_type = box_data.Mid (0, 4);
         name = box_data.Mid (16, box_data.Find ((byte) 0, 16) - 16).ToString ();
      }
      
      public IsoHandlerBox (ByteVector handlerType, string name) : base ("hdlr", 0, 0)
      {
         if (handlerType == null)
            throw new System.ArgumentNullException ("handlerType");
         
         this.handler_type = handlerType.Mid (0,4);
         this.name = name;
      }
      
      public override ByteVector Data
      {
         get
         {
            ByteVector output = new ByteVector (4);
            output.Add (handler_type);
            output.Add (new ByteVector (12));
            output.Add (ByteVector.FromString (name));
            output.Add (new ByteVector (2));
            return output;
         }
      }
      
      public ByteVector HandlerType {get {return handler_type;}}
      public string     Name        {get {return name;}}
   }
}
