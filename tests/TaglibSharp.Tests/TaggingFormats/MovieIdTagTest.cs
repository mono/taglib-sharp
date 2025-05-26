using NUnit.Framework;
using TagLib;

namespace TaglibSharp.Tests.TaggingFormats
{
	[TestFixture]
	public class MovieIdTagTest
	{
		static readonly string val_sing =
			"01234567890123456789012345678901234567890123456789";

		static readonly string[] val_mult = {"A123456789",
			"B123456789", "C123456789", "D123456789", "E123456789"};

		static readonly string[] val_gnre = {"Rap",
			"Jazz", "Non-Genre", "Blues"};

		[Test]
		public void TestTitle ()
		{
			var tag = new TagLib.Riff.MovieIdTag ();

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				ClassicAssert.IsNull (t.Title, "Initial (Null): " + m);
			});

			tag.Title = val_sing;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				ClassicAssert.AreEqual (val_sing, t.Title, "Value Set (!Null): " + m);
			});

			tag.Title = string.Empty;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				ClassicAssert.IsNull (t.Title, "Value Cleared (Null): " + m);
			});

		}

		[Test]
		public void TestPerformers ()
		{
			var tag = new TagLib.Riff.MovieIdTag ();

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				ClassicAssert.AreEqual (0, t.Performers.Length, "Initial (Zero): " + m);
			});

			tag.Performers = val_mult;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				ClassicAssert.AreEqual (val_mult.Length, t.Performers.Length, "Value Set: " + m);
				for (int i = 0; i < val_mult.Length; i++) {
					ClassicAssert.AreEqual (val_mult[i], t.Performers[i], "Value Set: " + m);
				}
			});

			tag.Performers = new string[0];

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				ClassicAssert.AreEqual (0, t.Performers.Length, "Value Cleared (Zero): " + m);
			});
		}

		[Test]
		public void TestComment ()
		{
			var tag = new TagLib.Riff.MovieIdTag ();

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				ClassicAssert.IsNull (t.Comment, "Initial (Null): " + m);
			});

			tag.Comment = val_sing;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				ClassicAssert.AreEqual (val_sing, t.Comment, "Value Set (!Null): " + m);
			});

			tag.Comment = string.Empty;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				ClassicAssert.IsNull (t.Comment, "Value Cleared (Null): " + m);
			});
		}

		[Test]
		public void TestGenres ()
		{
			var tag = new TagLib.Riff.MovieIdTag ();

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				ClassicAssert.AreEqual (0, t.Genres.Length, "Initial (Zero): " + m);
			});

			tag.Genres = val_gnre;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				ClassicAssert.AreEqual (val_gnre.Length, t.Genres.Length, "Value Set: " + m);
				for (int i = 0; i < val_gnre.Length; i++) {
					ClassicAssert.AreEqual (val_gnre[i], t.Genres[i], "Value Set: " + m);
				}
			});

			tag.Genres = val_mult;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				ClassicAssert.AreEqual (val_mult.Length, t.Genres.Length, "Value Set: " + m);
				for (int i = 0; i < val_mult.Length; i++) {
					ClassicAssert.AreEqual (val_mult[i], t.Genres[i], "Value Set: " + m);
				}
			});

			tag.Genres = new string[0];

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				ClassicAssert.AreEqual (0, t.Genres.Length, "Value Cleared (Zero): " + m);
			});
		}

		[Test]
		public void TestTrack ()
		{
			var tag = new TagLib.Riff.MovieIdTag ();

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				ClassicAssert.AreEqual (0, tag.Track, "Initial (Zero): " + m);
			});

			tag.Track = 199;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				ClassicAssert.AreEqual (199, tag.Track, "Value Set: " + m);
			});

			tag.Track = 0;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				ClassicAssert.AreEqual (0, t.Track, "Value Cleared (Zero): " + m);
			});
		}

		[Test]
		public void TestTrackCount ()
		{
			var tag = new TagLib.Riff.MovieIdTag ();

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				ClassicAssert.AreEqual (0, tag.TrackCount, "Initial (Zero): " + m);
			});

			tag.TrackCount = 199;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				ClassicAssert.AreEqual (199, tag.TrackCount, "Value Set: " + m);
			});

			tag.TrackCount = 0;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.MovieIdTag t, string m) {
				ClassicAssert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				ClassicAssert.AreEqual (0, t.TrackCount, "Value Cleared (Zero): " + m);
			});
		}

		[Test]
		public void TestClear ()
		{
			var tag = new TagLib.Riff.MovieIdTag {
				Title = "A",
				Performers = new[] { "B" },
				AlbumArtists = new[] { "C" },
				Composers = new[] { "D" },
				Album = "E",
				Comment = "F",
				Genres = new[] { "Blues" },
				Year = 123,
				Track = 234,
				TrackCount = 234,
				Disc = 234,
				DiscCount = 234,
				Lyrics = "G",
				Grouping = "H",
				BeatsPerMinute = 234,
				Conductor = "I",
				Copyright = "J",
				Pictures = new[] { new Picture (TestPath.Covers + "sample_a.png") }
			};


			ClassicAssert.IsFalse (tag.IsEmpty, "Should be full.");
			tag.Clear ();

			ClassicAssert.IsNull (tag.Title, "Title");
			ClassicAssert.AreEqual (0, tag.Performers.Length, "Performers");
			ClassicAssert.AreEqual (0, tag.AlbumArtists.Length, "AlbumArtists");
			ClassicAssert.AreEqual (0, tag.Composers.Length, "Composers");
			ClassicAssert.IsNull (tag.Album, "Album");
			ClassicAssert.IsNull (tag.Comment, "Comment");
			ClassicAssert.AreEqual (0, tag.Genres.Length, "Genres");
			ClassicAssert.AreEqual (0, tag.Year, "Year");
			ClassicAssert.AreEqual (0, tag.Track, "Track");
			ClassicAssert.AreEqual (0, tag.TrackCount, "TrackCount");
			ClassicAssert.AreEqual (0, tag.Disc, "Disc");
			ClassicAssert.AreEqual (0, tag.DiscCount, "DiscCount");
			ClassicAssert.IsNull (tag.Lyrics, "Lyrics");
			ClassicAssert.IsNull (tag.Comment, "Comment");
			ClassicAssert.AreEqual (0, tag.BeatsPerMinute, "BeatsPerMinute");
			ClassicAssert.IsNull (tag.Conductor, "Conductor");
			ClassicAssert.IsNull (tag.Copyright, "Copyright");
			ClassicAssert.AreEqual (0, tag.Pictures.Length, "Pictures");
			ClassicAssert.IsTrue (tag.IsEmpty, "Should be empty.");
		}

		delegate void TagTestFunc (TagLib.Riff.MovieIdTag tag, string msg);

		void TagTestWithSave (ref TagLib.Riff.MovieIdTag tag, TagTestFunc testFunc)
		{
			testFunc (tag, "Before Save");
			//Extras.DumpHex (tag.Render ().Data);
			tag = new TagLib.Riff.MovieIdTag (tag.Render ());
			testFunc (tag, "After Save");
		}
	}
}
