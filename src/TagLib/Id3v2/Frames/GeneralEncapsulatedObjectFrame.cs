/***************************************************************************
    copyright            : (C) 2007 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : generalencapsulatedobjectframe.cpp from TagLib
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

namespace TagLib.Id3v2
{
   public class GeneralEncapsulatedObjectFrame : Frame
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      StringType text_encoding;
      string     mime_type;
      string     file_name;
      string     description;
      ByteVector data;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public GeneralEncapsulatedObjectFrame () : base ("GEOB")
      {
         text_encoding = StringType.UTF8;
         mime_type   = null;
         file_name   = null;
         description = null;
         data        = null;
      }
      
      public GeneralEncapsulatedObjectFrame (ByteVector data) : base (data)
      {
         text_encoding = StringType.UTF8;
         mime_type   = null;
         file_name   = null;
         description = null;
         this.data   = null;
         SetData (data, 0);
      }
      
      public override string ToString ()
      {
         string text = "[" + mime_type + "]";
         
         if(file_name != null && file_name != string.Empty)
            text += " " + file_name;
         
         if(description != null && description != string.Empty)
            text += " " + description;
         
         return text;
      }
      
      public static GeneralEncapsulatedObjectFrame Find (Tag tag, string description)
      {
         foreach (GeneralEncapsulatedObjectFrame f in tag.GetFrames ("GEOB"))
            if (f != null && f.Description == description)
               return f;
         return null;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public StringType TextEncoding
      {
         get {return text_encoding;}
         set {text_encoding = value;}
      }
      
      public string MimeType
      {
         get {return mime_type;}
         set {mime_type = value;}
      }
      
      public string FileName
      {
         get {return file_name;}
         set {file_name = value;}
      }
      
      public string Description
      {
         get {return description;}
         set {description = value;}
      }
      
      public ByteVector Object
      {
         get {return data;}
         set {data = value;}
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected override void ParseFields (ByteVector data)
      {
         if (data.Count < 4)
         {
            Debugger.Debug ("An object frame must contain at least 4 bytes.");
            return;
         }
         
         int field_start = 0;
         
         text_encoding =  (StringType) data [field_start++];
         int byte_align = text_encoding == StringType.Latin1 || text_encoding == StringType.UTF8 ? 1 : 2;
         
         int field_end = data.Find (TextDelimiter (StringType.Latin1), field_start);
         
         if (field_end < field_start)
            return;
         
         mime_type = data.Mid (field_start, field_end - field_start).ToString (StringType.Latin1);
         field_start = field_end + 1;
         field_end = data.Find (TextDelimiter (text_encoding), field_start, byte_align);
         
         if (field_end < field_start)
            return;
         
         file_name = data.Mid (field_start, field_end - field_start).ToString (text_encoding);
         field_start = field_end + 1;
         field_end = data.Find (TextDelimiter (text_encoding), field_start, byte_align);
         
         if (field_end < field_start)
            return;
         
         description = data.Mid (field_start, field_end - field_start).ToString (text_encoding);
         field_start = field_end + 1;
         
         this.data = data.Mid (field_start);
      }

      protected override ByteVector RenderFields ()
      {
         ByteVector v = new ByteVector ();
         
         v.Add ((byte) text_encoding);
         
         if (MimeType != null)
            v.Add (ByteVector.FromString (MimeType, StringType.Latin1));
         v.Add (TextDelimiter (StringType.Latin1));
         
         if (FileName != null)
            v.Add (ByteVector.FromString (FileName, text_encoding));
         v.Add (TextDelimiter (text_encoding));
         
         if (Description != null)
            v.Add (ByteVector.FromString (Description, text_encoding));
         v.Add (TextDelimiter (text_encoding));
         
         v.Add (data);
         
         return v;
      }

      protected internal GeneralEncapsulatedObjectFrame (ByteVector data, int offset, FrameHeader h) : base (h)
      {
         text_encoding = StringType.UTF8;
         mime_type   = null;
         file_name   = null;
         description = null;
         this.data   = null;
         ParseFields (FieldData (data, offset));
      }
   }
}
