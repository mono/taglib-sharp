/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
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

namespace TagLib.Id3v1
{
   public class GenreList
   {
      public static readonly string [] Genres = {
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
      
      public static string Genre (byte i)
      {
        if(i >= 0 && i < Genres.Length)
          return Genres [i];
        return null;
      }
      
      public static byte GenreIndex (string name)
      {
         for (byte i = 0; i < Genres.Length; i ++)
            if (name == Genres [i])
               return i;
        return 255;
      }
      
      private GenreList () {}
   }
}
