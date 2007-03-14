namespace TagLib.Mpeg4
{
   public class IsoChunkOffsetBox : FullBox
   {
      #region Private Properties
      private uint [] offsets;
      #endregion
      
      #region Constructors
      public IsoChunkOffsetBox (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         ByteVector box_data = LoadData (file);
         
         offsets = new uint [(int) box_data.Mid (0, 4).ToUInt ()];
         
         for (int i = 0; i < offsets.Length; i ++)
           offsets [i] = box_data.Mid (4 + i * 4, 4).ToUInt ();
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
            if (offsets [i] >= after)
               offsets [i] = (uint) (offsets [i] + size_difference);
            output.Add (ByteVector.FromUInt (offsets [i]));
         }
         
         return Render (output);
      }
      
      public override ByteVector Render ()
      {
         return Render (0, 0);
      }
      #endregion
      
      #region Public Properties
      public uint [] Offsets {get {return offsets;}}
      #endregion
   }
}
