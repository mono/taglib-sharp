namespace TagLib.Mpeg4
{
   public class IsoSampleEntry : Box
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private ushort data_reference_index;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public IsoSampleEntry (BoxHeader header, Box parent) : base (header, parent)
      {
         data_reference_index   = (ushort) InternalData.Mid (6, 2).ToShort ();
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public             ushort DataReferenceIndex {get {return data_reference_index;}}
      protected override long   DataPosition       {get {return base.DataPosition + 8;}}
      protected override ulong  DataSize           {get {return base.DataSize - 8;}}
   }
   
   // Audio Sequences
   public class IsoAudioSampleEntry : IsoSampleEntry
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private ushort channel_count;
      private ushort sample_size;
      private uint   sample_rate;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public IsoAudioSampleEntry (BoxHeader header, Box parent) : base (header, parent)
      {
         channel_count = (ushort) InternalData.Mid (16, 2).ToShort (); //  8 - 10
         sample_size   = (ushort) InternalData.Mid (18, 2).ToShort (); // 10 - 12
         sample_rate   = (uint)   InternalData.Mid (24, 4).ToUInt  (); // 16 - 20
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public             ushort ChannelCount {get {return channel_count;}}
      public             ushort SampleSize   {get {return sample_size;}}
      public             uint   SampleRate   {get {return sample_rate >> 16;}}
      
      public    override bool   HasChildren  {get {return true;}}
      
      protected override long   DataPosition {get {return base.DataPosition + 20;}}
      protected override ulong  DataSize     {get {return base.DataSize - 20;}}
   }
}
