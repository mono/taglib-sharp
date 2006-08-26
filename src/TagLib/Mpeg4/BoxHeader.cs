namespace TagLib.Mpeg4
{
   public class BoxHeader
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private File       file;
      private long       position;
      
      private ByteVector box_type;
      private ByteVector extended_type;
      private ulong      large_size;
      private uint       size;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      
      // Create a new header and read its info from the file.
      public BoxHeader (File file, long position)
      {
         this.file      = file;
         this.position  = position;
         box_type       = null;
         extended_type  = null;
         large_size     = 0;
         size           = 0;
         
         Read ();
      }
      
      // Create a new header with the provided box type.
      public BoxHeader (ByteVector type)
      {
         this.file      = null;
         this.position  = -1;
         box_type       = type;
         extended_type  = null;
         large_size     = 0;
         size           = 0;
      }
      
      // Render the box header and try to keep the size the same.
      public ByteVector Render ()
      {
         // The size is zero because the box header was created not read.
         // Increase the sizes to account for this.
         if (size == 0)
         {
            size = (uint) (extended_type != null ? 24 : 8);
            large_size += size;
         }
         
         // Enlarge for large size if necessary. If large size is in use, the
         // header will be 16 or 32 big as opposed to 8 or 24.
         if ((size == 8 || size == 24) && large_size > System.UInt32.MaxValue)
         {
            size += 8;
            large_size += 8;
         }
         
         // Get ready to output.
         ByteVector output = new ByteVector ();
         
         // Add the box size and type to the output.
         output += ByteVector.FromUInt ((size == 8 || size == 24) ? (uint) large_size : 1);
         output += box_type;
         
         // If the box size is 16 or 32, we must have more a large header to
         // append.
         if (size == 16 || size == 32)
            output += ByteVector.FromLong ((long) large_size);
         
         // The only reason for such a big size is an extended type. Extend!!!
         if (size >= 24)
            output += (extended_type != null) ? extended_type.Mid (0, 16) : new ByteVector (16);
         
         return output;
      }
      
      // Get header information from the file.
      private void Read ()
      {
         // How much can we actually read?
         long space_available = file.Length - position;
         
         // The size has to be at least 8.
         size = 8;
         
         // If we can't read that much, return.
         if (space_available < size)
            return;
         
         // Get into position.
         file.Seek (position);
         
         // Read the size and type of the block.
         large_size = file.ReadBlock (4).ToUInt ();
         box_type = file.ReadBlock (4);
         
         // If the size is zero, the block includes the rest of the file.
         if (large_size == 0)
            large_size = (ulong) space_available;
         // If the size is 1, that just tells us we have a massive ULONG size
         // waiting for us in the next 8 bytes.
         else if (large_size == 1)
         {
            // The size is 8 bigger.
            size += 8;
            
            // If we don't have room, we're lost. Abort.
            if (space_available < size)
            {
               large_size = 0;
               return;
            }
            
            // This file is huge. 4GB+ I don't think we can even read it.
            large_size = (ulong) file.ReadBlock (8).ToLong ();
         }
         
         // UUID has a special header with 16 extra bytes.
         if (box_type == "uuid")
         {
            // Size is 16 bigger.
            size += 16;
            
            // If we don't have room, we're lost. Abort.
            if (space_available < size)
            {
               large_size = 0;
               return;
            }
            
            // Load the extended type.
            extended_type = file.ReadBlock (16);
         }
      }
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      
      // The file the data is loaded from.
      public File File               {get {return file;}}
      
      // If data was read, then the size is non-zero. If the size is non-zero,
      // the header is valid. p-> q -> r.
      public bool  IsValid           {get {return large_size != 0;}}
      
      // the box's type.
      public ByteVector BoxType      {get {return box_type;}}
      
      // The extended type (for UUID only)
      public ByteVector ExtendedType {get {return extended_type;}}

      // The total size of the box.
      public ulong BoxSize           {get {return large_size;}}
      
      // The size of the header.
      public uint  HeaderSize        {get {return size;}}
      
      // The size of the data.
      public ulong DataSize          {get {return large_size - size;} set {large_size = value + size;}}
      
      // The position of the box.
      public long Position           {get {return position;}}
      
      // The position of the data.
      public long DataPosition       {get {return position + size;}}
      
      // the position of the next box.
      public long NextBoxPosition    {get {return (long) (position + (long) large_size);}}
      
   }
}
