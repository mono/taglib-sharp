using NUnit.Framework;
using System;
using TagLib;

using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats
{
	public static class StandardTests
	{
		static readonly string sample_picture = TestPath.Samples + "sample_gimp.gif";
		static readonly string sample_other = TestPath.Samples + "apple_tags.m4a";

		public enum TestTagLevel
		{
			Normal,
			Medium,
			High
		}

		public static void ReadAudioProperties (File file)
		{
			ClassicAssert.AreEqual (44100, file.Properties.AudioSampleRate);
			ClassicAssert.AreEqual (5, file.Properties.Duration.Seconds);
		}

		public static void WriteStandardTags (string sample_file, string tmp_file,
			TestTagLevel level = TestTagLevel.Normal, TagTypes types = TagTypes.AllTags)
		{

			if (sample_file != tmp_file &&
				System.IO.File.Exists (tmp_file))
				System.IO.File.Delete (tmp_file);

			if (sample_file != tmp_file)
				System.IO.File.Copy (sample_file, tmp_file);

			var tmp = File.Create (tmp_file);

			if (types != TagTypes.AllTags) {
				tmp.RemoveTags (~types);
			}

			SetTags (tmp.Tag, level);
			tmp.Save ();

			tmp = File.Create (tmp_file);
			CheckTags (tmp.Tag, level);
		}

		public static void WriteStandardPictures (string sample_file, string tmp_file,
												 ReadStyle readStyle = ReadStyle.Average,
												 TestTagLevel level = TestTagLevel.Medium)
		{
			if (System.IO.File.Exists (tmp_file))
				System.IO.File.Delete (tmp_file);

			System.IO.File.Copy (sample_file, tmp_file);
			var file = File.Create (tmp_file, readStyle);
			ClassicAssert.NotNull (file);

			var pics = file.Tag.Pictures;

			// Raw Picture data references
			var raws = new ByteVector[3];

			// Insert new picture
			Array.Resize (ref pics, 3);
			raws[0] = ByteVector.FromPath (sample_picture);
			pics[0] = new Picture (sample_picture) {
				Type = PictureType.BackCover,
				Description = "TEST description 1"
			};

			raws[1] = ByteVector.FromPath (sample_other);
			pics[1] = new Picture (sample_other) {
				Description = "TEST description 2"
			};

			raws[2] = raws[0];
			pics[2] = new Picture (sample_picture) {
				Filename = "renamed.gif",
				Type = PictureType.Other,
				Description = "TEST description 3"
			};
			file.Tag.Pictures = pics;

			file.Save ();

			// Read back the tags
			file = File.Create (tmp_file, readStyle);
			ClassicAssert.NotNull (file);
			pics = file.Tag.Pictures;

			ClassicAssert.AreEqual (3, pics.Length);

			// Lazy picture check
			var isLazy = (readStyle & ReadStyle.PictureLazy) != 0;
			for (int i = 0; i < 3; i++) {
				if (isLazy) {
					ClassicAssert.IsTrue (pics[i] is ILazy);
					if (pics[i] is ILazy lazy) {
						ClassicAssert.IsFalse (lazy.IsLoaded);
					}
				} else {
					if (pics[i] is ILazy lazy) {
						ClassicAssert.IsTrue (lazy.IsLoaded);
					}
				}
			}

			ClassicAssert.AreEqual ("TEST description 1", pics[0].Description);
			ClassicAssert.AreEqual ("image/gif", pics[0].MimeType);
			ClassicAssert.AreEqual (73, pics[0].Data.Count);
			ClassicAssert.AreEqual (raws[0], pics[0].Data);

			ClassicAssert.AreEqual ("TEST description 2", pics[1].Description);
			ClassicAssert.AreEqual (102400, pics[1].Data.Count);
			ClassicAssert.AreEqual (raws[1], pics[1].Data);

			ClassicAssert.AreEqual ("TEST description 3", pics[2].Description);
			ClassicAssert.AreEqual ("image/gif", pics[2].MimeType);
			ClassicAssert.AreEqual (73, pics[2].Data.Count);
			ClassicAssert.AreEqual (raws[2], pics[2].Data);

			// Types and Mime-Types assumed to be properly supported at Medium level test
			if (level >= TestTagLevel.Medium) {
				ClassicAssert.AreEqual ("audio/mp4", pics[1].MimeType);
				ClassicAssert.AreEqual (PictureType.BackCover, pics[0].Type);
				ClassicAssert.AreEqual (PictureType.NotAPicture, pics[1].Type);
				ClassicAssert.AreEqual (PictureType.Other, pics[2].Type);
			} else {
				ClassicAssert.AreNotEqual (PictureType.NotAPicture, pics[0].Type);
				ClassicAssert.AreEqual (PictureType.NotAPicture, pics[1].Type);
				ClassicAssert.AreNotEqual (PictureType.NotAPicture, pics[2].Type);
			}

			// Filename assumed to be properly supported at High level test
			if (level >= TestTagLevel.High) {
				ClassicAssert.AreEqual ("apple_tags.m4a", pics[1].Filename);
			} else if (level >= TestTagLevel.Medium) {
				if (pics[1].Filename != null)
					ClassicAssert.AreEqual ("apple_tags.m4a", pics[1].Filename);
			}
		}

		public static void RemoveStandardTags (string sample_file, string tmp_file, TagTypes types = TagTypes.AllTags)
		{
			if (System.IO.File.Exists (tmp_file))
				System.IO.File.Delete (tmp_file);

			System.IO.File.Copy (sample_file, tmp_file);

			var tmp = File.Create (tmp_file);
			tmp.RemoveTags (types);
			tmp.Save ();

			// Check only if all tags have been removed
			if (types == TagTypes.AllTags) {
				tmp = File.Create (tmp_file);
				CheckNoTags (tmp.Tag);
			}
		}

		public static void SetTags (Tag tag, TestTagLevel level = TestTagLevel.Normal)
		{
			if (level >= TestTagLevel.Medium) {
				tag.TitleSort = "title sort, TEST";
				tag.AlbumSort = "album sort, TEST";
				tag.PerformersSort = new[] { "performer sort 1, TEST", "performer sort 2, TEST" };
				tag.ComposersSort = new[] { "composer sort 1, TEST", "composer sort 2, TEST" };
				tag.AlbumArtistsSort = new[] { "album artist sort 1, TEST", "album artist sort 2, TEST" };
			}

			tag.Album = "TEST album";
			tag.AlbumArtists = new[] { "TEST artist 1", "TEST artist 2" };
			tag.BeatsPerMinute = 120;
			tag.Comment = "TEST comment";
			tag.Composers = new[] { "TEST composer 1", "TEST composer 2" };
			tag.Conductor = "TEST conductor";
			tag.Copyright = "TEST copyright";
			tag.DateTagged = new DateTime (2017, 09, 12, 22, 47, 42);
			tag.Disc = 100;
			tag.DiscCount = 101;
			tag.Genres = new[] { "TEST genre 1", "TEST genre 2" };
			tag.Grouping = "TEST grouping";
			tag.Lyrics = "TEST lyrics 1\r\nTEST lyrics 2";
			tag.Performers = new[] { "TEST performer 1", "TEST performer 2" };
			tag.PerformersRole = new[] { "TEST role 1a; TEST role 1b", "TEST role 2" };
			tag.Title = "TEST title";
			tag.Subtitle = "TEST subtitle";
			tag.Description = "TEST description";
			tag.Track = 98;
			tag.TrackCount = 99;
			tag.Year = 1999;
		}

		public static void CheckTags (Tag tag, TestTagLevel level = TestTagLevel.Normal)
		{
			ClassicAssert.AreEqual ("TEST album", tag.Album);
			ClassicAssert.AreEqual ("TEST artist 1; TEST artist 2", tag.JoinedAlbumArtists);
			ClassicAssert.AreEqual ("TEST comment", tag.Comment);
			ClassicAssert.AreEqual ("TEST composer 1; TEST composer 2", tag.JoinedComposers);
			ClassicAssert.AreEqual ("TEST conductor", tag.Conductor);
			ClassicAssert.AreEqual ("TEST copyright", tag.Copyright);
			ClassicAssert.AreEqual (100, tag.Disc);
			ClassicAssert.AreEqual (101, tag.DiscCount);
			ClassicAssert.AreEqual ("TEST genre 1; TEST genre 2", tag.JoinedGenres);
			ClassicAssert.AreEqual ("TEST grouping", tag.Grouping);
			ClassicAssert.AreEqual ("TEST lyrics 1\r\nTEST lyrics 2", tag.Lyrics);
			ClassicAssert.AreEqual ("TEST performer 1; TEST performer 2", tag.JoinedPerformers);
			ClassicAssert.AreEqual ("TEST title", tag.Title);
			ClassicAssert.AreEqual ("TEST subtitle", tag.Subtitle);
			ClassicAssert.AreEqual ("TEST description", tag.Description);
			ClassicAssert.AreEqual (98, tag.Track);
			ClassicAssert.AreEqual (99, tag.TrackCount);
			ClassicAssert.AreEqual (1999, tag.Year);

			if (level >= TestTagLevel.Medium) {
				ClassicAssert.AreEqual ("title sort, TEST", tag.TitleSort);
				ClassicAssert.AreEqual ("album sort, TEST", tag.AlbumSort);
				ClassicAssert.AreEqual ("performer sort 1, TEST; performer sort 2, TEST", tag.JoinedPerformersSort);
				ClassicAssert.AreEqual ("composer sort 1, TEST; composer sort 2, TEST", string.Join ("; ", tag.ComposersSort));
				ClassicAssert.AreEqual ("album artist sort 1, TEST; album artist sort 2, TEST", string.Join ("; ", tag.AlbumArtistsSort));
				ClassicAssert.AreEqual (120, tag.BeatsPerMinute);
				ClassicAssert.AreEqual (new DateTime (2017, 09, 12, 22, 47, 42), tag.DateTagged);
				ClassicAssert.AreEqual ("TEST role 1a; TEST role 1b\nTEST role 2", string.Join ("\n", tag.PerformersRole));
			}
		}

		public static void CheckNoTags (Tag tag)
		{
			ClassicAssert.IsNull (tag.Album);
			ClassicAssert.IsNull (tag.JoinedAlbumArtists);
			ClassicAssert.IsNull (tag.Comment);
			ClassicAssert.IsNull (tag.Conductor);
			ClassicAssert.IsNull (tag.Copyright);
			ClassicAssert.IsNull (tag.Grouping);
			ClassicAssert.IsNull (tag.Lyrics);

			ClassicAssert.AreEqual (0, tag.BeatsPerMinute);
			ClassicAssert.AreEqual (0, tag.Disc);
			ClassicAssert.AreEqual (0, tag.DiscCount);
			ClassicAssert.AreEqual (0, tag.Track);
			ClassicAssert.AreEqual (0, tag.TrackCount);
			ClassicAssert.AreEqual (0, tag.Year);

			ClassicAssert.IsTrue (string.IsNullOrEmpty (tag.JoinedComposers));
			ClassicAssert.IsTrue (string.IsNullOrEmpty (tag.JoinedGenres));
			ClassicAssert.IsTrue (string.IsNullOrEmpty (tag.JoinedPerformers));

			ClassicAssert.IsNull (tag.Title);
			ClassicAssert.IsNull (tag.Description);
			ClassicAssert.IsNull (tag.DateTagged);
			ClassicAssert.IsTrue (tag.Performers == null || tag.Performers.Length == 0);
			ClassicAssert.IsTrue (tag.PerformersSort == null || tag.PerformersSort.Length == 0);
			ClassicAssert.IsTrue (tag.PerformersRole == null || tag.PerformersRole.Length == 0);
			ClassicAssert.IsTrue (tag.AlbumArtistsSort == null || tag.AlbumArtistsSort.Length == 0);
			ClassicAssert.IsTrue (tag.AlbumArtists == null || tag.AlbumArtists.Length == 0);
			ClassicAssert.IsTrue (tag.Composers == null || tag.Composers.Length == 0);
			ClassicAssert.IsTrue (tag.ComposersSort == null || tag.ComposersSort.Length == 0);

			ClassicAssert.IsTrue (tag.IsEmpty);
		}


		public static void TestCorruptionResistance (string path)
		{
			try {
				File.Create (path);
			} catch (CorruptFileException) {
			} catch (NullReferenceException) {
				throw;
			} catch {
			}
		}
	}
}
