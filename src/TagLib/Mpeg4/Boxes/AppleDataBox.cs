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
      
      #region Protected Methods
      protected override ByteVector Render (ByteVector data)
      {
         ByteVector output = new ByteVector (4);
         output.Add (data);
         
         return base.Render (output);
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
      protected override long DataPosition {get {return base.DataPosition + 4;}}
      public override ByteVector Data {get {return data;} set {data = value;}}
      #endregion
   }
}
