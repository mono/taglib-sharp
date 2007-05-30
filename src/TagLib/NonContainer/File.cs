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

namespace TagLib.NonContainer
{
   public abstract class File : TagLib.File
   {
      #region Properties
      private TagLib.NonContainer.Tag tag;
      private Properties properties;
      #endregion
      
#region Constructors
      protected File (string path, ReadStyle propertiesStyle) : this (new File.LocalFileAbstraction (path), propertiesStyle)
      {}
      
      protected File (string path) : this (path, ReadStyle.Average)
      {}
      
      protected File (File.IFileAbstraction abstraction, ReadStyle propertiesStyle) : base (abstraction)
      {
         Mode = AccessMode.Read;
         tag = new Tag (this);
         
         // Read the tags and property data at the beginning of the file.
         long start = tag.ReadStart ();
         TagTypesOnDisk |= StartTag.TagTypes;
         ReadStart (start, propertiesStyle);
         
         // Read the tags and property data at the end of the file.
         long end = tag.ReadEnd ();
         TagTypesOnDisk |= EndTag.TagTypes;
         ReadEnd (end, propertiesStyle);
         
         // Read the audio properties.
         properties = (propertiesStyle != ReadStyle.None) ?
            ReadProperties (start, end, propertiesStyle) : null;
         
         Mode = AccessMode.Closed;
      }
      
      protected File (File.IFileAbstraction abstraction) : this (abstraction, ReadStyle.Average)
      {}
#endregion
      
      
      protected virtual void ReadStart (long start, ReadStyle propertiesStyle) {}
      protected virtual void ReadEnd   (long end,   ReadStyle propertiesStyle) {}
      protected abstract TagLib.Properties ReadProperties (long start, long end, ReadStyle propertiesStyle);
      
      public override void Save ()
      {
         long start, end;
         Mode = AccessMode.Write;
         tag.Write (out start, out end);
         Mode = AccessMode.Closed;
         TagTypesOnDisk = TagTypes;
      }
      
      public override void RemoveTags (TagTypes types)
      {
         tag.RemoveTags (types);
      }
      
      public override TagLib.Tag Tag {get {return tag;}}
      
      public override TagLib.Properties Properties {get {return properties;}}
      
      protected StartTag StartTag {get {return tag.StartTag;}}
      protected EndTag EndTag {get {return tag.EndTag;}}
   }
}
