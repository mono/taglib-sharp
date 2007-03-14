namespace TagLib.Mpeg4
{
   public class IsoSampleDescriptionBox : FullBox
   {
      #region Private Properties
      private uint entry_count;
      private BoxList children;
      #endregion
      
      #region Constructors
      public IsoSampleDescriptionBox (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         file.Seek (DataOffset);
         entry_count = file.ReadBlock (4).ToUInt ();
         children = LoadChildren (file);
      }
      #endregion
      
      #region Public Properties
      public uint EntryCount {get {return entry_count;}}
      public override BoxList Children {get {return children;}}
      protected override long DataOffset     {get {return base.DataOffset + 4;}}
      #endregion
   }
}
