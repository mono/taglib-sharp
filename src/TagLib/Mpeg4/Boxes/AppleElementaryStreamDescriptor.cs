namespace TagLib.Mpeg4
{
   public class AppleElementaryStreamDescriptor : FullBox
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private short es_id;
      private byte stream_priority;
      private byte object_type_id;
      private byte stream_type;
      private uint buffer_size_db;
      private uint max_bitrate;
      private uint average_bitrate;
      private ByteVector decoder_config;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public AppleElementaryStreamDescriptor (BoxHeader header, Box parent) : base (header, parent)
      {
         decoder_config = new ByteVector ();
         
         uint length;
         
         // This box contains a ton of information.
         int offset = 0;
         
         // Elementary Stream Descriptor Tag
         if (Data [offset ++] == 3)
         {
            // We have a descriptor tag. Check that it's at least 20 long.
            if ((length = ReadLength (offset)) < 20)
            {
               Debugger.Debug ("TagLib.Mpeg4.AppleElementaryStreamDescriptor () - Could not read data. Too small.");
               return;
            }
            offset += 4;
            
            es_id = Data.Mid (offset, 2).ToShort ();
            offset += 2;
            
            stream_priority = Data [offset ++];
         }
         else
         {
            // The tag wasn't found, so the next two byte are the ID, and
            // after that, business as usual.
            es_id = Data.Mid (offset, 2).ToShort ();
            offset += 2;
         }
         
         // Verify that the next data is the Decoder Configuration Descriptor
         // Tag and escape if it won't work out.
         if (Data [offset ++] != 4)
         {
            Debugger.Debug ("TagLib.Mpeg4.AppleElementaryStreamDescriptor () - Could not identify decoder configuration descriptor.");
            return;
         }

         // Check that it's at least 15 long.
         if ((length = ReadLength (offset)) < 15)
         {
            Debugger.Debug ("TagLib.Mpeg4.AppleElementaryStreamDescriptor () - Could not read data. Too small.");
            return;
         }
         offset += 4;
         
         // Read a lot of good info.
         object_type_id  = Data [offset ++];
         stream_type     = Data [offset ++];
         buffer_size_db  = Data.Mid (offset, 3).ToUInt ();
         offset += 3;
         max_bitrate     = Data.Mid (offset, 4).ToUInt ();
         offset += 4;
         average_bitrate = Data.Mid (offset, 4).ToUInt ();
         offset += 4;
         
         // Verify that the next data is the Decoder Specific Descriptor
         // Tag and escape if it won't work out.
         if (Data [offset ++] != 5)
         {
            Debugger.Debug ("TagLib.Mpeg4.AppleElementaryStreamDescriptor () - Could not identify decoder specific descriptor.");
            return;
         }
         
         // The rest of the info is decoder specific.
         length = ReadLength (offset); 
         offset += 4;
         decoder_config = Data.Mid (offset, (int) length);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public short      StreamId       {get {return es_id;}}
      public byte       StreamPriority {get {return stream_priority;}}
      public byte       ObjectTypeId   {get {return object_type_id;}}
      public byte       StreamType     {get {return stream_type;}}
      public uint       BufferSizeDb   {get {return buffer_size_db;}}
      public uint       MaximumBitrate {get {return max_bitrate / 1000;}}
      public uint       AverageBitrate {get {return average_bitrate / 1000;}}
      public ByteVector DecoderConfig  {get {return decoder_config;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      
      // The Stream Descriptor has a very special way of storing length. This
      // function reads the length from a ByteVector and returns it.
      private uint ReadLength (int offset)
      {
         byte b;
         int  end    = offset + 4;
         uint length = 0;
         
         do
         {
            b = Data [offset ++];
            length = (uint) (length << 7) | (uint) (b & 0x7f);
         } while ((b & 0x80) != 0 && offset <= end);
         
         return length;
      }
   }
}
