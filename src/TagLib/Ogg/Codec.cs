//
// Codec.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
//
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

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
