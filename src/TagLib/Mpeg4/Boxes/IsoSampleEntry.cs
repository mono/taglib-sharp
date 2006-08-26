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
         File.Seek (base.DataPosition + 6);
         data_reference_index   = (ushort) File.ReadBlock (2).ToShort ();
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
         File.Seek (base.DataPosition + 8);
         channel_count = (ushort) File.ReadBlock (2).ToShort ();
         sample_size   = (ushort) File.ReadBlock (2).ToShort ();
         
         File.Seek (base.DataPosition + 16);
         sample_rate   = (uint)   File.ReadBlock (4).ToUInt  ();
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
