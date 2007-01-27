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

using System.Collections;
using System;

namespace TagLib.Asf
{
   [SupportedMimeType("taglib/wma", "wma")]
   [SupportedMimeType("taglib/asf", "asf")]
   [SupportedMimeType("audio/x-ms-wma")]
   [SupportedMimeType("video/x-ms-asf")]
   public class File : TagLib.File
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Asf.Tag      asf_tag;
      private Properties   properties;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public File (string file, Properties.ReadStyle properties_style) : base (file)
      {
         asf_tag    = null;
         properties = null;
         
         Mode = AccessMode.Read;
         Read (properties_style);
         Mode = AccessMode.Closed;
      }

      public File (string file) : this (file, Properties.ReadStyle.Average)
      {
      }
      
      public override void Save ()
      {
         Mode = AccessMode.Write;
         
         HeaderObject header = new HeaderObject (this, 0);
         header.AddUniqueObject (asf_tag.ContentDescriptionObject);
         header.AddUniqueObject (asf_tag.ExtendedContentDescriptionObject);
         
         Insert (header.Render (), 0, header.OriginalSize);
         
         Mode = AccessMode.Closed;
      }
      
      public override TagLib.Tag GetTag (TagTypes type, bool create)
      {
         if (type == TagTypes.Asf)
            return asf_tag;
         
         return null;
      }
      
      public short ReadWord ()
      {
         ByteVector v = ReadBlock (2);
         return v.ToShort (false);
      }
      
      public uint ReadDWord ()
      {
         ByteVector v = ReadBlock (4);
         return v.ToUInt (false);
      }
      
      public long ReadQWord ()
      {
         ByteVector v = ReadBlock (8);
         return v.ToLong (false);
      }
      
      public Guid ReadGuid ()
      {
         ByteVector v = ReadBlock (16);
         return new Guid (v);
      }

      public string ReadUnicode (int length)
      {
         ByteVector data = ReadBlock (length);
         string output = data.ToString (StringType.UTF16LE);
         int i = output.IndexOf ('\0');
         return (i >= 0) ? output.Substring (0, i) : output;
      }
      
      public Object [] ReadObjects (uint count, long position)
      {
         ArrayList l = new ArrayList ();
         for (int i = 0; i < (int) count; i ++)
         {
            Seek (position);
            Guid id = ReadGuid ();
            
            Object obj;
            
            if (id.Equals (Guid.AsfFilePropertiesObject))
               obj = new FilePropertiesObject (this, position);
            else if (id.Equals (Guid.AsfStreamPropertiesObject))
               obj = new StreamPropertiesObject (this, position);
            else if (id.Equals (Guid.AsfContentDescriptionObject))
               obj = new ContentDescriptionObject (this, position);
            else if (id.Equals (Guid.AsfExtendedContentDescriptionObject))
               obj = new ExtendedContentDescriptionObject (this, position);
            else if (id.Equals (Guid.AsfPaddingObject))
               obj = new PaddingObject (this, position);
            else
               obj = new UnknownObject (this, position);
            
            l.Add (obj);
            position += obj.OriginalSize;
         }
         
         return (Object []) l.ToArray (typeof (Object));
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TagLib.Tag Tag {get {return asf_tag;}}
      
      public override TagLib.AudioProperties AudioProperties {get {return properties;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Read (Properties.ReadStyle properties_style)
      {
         HeaderObject header = new HeaderObject (this, 0);
         
         asf_tag = new Asf.Tag (header);
         
         if(properties_style != Properties.ReadStyle.None)
            properties = new Properties (header, properties_style);
      }
   }
}
