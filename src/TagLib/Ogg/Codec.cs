using System;
using System.Collections.Generic;

namespace TagLib.Ogg
{
   public abstract class Codec : ICodec
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
      
      public abstract string     Description     {get;}
      public abstract MediaTypes MediaTypes      {get;}
      public abstract ByteVector CommentData     {get;}
      public          TimeSpan   Duration        {get {return TimeSpan.Zero;}}
      
      public abstract bool ReadPacket (ByteVector packet, int index);
      public abstract TimeSpan   GetDuration (long firstGranularPosition, long lastGranularPosition);
      
      public abstract void SetCommentPacket (ByteVectorCollection packets, XiphComment comment);
   }
}
