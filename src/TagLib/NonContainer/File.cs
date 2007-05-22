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
      public File (string file, ReadStyle properties_style) : base (file)
      {
         Mode = AccessMode.Read;
         tag = new Tag (this);
         
         // Read the tags and property data at the beginning of the file.
         long start = tag.ReadStart ();
         TagTypesOnDisk |= StartTag.TagTypes;
         ReadStart (start, properties_style);
         
         // Read the tags and property data at the end of the file.
         long end = tag.ReadEnd ();
         TagTypesOnDisk |= EndTag.TagTypes;
         ReadEnd (end, properties_style);
         
         // Read the audio properties.
         properties = (properties_style != ReadStyle.None) ?
            ReadProperties (start, end, properties_style) : null;
         
         Mode = AccessMode.Closed;
      }
      
      public File (string file) : this (file, ReadStyle.Average)
      {
      }
#endregion
      
      public override TagTypes TagTypes
      {
         get {return tag.TagTypes;}
      }
      
      
      protected virtual void ReadStart (long start, ReadStyle style) {}
      protected virtual void ReadEnd   (long end,   ReadStyle style) {}
      protected abstract TagLib.Properties ReadProperties (long start, long end, ReadStyle style);
      
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
