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
   public class AudioFile : TagLib.NonContainer.File
   {
      private AudioHeader first_header;
      
      #region Constructors
      public AudioFile (string path, ReadStyle propertiesStyle) : base (path, propertiesStyle)
      {}
      
      public AudioFile (string path) : base (path)
      {}
      
      public AudioFile (File.IFileAbstraction abstraction, ReadStyle propertiesStyle) : base (abstraction, propertiesStyle)
      {}
      
      public AudioFile (File.IFileAbstraction abstraction) : base (abstraction)
      {}
      #endregion
      
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

      protected override void ReadStart (long start, ReadStyle propertiesStyle)
      {
         if (propertiesStyle != ReadStyle.None && !AudioHeader.Find (out first_header, this, start))
            throw new CorruptFileException ("MPEG audio header not found.");
      }
      
      protected override void ReadEnd (long end, ReadStyle propertiesStyle)
      {
         // Make sure we have ID3v1 and ID3v2 tags.
         GetTag (TagTypes.Id3v1, true);
         GetTag (TagTypes.Id3v2, true);
      }
      
      protected override TagLib.Properties ReadProperties (long start, long end, ReadStyle propertiesStyle)
      {
         first_header.SetStreamLength (end - start);
         return new Properties (TimeSpan.Zero, first_header);
      }
   }
}