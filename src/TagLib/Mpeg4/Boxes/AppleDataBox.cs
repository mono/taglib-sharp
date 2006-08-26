namespace TagLib.Mpeg4
{
   public class AppleDataBox : FullBox
   {
      // This shows the type of data stored in the box.
      public enum FlagTypes
      {
          ContainsText      = 0x01,
          ContainsData      = 0x00,
          ForTempo          = 0x15,
          ContainsJpegData  = 0x0D,
          ContainsPngData   = 0x0D
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public AppleDataBox (BoxHeader header, Box parent) : base (header, parent)
      {
         // Ensure that the data is loaded before the file is closed.
         LoadData ();
      }
      
      // Make a data box from the given data and flags.
      public AppleDataBox (ByteVector data, uint flags, Box parent) : base ("data", flags, parent)
      {
         Data = data;
      }
      
      // Make a data box from the given data and flags.
      public AppleDataBox (ByteVector data, uint flags) : this (data, flags, null)
      {
      }
      
      // Render the box. It starts with four reserved bytes.
      public override ByteVector Render ()
      {
         return Render (new ByteVector (4));
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      
      // If the flag type is ContainsText, then the data can be read as a UTF8
      // string. We can also store a UTF8 string as text if we set the flags to
      // show that.
      public string Text
      {
         get {return ((Flags & (int) FlagTypes.ContainsText) != 0) ? Data.ToString (StringType.UTF8) : null;}
         set
         {
            Flags = (int) FlagTypes.ContainsText;
            Data = ByteVector.FromString (value, StringType.UTF8);
         }
      }
      
      // Move the position and length to account for the reserved space.
      protected override long  DataPosition {get {return base.DataPosition + 4;}}
      protected override ulong DataSize     {get {return base.DataSize - 4;}}
   }
}
