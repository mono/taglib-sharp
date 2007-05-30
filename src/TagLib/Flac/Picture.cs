/***************************************************************************
    copyright            : (C) 2007 by Brian Nickel
    email                : brian.nickel@gmail.com
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

using System;

namespace TagLib.Flac
{
   public class Picture : IPicture
   {
      private PictureType type;
      private string mime_type;
      private string description;
      private int width = 0;
      private int height = 0;
      private int color_depth = 0;
      private int indexed_colors = 0;
      private ByteVector data;
      
      public Picture (ByteVector data)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         if (data.Count < 32)
            throw new CorruptFileException ("Data must be at least 32 bytes long");
         
         int pos = 0;
         type = (PictureType) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         int mime_type_length = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         mime_type = data.Mid (pos, mime_type_length).ToString (StringType.Latin1);
         pos += mime_type_length;
         
         int description_length = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         description = data.Mid (pos, description_length).ToString (StringType.UTF8);
         pos += description_length;
         
         width = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         height = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         color_depth = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         indexed_colors = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         int data_length = (int) data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         this.data = data.Mid (pos, data_length);
      }
      
      public Picture (IPicture picture)
      {
         if (picture == null)
            throw new ArgumentNullException ("picture");
         this.type = picture.Type;
         this.mime_type = picture.MimeType;
         this.description = picture.Description;
         this.data = picture.Data;
         
         TagLib.Flac.Picture flac_picture = picture as TagLib.Flac.Picture;
         if (flac_picture != null)
         {
            this.width          = flac_picture.Width;
            this.height         = flac_picture.Height;
            this.color_depth    = flac_picture.ColorDepth;
            this.indexed_colors = flac_picture.IndexedColors;
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
      
      public PictureType Type          {get {return type;}           set {type = value;}}
      public string      MimeType      {get {return mime_type;}      set {mime_type = value;}}
      public string      Description   {get {return description;}    set {description = value;}}
      public int         Width         {get {return width;}          set {width = value;}}
      public int         Height        {get {return height;}         set {height = value;}}
      public int         ColorDepth    {get {return color_depth;}    set {color_depth = value;}}
      public int         IndexedColors {get {return indexed_colors;} set {indexed_colors = value;}}
      public ByteVector  Data          {get {return data;}           set {data = value;}}
   }
}
