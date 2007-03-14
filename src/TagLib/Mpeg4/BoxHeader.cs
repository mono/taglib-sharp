using System;

namespace TagLib.Mpeg4
{
   public class BoxHeader
   {
      #region Private Properties
      private ByteVector box_type;
      private ByteVector extended_type;
      private long       box_size;
      private uint       header_size;
      private long       position;
      #endregion
      
      #region Constructors
      private BoxHeader ()
      {
         box_type      = null;
         extended_type = null;
         box_size      = 0;
         header_size   = 0;
         position      = -1;
      }
      
      public BoxHeader (TagLib.File file, long position) : this ()
      {
         this.position = position;
         file.Seek (position);
         Parse (file.ReadBlock (32), 0);
      }
      
      public BoxHeader (ByteVector data, int offset) : this ()
      {
         Parse (data, offset);
      }
      
      public BoxHeader (ByteVector type) : this (type, null)
      {}
      
      public BoxHeader (ByteVector type, ByteVector extended_type) : this ()
      {
         box_type = type;
         if (type.Count != 4)
            throw new ArgumentException ("Box type must be 4 bytes in length.", "type");
         box_size = header_size = 8;
         
         if (type != "uuid")
         {
            if (extended_type != null)
               throw new ArgumentException ("Extended type only permitted for 'uuid'.", "extended_type");
         }
         else
         {
            if (extended_type.Count != 16)
               throw new ArgumentException ("Extended type must be 16 bytes in length.", "extended_type");
            
            this.extended_type = extended_type;
            box_size = header_size = 24;
         }
      }
      #endregion
      
      #region Public Methods
      public long Overwrite (File file, long size_change)
      {
         if (position < 0)
            throw new Exception ("Cannot overwrite headers created from ByteVectors.");
         
         long old_header_size = HeaderSize;
         DataSize += size_change;
         file.Insert (Render (), position, old_header_size);
         return size_change + HeaderSize - old_header_size;
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
            output.Add (ByteVector.FromLong (box_size));
         
         // The only reason for such a big size is an extended type. Extend!!!
         if (header_size >= 24)
            output.Add (extended_type);
         
         return output;
      }
      #endregion
      
      #region Private Methods
      private void Parse (ByteVector data, int offset)
      {
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
            box_size = data.Mid (offset, 8).ToLong ();
            offset += 8;
         }
         
         // UUID has a special header with 16 extra bytes.
         if (box_type == BoxTypes.Uuid)
         {
            if (data.Count < 16 + offset)
               throw new CorruptFileException ("Not enough data in box header.");
            
            header_size += 16;
            extended_type = data.Mid (offset, 16);
         }
      }
      #endregion
      
      #region Public Properties
      public ByteVector BoxType      {get {return box_type;}}
      public ByteVector ExtendedType {get {return extended_type;}}
      public long       HeaderSize   {get {return header_size;}}
      public long       DataSize     {get {return box_size - header_size;} set {box_size = value + header_size;}}
      public long       DataOffset   {get {return header_size;}}
      public long       TotalBoxSize {get {return box_size;}}
      public long       Position     {get {return position;}}
      #endregion
   }
}
