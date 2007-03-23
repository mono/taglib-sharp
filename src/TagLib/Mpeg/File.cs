/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : mpegfile.cpp from TagLib
 ***************************************************************************/

/***************************************************************************
 *   This library is free software; you can redistribute it and/or modify  *
 *   it  under the terms of the GNU Lesser General Public License version  *
 *   2.1 as published by the Free Software Foundation.                     *
 *                                                                         *
 *   This library is distributed in the hope that it will be useful, but   *
 *   WITHOUT ANY WARRANTY; without even the implied warranty of            *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU     *
 *   Lesser General Public License for more details.                       *
 *                                                                         *
 *   You should have received a copy of the GNU Lesser General Public      *
 *   License along with this library; if not, write to the Free Software   *
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  *
 *   USA                                                                   *
 ***************************************************************************/

using System.Collections;
using System;

namespace TagLib.Mpeg
{
   [SupportedMimeType("taglib/mp3", "mp3")]
   [SupportedMimeType("audio/x-mp3")]
   [SupportedMimeType("application/x-id3")]
   [SupportedMimeType("audio/mpeg")]
   [SupportedMimeType("audio/x-mpeg")]
   [SupportedMimeType("audio/x-mpeg-3")]
   [SupportedMimeType("audio/mpeg3")]
   [SupportedMimeType("audio/mp3")]
   public class File : TagLib.NonContainer.File
   {
      private Header first_header = null;
      
      public File (string file, AudioProperties.ReadStyle properties_style) : base (file, properties_style)
      {}
      
      public File (string file) : this (file, Properties.ReadStyle.Average)
      {}
      
      public override TagLib.Tag GetTag (TagTypes type, bool create)
      {
         Tag t = (Tag as TagLib.NonContainer.Tag).GetTag (type);
         
         if (t != null || !create)
            return t;
         
         switch (type)
         {
         case TagTypes.Id3v1:
            return EndTag.AddTag (type, Tag);
         
         case TagTypes.Id3v2:
            return StartTag.AddTag (type, Tag);
         
         case TagTypes.Ape:
            return EndTag.AddTag (type, Tag);
         
         default:
            return null;
         }
      }

      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      private Header FindFirstFrameHeader (long start)
      {
         long position = start;
         Seek (position);
         ByteVector buffer = ReadBlock (3);
         
         if (buffer.Count < 3)
            return null;
         
         do
         {
            Seek (position + 3);
            buffer = buffer.Mid (buffer.Count - 3);
            buffer.Add (ReadBlock ((int) BufferSize));
            
            for (int i = 0; i < buffer.Count - 3; i++)
               if (buffer [i] == 0xFF && SecondSynchByte (buffer [i + 1]))
                  try {return new Header (buffer.Mid (i, 4), position + i);} catch {}
            
            position += BufferSize;
         }
         while (buffer.Count > 3);
        
         return null;
      }
      
      protected override void ReadStart (long start, AudioProperties.ReadStyle style)
      {
         if (first_header == null && style != AudioProperties.ReadStyle.None)
            first_header = FindFirstFrameHeader (start);
      }
      
      protected override void ReadEnd (long end, AudioProperties.ReadStyle style)
      {
         // Make sure we have ID3v1 and ID3v2 tags.
         GetTag (TagTypes.Id3v1, true);
         GetTag (TagTypes.Id3v2, true);
      }
      
      protected override AudioProperties ReadProperties (long start, long end, AudioProperties.ReadStyle style)
      {
         return new Properties (this, first_header, style);
      }
      
      private bool SecondSynchByte (byte b)
      {
         return b >= 0xE0;
      }
   }
}
