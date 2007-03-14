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

using System;

namespace TagLib
{
   public class Tag
   {
      public virtual string    Title           {get {return null;} set {}}
      public virtual string [] AlbumArtists    {get {return new string [] {};} set {}}
      public virtual string [] Performers      {get {return new string [] {};} set {}}
      public virtual string [] Composers       {get {return new string [] {};} set {}}
      public virtual string    Album           {get {return null;} set {}}
      public virtual string    Comment         {get {return null;} set {}}
      public virtual string [] Genres          {get {return new string [] {};} set {}}
      public virtual uint      Year            {get {return 0;}    set {}}
      public virtual uint      Track           {get {return 0;}    set {}}
      public virtual uint      TrackCount      {get {return 0;}    set {}}
      public virtual uint      Disc            {get {return 0;}    set {}}
      public virtual uint      DiscCount       {get {return 0;}    set {}}
      public virtual string    Lyrics          {get {return null;} set {}}
      
      [Obsolete("This property is obsolete; use the AlbumArtists property instead.")]
      public string [] Artists {get {return AlbumArtists;} set {AlbumArtists = value;}}
      
      public virtual IPicture [] Pictures { get { return new Picture [] { }; } set { } }
      
      public string FirstArtist    { get { return FirstInGroup(AlbumArtists);    } }
      public string FirstPerformer { get { return FirstInGroup(Performers); } }
      public string FirstComposer  { get { return FirstInGroup(Composers);  } }
      public string FirstGenre     { get { return FirstInGroup(Genres);     } }
      
      public string JoinedArtists    { get { return JoinGroup(AlbumArtists);     } }
      public string JoinedPerformers { get { return JoinGroup(Performers);  } } 
      public string JoinedComposers  { get { return JoinGroup(Composers);   } }
      public string JoinedGenres     { get { return JoinGroup(Genres);      } }

      private static string FirstInGroup(string [] group)
      {
         return group == null || group.Length == 0 ? null : group[0];
      }
      
      private static string JoinGroup(string [] group)
      {
         return new StringList(group).ToString(", ");
      }

      public virtual bool IsEmpty
      {
         get
         {
            return ((Title == null || Title.Trim () == String.Empty) &&
                    (AlbumArtists == null || AlbumArtists.Length == 0) &&
                    (Performers == null || Performers.Length == 0) &&
                    (Composers == null || Composers.Length == 0) &&
                    (Album == null || Album.Trim () == String.Empty) &&
                    (Comment == null || Comment.Trim () == String.Empty) &&
                    (Genres == null || Genres.Length == 0) &&
                    Year == 0 &&
                    Track == 0 &&
                    TrackCount == 0 &&
                    Disc == 0 &&
                    DiscCount == 0);
         }
      }
      
      public static void Duplicate (Tag source, Tag target, bool overwrite)
      {
         if (overwrite || target.Title == null || target.Title.Trim () == String.Empty)
            target.Title = source.Title;
         if (overwrite || target.AlbumArtists == null || target.AlbumArtists.Length == 0)
            target.AlbumArtists = source.AlbumArtists;
         if (overwrite || target.Performers == null || target.Performers.Length == 0)
            target.Performers = source.Performers;
         if (overwrite || target.Composers == null || target.Composers.Length == 0)
            target.Composers = source.Composers;
         if (overwrite || target.Album == null || target.Album.Trim () == String.Empty)
            target.Album = source.Album;
         if (overwrite || target.Comment == null || target.Comment.Trim () == String.Empty)
            target.Comment = source.Comment;
         if (overwrite || target.Genres == null || target.Genres.Length == 0)
            target.Genres = source.Genres;
         if (overwrite || target.Year == 0)
            target.Year = source.Year;
         if (overwrite || target.Track == 0)
            target.Track = source.Track;
         if (overwrite || target.TrackCount == 0)
            target.TrackCount = source.TrackCount;
         if (overwrite || target.Disc == 0)
            target.Disc = source.Disc;
         if (overwrite || target.DiscCount == 0)
            target.DiscCount = source.DiscCount;
      }
   }
}
