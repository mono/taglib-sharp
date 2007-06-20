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
using System.Globalization;

namespace TagLib.Id3v1
{
   public class Tag : TagLib.Tag
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
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
      public static readonly ReadOnlyByteVector FileIdentifier = "TAG";
      public const uint Size = 128;
      
      public static StringHandler DefaultStringHandler
      {
         get {return string_handler;}
         set {string_handler = value;}
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Tag ()
      {
         Clear ();
      }

      public Tag (File file, long position)
      {
         if (file == null)
            throw new ArgumentNullException ("file");
         
         file.Seek (position);
         
         // read the tag -- always 128 bytes
         ByteVector data = file.ReadBlock ((int)Size);
         
         // some initial sanity checking
         if (data.Count != Size || !data.StartsWith (FileIdentifier))
            throw new CorruptFileException
               ("ID3v1 tag is not valid or could not be read at the specified offset.");
         
         title  = string_handler.Parse (data.Mid ( 3, 30));
         artist = string_handler.Parse (data.Mid (33, 30));
         album  = string_handler.Parse (data.Mid (63, 30));
         year   = string_handler.Parse (data.Mid (93,  4));

         // Check for ID3v1.1 -- Note that ID3v1 *does not* support "track zero"
         // -- this is not a bug in TagLib.  Since a zeroed byte is what we 
         // would expect to indicate the end of a C-String, specifically the 
         // comment string, a value of zero must be assumed to be just that.

         if (data [125] == 0 && data [126] != 0)
         {
            // ID3v1.1 detected
            comment = string_handler.Parse (data.Mid (97, 28));
            track = data [126];
         }
         else
            comment = string_handler.Parse (data.Mid (97, 30));

         genre = data [127];
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
      public override TagTypes TagTypes {get {return TagTypes.Id3v1;}}
      
      public override string Title
      {
         get {return title;}
         set {title = value != null ? value.Trim () : String.Empty;}
      }
      
      public override string [] Performers
      {
         get {return new string [] {artist};}
         set {artist = (new StringCollection (value)).ToString (",").Trim ();}
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
            string genre_name = TagLib.Genres.IndexToAudio (genre);
            return (genre_name != null) ? new string [] {genre_name} : new string [] {};
         }
         set {genre = (value != null && value.Length > 0) ? TagLib.Genres.AudioToIndex (value [0].Trim ()) : (byte) 255;}
      }
      
      public override uint Year
      {
         get
         {
            uint value;
            return uint.TryParse (year, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) ? value : 0;
         }
         set {year = value > 0 ? value.ToString (CultureInfo.InvariantCulture) : String.Empty;}
      }
      
      public override uint Track
      {
         get {return track;}
         set {track = (byte) (value < 256 ? value : 0);}
      }
      
      public override void Clear ()
      {
         title = artist = album = year = comment = String.Empty;
         genre = 255;
      }

public override bool IsEmpty {get {return true;}}
   }
}
