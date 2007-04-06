/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : tag.cpp from TagLib
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

using System.Collections.Generic;

namespace TagLib.Ogg
{
   public class GroupedComment : Tag
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Dictionary<uint, XiphComment> comment_hash;
      private List<XiphComment> tags;
      
      //////////////////////////////////////////////////////////////////////////
      // public members
      //////////////////////////////////////////////////////////////////////////
      public GroupedComment () : base ()
      {
         comment_hash = new Dictionary <uint, XiphComment> ();
         tags = new List<XiphComment> ();
      }
      
      public XiphComment [] Comments
      {
         get
         {
            return tags.ToArray ();
         }
      }
      
      public XiphComment GetComment (uint stream_serial_number)
      {
         return comment_hash [stream_serial_number];
      }
      
      public void AddComment (uint stream_serial_number, XiphComment comment)
      {
         comment_hash.Add (stream_serial_number, comment);
         tags.Add (comment);
      }
      
      public void AddComment (uint stream_serial_number, ByteVector data)
      {
         AddComment (stream_serial_number, new XiphComment (data));
      }
      
      public void Clear ()
      {
         foreach (XiphComment tag in tags)
            tag.Clear ();
      }
      
      public override string Title
      {
         get
         {
            string output = null;
            foreach (XiphComment tag in tags)
               if (tag != null && output == null)
                  output = tag.Title;
            return output;
         }
         set
         {
            tags [0].Title = value;
         }
      }

      public override string [] AlbumArtists
      {
         get
         {
            string [] output = new string [] {};
            foreach (XiphComment tag in tags)
               if (tag != null && output.Length == 0)
                  output = tag.AlbumArtists;
            return output;
         }
         set
         {
            tags [0].AlbumArtists = value;
         }
      }

      public override string [] Performers
      {
         get
         {
            string [] output = new string [] {};
            foreach (XiphComment tag in tags)
               if (tag != null && output.Length == 0)
                  output = tag.Performers;
            return output;
         }
         set
         {
            tags [0].Performers = value;
         }
      }

      public override string [] Composers
      {
         get
         {
            string [] output = new string [] {};
            foreach (XiphComment tag in tags)
               if (tag != null && output.Length == 0)
                  output = tag.Composers;
            return output;
         }
         set
         {
            tags [0].Composers = value;
         }
      }

      public override string Album
      {
         get
         {
            string output = null;
            foreach (XiphComment tag in tags)
               if (tag != null && output == null)
                  output = tag.Album;
            return output;
         }
         set
         {
            tags [0].Album = value;
         }
      }

      public override string Comment
      {
         get
         {
            string output = null;
            foreach (XiphComment tag in tags)
               if (tag != null && output == null)
                  output = tag.Comment;
            return output;
         }
         set
         {
            tags [0].Comment = value;
         }
      }

      public override string [] Genres
      {
         get
         {
            string [] output = new string [] {};
            foreach (XiphComment tag in tags)
               if (tag != null && output.Length == 0)
                  output = tag.Genres;
            return output;
         }
         set
         {
            tags [0].Genres = value;
         }
      }

      public override uint Year
      {
         get
         {
            uint output = 0;
            foreach (XiphComment tag in tags)
               if (tag != null && output == 0)
                  output = tag.Year;
            return output;
         }
         set
         {
            tags [0].Year = value;
         }
      }

      public override uint Track
      {
         get
         {
            uint output = 0;
            foreach (XiphComment tag in tags)
               if (tag != null && output == 0)
                  output = tag.Track;
            return output;
         }
         set
         {
            tags [0].Track = value;
         }
      }

      public override uint TrackCount
      {
         get
         {
            uint output = 0;
            foreach (XiphComment tag in tags)
               if (tag != null && output == 0)
                  output = tag.TrackCount;
            return output;
         }
         set
         {
            tags [0].TrackCount = value;
         }
      }

      public override uint Disc
      {
         get
         {
            uint output = 0;
            foreach (XiphComment tag in tags)
               if (tag != null && output == 0)
                  output = tag.Disc;
            return output;
         }
         set
         {
            tags [0].Disc = value;
         }
      }

      public override uint DiscCount
      {
         get
         {
            uint output = 0;
            foreach (XiphComment tag in tags)
               if (tag != null && output == 0)
                  output = tag.DiscCount;
            return output;
         }
         set
         {
            tags [0].DiscCount = value;
         }
      }
      
      public override string Lyrics
      {
         get
         {
            string output = null;
            foreach (XiphComment tag in tags)
               if (tag != null && output == null)
                  output = tag.Lyrics;
            return output;
         }
         set
         {
            tags [0].Lyrics = value;
         }
      }

      public override IPicture [] Pictures {
         get {
            foreach(XiphComment tag in tags) {
               if(tag != null && tag.Pictures.Length > 0) {
                  return tag.Pictures;
               }
            }
            
            return base.Pictures;
         }
         
         set {
            tags [0].Pictures = value;
         }
      } 
   }
}
