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
      private int width;
      private int height;
      private int color_depth;
      private int indexed_colors;
      private ByteVector data;
      
      public Picture (ByteVector data)
      {
         if (data.Count < 32)
            throw new ArgumentException ("Data must be at least 32 bytes long", "data");
         
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
      
      public static ByteVector Render (IPicture picture)
      {
         ByteVector data = new ByteVector ();
         
         data.Add (ByteVector.FromUInt ((uint) picture.Type));
         
         ByteVector mime_data = ByteVector.FromString (picture.MimeType, StringType.Latin1);
         data.Add (ByteVector.FromUInt ((uint) mime_data.Count));
         data.Add (mime_data);
         
         ByteVector decription_data = ByteVector.FromString (picture.Description, StringType.UTF8);
         data.Add (ByteVector.FromUInt ((uint) decription_data.Count));
         data.Add (decription_data);
         
         if (picture is Picture)
         {
            data.Add (ByteVector.FromUInt ((uint) (picture as Picture).Width));
            data.Add (ByteVector.FromUInt ((uint) (picture as Picture).Height));
            data.Add (ByteVector.FromUInt ((uint) (picture as Picture).ColorDepth));
            data.Add (ByteVector.FromUInt ((uint) (picture as Picture).IndexedColors));
         }
         else
            data.Add (new ByteVector (16));
         
         data.Add (ByteVector.FromUInt ((uint) picture.Data.Count));
         data.Add (picture.Data);
         
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
