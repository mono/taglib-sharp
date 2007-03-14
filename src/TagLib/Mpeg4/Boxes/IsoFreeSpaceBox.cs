namespace TagLib.Mpeg4
{
   public class IsoFreeSpaceBox : Box
   {
      #region Private Properties
      private long padding;
      #endregion
      
      #region Constructors
      public IsoFreeSpaceBox (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         padding = DataSize;
      }
      
      public IsoFreeSpaceBox (long padding) : base ("free")
      {
         PaddingSize = padding;
      }
      #endregion
      
      #region Public Properties
      public override ByteVector Data
      {
         // Get returns an array comprized entirely of zeros.
         get {return new ByteVector ((int) padding);}
         set {PaddingSize = (value != null) ? value.Count : 0;}
      }
      
      public long PaddingSize
      {
         get {return padding + 8;}
         set {padding = value - 8;}
      }
      #endregion
   }
}
