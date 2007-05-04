using System;
using System.Text;

namespace TagLib.Riff
{
	public abstract class AviStream : ICodec
	{
      private AviStreamHeader header;
      
      public AviStream (AviStreamHeader header)
      {
         this.header = header;
      }
      
      public virtual void ParseItem (ByteVector id, ByteVector data, int start, int length)
      {
      }
      
      public AviStreamHeader Header {get {return header;}}
      
      public static AviStream ParseStreamList (ByteVector data)
      {
         AviStream stream = null;
         int pos = 4;
         
         if (data.StartsWith ("strl"))
            while (pos + 8 < data.Count)
            {
               ByteVector id = data.Mid (pos, 4);
               int block_length = (int) data.Mid (pos + 4, 4).ToUInt (false);
               
               if (id == "strh" && stream == null)
               {
                  AviStreamHeader stream_header = new AviStreamHeader (data, pos + 8);
                  if (stream_header.Type == "vids")
                     stream = new AviVideoStream (stream_header);
                  else if (stream_header.Type == "auds")
                     stream = new AviAudioStream (stream_header);
               }
               else if (stream != null)
                  stream.ParseItem (id, data, pos + 8, block_length);
               
               pos += block_length + 8;
            }
         
         return stream;
      }
      
      public abstract string Description {get;}
      public abstract MediaTypes MediaTypes {get;}
      public TimeSpan Duration {get {return TimeSpan.Zero;}}
	}
   
   public class AviAudioStream : AviStream, IAudioCodec
   {
      private WaveFormatEx wave_format_ex;
      
      public AviAudioStream (AviStreamHeader stream_header) : base (stream_header)
      {
      }
      
      public override void ParseItem (ByteVector id, ByteVector data, int start, int length)
      {
         if (id == "strf")
            wave_format_ex = new WaveFormatEx (data, start);
      }
      
      public int AudioSampleRate {get {return (int) wave_format_ex.SamplesPerSecond;}}
      public int AudioChannels {get {return wave_format_ex.Channels;}}
      public int AudioBitrate {get {return (int) Math.Round (wave_format_ex.AverageBytesPerSecond * 8d / 1000d);}}
      public override string Description {get {return wave_format_ex.Description;}}
      public override MediaTypes MediaTypes {get {return MediaTypes.Audio;}}
   }
   
   public class AviVideoStream : AviStream, IVideoCodec
   {
      BitmapInfoHeader header;
      
      public AviVideoStream (AviStreamHeader stream_header) : base (stream_header)
      {
      }
      
      public override void ParseItem (ByteVector id, ByteVector data, int start, int length)
      {
         if (id == "strf")
            header = new BitmapInfoHeader (data, start);
      }
      
      public int VideoWidth  {get {return (int) header.Width;}}
      public int VideoHeight {get {return (int) header.Height;}}
      public override string Description {get {return header.Description;}}
      public override MediaTypes MediaTypes {get {return MediaTypes.Video;}}
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
      ushort left;
      ushort top;
      ushort right;
      ushort bottom;

      public AviStreamHeader (ByteVector data) : this (data, 0) {}
      
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
         left                  = data.Mid (offset + 48, 2).ToUShort (false);
         top                   = data.Mid (offset + 50, 2).ToUShort (false);
         right                 = data.Mid (offset + 52, 2).ToUShort (false);
         bottom                = data.Mid (offset + 54, 2).ToUShort (false);
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
      public ushort Left {get {return left;}}
      public ushort Top {get {return top;}}
      public ushort Right {get {return right;}}
      public ushort Bottom {get {return bottom;}}
   }
}