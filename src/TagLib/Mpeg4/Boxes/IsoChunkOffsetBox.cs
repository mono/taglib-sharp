namespace TagLib.Mpeg4
{
   public class IsoChunkOffsetBox : FullBox
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private uint [] offsets;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public IsoChunkOffsetBox (BoxHeader header, Box parent) : base (header, parent)
      {
         File.Seek (base.DataPosition);
         offsets = new uint [(int) File.ReadBlock (4).ToUInt ()];
         
         for (int i = 0; i < offsets.Length; i ++)
	         offsets [i] = File.ReadBlock (4).ToUInt ();
      }
      
      private ByteVector UpdateOffsetInternal (int size_difference)
      {
      	ByteVector output = ByteVector.FromUInt ((uint) offsets.Length);
         for (int i = 0; i < offsets.Length; i ++)
         {
         	offsets [i] = (uint) (offsets [i] + size_difference);
	         output += ByteVector.FromUInt (offsets [i]);
         }
         
         return output;
      }
      
      public ByteVector Render (int size_difference)
      {
         
         return Render (UpdateOffsetInternal (size_difference));
      }
      
      public override ByteVector Render ()
      {
         return Render (0);
      }
      
      public void UpdateOffset (int size_difference)
      {
      	ByteVector new_data = UpdateOffsetInternal (size_difference);
         File.Insert (new_data, DataPosition, new_data.Count);
      }
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public          uint [] Offsets {get {return offsets;}}
      public override ByteVector Data         {get {return null;} set {}}
   }
}
