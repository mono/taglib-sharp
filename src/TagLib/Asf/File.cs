/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : 
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

using System.Collections.Generic;
using System;

namespace TagLib.Asf
{
   [SupportedMimeType("taglib/wma", "wma")]
   [SupportedMimeType("taglib/wmv", "wmv")]
   [SupportedMimeType("taglib/asf", "asf")]
   [SupportedMimeType("audio/x-ms-wma")]
   [SupportedMimeType("video/x-ms-asf")]
   public class File : TagLib.File
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Asf.Tag      asf_tag = null;
      private Properties   properties = null;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public File (string path, ReadStyle propertiesStyle) : this (new File.LocalFileAbstraction (path), propertiesStyle)
      {}
      
      public File (string path) : this (path, ReadStyle.Average)
      {}
      
      public File (File.IFileAbstraction abstraction, ReadStyle propertiesStyle) : base (abstraction)
      {
         Mode = AccessMode.Read;
         Read (propertiesStyle);
         Mode = AccessMode.Closed;
      }

      public File (File.IFileAbstraction abstraction) : this (abstraction, ReadStyle.Average)
      {}
      
      public override void Save ()
      {
         Mode = AccessMode.Write;
         
         HeaderObject header = new HeaderObject (this, 0);
         
         if (asf_tag == null)
         {
            header.RemoveContentDescriptors ();
            TagTypesOnDisk &= ~ TagTypes.Asf;
         }
         else
         {
            TagTypesOnDisk |= TagTypes.Asf;
            header.AddUniqueObject (asf_tag.ContentDescriptionObject);
            header.AddUniqueObject (asf_tag.ExtendedContentDescriptionObject);
         }
         
         Insert (header.Render (), 0, (long) header.OriginalSize);
         
         Mode = AccessMode.Closed;
      }
      
      public override TagLib.Tag GetTag (TagTypes type, bool create)
      {
         if (type == TagTypes.Asf)
         {
            if (asf_tag == null)
               asf_tag = new Tag ();
            
            return asf_tag;
         }
         
         return null;
      }
      
      public override void RemoveTags (TagTypes types)
      {
         if ((types & TagTypes.Asf) == TagTypes.Asf)
            asf_tag = null;
      }
      
      public ushort ReadWord ()
      {
         return ReadBlock (2).ToUShort (false);
      }
      
      public uint ReadDWord ()
      {
         return ReadBlock (4).ToUInt (false);
      }
      
      public ulong ReadQWord ()
      {
         return ReadBlock (8).ToULong (false);
      }
      
      public System.Guid ReadGuid ()
      {
         return new System.Guid (ReadBlock (16).Data);
      }

      public string ReadUnicode (int length)
      {
         ByteVector data = ReadBlock (length);
         string output = data.ToString (StringType.UTF16LE);
         int i = output.IndexOf ('\0');
         return (i >= 0) ? output.Substring (0, i) : output;
      }
      
      public IEnumerable<Object> ReadObjects (uint count, long position)
      {
         for (int i = 0; i < (int) count; i ++)
         {
            Object obj = ReadObject (position);
            position += (long) obj.OriginalSize;
            yield return obj;
         }
      }
      
      public Object ReadObject (long position)
      {
         Seek (position);
         System.Guid id = ReadGuid ();
         
         if (id.Equals (Guid.AsfFilePropertiesObject))
            return new FilePropertiesObject (this, position);
         
         if (id.Equals (Guid.AsfStreamPropertiesObject))
            return new StreamPropertiesObject (this, position);
         
         if (id.Equals (Guid.AsfContentDescriptionObject))
            return new ContentDescriptionObject (this, position);
         
         else if (id.Equals (Guid.AsfExtendedContentDescriptionObject))
            return new ExtendedContentDescriptionObject (this, position);
         
         else if (id.Equals (Guid.AsfPaddingObject))
            return new PaddingObject (this, position);
         
         else if (id.Equals (Guid.AsfHeaderExtensionObject))
            return new HeaderExtensionObject (this, position);
         
         return new UnknownObject (this, position);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TagLib.Tag Tag {get {return asf_tag;}}
      
      public override TagLib.Properties Properties {get {return properties;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Read (ReadStyle properties_style)
      {
         HeaderObject header = new HeaderObject (this, 0);
         
         if (header.HasContentDescriptors)
            TagTypesOnDisk |= TagTypes.Asf;
         
         asf_tag = new Asf.Tag (header);
         
         if(properties_style != ReadStyle.None)
            properties = header.Properties;
      }
   }
}
