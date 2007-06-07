namespace TagLib.Mpeg4
{
   public class AppleAdditionalInfoBox : FullBox
   {
      #region Private Properties
      private ByteVector data;
      #endregion
      
      #region Constructors
      /*public AppleAdditionalInfoBox (BoxHeader header, ByteVector data, int data_offset, Box handler) : base (header, data, data_offset, handler)
      {
         Data = LoadData (data);
         text = Data.ToString (StringType.Latin1);
      }*/
      public AppleAdditionalInfoBox (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, file, handler)
      {
         Data = file.ReadBlock (DataSize);
      }
      #endregion
      
      #region Public Properties
      public string Text
      {
         // When we set the value, store it as a the data too.
         get {return Data.ToString (StringType.Latin1);}
         set {Data = ByteVector.FromString (value, StringType.Latin1);}
      }
      
      public override ByteVector Data
      {
         get {return data;} set {data = value;}
      }
      #endregion
   }
}
