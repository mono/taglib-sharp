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
      #region Private Properties
      private StringType  text_encoding = StringType.UTF8;
      private string      mime_type     = null;
      private PictureType type          = PictureType.Other;
      private string      description   = null;
      private ByteVector  data          = null;
      
      // Performance savings for simple processes.
      private ByteVector  raw_data      = null;
      private byte        raw_version   = 0;
      #endregion
      
      
      
      #region Constructors
      public AttachedPictureFrame () : base ("APIC", 4)
      {}
      
      public AttachedPictureFrame (IPicture picture) : base("APIC", 4)
      {
         if (picture == null)
            throw new ArgumentNullException ("picture");
         
         mime_type   = picture.MimeType;
         type        = picture.Type;
         description = picture.Description;
         data        = picture.Data;
      }

      public AttachedPictureFrame (ByteVector data, byte version) : base (data, version)
      {
         SetData (data, 0, version, true);
      }
      
      protected internal AttachedPictureFrame (ByteVector data, int offset, FrameHeader header, byte version) : base(header)
      {
         SetData (data, offset, version, false);
      }
      #endregion
      
      
      
      #region Public Properties
      public StringType TextEncoding
      {
         get {ParseRawData (); return text_encoding;}
         set {text_encoding = value;}
      }
      
      public string MimeType
      {
         get {ParseRawData (); return mime_type;}
         set {mime_type = value;}
      }
      
      public PictureType Type
      {
         get {ParseRawData (); return type;}
         set {type = value;}
      }
      
      public string Description
      {
         get {ParseRawData (); return description;}
         set {description = value;}
      }
      
      public ByteVector Data
      {
         get {ParseRawData (); return data;}
         set {data = value;}
      }
      #endregion
      
      
      
      #region Public Methods
      public override string ToString ()
      {
         ParseRawData ();
         string s = "[" + mime_type + "]";
         return description != null ? s : description + " " + s;
      }
      #endregion
      
      
      
      #region Public Static Methods
      public static AttachedPictureFrame Get (Tag tag, string description, bool create)
      {
         foreach (Frame f in tag.GetFrames ("APIC"))
            if (f is AttachedPictureFrame && (f as AttachedPictureFrame).Description == description)
               return f as AttachedPictureFrame;
         
         if (!create) return null;
         
         AttachedPictureFrame frame = new AttachedPictureFrame ();
         frame.Description = description;
         tag.AddFrame (frame);
         return frame;
      }
      
      public static AttachedPictureFrame Get (Tag tag, PictureType type, bool create)
      {
         foreach (AttachedPictureFrame f in tag.GetFrames ("APIC"))
            if (f != null && f.Type == type)
               return f;
         
         if (!create) return null;
         
         AttachedPictureFrame frame = new AttachedPictureFrame ();
         frame.Type = type;
         tag.AddFrame (frame);
         return frame;
      }
      
      public static AttachedPictureFrame Get (Tag tag, string description, PictureType type, bool create)
      {
         foreach (AttachedPictureFrame f in tag.GetFrames ("APIC"))
            if (f != null && f.Description == description && f.Type == type)
               return f;
         
         if (!create) return null;
         
         AttachedPictureFrame frame = new AttachedPictureFrame ();
         frame.Description = description;
         frame.Type = type;
         tag.AddFrame (frame);
         return frame;
      }
      #endregion
      
      
      
      #region Protected Methods
      protected override void ParseFields (ByteVector data, byte version)
      {
         if (data.Count < 5)
            throw new CorruptFileException ("A picture frame must contain at least 5 bytes.");
         
         raw_data = data;
         raw_version = version;
      }
      
      protected void ParseRawData ()
      {
         if (raw_data == null)
            return;
         
         int pos = 0;

         text_encoding = (StringType) raw_data [pos++];
         int byte_align = text_encoding == StringType.Latin1 || text_encoding == StringType.UTF8 ? 1 : 2;
         
         int offset;
         
         if (raw_version > 2)
         {
            offset = raw_data.Find (TextDelimiter (StringType.Latin1), pos);

            if(offset < pos)
               return;

            mime_type = raw_data.Mid (pos, offset - pos).ToString (StringType.Latin1);
            pos = offset + 1;
         }
         else
         {
            ByteVector ext = raw_data.Mid (pos, 3);
            if (ext == "JPG")
               mime_type = "image/jpeg";
            else if (ext == "PNG")
               mime_type = "image/png";
            else
               mime_type = "image/unknown";
            
            pos += 3;
         }
         
         type = (PictureType) raw_data [pos++];
         
         offset = raw_data.Find (TextDelimiter (text_encoding), pos, byte_align);

         if(offset < pos)
            return;  
         
         description = raw_data.Mid (pos, offset - pos).ToString (text_encoding);
         pos = offset + 1;
         
         raw_data.RemoveRange (0, pos);
         this.data = raw_data;
         this.raw_data = null;
      }
      
      protected override ByteVector RenderFields (byte version)
      {
         if (raw_data != null && raw_version == version)
            return raw_data;
         
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
      #endregion
   }
}
