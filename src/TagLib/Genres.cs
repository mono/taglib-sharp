/***************************************************************************
    copyright            : (C) 2005-2007 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v1genres.cpp from TagLib
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

namespace TagLib
{
   public static class Genres
   {
      private static readonly string [] audio = {
         "Blues",
         "Classic Rock",
         "Country",
         "Dance",
         "Disco",
         "Funk",
         "Grunge",
         "Hip-Hop",
         "Jazz",
         "Metal",
         "New Age",
         "Oldies",
         "Other",
         "Pop",
         "R&B",
         "Rap",
         "Reggae",
         "Rock",
         "Techno",
         "Industrial",
         "Alternative",
         "Ska",
         "Death Metal",
         "Pranks",
         "Soundtrack",
         "Euro-Techno",
         "Ambient",
         "Trip-Hop",
         "Vocal",
         "Jazz+Funk",
         "Fusion",
         "Trance",
         "Classical",
         "Instrumental",
         "Acid",
         "House",
         "Game",
         "Sound Clip",
         "Gospel",
         "Noise",
         "Alternative Rock",
         "Bass",
         "Soul",
         "Punk",
         "Space",
         "Meditative",
         "Instrumental Pop",
         "Instrumental Rock",
         "Ethnic",
         "Gothic",
         "Darkwave",
         "Techno-Industrial",
         "Electronic",
         "Pop-Folk",
         "Eurodance",
         "Dream",
         "Southern Rock",
         "Comedy",
         "Cult",
         "Gangsta",
         "Top 40",
         "Christian Rap",
         "Pop/Funk",
         "Jungle",
         "Native American",
         "Cabaret",
         "New Wave",
         "Psychedelic",
         "Rave",
         "Showtunes",
         "Trailer",
         "Lo-Fi",
         "Tribal",
         "Acid Punk",
         "Acid Jazz",
         "Polka",
         "Retro",
         "Musical",
         "Rock & Roll",
         "Hard Rock",
         "Folk",
         "Folk/Rock",
         "National Folk",
         "Swing",
         "Fusion",
         "Bebob",
         "Latin",
         "Revival",
         "Celtic",
         "Bluegrass",
         "Avantgarde",
         "Gothic Rock",
         "Progressive Rock",
         "Psychedelic Rock",
         "Symphonic Rock",
         "Slow Rock",
         "Big Band",
         "Chorus",
         "Easy Listening",
         "Acoustic",
         "Humour",
         "Speech",
         "Chanson",
         "Opera",
         "Chamber Music",
         "Sonata",
         "Symphony",
         "Booty Bass",
         "Primus",
         "Porn Groove",
         "Satire",
         "Slow Jam",
         "Club",
         "Tango",
         "Samba",
         "Folklore",
         "Ballad",
         "Power Ballad",
         "Rhythmic Soul",
         "Freestyle",
         "Duet",
         "Punk Rock",
         "Drum Solo",
         "A Cappella",
         "Euro-House",
         "Dance Hall",
         "Goa",
         "Drum & Bass",
         "Club-House",
         "Hardcore",
         "Terror",
         "Indie",
         "BritPop",
         "Negerpunk",
         "Polsk Punk",
         "Beat",
         "Christian Gangsta Rap",
         "Heavy Metal",
         "Black Metal",
         "Crossover",
         "Contemporary Christian",
         "Christian Rock",
         "Merengue",
         "Salsa",
         "Thrash Metal",
         "Anime",
         "Jpop",
         "Synthpop"
      };
      
      private static readonly string [] video = new string []
      {
         "Action",
         "Action/Adventure",
         "Adult",
         "Adventure",
         "Catastrophe",
         "Child's",
         "Claymation",
         "Comedy",
         "Concert",
         "Documentary",
         "Drama",
         "Eastern",
         "Entertaining",
         "Erotic",
         "Extremal Sport",
         "Fantasy",
         "Fashion",
         "Historical",
         "Horror",
         "Horror/Mystic",
         "Humor",
         "Indian",
         "Informercial",
         "Melodrama",
         "Military & War",
         "Music Video",
         "Musical",
         "Mystery",
         "Nature",
         "Political Satire",
         "Popular Science",
         "Psychological Thriller",
         "Religion",
         "Science Fiction",
         "Scifi Action",
         "Slapstick",
         "Splatter",
         "Sports",
         "Thriller",
         "Western"
      };
      
      public static string [] Audio
      {
         get
         {
            return (string []) audio.Clone ();
         }
      }
      
      public static string [] Video
      {
         get
         {
            return (string []) video.Clone ();
         }
      }
      
      public static byte AudioToIndex (string name)
      {
         for (byte i = 0; i < audio.Length; i ++)
            if (name == audio [i])
               return i;
         return 255;
      }
      
      public static string IndexToAudio (byte index)
      {
         return (index < audio.Length) ? audio [index] : null;
      }
      
      public static string IndexToAudio (string text)
      {
         return IndexToAudio (StringToByte (text));
      }
      
      public static byte VideoToIndex (string name)
      {
         for (byte i = 0; i < video.Length; i ++)
            if (name == video [i])
               return i;
         return 255;
      }
      
      public static string IndexToVideo (byte index)
      {
         return (index < video.Length) ? video [index] : null;
      }
      
      public static string IndexToVideo (string text)
      {
         return IndexToVideo (StringToByte (text));
      }
      
      private static byte StringToByte (string text)
      {
         byte value;
         int last_pos;
         if (text != null && text.Length > 2 && text [0] == '(' && (last_pos = text.IndexOf (')')) != -1 && 
             byte.TryParse (text.Substring (1, last_pos - 1), out value))
            return value;
         
         if (text != null && byte.TryParse (text, out value))
            return value;
         
         return 255;
      }
   }
}
