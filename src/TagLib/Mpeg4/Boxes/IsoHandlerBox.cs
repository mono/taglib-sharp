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
         // Reserved
         File.Seek (base.DataPosition + 4);
         
         // Read the handler type.
         handler_type = File.ReadBlock (4);
         
         // Reserved
         File.Seek (base.DataPosition + 20);
         
         // Find the terminating byte and read a string from the data before it.
         long end = File.Find ((byte) 0, File.Tell);
         name = File.ReadBlock ((int) (end - File.Tell)).ToString ();
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
         output += handler_type;
         output += new ByteVector (12);
         output += ByteVector.FromString (name);
         output += new ByteVector (2);
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
