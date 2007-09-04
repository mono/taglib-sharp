//
// GroupedComment.cs:
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
      
      public IEnumerable<XiphComment> Comments
      {
         get
         {
            return tags;
         }
      }
      
      public XiphComment GetComment (uint streamSerialNumber)
      {
         return comment_hash [streamSerialNumber];
      }
      
      public void AddComment (uint streamSerialNumber, XiphComment comment)
      {
         comment_hash.Add (streamSerialNumber, comment);
         tags.Add (comment);
      }
      
      public void AddComment (uint streamSerialNumber, ByteVector data)
      {
         AddComment (streamSerialNumber, new XiphComment (data));
      }
      
      public override void Clear ()
      {
         foreach (XiphComment tag in tags)
            tag.Clear ();
      }
      
      public override TagTypes TagTypes
      {
         get
         {
            TagTypes types = TagTypes.None;
            foreach (XiphComment tag in tags)
               if (tag != null)
                  types |= tag.TagTypes;
            return types;
         }
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

      public override string Grouping
      {
         get
         {
            string output = null;
            foreach (XiphComment tag in tags)
               if (tag != null && output == null)
                  output = tag.Grouping;
            return output;
         }
         set
         {
            tags [0].Grouping = value;
         }
      }
      
      public override uint BeatsPerMinute
      {
         get
         {
            uint output = 0;
            foreach (XiphComment tag in tags)
               if (tag != null && output == 0)
                  output = tag.BeatsPerMinute;
            return output;
         }
         set
         {
            tags [0].BeatsPerMinute = value;
         }
      }
      
      public override string Conductor
      {
         get
         {
            string output = null;
            foreach (XiphComment tag in tags)
               if (tag != null && output == null)
                  output = tag.Conductor;
            return output;
         }
         set
         {
            tags [0].Conductor = value;
         }
      }
      
      public override string Copyright
      {
         get
         {
            string output = null;
            foreach (XiphComment tag in tags)
               if (tag != null && output == null)
                  output = tag.Copyright;
            return output;
         }
         set
         {
            tags [0].Copyright = value;
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
      
      public override bool IsEmpty
      {
         get
         {
            foreach (XiphComment tag in tags)
               if (!tag.IsEmpty)
                  return false;
            
            return true;
         }
      }
   }
}
