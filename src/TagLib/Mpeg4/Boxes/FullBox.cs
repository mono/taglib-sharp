namespace TagLib.Mpeg4
{
   public abstract class FullBox : Box
   {
      #region Private Properties
      private byte version;
      private uint flags;
      #endregion
      
      #region Constructors
      protected FullBox (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, handler)
      {
         if (file == null)
            throw new System.ArgumentNullException ("file");
         
         file.Seek (base.DataPosition);
         ByteVector header_data = file.ReadBlock (4);
         version = header_data [0];
         flags   = header_data.Mid (1, 3).ToUInt ();
      }
      
      protected FullBox (BoxHeader header, byte version, uint flags) : base (header)
      {
         this.version = version;
         this.flags = flags;
      }
      
      protected FullBox (ByteVector type, byte version, uint flags) : this (new BoxHeader (type), version, flags)
      {}
      #endregion
      
      #region Protected Methods
      protected override ByteVector Render (ByteVector topData)
      {
         ByteVector output = new ByteVector ((byte) version);
         output.Add (ByteVector.FromUInt (flags).Mid (1,3));
         output.Add (topData);
         
         return base.Render (output);
      }
      #endregion
      
      #region Public Properties
      public uint Version {get {return version;} set {version = (byte)value;}}
      public uint Flags   {get {return flags;}   set {flags = value;}}
      protected override long DataPosition     {get {return base.DataPosition + 4;}}
      #endregion
   }
}
