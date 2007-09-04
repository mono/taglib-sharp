//
// FrameFactory.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   id3v2framefactory.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
// Copyright (C) 2002,2003 Scott Wheeler (Original Implementation)
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

using System.Collections.Generic;
 
namespace TagLib.Id3v2
{
   public static class FrameFactory
   {
      public delegate Frame FrameCreator (ByteVector data, int offset, FrameHeader header, byte version);

      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private static List<FrameCreator>  frame_creators       = new List<FrameCreator> ();
      
      //////////////////////////////////////////////////////////////////////////
      // public members
      //////////////////////////////////////////////////////////////////////////
      public static Frame CreateFrame (ByteVector data, ref int offset, byte version)
      {
         int position = offset;
         
         FrameHeader header = new FrameHeader (data.Mid (position, (int) FrameHeader.Size (version)), version);
         
         offset += (int) (header.FrameSize + FrameHeader.Size (version));
         
         if (header.FrameId == null)
            throw new System.NotImplementedException ();
         
         foreach (byte b in header.FrameId)
         {
            char c = (char) b;
            if ((c < 'A' || c > 'Z') && (c < '1' || c > '9'))
               return null;
         }
         
         // Windows Media Player may create zero byte frames. Just send them
         // off as unknown and delete them.
         if (header.FrameSize == 0)
         {
            header.Flags |= FrameFlags.TagAlterPreservation;
            return new UnknownFrame (data, position, header, version);
         }
         
         // TODO: Support Compression.
         if ((header.Flags & FrameFlags.Compression) != 0)
            throw new System.NotImplementedException ();
         
         // TODO: Support Encryption.
         if ((header.Flags & FrameFlags.Encryption) != 0)
            throw new System.NotImplementedException ();
         
         foreach (FrameCreator creator in frame_creators)
         {
            Frame frame = creator (data, position, header, version);
            if (frame != null)
               return frame;
         }
         
         // This is where things get necissarily nasty.  Here we determine which
         // Frame subclass (or if none is found simply an Frame) based
         // on the frame ID.  Since there are a lot of possibilities, that means
         // a lot of if blocks.
         
         // Text Identification (frames 4.2)
         if (header.FrameId == FrameType.TXXX)
            return new UserTextInformationFrame (data, position, header, version);
      	 
         if (header.FrameId [0] == (byte) 'T')
            return new TextInformationFrame (data, position, header, version);
      	 
         // Unique File Identifier (frames 4.1)
         if (header.FrameId == FrameType.UFID)
            return new UniqueFileIdentifierFrame (data, position, header, version);
         
         // Music CD Identifier (frames 4.5)
         if (header.FrameId == FrameType.MCDI)
            return new MusicCdIdentifierFrame (data, position, header, version);

         // Unsynchronized Lyrics (frames 4.8)
         if (header.FrameId == FrameType.USLT)
            return new UnsynchronisedLyricsFrame (data, position, header, version);

         // Synchronized Lyrics (frames 4.9)
         if (header.FrameId == FrameType.SYLT)
            return new SynchronisedLyricsFrame (data, position, header, version);

         // Comments (frames 4.10)
         if (header.FrameId == FrameType.COMM)
            return new CommentsFrame (data, position, header, version);

         // Relative Volume Adjustment (frames 4.11)
         if (header.FrameId == FrameType.RVA2)
            return new RelativeVolumeFrame (data, position, header, version);

         // Attached Picture (frames 4.14)
         if (header.FrameId == FrameType.APIC)
            return new AttachedPictureFrame (data, position, header, version);

         // General Encapsulated Object (frames 4.15)
         if(header.FrameId == FrameType.GEOB)
            return new GeneralEncapsulatedObjectFrame (data, position, header, version);
         
         // Play Count (frames 4.16)
         if(header.FrameId == FrameType.PCNT)
            return new PlayCountFrame (data, position, header, version);
         
         // Play Count (frames 4.17)
         if(header.FrameId == FrameType.POPM)
            return new PopularimeterFrame (data, position, header, version);
         
         // Terms of Use (frames 4.22)
         if(header.FrameId == FrameType.USER)
            return new TermsOfUseFrame (data, position, header, version);
         
         // Private (frames 4.27)
         if (header.FrameId == FrameType.PRIV)
            return new PrivateFrame (data, position, header, version);
         
         return new UnknownFrame (data, position, header, version);
      }

      public static void AddFrameCreator (FrameCreator creator)
      {
         if (creator != null)
            frame_creators.Insert (0, creator);
      }
      
   }
}
