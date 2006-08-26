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

namespace TagLib.Id3v2
{
   public enum PictureType
   {
      Other              = 0x00, // A type not enumerated below
      FileIcon           = 0x01, // 32x32 PNG image that should be used as the file icon
      OtherFileIcon      = 0x02, // File icon of a different size or format
      FrontCover         = 0x03, // Front cover image of the album
      BackCover          = 0x04, // Back cover image of the album
      LeafletPage        = 0x05, // Inside leaflet page of the album
      Media              = 0x06, // Image from the album itself
      LeadArtist         = 0x07, // Picture of the lead artist or soloist
      Artist             = 0x08, // Picture of the artist or performer
      Conductor          = 0x09, // Picture of the conductor
      Band               = 0x0A, // Picture of the band or orchestra
      Composer           = 0x0B, // Picture of the composer
      Lyricist           = 0x0C, // Picture of the lyricist or text writer
      RecordingLocation  = 0x0D, // Picture of the recording location or studio
      DuringRecording    = 0x0E, // Picture of the artists during recording
      DuringPerformance  = 0x0F, // Picture of the artists during performance
      MovieScreenCapture = 0x10, // Picture from a movie or video related to the track
      ColouredFish       = 0x11, // Picture of a large, coloured fish
      Illustration       = 0x12, // Illustration related to the track
      BandLogo           = 0x13, // Logo of the band or performer
      PublisherLogo      = 0x14  // Logo of the publisher (record company)
   }
   
   public class AttachedPictureFrame : Frame
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
      public AttachedPictureFrame () : base ("APIC")
      {
         text_encoding = StringType.UTF8;
         mime_type = null;
         type = PictureType.Other;
         description = null;
         data = null;
      }

      public AttachedPictureFrame (ByteVector data) : base (data)
      {
         text_encoding = StringType.UTF8;
         mime_type = null;
         type = PictureType.Other;
         description = null;
         this.data = null;
         
         SetData (data);
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
      
      public ByteVector Picture
      {
         get {return data;}
         set {data = value;}
      }


      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected override void ParseFields (ByteVector data)
      {
         if (data.Count < 5)
         {
            Debugger.Debug ("A picture frame must contain at least 5 bytes.");
            return;
         }

         int pos = 0;

         text_encoding = (StringType) data [pos];
         pos += 1;
         
         int offset;
         
         if (Header.Version > 2)
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
         
         type = (PictureType) data [pos];
         pos += 1;
         
         offset = data.Find (TextDelimiter (text_encoding), pos);

         if(offset < pos)
            return;  
         
         description = data.Mid (pos, offset - pos).ToString (text_encoding);
         pos = offset + 1;
         
         this.data = data.Mid (pos);
      }
      
      protected override ByteVector RenderFields ()
      {
         ByteVector data = new ByteVector ();

         data.Add ((byte) TextEncoding);
         data.Add (ByteVector.FromString (MimeType, TextEncoding));
         data.Add (TextDelimiter (StringType.Latin1));
         data.Add ((byte) type);
         data.Add (ByteVector.FromString (Description, TextEncoding));
         data.Add (TextDelimiter (TextEncoding));
         data.Add (this.data);

         return data;
      }
      
      protected internal AttachedPictureFrame (ByteVector data, FrameHeader h) : base (h)
      {
         text_encoding = StringType.UTF8;
         mime_type = null;
         type = PictureType.Other;
         description = null;
         this.data = null;
         
         ParseFields (FieldData (data));
      }
   }
}
