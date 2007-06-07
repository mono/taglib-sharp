using System;

namespace TagLib.Mpeg4
{
   public struct BoxHeader
   {
      #region Private Properties
      private ByteVector box_type;
      private ByteVector extended_type;
      private ulong      box_size;
      private uint       header_size;
      private long       position;
      private Box        box;
      private bool       from_disk;
      #endregion
      
      public static readonly BoxHeader Empty = new BoxHeader (null);
      
      #region Constructors
      public BoxHeader (TagLib.File file, long position)
      {
         if (file == null)
            throw new ArgumentNullException ("file");
         
         this.box = null;
         this.from_disk = true;
         this.position = position;
         file.Seek (position);
         
         ByteVector data = file.ReadBlock (32);
         int offset = 0;
         
         if (data.Count < 8 + offset)
            throw new CorruptFileException ("Not enough data in box header.");
         
         header_size = 8;
         box_size = data.Mid (offset, 4).ToUInt ();
         box_type = data.Mid (offset + 4, 4);
         
         // If the size is 1, that just tells us we have a massive ULONG size
         // waiting for us in the next 8 bytes.
         if (box_size == 1)
         {
            if (data.Count < 8 + offset)
               throw new CorruptFileException ("Not enough data in box header.");
            
            header_size += 8;
            box_size = data.Mid (offset, 8).ToULong ();
            offset += 8;
         }
         
         // UUID has a special header with 16 extra bytes.
         if (box_type == Mpeg4.BoxType.Uuid)
         {
            if (data.Count < 16 + offset)
               throw new CorruptFileException ("Not enough data in box header.");
            
            header_size += 16;
            extended_type = data.Mid (offset, 16);
         }
         else extended_type = null;
      }
      
      public BoxHeader (ByteVector type) : this (type, null)
      {}
      
      public BoxHeader (ByteVector type, ByteVector extendedType)
      {
         position = -1;
         box = null;
         from_disk = false;
         box_type = type;
         
         if (type != null && type.Count != 4)
            throw new ArgumentException ("Box type must be 4 bytes in length.", "type");
         box_size = header_size = 8;
         
         if (type != "uuid")
         {
            if (extendedType != null)
               throw new ArgumentException ("Extended type only permitted for 'uuid'.", "extendedType");
         }
         else
         {
            if (extendedType == null)
               throw new ArgumentNullException ("extendedType");
         
            if (extendedType.Count != 16)
               throw new ArgumentException ("Extended type must be 16 bytes in length.", "extendedType");
            
            box_size = header_size = 24;
         }
         
         this.extended_type = extendedType;
      }
      #endregion
      
      #region Public Methods
      public long Overwrite (TagLib.File file, long sizeChange)
      {
         if (file == null)
            throw new ArgumentNullException ("file");
         
         if (!from_disk)
            throw new InvalidOperationException ("Cannot overwrite headers created from ByteVectors.");
         
         long old_header_size = HeaderSize;
         DataSize += sizeChange;
         file.Insert (Render (), position, old_header_size);
         return sizeChange + HeaderSize - old_header_size;
      }
      
      public ByteVector Render ()
      {
         // Enlarge for size if necessary.
         if ((header_size == 8 || header_size == 24) && box_size > uint.MaxValue)
         {
            header_size += 8;
            box_size += 8;
         }
         
         // Add the box size and type to the output.
         ByteVector output = ByteVector.FromUInt ((header_size == 8 || header_size == 24) ? (uint) box_size : 1);
         output.Add (box_type);
         
         // If the box size is 16 or 32, we must have more a large header to
         // append.
         if (header_size == 16 || header_size == 32)
            output.Add (ByteVector.FromULong (box_size));
         
         // The only reason for such a big size is an extended type. Extend!!!
         if (header_size >= 24)
            output.Add (extended_type);
         
         return output;
      }
      #endregion
      
      #region Public Properties
      public ByteVector BoxType      {get {return box_type;}}
      public ByteVector ExtendedType {get {return extended_type;}}
      public long       HeaderSize   {get {return header_size;}}
      public long       DataSize     {get {return (long)(box_size - header_size);} set {box_size = (ulong)value + header_size;}}
      public long       DataOffset   {get {return header_size;}}
      public long       TotalBoxSize {get {return (long)box_size;}}
      public long       Position     {get {return from_disk ? position : -1;}}
      #endregion
      
      #region Internal Properties
      internal Box Box {get {return box;} set {box = value;}}
      #endregion
   }
}
