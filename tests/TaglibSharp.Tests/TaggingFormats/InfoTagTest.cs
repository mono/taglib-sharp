using TagLib.Riff;

namespace TaglibSharp.Tests.TaggingFormats
{
	// NOTE: If StringType == UTF8 (default) then don't set it during the test.
	//       We want to make sure that default still works as expected
	[TestClass]
	public class InfoTagTest
	{
		static readonly string val_sing =
			"01234567890123456789012345678901234567890123456789";

		static readonly string[] val_mult = {"A123456789",
			"B123456789", "C123456789", "D123456789", "E123456789"};

		static readonly string[] val_gnre = {"Rap",
			"Jazz", "Non-Genre", "Blues"};

		[TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestTitle (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Title, "Initial (Null): " + m);
			});

			tag.Title = val_sing;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Title, "Value Set (!Null): " + m);
			});

			tag.Title = string.Empty;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Title, "Value Cleared (Null): " + m);
			});
		}

        [TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestDescription (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Description, "Initial (Null): " + m);
			});

			tag.Description = val_sing;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Description, "Value Set (!Null): " + m);
			});

			tag.Description = string.Empty;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Description, "Value Cleared (Null): " + m);
			});
		}

        [TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestAlbum (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Album, "Initial (Null): " + m);
			});

			tag.Album = val_sing;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Album, "Value Set (!Null): " + m);
			});

			tag.Album = string.Empty;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Album, "Value Cleared (Null): " + m);
			});
		}

        [TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestConductor (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Conductor, "Initial (Null): " + m);
			});

			tag.Conductor = val_sing;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Conductor, "Value Set (!Null): " + m);
			});

			tag.Conductor = string.Empty;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Conductor, "Value Cleared (Null): " + m);
			});
		}

        [TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestPerformers (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0, t.Performers.Length, "Initial (Zero): " + m);
			});

			tag.Performers = val_mult;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_mult.Length, t.Performers.Length, "Value Set: " + m);
				for (int i = 0; i < val_mult.Length; i++) {
					Assert.AreEqual (val_mult[i], t.Performers[i], "Value Set: " + m);
				}
			});

			tag.Performers = new string[0];

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0, t.Performers.Length, "Value Cleared (Zero): " + m);
			});
		}

        [TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestAlbumArtists (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0, t.AlbumArtists.Length, "Initial (Zero): " + m);
			});

			tag.AlbumArtists = val_mult;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_mult.Length, t.AlbumArtists.Length, "Value Set: " + m);
				for (int i = 0; i < val_mult.Length; i++) {
					Assert.AreEqual (val_mult[i], t.AlbumArtists[i], "Value Set: " + m);
				}
			});

			tag.AlbumArtists = new string[0];

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0, t.AlbumArtists.Length, "Value Cleared (Zero): " + m);
			});
		}

        [TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestComposers (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0, t.Composers.Length, "Initial (Zero): " + m);
			});

			tag.Composers = val_mult;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_mult.Length, t.Composers.Length, "Value Set: " + m);
				for (int i = 0; i < val_mult.Length; i++) {
					Assert.AreEqual (val_mult[i], t.Composers[i], "Value Set: " + m);
				}
			});

			tag.Composers = new string[0];

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0, t.Composers.Length, "Value Cleared (Zero): " + m);
			});
		}

        [TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestComments (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Comment, "Initial (Null): " + m);
			});

			tag.Comment = val_sing;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Comment, "Value Set (!Null): " + m);
			});

			tag.Comment = string.Empty;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Comment, "Value Cleared (Null): " + m);
			});
		}

        [TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestGenres (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0, t.Genres.Length, "Initial (Zero): " + m);
			});

			tag.Genres = val_gnre;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_gnre.Length, t.Genres.Length, "Value Set: " + m);
				for (int i = 0; i < val_gnre.Length; i++) {
					Assert.AreEqual (val_gnre[i], t.Genres[i], "Value Set: " + m);
				}
			});

			tag.Genres = val_mult;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_mult.Length, t.Genres.Length, "Value Set: " + m);
				for (int i = 0; i < val_mult.Length; i++) {
					Assert.AreEqual (val_mult[i], t.Genres[i], "Value Set: " + m);
				}
			});

			tag.Genres = new string[0];

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0, t.Genres.Length, "Value Cleared (Zero): " + m);
			});
		}

        [TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestYear (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0u, tag.Year, "Initial (Zero): " + m);
			});

			tag.Year = 1999;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (1999u, tag.Year, "Value Set: " + m);
			});

			tag.Year = 0;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0u, t.Year, "Value Cleared (Zero): " + m);
			});
		}

        [TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestTrack (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0u, tag.Track, "Initial (Zero): " + m);
			});

			tag.Track = 199;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (199u, tag.Track, "Value Set: " + m);
			});

			tag.Track = 0;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0u, t.Track, "Value Cleared (Zero): " + m);
			});
		}

        [TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestTrackCount (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0u, tag.TrackCount, "Initial (Zero): " + m);
			});

			tag.TrackCount = 199;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (199u, tag.TrackCount, "Value Set: " + m);
			});

			tag.TrackCount = 0;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0u, t.TrackCount, "Value Cleared (Zero): " + m);
			});
		}

        [TestMethod]
        [DataRow (StringType.Latin1)]
		[DataRow (StringType.UTF8)]
		public void TestCopyright (StringType stringType)
		{
			InfoTag tag;
			if (stringType == StringType.UTF8)
				tag = new TagLib.Riff.InfoTag ();
			else
				tag = new TagLib.Riff.InfoTag { StringType = stringType };

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Copyright, "Initial (Null): " + m);
			});

			tag.Copyright = val_sing;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Copyright, "Value Set (!Null): " + m);
			});

			tag.Copyright = string.Empty;

			TagTestWithSave (ref tag, delegate (TagLib.Riff.InfoTag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Copyright, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestClear ()
		{
			var tag = new TagLib.Riff.InfoTag {
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


			Assert.IsFalse (tag.IsEmpty, "Should be full.");
			tag.Clear ();

			Assert.IsNull (tag.Title, "Title");
			Assert.AreEqual (0, tag.Performers.Length, "Performers");
			Assert.AreEqual (0, tag.AlbumArtists.Length, "AlbumArtists");
			Assert.AreEqual (0, tag.Composers.Length, "Composers");
			Assert.IsNull (tag.Album, "Album");
			Assert.IsNull (tag.Comment, "Comment");
			Assert.AreEqual (0, tag.Genres.Length, "Genres");
			Assert.AreEqual (0u, tag.Year, "Year");
			Assert.AreEqual (0u, tag.Track, "Track");
			Assert.AreEqual (0u, tag.TrackCount, "TrackCount");
			Assert.AreEqual (0u, tag.Disc, "Disc");
			Assert.AreEqual (0u, tag.DiscCount, "DiscCount");
			Assert.IsNull (tag.Lyrics, "Lyrics");
			Assert.IsNull (tag.Comment, "Comment");
			Assert.AreEqual (0u, tag.BeatsPerMinute, "BeatsPerMinute");
			Assert.IsNull (tag.Conductor, "Conductor");
			Assert.IsNull (tag.Copyright, "Copyright");
			Assert.AreEqual (0, tag.Pictures.Length, "Pictures");
			Assert.IsTrue (tag.IsEmpty, "Should be empty.");
		}

		delegate void TagTestFunc (TagLib.Riff.InfoTag tag, string msg);

		void TagTestWithSave (ref TagLib.Riff.InfoTag tag, TagTestFunc testFunc)
		{
			testFunc (tag, "Before Save");
			//Extras.DumpHex (tag.Render ().Data);
			tag = new TagLib.Riff.InfoTag (tag.Render ());
			testFunc (tag, "After Save");
		}
	}
}
