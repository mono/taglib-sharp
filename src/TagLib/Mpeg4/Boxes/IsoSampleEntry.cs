namespace TagLib.Mpeg4
{
   public class IsoSampleEntry : Box
   {
#region Private Properties
      
      private ushort data_reference_index;
      
#endregion
      
      
#region Constructors
      
      public IsoSampleEntry (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, handler)
      {
         if (file == null)
            throw new System.ArgumentNullException ("file");
         
         file.Seek (base.DataPosition + 6);
         data_reference_index   = file.ReadBlock (2).ToUShort ();
      }
      
#endregion
      
      
#region Public Properties
      
      public             ushort DataReferenceIndex {get {return data_reference_index;}}
      protected override long   DataPosition       {get {return base.DataPosition + 8;}}
      
#endregion
   }
}
