namespace TagLib.Mpeg4
{
   public class IsoUserDataBox : Box
   {
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public IsoUserDataBox (BoxHeader header, Box parent) : base (header, parent)
      {
      }
      
      // This box can be created without loading it.
      public IsoUserDataBox (Box parent) : base ("udta", parent)
      {
      }
      
      public IsoUserDataBox () : this (null)
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
