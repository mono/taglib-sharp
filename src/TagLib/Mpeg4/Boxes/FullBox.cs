namespace TagLib.Mpeg4
{
   public abstract class FullBox : Box
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private byte version;
      private uint flags;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public FullBox (BoxHeader header, Box parent) : base (header, parent)
      {
         File.Seek (base.DataPosition);
         
         // First 4 bytes contain version and flag data.
         version = File.ReadBlock (1) [0];
         flags   = File.ReadBlock (3).ToUInt ();
      }
      
      // We can create our own box.
      public FullBox (ByteVector type, uint flags, Box parent) : base (new BoxHeader (type), parent)
      {
         version = 0;
         this.flags = flags;
      }
      
      public FullBox (ByteVector type, uint flags) : this (type, flags, null)
      {
      }
      
      public FullBox (ByteVector type, Box parent) : this (type, 0, parent)
      {
      }
      
      public FullBox (ByteVector type) : this (type, 0)
      {
      }
      
      // Render this box with the extra data at the beginning.
      protected override ByteVector Render (ByteVector data)
      {
         ByteVector output = new ByteVector ((byte) version);
         
         output.Add (ByteVector.FromUInt (flags).Mid (1,3));
         output.Add (data);
         
         return base.Render (output);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public uint Version {get {return version;} set {version = (byte)value;}}
      public uint Flags   {get {return flags;}   set {flags = value;}}
      
      // Offset for those foru bytes.
      protected override long DataPosition     {get {return base.DataPosition + 4;}}
      protected override ulong DataSize        {get {return base.DataSize - 4;}}
   }
}
