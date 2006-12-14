namespace TagLib.Mpeg4
{
   public class IsoHandlerBox : FullBox
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private ByteVector handler_type;
      private string name;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public IsoHandlerBox (BoxHeader header, Box parent) : base (header, parent)
      {
         long end = File.Find ((byte) 0, base.DataPosition + 20);
         File.Seek (base.DataPosition + 4);
         ByteVector data = File.ReadBlock ((int)(end - base.DataPosition - 4));

         handler_type = data.Mid (4);
         name = data.Mid (16).ToString ();
      }
      
      // We can make our own handler.
      public IsoHandlerBox (ByteVector handler_type, string name, Box parent) : base ("hdlr", 0, parent)
      {
         this.handler_type = handler_type.Mid (0,4);
         this.name = name;
      }
      
      // Render everything.
      public override ByteVector Render ()
      {
         ByteVector output = new ByteVector (4);
         output.Add (handler_type);
         output.Add (new ByteVector (12));
         output.Add (ByteVector.FromString (name));
         output.Add (new ByteVector (2));
         return Render (output);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public ByteVector HandlerType {get {return handler_type;}}
      
      public string Name {get {return name;}}
      
      // This box has no readable data.
      public override ByteVector Data
      {
         get {return null;}
         set {}
      }
   }
}
