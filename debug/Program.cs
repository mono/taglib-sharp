using System;
using System.IO;

namespace debug
{
    class Program
    {
        public static readonly string Samples = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(Program)).Location) + @"\..\..\..\tests\samples\";

        /// <summary>
        /// Ouput message on the console and on the Visual Studio Output
        /// </summary>
        /// <param name="str"></param>
        static void log(string str)
        {
            Console.WriteLine(str);
            System.Diagnostics.Debug.WriteLine(str);
        }


        static void Main(string[] args)
        {
            log("--------------------");
            log("* Start : Samples directory: " + Samples);
            log("");

            foreach (var fname in args) {
                var fpath = Samples + fname;
                var tpath = Samples + "tmpwrite" + Path.GetExtension(fname);

                log("+ File  : " + fpath);
                if(!File.Exists(fpath))
                {
                    log("  # File not found: " + fpath);
                    continue;
                }

                File.Copy(fpath, tpath, true);

                var file = TagLib.File.Create(tpath);
                log("  Type  : " + file.MimeType);

                var tag = file.Tag;
                var pics = file.Tag.Pictures;

                log("    Picture            : " + pics[0].Description);


                log("  Erase : " + tag.Title);

                file.RemoveTags(TagLib.TagTypes.Matroska);
                file.Save();

                log("    IsEmpty            : " + tag.IsEmpty);


                log("  Write : " + tag.Title);
                tag.Album = "TEST album";
                tag.AlbumArtists = new string[] { "TEST artist 1", "TEST artist 2" };
                tag.BeatsPerMinute = 120;
                tag.Comment = "TEST comment";
                tag.Composers = new string[] { "TEST composer 1", "TEST composer 2" };
                tag.Conductor = "TEST conductor";
                tag.Copyright = "TEST copyright";
                tag.Disc = 100;
                tag.DiscCount = 101;
                tag.Genres = new string[] { "TEST genre 1", "TEST genre 2" };
                tag.Grouping = "TEST grouping";
                tag.Lyrics = "TEST lyrics 1\r\nTEST lyrics 2";
                tag.Performers = new string[] { "TEST performer 1", "TEST performer 2" };
                tag.Title = "TEST title";
                tag.Track = 98;
                tag.TrackCount = 99;
                tag.Year = 1999;

                file.Save();


                log("  Done  : " + tag.Title);

                // Now read it again
                file = TagLib.File.Create(tpath);
                tag = file.Tag;

                log("  Read  : " + tag.Title);

                log("    Album              : " + tag.Album);
                log("    JoinedAlbumArtists : " + tag.JoinedAlbumArtists);
                log("    BeatsPerMinute     : " + tag.BeatsPerMinute);
                log("    Comment            : " + tag.Comment);
                log("    JoinedComposers    : " + tag.JoinedComposers);
                log("    Conductor          : " + tag.Conductor);
                log("    Copyright          : " + tag.Copyright);
                log("    Disc               : " + tag.Disc);
                log("    DiscCount          : " + tag.DiscCount);
                log("    JoinedGenres       : " + tag.JoinedGenres);
                log("    Grouping           : " + tag.Grouping);
                log("    Lyrics             : " + tag.Lyrics);
                log("    JoinedPerformers   : " + tag.JoinedPerformers);
                log("    Title              : " + tag.Title);
                log("    Track              : " + tag.Track);
                log("    TrackCount         : " + tag.TrackCount);
                log("    Year               : " + tag.Year);

                log("  Done  : " + tag.Title);
            }

            log("* End");
        }

    }
}
