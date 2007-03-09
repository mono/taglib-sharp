/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : wvfile.cpp from libtunepimp
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

namespace TagLib.WavPack
{
   [SupportedMimeType("taglib/wv", "wv")]
   [SupportedMimeType("audio/x-wavpack")]
   public class File : TagLib.NonContainer.File
   {
      private ByteVector header_block = null;
      
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
      
      protected override void ReadStart (long start, AudioProperties.ReadStyle style)
      {
         if (header_block == null && style != AudioProperties.ReadStyle.None)
         {
            Seek (start);
            header_block = ReadBlock ((int) Properties.HeaderSize);
         }
      }
      
      protected override AudioProperties ReadProperties (long start, long end, AudioProperties.ReadStyle style)
      {
         return new Properties (header_block, end - start);
      }
   }
}
