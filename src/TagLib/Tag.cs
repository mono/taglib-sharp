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
   [Flags]
   public enum TagTypes : uint
   {
      None         = 0x00000000,
      Xiph         = 0x00000001,
      Id3v1        = 0x00000002,
      Id3v2        = 0x00000004,
      Ape          = 0x00000008,
      Apple        = 0x00000010,
      Asf          = 0x00000020,
      RiffInfo     = 0x00000040,
      MovieId      = 0x00000080,
      DivX         = 0x00000100,
      FlacMetadata = 0x00000200,
      AllTags      = 0xFFFFFFFF
   }
   
   public abstract class Tag
   {
      public abstract TagTypes TagTypes {get;}
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
      public virtual string    Grouping        {get {return null;} set {}}
      public virtual uint      BeatsPerMinute  {get {return 0;}    set {}}
      public virtual string    Conductor       {get {return null;} set {}}
      public virtual string    Copyright       {get {return null;} set {}}
      
      public virtual IPicture [] Pictures { get { return new Picture [] { }; } set { } }
      
      public string FirstArtist    { get { return FirstInGroup(AlbumArtists);} }
      public string FirstPerformer { get { return FirstInGroup(Performers);  } }
      public string FirstComposer  { get { return FirstInGroup(Composers);   } }
      public string FirstGenre     { get { return FirstInGroup(Genres);      } }
      
      public string JoinedArtists    { get { return JoinGroup(AlbumArtists);} }
      public string JoinedPerformers { get { return JoinGroup(Performers);  } } 
      public string JoinedComposers  { get { return JoinGroup(Composers);   } }
      public string JoinedGenres     { get { return JoinGroup(Genres);      } }

      private static string FirstInGroup(string [] group)
      {
         return group == null || group.Length == 0 ? null : group[0];
      }
      
      private static string JoinGroup(string [] group)
      {
         return new StringCollection(group).ToString(", ");
      }

      public virtual bool IsEmpty
      {
         get
         {
            return (IsNullOrLikeEmpty (Title) &&
                    IsNullOrLikeEmpty (Grouping) &&
                    IsNullOrLikeEmpty (AlbumArtists) &&
                    IsNullOrLikeEmpty (Performers) &&
                    IsNullOrLikeEmpty (Composers) &&
                    IsNullOrLikeEmpty (Conductor) &&
                    IsNullOrLikeEmpty (Copyright) &&
                    IsNullOrLikeEmpty (Album) &&
                    IsNullOrLikeEmpty (Comment) &&
                    IsNullOrLikeEmpty (Genres) &&
                    Year == 0 &&
                    BeatsPerMinute == 0 &&
                    Track == 0 &&
                    TrackCount == 0 &&
                    Disc == 0 &&
                    DiscCount == 0);
         }
      }
      
      public static void Duplicate (Tag source, Tag target, bool overwrite)
      {
         if (source == null)
            throw new ArgumentNullException ("source");
         
         if (target == null)
            throw new ArgumentNullException ("target");
         
         if (overwrite || IsNullOrLikeEmpty (target.Title))
            target.Title = source.Title;
         if (overwrite || IsNullOrLikeEmpty (target.AlbumArtists))
            target.AlbumArtists = source.AlbumArtists;
         if (overwrite || IsNullOrLikeEmpty (target.Performers))
            target.Performers = source.Performers;
         if (overwrite || IsNullOrLikeEmpty (target.Composers))
            target.Composers = source.Composers;
         if (overwrite || IsNullOrLikeEmpty (target.Album))
            target.Album = source.Album;
         if (overwrite || IsNullOrLikeEmpty (target.Comment))
            target.Comment = source.Comment;
         if (overwrite || IsNullOrLikeEmpty (target.Genres))
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
         if (overwrite || target.BeatsPerMinute == 0)
            target.BeatsPerMinute = source.BeatsPerMinute;
         if (overwrite || IsNullOrLikeEmpty (target.Grouping))
            target.Grouping = source.Grouping;
         if (overwrite || IsNullOrLikeEmpty (target.Conductor))
            target.Conductor = source.Conductor;
         if (overwrite || IsNullOrLikeEmpty (target.Copyright))
            target.Conductor = source.Copyright;
      }
      
      private static bool IsNullOrLikeEmpty (string value)
      {
         return value == null || value.Trim ().Length == 0;
      }
      
      private static bool IsNullOrLikeEmpty (string [] value)
      {
         if (value == null)
            return true;
         
         foreach (string s in value)
            if (!IsNullOrLikeEmpty (s))
               return false;
         
         return true;
      }
   }
}
