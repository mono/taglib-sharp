/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : mpcfile.cpp from TagLib
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

namespace TagLib.MusePack
{
   [SupportedMimeType("taglib/mpc", "mpc")]
   [SupportedMimeType("taglib/mp+", "mp+")]
   [SupportedMimeType("taglib/mpp", "mpp")]
   [SupportedMimeType("audio/x-musepack")]
   public class File : TagLib.NonContainer.File
   {
      #region Private Properties
      private ByteVector _header_block = null;
      #endregion
      
      
      
      #region Constructors
      public File (string path, ReadStyle propertiesStyle) : base (path, propertiesStyle)
      {}
      
      public File (string path) : base (path)
      {}
      
      public File (File.IFileAbstraction abstraction, ReadStyle propertiesStyle) : base (abstraction, propertiesStyle)
      {}
      
      public File (File.IFileAbstraction abstraction) : base (abstraction)
      {}
      #endregion
      
      
      
      #region Public Methods
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
      #endregion
      
      
      
      #region Protected Methods
      protected override void ReadStart (long start, ReadStyle propertiesStyle)
      {
         if (_header_block == null && propertiesStyle != ReadStyle.None)
         {
            Seek (start);
            _header_block = ReadBlock ((int) StreamHeader.Size);
         }
      }
      
      protected override void ReadEnd (long end, ReadStyle propertiesStyle)
      {
         // Make sure we have an APE tag.
         GetTag (TagTypes.Ape, true);
      }
      
      protected override TagLib.Properties ReadProperties (long start, long end, ReadStyle propertiesStyle)
      {
         StreamHeader header = new StreamHeader (_header_block, end - start);
         return new Properties (TimeSpan.Zero, header);
      }
      #endregion
   }
}