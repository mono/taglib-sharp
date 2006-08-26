namespace TagLib.Mpeg4
{
   public class IsoMediaBox : Box
   {
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public IsoMediaBox (BoxHeader header, Box parent) : base (header, parent)
      {
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      
      // This box has children, and no readable data.
      public override bool HasChildren {get {return true;}}
      
      public override ByteVector Data
      {
         get {return null;}
         set {}
      }
   }
}
