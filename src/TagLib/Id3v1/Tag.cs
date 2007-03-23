/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v1tag.cpp from TagLib
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

namespace TagLib.Id3v1
{
   public class Tag : TagLib.Tag
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      File file;
      long tag_offset;
      
      private string title;
      private string artist;
      private string album;
      private string year;
      private string comment;
      private byte   track;
      private byte   genre;
      
      private static StringHandler string_handler = new StringHandler ();
      
      
      //////////////////////////////////////////////////////////////////////////
      // public static fields
      //////////////////////////////////////////////////////////////////////////
      public static readonly ByteVector FileIdentifier = "TAG";
      public static readonly uint Size = 128;
      
      public static StringHandler DefaultStringHandler
      {
         set {string_handler = value;}
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Tag ()
      {
         file = null;
         tag_offset = -1;
         
         title = artist = album = year = comment = String.Empty;
         
         track = 0;
         genre = 255;
      }

      public Tag (File file, long tag_offset)
      {
         this.file = file;
         this.tag_offset = tag_offset;

         Read ();
      }
      
      public ByteVector Render ()
      {
         ByteVector data = new ByteVector ();

         data.Add (FileIdentifier);
         data.Add (string_handler.Render (title  ).Resize (30));
         data.Add (string_handler.Render (artist ).Resize (30));
         data.Add (string_handler.Render (album  ).Resize (30));
         data.Add (string_handler.Render (year   ).Resize ( 4));
         data.Add (string_handler.Render (comment).Resize (28));
         data.Add ((byte) 0);
         data.Add (track);
         data.Add (genre);

         return data;

      }

      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override string Title
      {
         get {return title;}
         set {title = value != null ? value.Trim () : String.Empty;}
      }
      
      public override string [] AlbumArtists
      {
         get {return new string [] {artist};}
         set {artist = (new StringList (value)).ToString (",").Trim ();}
      }
      
      public override string Album
      {
         get {return album;}
         set {album = value != null ? value.Trim () : String.Empty;}
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
            string genre_name = GenreList.Genre (genre);
            return (genre_name != null) ? new string [] {genre_name} : new string [] {};
         }
         set {genre = (value != null && value.Length > 0) ? GenreList.GenreIndex (value [0].Trim ()) : (byte) 255;}
      }
      
      public  override uint Year
      {
         get
         {
            uint value;
            return uint.TryParse (year, out value) ? value : 0;
         }
         set {year = value > 0 ? value.ToString () : String.Empty;}
      }
      
      public override uint Track
      {
         get {return track;}
         set {track = (byte) (value < 256 ? value : 0);}
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////

      protected void Read ()
      {
         file.Seek (tag_offset);
         
         // read the tag -- always 128 bytes
         ByteVector data = file.ReadBlock ((int)Size);
         
         // some initial sanity checking
         if (data.Count != Size || !data.StartsWith ("TAG"))
            throw new CorruptFileException  ("ID3v1 tag is not valid or could "
                                           + " not be read at the specified "
                                           + "offset.");
         
         int offset = 3;

         title = string_handler.Parse (data.Mid (offset, 30));
         offset += 30;

         artist = string_handler.Parse (data.Mid (offset, 30));
         offset += 30;

         album = string_handler.Parse (data.Mid (offset, 30));
         offset += 30;

         year = string_handler.Parse (data.Mid (offset, 4));
         offset += 4;

         // Check for ID3v1.1 -- Note that ID3v1 *does not* support "track zero" -- this
         // is not a bug in TagLib.  Since a zeroed byte is what we would expect to
         // indicate the end of a C-String, specifically the comment string, a value of
         // zero must be assumed to be just that.

         if (data [offset + 28] == 0 && data[offset + 29] != 0)
         {
            // ID3v1.1 detected

            comment = string_handler.Parse (data.Mid (offset, 28));
            track = data [offset + 29];
         }
         else
            comment = string_handler.Parse (data.Mid (offset, 30));

         offset += 30;

         genre = data [offset];
      }
   }
}
