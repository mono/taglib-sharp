namespace TagLib.Mpeg4
{
   public class AppleDataBox : FullBox
   {
      #region Enums
      // This shows the type of data stored in the box.
      public enum FlagTypes
      {
          ContainsText      = 0x01,
          ContainsData      = 0x00,
          ForTempo          = 0x15,
          ContainsJpegData  = 0x0D,
          ContainsPngData   = 0x0E
      }
      #endregion
      
      #region Private Propertes
      private ByteVector data;
      #endregion
      
      #region Constructors
      public AppleDataBox (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         Data = LoadData (file);
      }
      
      public AppleDataBox (ByteVector data, uint flags) : base ("data", 0, flags)
      {
         Data = data;
      }
      #endregion
      
      #region Public Methods
      public override ByteVector Render ()
      {
         return Render (new ByteVector (4));
      }
      #endregion
      
      #region Public Properties
      public string Text
      {
         get
         {
            return ((Flags & (int) FlagTypes.ContainsText) != 0) ? Data.ToString (StringType.UTF8) : null;
         }
         set
         {
            Flags = (int) FlagTypes.ContainsText;
            Data = ByteVector.FromString (value, StringType.UTF8);
         }
      }
      protected override long DataOffset {get {return base.DataOffset + 4;}}
      public override ByteVector Data {get {return data;} set {data = value;}}
      #endregion
   }
}
