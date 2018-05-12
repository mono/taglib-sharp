using System;
using System.IO;

/// <summary>
/// Stub program to debug some scenarios. Modify it as you need, this is not meant to be reuseable program.
/// </summary>
namespace debug
{
	class Program
	{
		public static readonly string Samples = 
			Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(Program)).Location) 
			+ @"\..\..\..\..\tests\samples\";

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

			// Override command arguments
			args = new string[]  { "sample.wav" };

			foreach (var fname in args) {
				var fpath = Samples + fname;
				var tpath = Samples + "tmpwrite" + Path.GetExtension(fname);

				log("+ File  : " + fpath);
				if(!File.Exists(fpath))
				{
					log("  # File not found: " + fpath);
					continue;
				}

				log("  read  : " + fpath);
				var rfile = TagLib.File.Create(fpath);
				log("  Type  : " + rfile.MimeType);

				File.Copy(fpath, tpath, true);

				var file = TagLib.File.Create(tpath);
				log("  Type  : " + file.MimeType);
				
				log("  rboy1 test start  : " + file.MimeType);

				var MKVTag = (TagLib.Matroska.Tag)file.GetTag(TagLib.TagTypes.Matroska);
				MKVTag.Title = "my Title";
				MKVTag.Set("SUBTITLE", null, "my SubTitle");
				MKVTag.Set("DESCRIPTION", null, "my Description");
				MKVTag.Set("TVCHANNEL", null, "my Network");
				MKVTag.Set("LAW_RATING", null, "my Rating");
				MKVTag.Set("ACTOR", null, "my MediaCredits");
				MKVTag.Set("GENRE", null, "my Genres");
				MKVTag.Set("SEASON", null, "my Season");
				MKVTag.Set("EPISODE", null, "my Episode");

				var BannerFile = Samples + "sample_invalidifdoffset.jpg";
				TagLib.Picture VideoPicture = new TagLib.Picture(BannerFile);
				MKVTag.Pictures = new TagLib.Picture[] { VideoPicture };

				log("  rboy1 test save  : " + file.MimeType);
				file.Save();
				
				log("  rboy1 test read  : " + file.MimeType);
				TagLib.File TagFile = TagLib.File.Create(tpath);

				log("  rboy1 test end  : " + file.MimeType);

				var tag = file.Tag;
				var pics = file.Tag.Pictures;

				var mtag = (TagLib.Matroska.Tag)file.GetTag(TagLib.TagTypes.Matroska);
				mtag.PerformersRole = new string[] { "TEST role 1", "TEST role 2" };

				log("    Picture            : " + pics[0].Description);

				var tracks = mtag.Tags.Tracks;
				var audiotag = mtag.Tags.Get(tracks[1]);
				if (audiotag != null)
				{
					audiotag.Clear();
					audiotag.Title = "The Noise";
					audiotag.Set("DESCRIPTION", null, "Useless background noise");
				}

				log("  Erase : " + tag.Title);
				file.RemoveTags(TagLib.TagTypes.Matroska);
				file.Save();

				log("  Write : " + tag.Title);

				tag.TitleSort = "title, TEST";
				tag.AlbumSort = "album, TEST";
				tag.PerformersSort = new string[] { "performer 1, TEST", "performer 2, TEST" };
				tag.ComposersSort = new string[] { "composer 1, TEST", "composer 2, TEST" };
				tag.AlbumArtistsSort = new string[] { "album artist 1, TEST", "album artist 2, TEST" };


				tag.Album = "TEST album";
				tag.AlbumArtists = new string[] { "TEST album artist 1", "TEST album artist 2" };
				tag.BeatsPerMinute = 120;
				tag.Comment = "TEST comment";
				tag.Composers = new string[] { "TEST composer 1", "TEST composer 2" };
				tag.Conductor = "TEST conductor";
				tag.Copyright = "TEST copyright";
				tag.Disc = 1;
				tag.DiscCount = 2;
				tag.Genres = new string[] { "TEST genre 1", "TEST genre 2" };
				tag.Grouping = "TEST grouping";
				tag.Lyrics = "TEST lyrics 1\r\nTEST lyrics 2";
				tag.Performers = new string[] { "TEST performer 1", "TEST performer 2" };
				tag.Title = "TEST title";
				tag.Track = 5;
				tag.TrackCount = 10;
				tag.Year = 1999;

				// Insert new picture
				Array.Resize(ref pics, 2);
				pics[1] = new TagLib.Picture(Samples + "sample_sony2.jpg");
				file.Tag.Pictures = pics;

				file.Save();


				log("  Done  : " + tag.Title);

				// Now read it again
				file = TagLib.File.Create(tpath);
				tag = file.Tag;
				mtag = (TagLib.Matroska.Tag)file.GetTag(TagLib.TagTypes.Matroska);

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

				log("    TitleSort          : " + tag.TitleSort);
				log("    AlbumSort          : " + tag.AlbumSort);
				log("    PerformersSort     : " + tag.JoinedPerformersSort);
				log("    ComposersSort      : " + string.Join("; ", tag.ComposersSort));
				log("    AlbumArtistsSort   : " + string.Join("; ", tag.AlbumArtistsSort));


				log("    PerformersRole     : " + string.Join("; ", mtag.PerformersRole));

				log("  Done  : " + tag.Title);
			}

			log("* End");
		}

	}
}
