namespace TagLib.Mpeg4
{
   public class AppleItemListBox : Box
   {
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public AppleItemListBox (BoxHeader header, Box parent) : base (header, parent)
      {
      }
      
      // This box can be created without loading it.
      public AppleItemListBox (Box parent) : base ("ilst", parent)
      {
      }
      
      public AppleItemListBox () : this (null)
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
