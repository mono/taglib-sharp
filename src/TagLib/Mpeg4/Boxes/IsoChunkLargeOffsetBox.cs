namespace TagLib.Mpeg4
{
   public class IsoChunkLargeOffsetBox : FullBox
   {
      #region Private Properties
      private ulong [] offsets;
      #endregion
      
      #region Constructors
      public IsoChunkLargeOffsetBox (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, file, handler)
      {
         ByteVector box_data = file.ReadBlock (DataSize);
         
         offsets = new ulong [(int) box_data.Mid (0, 4).ToUInt ()];
         
         for (int i = 0; i < offsets.Length; i ++)
           offsets [i] = box_data.Mid (4 + i * 8, 8).ToULong ();
      }
      #endregion
      
      #region Public Methods
      public void Overwrite (File file, long sizeDifference, long after)
      {
         if (file == null)
            throw new System.ArgumentNullException ("file");
         
         if (Header.Position < 0)
            throw new System.InvalidOperationException ("Cannot overwrite headers created from ByteVectors.");
         
         file.Insert (Render (sizeDifference, after), Header.Position, Size);
      }
      
      public ByteVector Render (long sizeDifference, long after)
      {
         for (int i = 0; i < offsets.Length; i ++)
            if (offsets [i] >= (ulong) after)
               offsets [i] = (ulong) ((long)offsets [i] + sizeDifference);
         
         return Render ();
      }
      
      public override ByteVector Data {
         get
         {
         	ByteVector output = ByteVector.FromUInt ((uint) offsets.Length);
            for (int i = 0; i < offsets.Length; i ++)
               output.Add (ByteVector.FromULong (offsets [i]));
            return output;
         }
      }

      #endregion
      
      #region Public Properties
      public          ulong [] Offsets {get {return offsets;}}
      #endregion
   }
}
