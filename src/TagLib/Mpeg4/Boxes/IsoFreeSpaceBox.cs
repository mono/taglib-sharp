namespace TagLib.Mpeg4
{
   public class IsoFreeSpaceBox : Box
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private ulong padding;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public IsoFreeSpaceBox (BoxHeader header, Box parent) : base (header, parent)
      {
         // set padding equal to the size of the zero space.
         padding = DataSize;
      }
      
      public IsoFreeSpaceBox (ulong padding, Box parent) : base ("free", parent)
      {
         PaddingSize = padding;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override ByteVector Data
      {
         // Get returns an array comprized entirely of zeros.
         get {return new ByteVector ((int) padding);}
         set {}
      }
      
      public ulong PaddingSize
      {
         // PaddingSize equals the total rendered size of the box.
         get {return padding + 8;}
         set {padding = value - 8;}
      }
   }
}
