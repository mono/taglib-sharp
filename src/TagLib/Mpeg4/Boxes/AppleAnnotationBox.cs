namespace TagLib.Mpeg4
{
   public class AppleAnnotationBox : Box
   {
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public AppleAnnotationBox (BoxHeader header, Box parent) : base (header, parent)
      {
      }
      
      // This box can be created without loading it.
      public AppleAnnotationBox (ByteVector type, Box parent) : base (type, parent)
      {
      }
      
      public AppleAnnotationBox (ByteVector type) : this (type, null)
      {
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      
      // This box has children, and no readable data.
      public override bool  HasChildren       {get {return true;}}
      
      public override ByteVector Data
      {
         get {return null;}
         set {}
      }
   }
}
