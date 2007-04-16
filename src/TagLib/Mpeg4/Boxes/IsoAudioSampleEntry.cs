namespace TagLib.Mpeg4
{
   public class IsoAudioSampleEntry : IsoSampleEntry
   {
#region Private Properties
      
      private ushort channel_count;
      private ushort sample_size;
      private uint   sample_rate;
      private BoxList children;
      
#endregion
      
      
#region Constructors
      
      public IsoAudioSampleEntry (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         file.Seek (base.DataOffset + 8);
         channel_count = (ushort) file.ReadBlock (2).ToShort ();
         sample_size   = (ushort) file.ReadBlock (2).ToShort ();
         file.Seek (base.DataOffset + 16);
         sample_rate   = (uint)   file.ReadBlock (4).ToUInt  ();
         children = LoadChildren (file);
      }
      
#endregion
      
      
#region Public Properties
      
      public             ushort  ChannelCount {get {return channel_count;}}
      public             ushort  SampleSize   {get {return sample_size;}}
      public             uint    SampleRate   {get {return sample_rate >> 16;}}
      protected override long    DataOffset   {get {return base.DataOffset + 20;}}
      public    override BoxList Children     {get {return children;}}
      
#endregion
   }
}
