using System;
using System.Collections.Generic;

namespace TagLib.Ogg
{
   public abstract class Codec
   {
      public delegate Codec CodecProvider (ByteVector packet);
      
      private static List<CodecProvider> providers = new List<CodecProvider> ();
      
      public static Codec GetCodec (ByteVector packet)
      {
         Codec c = null;
         
         foreach (CodecProvider p in providers)
         {
            c = p (packet);
            if (c != null) return c;
         }
         
         c = Codecs.Vorbis.FromPacket (packet);
         if (c != null) return c;
         
         c = Codecs.Theora.FromPacket (packet);
         if (c != null) return c;
         
         throw new UnsupportedFormatException ("Unknown codec.");
      }
      
      public static void AddCodecProvider (CodecProvider provider)
      {
         providers.Add (provider);
      }
      
      public virtual  int        AudioBitrate    {get {return 0;}}
      public virtual  int        AudioSampleRate {get {return 0;}}
      public virtual  int        AudioChannels   {get {return 0;}}
      public virtual  int        VideoWidth      {get {return 0;}}
      public virtual  int        VideoHeight     {get {return 0;}}
      
      public abstract MediaTypes MediaTypes      {get;}
      public abstract ByteVector CommentData     {get;}
      
      public abstract bool ReadPacket (ByteVector packet, int index);
      public abstract TimeSpan   GetDuration (long last_granular_position, long first_granular_position);
      
      public abstract void SetCommentPacket (ByteVectorList packets, XiphComment comment);
   }
}
