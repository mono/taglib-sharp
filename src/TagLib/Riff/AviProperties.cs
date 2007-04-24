using System;
using System.Collections.Generic;

namespace TagLib.Riff
{
   public class AviProperties : Properties
   {
      private AviHeader avi_header;
      private List<AviStreamHeader> stream_headers = new List<AviStreamHeader> ();
      
      public AviProperties (ByteVector data, ReadStyle style) : base (style)
      {
         Parse (data);
      }
      
      public AviProperties (TagLib.File file, long position, int length, ReadStyle style) : base (style)
      {
         file.Seek (position);
         Parse (file.ReadBlock (length));
      }
      
      public override int AudioSampleRate
      {
         get
         {
            foreach (AviStreamHeader header in stream_headers)
            {
               if (header.Type == "auds")
                  return (int) header.Rate / (int) header.Scale;
            }
            
            return 0;
         }
      }
      
      public override MediaTypes MediaTypes
      {
         get
         {
            MediaTypes types = MediaTypes.Unknown;
            
            foreach (AviStreamHeader header in stream_headers)
            {
               if (header.Type == "vids")
                  types |= MediaTypes.Video;
               else if (header.Type == "auds")
                  types |= MediaTypes.Audio;
            }
            
            return types;
         }
      }
      public override int        VideoWidth      {get {return (int) avi_header.Width;}}
      public override int        VideoHeight     {get {return (int) avi_header.Height;}}
      public override TimeSpan Duration {
         get
         {
            return TimeSpan.FromMilliseconds ((double) avi_header.TotalFrames
                                            * (double) avi_header.MicrosecondsPerFrame
                                            / 1000.0);
         }
      }

      
      private void Parse (ByteVector data)
      {
         if (!data.StartsWith ("avih"))
            throw new CorruptFileException ("Avi header expected.");
         
         if (data.Mid (4, 4).ToUInt (false) != 0x38)
            throw new CorruptFileException ("Invalid header length.");
         
         avi_header = new AviHeader (data, 8);
         
         int offset = 0x40;
         while (offset + 12 < data.Count)
            ParseStreamList (data, ref offset);
      }
      
      private void ParseStreamList (ByteVector data, ref int offset)
      {
         int length = (int) data.Mid (offset + 4, 4).ToUInt (false);
         int pos = offset + 12;
         
         if (data.ContainsAt ("LIST", offset) && data.ContainsAt ("strl", offset + 8))
            while (pos < offset + length)
            {
               ByteVector id = data.Mid (pos, 4);
               int block_length = (int) data.Mid (pos + 4, 4).ToUInt (false);
               
               if (id == "strh")
                  stream_headers.Add (new AviStreamHeader (data, pos + 8));
               pos += block_length + 8;
            }
         
         offset += length + 8;
      }
   }
   
   public struct AviHeader
   {
      uint microseconds_per_frame;
      uint max_bytes_per_second;
      uint flags;
      uint total_frames;
      uint initial_frames;
      uint streams;
      uint suggested_buffer_size;
      uint width;
      uint height;
      
      public AviHeader (ByteVector data, int offset)
      {
         microseconds_per_frame = data.Mid (offset,      4).ToUInt (false);
         max_bytes_per_second   = data.Mid (offset +  4, 4).ToUInt (false);
         flags                  = data.Mid (offset + 12, 4).ToUInt (false);
         total_frames           = data.Mid (offset + 16, 4).ToUInt (false);
         initial_frames         = data.Mid (offset + 20, 4).ToUInt (false);
         streams                = data.Mid (offset + 24, 4).ToUInt (false);
         suggested_buffer_size  = data.Mid (offset + 28, 4).ToUInt (false);
         width                  = data.Mid (offset + 32, 4).ToUInt (false);
         height                 = data.Mid (offset + 36, 4).ToUInt (false);
      }
      
      public uint MicrosecondsPerFrame {get {return microseconds_per_frame;}}
      public uint MaxBytesPerSecond {get {return max_bytes_per_second;}}
      public uint Flags {get {return flags;}}
      public uint TotalFrames {get {return total_frames;}}
      public uint InitialFrames {get {return initial_frames;}}
      public uint Streams {get {return streams;}}
      public uint SuggestedBufferSize {get {return suggested_buffer_size;}}
      public uint Width {get {return width;}}
      public uint Height {get {return height;}}
   }
   
   public struct AviStreamHeader
   {
      ByteVector type;
      ByteVector handler;
      uint flags;
      uint priority;
      uint initial_frames;
      uint scale;
      uint rate;
      uint start;
      uint length;
      uint suggested_buffer_size;
      uint quality;
      uint sample_size;

      public AviStreamHeader (ByteVector data, int offset)
      {
         type                  = data.Mid (offset,      4);
         handler               = data.Mid (offset +  4, 4);
         flags                 = data.Mid (offset +  8, 4).ToUInt (false);
         priority              = data.Mid (offset + 12, 4).ToUInt (false);
         initial_frames        = data.Mid (offset + 16, 4).ToUInt (false);
         scale                 = data.Mid (offset + 20, 4).ToUInt (false);
         rate                  = data.Mid (offset + 24, 4).ToUInt (false);
         start                 = data.Mid (offset + 28, 4).ToUInt (false);
         length                = data.Mid (offset + 32, 4).ToUInt (false);
         suggested_buffer_size = data.Mid (offset + 36, 4).ToUInt (false);
         quality               = data.Mid (offset + 40, 4).ToUInt (false);
         sample_size           = data.Mid (offset + 44, 4).ToUInt (false);
      }
      
      public ByteVector Type {get {return type;}}
      public ByteVector Handler {get {return handler;}}
      public uint Flags {get {return flags;}}
      public uint Priority {get {return priority;}}
      public uint InitialFrames {get {return initial_frames;}}
      public uint Scale {get {return scale;}}
      public uint Rate {get {return rate;}}
      public uint Start {get {return start;}}
      public uint Length {get {return length;}}
      public uint SuggestedBufferSize {get {return suggested_buffer_size;}}
      public uint Quality {get {return quality;}}
      public uint SampleSize {get {return sample_size;}}
   }
}