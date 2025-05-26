using NUnit.Framework;
using System;
using TagLib;

using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats
{
	[TestFixture]
	public class MkvFormatTest : IFormatTest
	{
		static readonly string sample_file = TestPath.Samples + "Turning Lime.mkv";
		static readonly string sample_picture = TestPath.Samples + "sample_gimp.gif";
		static readonly string sample_other = TestPath.Samples + "apple_tags.m4a";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite.mkv";
		File file;


		[OneTimeSetUp]
		public void Init ()
		{
			file = File.Create (sample_file);
		}


		[Test]
		public void ReadAudioProperties ()
		{
			ClassicAssert.AreEqual (48000, file.Properties.AudioSampleRate);
			ClassicAssert.AreEqual (1120, file.Properties.Duration.TotalMilliseconds);
		}


		[Test]
		public void ReadTags ()
		{
			ClassicAssert.AreEqual ("Lime", file.Tag.FirstPerformer);
			ClassicAssert.AreEqual ("no comments", file.Tag.Comment);
			ClassicAssert.AreEqual ("Test", file.Tag.FirstGenre);
			ClassicAssert.AreEqual ("Turning Lime", file.Tag.Title);
			ClassicAssert.AreEqual (2017, file.Tag.Year);
			ClassicAssert.AreEqual ("Starwer", file.Tag.FirstComposer);
			ClassicAssert.AreEqual ("Starwer", file.Tag.Conductor);
			ClassicAssert.AreEqual ("Starwer 2017", file.Tag.Copyright);
		}


		[Test]
		public void ReadSpecificTags ()
		{
			// Specific Matroska Tag test
			var mkvTag = (TagLib.Matroska.Tag)file.GetTag (TagTypes.Matroska);
			ClassicAssert.AreEqual ("This is a test Video showing a lime moving on a table", mkvTag.SimpleTags["SUMMARY"][0]);

			var tracks = mkvTag.Tags.Tracks;
			ClassicAssert.AreEqual (MediaTypes.Video, tracks[0].MediaTypes);
			ClassicAssert.AreEqual (MediaTypes.Audio, tracks[1].MediaTypes);

			var videotag = mkvTag.Tags.Get (tracks[0]);
			ClassicAssert.IsNull (videotag);

			var audiotag = mkvTag.Tags.Get (tracks[1]);

			ClassicAssert.AreEqual ("The Noise", audiotag.Title);
			ClassicAssert.AreEqual ("Useless background noise", audiotag.SimpleTags["DESCRIPTION"][0]);
			ClassicAssert.AreEqual ("und", audiotag.SimpleTags["DESCRIPTION"][0].TagLanguage);
			ClassicAssert.AreEqual (true, audiotag.SimpleTags["DESCRIPTION"][0].TagDefault);

			// Recursive read
			ClassicAssert.AreEqual ("Starwer", audiotag.FirstComposer);
			ClassicAssert.AreEqual ("Starwer 2017", audiotag.Copyright);
		}


		[Test]
		public void ReadPictures ()
		{
			var pics = file.Tag.Pictures;
			ClassicAssert.AreEqual ("cover.png", pics[0].Description);
			ClassicAssert.AreEqual (PictureType.FrontCover, pics[0].Type);
			ClassicAssert.AreEqual ("image/png", pics[0].MimeType);
			ClassicAssert.AreEqual (17307, pics[0].Data.Count);
		}


		[Test]
		public void WritePictures ()
		{
			if (System.IO.File.Exists (tmp_file))
				System.IO.File.Delete (tmp_file);

			File file;
			try {
				System.IO.File.Copy (sample_file, tmp_file);
				file = File.Create (tmp_file);
			} finally { }

			ClassicAssert.NotNull (file);

			var pics = file.Tag.Pictures;
			ClassicAssert.AreEqual (1, pics.Length);

			// Insert new picture
			Array.Resize (ref pics, 4);
			pics[0].Description = "TEST description 0";
			pics[1] = new Picture (sample_picture) {
				Type = PictureType.BackCover,
				Description = "TEST description 1"
			};
			pics[2] = new Picture (sample_other) {
				Description = "TEST description 2"
			};
			pics[3] = new Picture (sample_picture) {
				Type = PictureType.Other,
				Description = "TEST description 3"
			};
			file.Tag.Pictures = pics;

			file.Save ();

			// Read back the Matroska-specific tags
			file = File.Create (tmp_file);
			ClassicAssert.NotNull (file);
			pics = file.Tag.Pictures;

			ClassicAssert.AreEqual (4, pics.Length);

			ClassicAssert.AreEqual ("cover.png", pics[0].Filename);
			ClassicAssert.AreEqual ("TEST description 0", pics[0].Description);
			ClassicAssert.AreEqual ("image/png", pics[0].MimeType);
			ClassicAssert.AreEqual (PictureType.FrontCover, pics[0].Type);
			ClassicAssert.AreEqual (17307, pics[0].Data.Count);

			// Filename has been changed to keep the PictureType information
			ClassicAssert.AreEqual (PictureType.BackCover, pics[1].Type);
			ClassicAssert.AreEqual ("BackCover.gif", pics[1].Filename);
			ClassicAssert.AreEqual ("TEST description 1", pics[1].Description);
			ClassicAssert.AreEqual ("image/gif", pics[1].MimeType);
			ClassicAssert.AreEqual (73, pics[1].Data.Count);

			ClassicAssert.AreEqual ("apple_tags.m4a", pics[2].Filename);
			ClassicAssert.AreEqual ("TEST description 2", pics[2].Description);
			ClassicAssert.AreEqual ("audio/mp4", pics[2].MimeType);
			ClassicAssert.AreEqual (PictureType.NotAPicture, pics[2].Type);
			ClassicAssert.AreEqual (102400, pics[2].Data.Count);

			ClassicAssert.AreEqual (PictureType.Other, pics[3].Type);
			ClassicAssert.AreEqual ("sample_gimp.gif", pics[3].Filename);
			ClassicAssert.AreEqual ("TEST description 3", pics[3].Description);
			ClassicAssert.AreEqual ("image/gif", pics[3].MimeType);
			ClassicAssert.AreEqual (73, pics[3].Data.Count);
		}


		[Test]
		public void WriteStandardTags ()
		{
			StandardTests.WriteStandardTags (sample_file, tmp_file, StandardTests.TestTagLevel.Medium);
		}


		[Test]
		public void WriteStandardPictures ()
		{
			StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.None, StandardTests.TestTagLevel.High);
		}

		[Test]
		public void WriteStandardPicturesLazy ()
		{
			StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy, StandardTests.TestTagLevel.High);
		}

		/// <summary>
		/// Use advanced Matroska-specific features.
		/// Matroska Tag Documentation: <see cref="https://www.matroska.org/technical/specs/tagging/index.html"/>.
		/// </summary>
		[Test]
		public void WriteSpecificTags ()
		{
			if (System.IO.File.Exists (tmp_file))
				System.IO.File.Delete (tmp_file);

			File file;
			System.IO.File.Copy (sample_file, tmp_file);
			file = File.Create (tmp_file);
			ClassicAssert.NotNull (file);

			// Write Matroska-specific tags
			var mtag = (TagLib.Matroska.Tag)file.GetTag (TagTypes.Matroska);
			ClassicAssert.NotNull (mtag);

			mtag.PerformersRole = new[] { "TEST role 1", "TEST role 2" };
			mtag.Set ("CHOREGRAPHER", null, "TEST choregrapher");

			// Retrieve Matroska 'Tags' structure
			var mtags = mtag.Tags;

			// Add a new Matroska 'Tag' structure, representing a collection, in the 'Tags' structure
			var collectag = new TagLib.Matroska.Tag (mtags, TagLib.Matroska.TargetType.COLLECTION);

			// Add a Matroska 'SimpleTag' (TagName: 'ARRANGER') in the 'Tag' structure
			collectag.Set ("ARRANGER", null, "TEST arranger");

			// Add a Matroska 'SimpleTag' (TagName: 'TITLE') in the 'Tag' structure
			collectag.Set ("TITLE", null, "TEST Album title"); // This should map to the standard TagLib Album tag

			// Get tracks
			var tracks = mtag.Tags.Tracks;

			// Create video tags
			var videotag = new TagLib.Matroska.Tag (mtag.Tags, TagLib.Matroska.TargetType.CHAPTER, tracks[0]) {
				Title = "The Video test"
			};
			videotag.Set ("DESCRIPTION", null, "Video track Tag test");
			videotag.SimpleTags["DESCRIPTION"][0].TagLanguage = "en";
			videotag.SimpleTags["DESCRIPTION"][0].TagDefault = false;

			// Add another description in another language (and check encoding correctness at the same time)
			videotag.SimpleTags["DESCRIPTION"].Add (new TagLib.Matroska.SimpleTag ("Test de piste vidéo"));
			videotag.SimpleTags["DESCRIPTION"][1].TagLanguage = "fr";

			// Remove Audio tags
			var audiotag = mtag.Tags.Get (tracks[1]);
			ClassicAssert.NotNull (audiotag);
			audiotag.Clear ();

			// Eventually save the changes
			file.Save ();

			// Read back the Matroska-specific tags
			file = File.Create (tmp_file);
			ClassicAssert.NotNull (mtag);

			mtag = (TagLib.Matroska.Tag)file.GetTag (TagTypes.Matroska);
			ClassicAssert.NotNull (mtag);

			ClassicAssert.AreEqual ("TEST role 1; TEST role 2", string.Join ("; ", mtag.PerformersRole));
			ClassicAssert.AreEqual ("TEST choregrapher", mtag.Get ("CHOREGRAPHER", null)[0]);
			ClassicAssert.AreEqual ("TEST arranger", mtags.Album.Get ("ARRANGER", null)[0]);
			ClassicAssert.AreEqual ("TEST Album title", mtag.Album);

			// Get tracks
			tracks = mtag.Tags.Tracks;
			ClassicAssert.NotNull (tracks);

			// Test Video Track Tag
			videotag = mtag.Tags.Get (tracks[0]);
			ClassicAssert.NotNull (videotag);
			ClassicAssert.AreEqual (TagLib.Matroska.TargetType.CHAPTER, videotag.TargetType);
			ClassicAssert.AreEqual (30, videotag.TargetTypeValue);
			ClassicAssert.AreEqual ("The Video test", videotag.Title);
			ClassicAssert.AreEqual ("Video track Tag test", videotag.SimpleTags["DESCRIPTION"][0]);
			ClassicAssert.AreEqual ("en", videotag.SimpleTags["DESCRIPTION"][0].TagLanguage);
			ClassicAssert.AreEqual (false, videotag.SimpleTags["DESCRIPTION"][0].TagDefault);

			// implicit or explicit conversion from TagLib.Matroska.SimpleTag to string is required to ensure a proper encoding
			ClassicAssert.AreEqual ("Test de piste vidéo", videotag.SimpleTags["DESCRIPTION"][1].ToString ());
			ClassicAssert.AreEqual ("fr", videotag.SimpleTags["DESCRIPTION"][1].TagLanguage);
			ClassicAssert.AreEqual (true, videotag.SimpleTags["DESCRIPTION"][1].TagDefault);

			// Test Audio Track Tag
			audiotag = mtag.Tags.Get (tracks[1]);
			ClassicAssert.IsNull (audiotag);

			ClassicAssert.AreEqual ("TEST Album title", mtag.Album);
		}

		[Test]
		public void RemoveStandardTags ()
		{
			StandardTests.RemoveStandardTags (sample_file, tmp_file);
		}

		[Test]
		public void TestCorruptionResistance ()
		{
			StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.mkv");
		}
	}
}
