//
// Picture.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
// 
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

using System;

namespace TagLib.Flac
{
   public class Picture : IPicture
   {
      private PictureType _type;
      private string      _mimetype;
      private string      _description;
      private int         _width = 0;
      private int         _height = 0;
      private int         _color_depth = 0;
      private int         _indexed_colors = 0;
      private ByteVector  _data;
      
      public Picture (ByteVector data)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         if (data.Count < 32)
            throw new CorruptFileException ("Data must be at least 32 bytes long");
         
         int pos = 0;
         _type = (PictureType) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         int mimetype_length = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         _mimetype = data.ToString (StringType.Latin1, pos, mimetype_length);
         pos += mimetype_length;
         
         int description_length = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         _description = data.ToString (StringType.UTF8, pos, description_length);
         pos += description_length;
         
         _width = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         _height = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         _color_depth = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         _indexed_colors = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         int data_length = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         _data = data.Mid (pos, data_length);
      }
      
      public Picture (IPicture picture)
      {
         if (picture == null)
            throw new ArgumentNullException ("picture");
         
         _type        = picture.Type;
         _mimetype    = picture.MimeType;
         _description = picture.Description;
         _data        = picture.Data;
         
         TagLib.Flac.Picture flac_picture = picture as TagLib.Flac.Picture;
         if (flac_picture != null)
         {
            _width          = flac_picture.Width;
            _height         = flac_picture.Height;
            _color_depth    = flac_picture.ColorDepth;
            _indexed_colors = flac_picture.IndexedColors;
         }
      }
      
      public ByteVector Render ()
      {
         ByteVector data = new ByteVector ();
         
         data.Add (ByteVector.FromUInt ((uint) Type));
         
         ByteVector mime_data = ByteVector.FromString (MimeType, StringType.Latin1);
         data.Add (ByteVector.FromUInt ((uint) mime_data.Count));
         data.Add (mime_data);
         
         ByteVector decription_data = ByteVector.FromString (Description, StringType.UTF8);
         data.Add (ByteVector.FromUInt ((uint) decription_data.Count));
         data.Add (decription_data);
         
         data.Add (ByteVector.FromUInt ((uint) Width));
         data.Add (ByteVector.FromUInt ((uint) Height));
         data.Add (ByteVector.FromUInt ((uint) ColorDepth));
         data.Add (ByteVector.FromUInt ((uint) IndexedColors));
         
         data.Add (ByteVector.FromUInt ((uint) Data.Count));
         data.Add (Data);
         
         return data;
      }
      
      public PictureType Type          {get {return _type;}           set {_type = value;}}
      public string      MimeType      {get {return _mimetype;}       set {_mimetype = value;}}
      public string      Description   {get {return _description;}    set {_description = value;}}
      public int         Width         {get {return _width;}          set {_width = value;}}
      public int         Height        {get {return _height;}         set {_height = value;}}
      public int         ColorDepth    {get {return _color_depth;}    set {_color_depth = value;}}
      public int         IndexedColors {get {return _indexed_colors;} set {_indexed_colors = value;}}
      public ByteVector  Data          {get {return _data;}           set {_data = value;}}
   }
}
