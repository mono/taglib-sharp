using System;
using System.IO;
using System.Reflection;
using TagLib;
using File = System.IO.File;

namespace Debug;

/// <summary>
/// Stub program to debug some scenarios. Modify it as you need, this is not meant to be reuseable program.
/// </summary>
class Program
{
	static readonly string AssemblyLocation = Path.GetDirectoryName (Assembly.GetAssembly (typeof (Program)).Location);
	public static readonly string Samples = Path.Combine (AssemblyLocation, "..", "..", "..", "..", "TaglibSharp.Tests", "samples");

	/// <summary>
	/// Output message on the console and on the Visual Studio Output
	/// </summary>
	/// <param name="str"></param>
	static void Log (string str)
	{
		Console.WriteLine (str);
		System.Diagnostics.Debug.WriteLine (str);
	}

	static void Main (string[] args)
	{
		Log ("--------------------");
		Log ($"* Start : Samples directory: {Samples}");
		Log ("");

		// Override command arguments
		args = ["sample.wav"];

		foreach (var fname in args) {
			var fpath = Samples + fname;
			var tpath = $"{Samples}tmpwrite{Path.GetExtension (fname)}";

			Log ($"+ File  : {fpath}");
			if (!File.Exists (fpath)) {
				Log ($"  # File not found: {fpath}");
				continue;
			}

			Log ($"  read  : {fpath}");
			var rfile = TagLib.File.Create (fpath);
			Log ($"  Type  : {rfile.MimeType}");

			File.Copy (fpath, tpath, true);

			var file = TagLib.File.Create (tpath);
			Log ($"  Type  : {file.MimeType}");

			Log ($"  rboy1 test start  : {file.MimeType}");

			var MKVTag = (TagLib.Matroska.Tag)file.GetTag (TagTypes.Matroska);
			MKVTag.Title = "my Title";
			MKVTag.Set ("SUBTITLE", null, "my SubTitle");
			MKVTag.Set ("DESCRIPTION", null, "my Description");
			MKVTag.Set ("TVCHANNEL", null, "my Network");
			MKVTag.Set ("LAW_RATING", null, "my Rating");
			MKVTag.Set ("ACTOR", null, "my MediaCredits");
			MKVTag.Set ("GENRE", null, "my Genres");
			MKVTag.Set ("SEASON", null, "my Season");
			MKVTag.Set ("EPISODE", null, "my Episode");

			var bannerFile = $"{Samples}sample_invalidifdoffset.jpg";
			var videoPicture = new Picture (bannerFile);
			MKVTag.Pictures = [videoPicture];

			Log ($"  rboy1 test save  : {file.MimeType}");
			file.Save ();

			Log ($"  rboy1 test read  : {file.MimeType}");
			var tagFile = TagLib.File.Create (tpath);

			Log ($"  rboy1 test end  : {file.MimeType}");

			var tag = file.Tag;
			var pics = file.Tag.Pictures;

			var mtag = (TagLib.Matroska.Tag)file.GetTag (TagTypes.Matroska);
			mtag.PerformersRole = ["TEST role 1", "TEST role 2"];

			Log ($"    Picture            : {pics[0].Description}");

			var tracks = mtag.Tags.Tracks;
			var audiotag = mtag.Tags.Get (tracks[1]);
			if (audiotag != null) {
				audiotag.Clear ();
				audiotag.Title = "The Noise";
				audiotag.Set ("DESCRIPTION", null, "Useless background noise");
			}

			Log ($"  Erase : {tag.Title}");
			file.RemoveTags (TagTypes.Matroska);
			file.Save ();

			Log ($"  Write : {tag.Title}");

			tag.TitleSort = "title, TEST";
			tag.AlbumSort = "album, TEST";
			tag.PerformersSort = ["performer 1, TEST", "performer 2, TEST"];
			tag.ComposersSort = ["composer 1, TEST", "composer 2, TEST"];
			tag.AlbumArtistsSort = ["album artist 1, TEST", "album artist 2, TEST"];


			tag.Album = "TEST album";
			tag.AlbumArtists = ["TEST album artist 1", "TEST album artist 2"];
			tag.BeatsPerMinute = 120;
			tag.Comment = "TEST comment";
			tag.Composers = ["TEST composer 1", "TEST composer 2"];
			tag.Conductor = "TEST conductor";
			tag.Copyright = "TEST copyright";
			tag.Disc = 1;
			tag.DiscCount = 2;
			tag.Genres = ["TEST genre 1", "TEST genre 2"];
			tag.Grouping = "TEST grouping";
			tag.Lyrics = "TEST lyrics 1\r\nTEST lyrics 2";
			tag.Performers = ["TEST performer 1", "TEST performer 2"];
			tag.Title = "TEST title";
			tag.Track = 5;
			tag.TrackCount = 10;
			tag.Year = 1999;

			// Insert new picture
			Array.Resize (ref pics, 2);
			pics[1] = new Picture ($"{Samples}sample_sony2.jpg");
			file.Tag.Pictures = pics;

			file.Save ();


			Log ($"  Done  : {tag.Title}");

			// Now read it again
			file = TagLib.File.Create (tpath);
			tag = file.Tag;
			mtag = (TagLib.Matroska.Tag)file.GetTag (TagTypes.Matroska);

			Log ($"  Read  : {tag.Title}");

			Log ($"    Album              : {tag.Album}");
			Log ($"    JoinedAlbumArtists : {tag.JoinedAlbumArtists}");
			Log ($"    BeatsPerMinute     : {tag.BeatsPerMinute}");
			Log ($"    Comment            : {tag.Comment}");
			Log ($"    JoinedComposers    : {tag.JoinedComposers}");
			Log ($"    Conductor          : {tag.Conductor}");
			Log ($"    Copyright          : {tag.Copyright}");
			Log ($"    Disc               : {tag.Disc}");
			Log ($"    DiscCount          : {tag.DiscCount}");
			Log ($"    JoinedGenres       : {tag.JoinedGenres}");
			Log ($"    Grouping           : {tag.Grouping}");
			Log ($"    Lyrics             : {tag.Lyrics}");
			Log ($"    JoinedPerformers   : {tag.JoinedPerformers}");
			Log ($"    Title              : {tag.Title}");
			Log ($"    Track              : {tag.Track}");
			Log ($"    TrackCount         : {tag.TrackCount}");
			Log ($"    Year               : {tag.Year}");

			Log ($"    TitleSort          : {tag.TitleSort}");
			Log ($"    AlbumSort          : {tag.AlbumSort}");
			Log ($"    PerformersSort     : {tag.JoinedPerformersSort}");
			Log ($"    ComposersSort      : {string.Join ("; ", tag.ComposersSort)}");
			Log ($"    AlbumArtistsSort   : {string.Join ("; ", tag.AlbumArtistsSort)}");

			Log ($"    PerformersRole     : {string.Join ("; ", mtag.PerformersRole)}");

			Log ($"  Done  : {tag.Title}");
		}

		Log ("* End");
	}
}
