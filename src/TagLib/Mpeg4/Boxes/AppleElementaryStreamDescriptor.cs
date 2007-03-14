namespace TagLib.Mpeg4
{
   public class AppleElementaryStreamDescriptor : FullBox
   {
      #region Private Properties
      private short es_id;
      private byte stream_priority;
      private byte object_type_id;
      private byte stream_type;
      private uint buffer_size_db;
      private uint max_bitrate;
      private uint average_bitrate;
      private ByteVector decoder_config;
      #endregion
      
      #region Constructors
      public AppleElementaryStreamDescriptor (BoxHeader header, File file, Box handler) : base (header, file, handler)
      {
         int offset = 0;
         ByteVector box_data = LoadData (file);
         decoder_config = new ByteVector ();
         
         // Elementary Stream Descriptor Tag
         if (box_data [offset ++] == 3)
         {
            // We have a descriptor tag. Check that it's at least 20 long.
            if (ReadLength (box_data, offset) < 20)
               throw new CorruptFileException ("Could not read data. Too small.");
            
            offset += 4;
            es_id = box_data.Mid (offset, 2).ToShort ();
            offset += 2;
            stream_priority = box_data [offset ++];
         }
         else
         {
            // The tag wasn't found, so the next two byte are the ID, and
            // after that, business as usual.
            es_id = box_data.Mid (offset, 2).ToShort ();
            offset += 2;
         }
         
         // Verify that the next data is the Decoder Configuration Descriptor
         // Tag and escape if it won't work out.
         if (box_data [offset ++] != 4)
            throw new CorruptFileException ("Could not identify decoder configuration descriptor.");

         // Check that it's at least 15 long.
         if (ReadLength (box_data, offset) < 15)
            throw new CorruptFileException ("Could not read data. Too small.");
         offset += 4;
         
         // Read a lot of good info.
         object_type_id  = box_data [offset ++];
         stream_type     = box_data [offset ++];
         buffer_size_db  = box_data.Mid (offset, 3).ToUInt ();
         offset += 3;
         max_bitrate     = box_data.Mid (offset, 4).ToUInt ();
         offset += 4;
         average_bitrate = box_data.Mid (offset, 4).ToUInt ();
         offset += 4;
         
         // Verify that the next data is the Decoder Specific Descriptor
         // Tag and escape if it won't work out.
         if (box_data [offset ++] != 5)
            throw new CorruptFileException ("Could not identify decoder specific descriptor.");
         
         // The rest of the info is decoder specific.
         uint length = ReadLength (box_data, offset); 
         offset += 4;
         decoder_config = box_data.Mid (offset, (int) length);
      }
      #endregion
      
      #region Public Properties
      public short      StreamId       {get {return es_id;}}
      public byte       StreamPriority {get {return stream_priority;}}
      public byte       ObjectTypeId   {get {return object_type_id;}}
      public byte       StreamType     {get {return stream_type;}}
      public uint       BufferSizeDb   {get {return buffer_size_db;}}
      public uint       MaximumBitrate {get {return max_bitrate / 1000;}}
      public uint       AverageBitrate {get {return average_bitrate / 1000;}}
      public ByteVector DecoderConfig  {get {return decoder_config;}}
      #endregion
      
      #region Private Methods
      private uint ReadLength (ByteVector data, int offset)
      {
         byte b;
         int  end    = offset + 4;
         uint length = 0;
         
         do
         {
            b = data [offset ++];
            length = (uint) (length << 7) | (uint) (b & 0x7f);
         } while ((b & 0x80) != 0 && offset <= end);
         
         return length;
      }
      #endregion
   }
}
