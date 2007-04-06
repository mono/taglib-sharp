/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2frame.cpp from TagLib
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

using TagLib;

namespace TagLib.Id3v2
{   
   public class AttachedPictureFrame : Frame, IPicture
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private StringType  text_encoding;
      private string      mime_type;
      private PictureType type;
      private string      description;
      private ByteVector  data;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public AttachedPictureFrame () : base ("APIC", 4)
      {
         text_encoding = StringType.UTF8;
         mime_type = null;
         type = PictureType.Other;
         description = null;
         data = null;
      }
      
      public AttachedPictureFrame (IPicture picture) : base("APIC", 4)
      {
         text_encoding = StringType.UTF8;
         mime_type = picture.MimeType;
         type = picture.Type;
         description = picture.Description;
         data = picture.Data;
      }

      public AttachedPictureFrame (ByteVector data, uint version) : base (data, version)
      {
         text_encoding = StringType.UTF8;
         mime_type = null;
         type = PictureType.Other;
         description = null;
         this.data = null;
         
         SetData (data, 0, version);
      }
      
      public override string ToString ()
      {
         string s = "[" + mime_type + "]";
         return description != null ? s : description + " " + s;
      }
      
      public static AttachedPictureFrame Find (Tag tag, string description)
      {
         foreach (AttachedPictureFrame f in tag.GetFrames ("APIC"))
            if (f != null && f.Description == description)
               return f;
         return null;
      }
      
      public static AttachedPictureFrame Find (Tag tag, PictureType type)
      {
         foreach (AttachedPictureFrame f in tag.GetFrames ("APIC"))
            if (f != null && f.Type == type)
               return f;
         return null;
      }
      
      public static AttachedPictureFrame Find (Tag tag, string description, PictureType type)
      {
         foreach (AttachedPictureFrame f in tag.GetFrames ("APIC"))
            if (f != null && f.Description == description && f.Type == type)
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
      
      public PictureType Type
      {
         get {return type;}
         set {type = value;}
      }
      
      public string Description
      {
         get {return description;}
         set {description = value;}
      }
      
      public ByteVector Data
      {
         get {return data;}
         set {data = value;}
      }
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected override void ParseFields (ByteVector data, uint version)
      {
         if (data.Count < 5)
            throw new CorruptFileException ("A picture frame must contain at least 5 bytes.");

         int pos = 0;

         text_encoding = (StringType) data [pos++];
         int byte_align = text_encoding == StringType.Latin1 || text_encoding == StringType.UTF8 ? 1 : 2;
         
         int offset;
         
         if (version > 2)
         {
            offset = data.Find (TextDelimiter (StringType.Latin1), pos);

            if(offset < pos)
               return;

            mime_type = data.Mid (pos, offset - pos).ToString (StringType.Latin1);
            pos = offset + 1;
         }
         else
         {
            ByteVector ext = data.Mid (pos, 3);
            if (ext == "JPG")
               mime_type = "image/jpeg";
            else if (ext == "PNG")
               mime_type = "image/png";
            else
               mime_type = "image/unknown";
            
            pos += 3;
         }
         
         type = (PictureType) data [pos++];
         
         offset = data.Find (TextDelimiter (text_encoding), pos, byte_align);

         if(offset < pos)
            return;  
         
         description = data.Mid (pos, offset - pos).ToString (text_encoding);
         pos = offset + 1;
         
         this.data = data.Mid (pos);
      }
      
      protected override ByteVector RenderFields (uint version)
      {
         StringType encoding = CorrectEncoding (TextEncoding, version);
         ByteVector data = new ByteVector ();

         data.Add ((byte) encoding);
         
         if (version == 2)
         {
            switch (MimeType)
            {
            case "image/png":
               data.Add ("PNG");
               break;
            case "image/jpeg":
               data.Add ("JPG");
               break;
            default:
               data.Add ("XXX");
               break;
            }
         }
         else
         {
            data.Add (ByteVector.FromString (MimeType, StringType.Latin1));
            data.Add (TextDelimiter (StringType.Latin1));
         }
         
         data.Add ((byte) type);
         data.Add (ByteVector.FromString (Description, encoding));
         data.Add (TextDelimiter (encoding));
         data.Add (this.data);

         return data;
      }
      
      protected internal AttachedPictureFrame (ByteVector data, int offset, FrameHeader h, uint version) : base (h)
      {
         text_encoding = StringType.UTF8;
         mime_type = null;
         type = PictureType.Other;
         description = null;
         this.data = null;
         
         ParseFields (FieldData (data, offset, version), version);
      }
   }
}
