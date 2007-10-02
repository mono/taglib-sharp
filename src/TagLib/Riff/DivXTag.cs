//
// DivXTag.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   TagLib.Id3v1.Tag
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

using System.Collections;
using System;
using System.Text;
using System.Globalization;

namespace TagLib.Riff
{
   public class DivXTag : TagLib.Tag
   {
      private string title;
      private string artist;
      private string year;
      private string comment;
      private string genre;
      private ByteVector extra_data;
      
      public static readonly ReadOnlyByteVector FileIdentifier = "DIVXTAG";
      public const uint Size = 128;
      
      public DivXTag ()
      {
         Clear ();
      }

      public DivXTag (File file, long position)
      {
         if (file == null)
            throw new ArgumentNullException ("file");
         
         file.Seek (position);
         
         // read the tag -- always 128 bytes
         ByteVector data = file.ReadBlock ((int)Size);
         
         // some initial sanity checking
         if (data.Count != Size || !data.EndsWith (FileIdentifier))
            throw new CorruptFileException  ("Malformed DivX tag.");
         
         title      = data.ToString (StringType.Latin1,  0, 32).Trim ();
         artist     = data.ToString (StringType.Latin1, 32, 28).Trim ();
         year       = data.ToString (StringType.Latin1, 60,  4).Trim ();
         comment    = data.ToString (StringType.Latin1, 64, 48).Trim ();
         genre      = data.ToString (StringType.Latin1,122,  3).Trim ();
         extra_data = data.Mid (115,  6);
      }
      
      public ByteVector Render ()
      {
         ByteVector data = new ByteVector ();
         data.Add (ByteVector.FromString (ResizeString (title,   32), StringType.Latin1));
         data.Add (ByteVector.FromString (ResizeString (artist,  28), StringType.Latin1));
         data.Add (ByteVector.FromString (ResizeString (year,     4), StringType.Latin1));
         data.Add (ByteVector.FromString (ResizeString (comment, 48), StringType.Latin1));
         data.Add (ByteVector.FromString (ResizeString (genre,    3), StringType.Latin1));
         data.Add (extra_data);
         data.Add (FileIdentifier);
         return data;
      }
      
      private static string ResizeString (string str, int size)
      {
         if (str == null)
            str = string.Empty;
         
         if (str.Length > size)
            return str.Substring (0, size);
         else if (str.Length == size)
            return str;
         
         StringBuilder b = new StringBuilder (str);
         while (b.Length < size)
            b.Append (' ');
         return b.ToString ();
      }
      
      public override TagTypes TagTypes {get {return TagTypes.DivX;}}
      
      public override string Title
      {
         get {return title;}
         set {title = value != null ? value.Trim () : String.Empty;}
      }
      
      public override string [] Performers
      {
         get {return new string [] {artist};}
			set {
				artist = value == null ?
					string.Empty :
					string.Join (",", value).Trim ();
			}
      }
      
      public override string Comment
      {
         get {return comment;}
         set {comment = value != null ? value.Trim () : String.Empty;}
      }
      
      public override string [] Genres
      {
         get
         {
            string genre_name = TagLib.Genres.IndexToVideo (genre);
            return (genre_name != null) ? new string [] {genre_name} : new string [] {};
         }
         set
         {
            genre = (value != null && value.Length > 0) ? TagLib.Genres.VideoToIndex (value [0].Trim ()).ToString (CultureInfo.InvariantCulture) : string.Empty;
         }
      }
      
      public override uint Year
      {
         get
         {
            uint value;
            return uint.TryParse (year, out value) ? value : 0;
         }
         set {year = value > 0 ? value.ToString (CultureInfo.InvariantCulture) : String.Empty;}
      }
      
      public override void Clear ()
      {
         artist = genre = year = comment = String.Empty;
         extra_data = new ByteVector (6);
      }
   }
}
