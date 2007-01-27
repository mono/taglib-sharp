namespace TagLib.Mpeg4
{
   public class IsoSampleDescriptionBox : FullBox
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private uint entry_count;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public IsoSampleDescriptionBox (BoxHeader header, Box parent) : base (header, parent)
      {
         // This box just contains a number saying how many of the first boxes
         // will be SampleEntries, since they can be named whatever they want to
         // be.
         entry_count = InternalData.Mid (4, 4).ToUInt ();
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public uint EntryCount {get {return entry_count;}}
      
      // This box contains no data and has children.
      public override bool  HasChildren       {get {return true;}}
      
      public override ByteVector Data
      {
         get {return null;}
         set {}
      }
      
      // Offset for those bytes.
      protected override long DataPosition     {get {return base.DataPosition + 4;}}
      protected override ulong DataSize        {get {return base.DataSize - 4;}}
   }
}
