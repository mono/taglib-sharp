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

namespace TagLib
{
   public class CombinedTag : Tag
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private List<Tag> tags;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public members
      //////////////////////////////////////////////////////////////////////////
      protected CombinedTag () : base ()
      {
         this.tags = new List<Tag> ();
      }
      
      public CombinedTag (params Tag [] tags) : this ()
      {
         SetTags (tags);
      }

      public void SetTags (params Tag [] tags)
      {
         this.tags.Clear ();
         this.tags.AddRange (tags);
      }
      
      public virtual Tag [] Tags
      {
         get
         {
            return tags.ToArray ();
         }
      }
      
      protected void InsertTag (int index, Tag tag)
      {
         this.tags.Insert (index, tag);
      }
      
      protected void AddTag (Tag tag)
      {
         this.tags.Add (tag);
      }
      
      protected void RemoveTag (Tag tag)
      {
         this.tags.Remove (tag);
      }
      
      protected void ClearTags ()
      {
         this.tags.Clear ();
      }
      
      public override string Title
      {
         get
         {
            string output = null;
            foreach (Tag tag in tags)
               if (tag != null && output == null)
                  output = tag.Title;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.Title = value;
         }
      }

      public override string [] AlbumArtists
      {
         get
         {
            string [] output = new string [] {};
            foreach (Tag tag in tags)
               if (tag != null && output.Length == 0)
                  output = tag.AlbumArtists;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.AlbumArtists = value;
         }
      }

      public override string [] Performers
      {
         get
         {
            string [] output = new string [] {};
            foreach (Tag tag in tags)
               if (tag != null && output.Length == 0)
                  output = tag.Performers;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.Performers = value;
         }
      }

      public override string [] Composers
      {
         get
         {
            string [] output = new string [] {};
            foreach (Tag tag in tags)
               if (tag != null && output.Length == 0)
                  output = tag.Composers;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.Composers = value;
         }
      }

      public override string Album
      {
         get
         {
            string output = null;
            foreach (Tag tag in tags)
               if (tag != null && output == null)
                  output = tag.Album;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.Album = value;
         }
      }

      public override string Comment
      {
         get
         {
            string output = null;
            foreach (Tag tag in tags)
               if (tag != null && output == null)
                  output = tag.Comment;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.Comment = value;
         }
      }

      public override string [] Genres
      {
         get
         {
            string [] output = new string [] {};
            foreach (Tag tag in tags)
               if (tag != null && output.Length == 0)
                  output = tag.Genres;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.Genres = value;
         }
      }

      public override uint Year
      {
         get
         {
            uint output = 0;
            foreach (Tag tag in tags)
               if (tag != null && output == 0)
                  output = tag.Year;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.Year = value;
         }
      }

      public override uint Track
      {
         get
         {
            uint output = 0;
            foreach (Tag tag in tags)
               if (tag != null && output == 0)
                  output = tag.Track;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.Track = value;
         }
      }

      public override uint TrackCount
      {
         get
         {
            uint output = 0;
            foreach (Tag tag in tags)
               if (tag != null && output == 0)
                  output = tag.TrackCount;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.TrackCount = value;
         }
      }

      public override uint Disc
      {
         get
         {
            uint output = 0;
            foreach (Tag tag in tags)
               if (tag != null && output == 0)
                  output = tag.Disc;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.Disc = value;
         }
      }

      public override uint DiscCount
      {
         get
         {
            uint output = 0;
            foreach (Tag tag in tags)
               if (tag != null && output == 0)
                  output = tag.DiscCount;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.DiscCount = value;
         }
      }
      
      public override string Lyrics
      {
         get
         {
            string output = null;
            foreach (Tag tag in tags)
               if (tag != null && output == null)
                  output = tag.Lyrics;
            return output;
         }
         set
         {
            foreach (Tag tag in tags)
               if (tag != null)
                  tag.Lyrics = value;
         }
      }

      public override IPicture [] Pictures {
         get {
            foreach(Tag tag in tags) {
               if(tag != null && tag.Pictures.Length > 0) {
                  return tag.Pictures;
               }
            }
            
            return base.Pictures;
         }
         
         set {
            foreach(Tag tag in tags) {
               if(tag != null) {
                  tag.Pictures = value;
               }
            }
         }
      } 
   }
}
