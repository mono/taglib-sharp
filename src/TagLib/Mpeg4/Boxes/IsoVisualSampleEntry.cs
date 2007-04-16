namespace TagLib.Mpeg4
{
   public class IsoVisualSampleEntry : IsoSampleEntry
   {
#region Private Properties
      
      private ushort width;
      private ushort height;
      // private BoxList children;
      
#endregion
      
      
#region Constructors
      
      public IsoVisualSampleEntry (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         file.Seek (base.DataOffset + 16);
         width  = (ushort) file.ReadBlock (2).ToShort ();
         height = (ushort) file.ReadBlock (2).ToShort ();
         // TODO: What are the children anyway?
         // children = LoadChildren (file);
      }
      
#endregion
      
      
#region Public Properties
      
      public             ushort  Height     {get {return width;}}
      public             ushort  Width      {get {return height;}}
      protected override long    DataOffset {get {return base.DataOffset + 62;}}
      // public    override BoxList Children   {get {return children;}}
      
#endregion
   }
}