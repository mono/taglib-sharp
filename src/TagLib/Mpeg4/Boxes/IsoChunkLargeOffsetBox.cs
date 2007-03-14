namespace TagLib.Mpeg4
{
   public class IsoChunkLargeOffsetBox : FullBox
   {
      #region Private Properties
      private ulong [] offsets;
      #endregion
      
      #region Constructors
      public IsoChunkLargeOffsetBox (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         ByteVector box_data = LoadData (file);
         
         offsets = new ulong [(int) box_data.Mid (0, 4).ToUInt ()];
         
         for (int i = 0; i < offsets.Length; i ++)
           offsets [i] = (ulong) box_data.Mid (4 + i * 8, 8).ToLong ();
      }
      #endregion
      
      #region Public Methods
      public void Overwrite (File file, long size_difference, long after)
      {
         if (Header.Position < 0)
            throw new System.Exception ("Cannot overwrite headers created from ByteVectors.");
         
         file.Insert (Render (size_difference, after), Header.Position, Size);
      }
      
      public ByteVector Render (long size_difference, long after)
      {
      	ByteVector output = ByteVector.FromUInt ((uint) offsets.Length);
         for (int i = 0; i < offsets.Length; i ++)
         {
            if (offsets [i] >= (ulong) after)
               offsets [i] = (ulong) ((long)offsets [i] + size_difference);
            output.Add (ByteVector.FromLong ((long) offsets [i]));
         }
         
         return Render (output);
      }
      
      public override ByteVector Render ()
      {
         return Render (0, 0);
      }
      #endregion
      
      #region Public Properties
      public          ulong [] Offsets {get {return offsets;}}
      #endregion
   }
}
