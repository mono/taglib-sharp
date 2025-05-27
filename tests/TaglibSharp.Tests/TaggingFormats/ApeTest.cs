using Tag = TagLib.Ape.Tag;

namespace TaglibSharp.Tests.TaggingFormats
{
	[TestClass]
	public class ApeTest
	{
		static readonly string val_sing =
			"01234567890123456789012345678901234567890123456789";

		static readonly string[] val_mult = {"A123456789",
			"B123456789", "C123456789", "D123456789", "E123456789"};

		static readonly string[] val_gnre = { "Rap", "Jazz", "Non-Genre", "Blues" };

		[TestMethod]
		public void TestTitle ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Title, "Initial (Null): " + m);
			});

			tag.Title = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Title, "Value Set (!Null): " + m);
			});

			tag.Title = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Title, "Value Cleared (Null): " + m);
			});

		}

		[TestMethod]
		public void TestPerformers ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0, t.Performers.Length, "Initial (Zero): " + m);
			});

			tag.Performers = val_mult;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_mult.Length, t.Performers.Length, "Value Set: " + m);
				for (int i = 0; i < val_mult.Length; i++) {
					Assert.AreEqual (val_mult[i], t.Performers[i], "Value Set: " + m);
				}
			});

			tag.Performers = new string[0];

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0, t.Performers.Length, "Value Cleared (Zero): " + m);
			});
		}

		[TestMethod]
		public void TestAlbumArtists ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0, t.AlbumArtists.Length, "Initial (Zero): " + m);
			});

			tag.AlbumArtists = val_mult;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_mult.Length, t.AlbumArtists.Length, "Value Set: " + m);
				for (int i = 0; i < val_mult.Length; i++) {
					Assert.AreEqual (val_mult[i], t.AlbumArtists[i], "Value Set: " + m);
				}
			});

			tag.AlbumArtists = new string[0];

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0, t.AlbumArtists.Length, "Value Cleared (Zero): " + m);
			});
		}

		[TestMethod]
		public void TestComposers ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0, t.Composers.Length, "Initial (Zero): " + m);
			});

			tag.Composers = val_mult;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_mult.Length, t.Composers.Length, "Value Set: " + m);
				for (int i = 0; i < val_mult.Length; i++) {
					Assert.AreEqual (val_mult[i], t.Composers[i], "Value Set: " + m);
				}
			});

			tag.Composers = new string[0];

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0, t.Composers.Length, "Value Cleared (Zero): " + m);
			});
		}

		[TestMethod]
		public void TestAlbum ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Album, "Initial (Null): " + m);
			});

			tag.Album = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Album, "Value Set (!Null): " + m);
			});

			tag.Album = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Album, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestComment ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Comment, "Initial (Null): " + m);
			});

			tag.Comment = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Comment, "Value Set (!Null): " + m);
			});

			tag.Comment = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Comment, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestGenres ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0, t.Genres.Length, "Initial (Zero): " + m);
			});

			tag.Genres = val_gnre;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_gnre.Length, t.Genres.Length, "Value Set: " + m);
				for (int i = 0; i < val_gnre.Length; i++) {
					Assert.AreEqual (val_gnre[i], t.Genres[i], "Value Set: " + m);
				}
			});

			tag.Genres = val_mult;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_mult.Length, t.Genres.Length, "Value Set: " + m);
				for (int i = 0; i < val_mult.Length; i++) {
					Assert.AreEqual (val_mult[i], t.Genres[i], "Value Set: " + m);
				}
			});

			tag.Genres = new string[0];

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0, t.Genres.Length, "Value Cleared (Zero): " + m);
			});
		}

		[TestMethod]
		public void TestYear ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0u, tag.Year, "Initial (Zero): " + m);
			});

			tag.Year = 1999;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (1999u, tag.Year, "Value Set: " + m);
			});

			tag.Year = 0;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0u, t.Year, "Value Cleared (Zero): " + m);
			});
		}

		[TestMethod]
		public void TestTrack ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0u, tag.Track, "Initial (Zero): " + m);
			});

			tag.Track = 199;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (199u, tag.Track, "Value Set: " + m);
			});

			tag.Track = 0;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0u, t.Track, "Value Cleared (Zero): " + m);
			});
		}

		[TestMethod]
		public void TestTrackCount ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0u, tag.TrackCount, "Initial (Zero): " + m);
			});

			tag.TrackCount = 199;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (199u, tag.TrackCount, "Value Set: " + m);
			});

			tag.TrackCount = 0;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0u, t.TrackCount, "Value Cleared (Zero): " + m);
			});
		}

		[TestMethod]
		public void TestDisc ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0u, tag.Disc, "Initial (Zero): " + m);
			});

			tag.Disc = 199;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (199u, tag.Disc, "Value Set: " + m);
			});

			tag.Disc = 0;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0u, t.Disc, "Value Cleared (Zero): " + m);
			});
		}

		[TestMethod]
		public void TestDiscCount ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0u, tag.DiscCount, "Initial (Zero): " + m);
			});

			tag.DiscCount = 199;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (199u, tag.DiscCount, "Value Set: " + m);
			});

			tag.DiscCount = 0;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0u, t.DiscCount, "Value Cleared (Zero): " + m);
			});
		}

		[TestMethod]
		public void TestLyrics ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Lyrics, "Initial (Null): " + m);
			});

			tag.Lyrics = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Lyrics, "Value Set (!Null): " + m);
			});

			tag.Lyrics = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Lyrics, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestGrouping ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Grouping, "Initial (Null): " + m);
			});

			tag.Grouping = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Grouping, "Value Set (!Null): " + m);
			});

			tag.Grouping = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Grouping, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestBeatsPerMinute ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0u, tag.BeatsPerMinute, "Initial (Zero): " + m);
			});

			tag.BeatsPerMinute = 199;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (199u, tag.BeatsPerMinute, "Value Set: " + m);
			});

			tag.BeatsPerMinute = 0;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0u, t.BeatsPerMinute, "Value Cleared (Zero): " + m);
			});
		}

		[TestMethod]
		public void TestConductor ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Conductor, "Initial (Null): " + m);
			});

			tag.Conductor = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Conductor, "Value Set (!Null): " + m);
			});

			tag.Conductor = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Conductor, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestCopyright ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.Copyright, "Initial (Null): " + m);
			});

			tag.Copyright = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.Copyright, "Value Set (!Null): " + m);
			});

			tag.Copyright = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.Copyright, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestPictures ()
		{
			var tag = new Tag ();

			Picture[] pictures = {
				new Picture (TestPath.Covers + "sample_a.png"),
				new Picture (TestPath.Covers + "sample_a.jpg"),
				new Picture (TestPath.Covers + "sample_b.png"),
				new Picture (TestPath.Covers + "sample_b.jpg"),
				new Picture (TestPath.Covers + "sample_c.png"),
				new Picture (TestPath.Covers + "sample_c.jpg")
			};

			for (int i = 0; i < 6; i++)
				pictures[i].Type = (PictureType)(i * 2);

			pictures[3].Description = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.AreEqual (0, t.Pictures.Length, "Initial (Zero): " + m);
			});

			tag.Pictures = pictures;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (pictures.Length, t.Pictures.Length, "Value Set: " + m);
				for (int i = 0; i < pictures.Length; i++) {
					string msg = "Value " + i + "Set: " + m;
					Assert.AreEqual (pictures[i].Data, t.Pictures[i].Data, msg);
					Assert.AreEqual (pictures[i].Type, t.Pictures[i].Type, msg);
					Assert.AreEqual (pictures[i].Description, t.Pictures[i].Description, msg);
					Assert.AreEqual (pictures[i].MimeType, t.Pictures[i].MimeType, msg);
				}
			});

			tag.Pictures = new Picture[0];

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.AreEqual (0, t.Pictures.Length, "Value Cleared (Zero): " + m);
			});
		}

		[TestMethod]
		public void TestMusicBrainzArtistID ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzArtistId, "Initial (Null): " + m);
			});

			tag.MusicBrainzArtistId = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.MusicBrainzArtistId, "Value Set (!Null): " + m);
			});

			tag.MusicBrainzArtistId = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzArtistId, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestMusicBrainzReleaseGroupID ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzReleaseGroupId, "Initial (Null): " + m);
			});

			tag.MusicBrainzReleaseGroupId = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.MusicBrainzReleaseGroupId, "Value Set (!Null): " + m);
			});

			tag.MusicBrainzReleaseGroupId = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzReleaseGroupId, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestMusicBrainzReleaseID ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzReleaseId, "Initial (Null): " + m);
			});

			tag.MusicBrainzReleaseId = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.MusicBrainzReleaseId, "Value Set (!Null): " + m);
			});

			tag.MusicBrainzReleaseId = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzReleaseId, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestMusicBrainzReleaseArtistID ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzReleaseArtistId, "Initial (Null): " + m);
			});

			tag.MusicBrainzReleaseArtistId = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.MusicBrainzReleaseArtistId, "Value Set (!Null): " + m);
			});

			tag.MusicBrainzReleaseArtistId = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzReleaseArtistId, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestMusicBrainzTrackID ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzTrackId, "Initial (Null): " + m);
			});

			tag.MusicBrainzTrackId = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.MusicBrainzTrackId, "Value Set (!Null): " + m);
			});

			tag.MusicBrainzTrackId = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzTrackId, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestMusicBrainzRecordingID ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzRecordingId, "Initial (Null): " + m);
			});

			tag.MusicBrainzRecordingId = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.MusicBrainzRecordingId, "Value Set (!Null): " + m);
			});

			tag.MusicBrainzRecordingId = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzRecordingId, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestMusicBrainzWorkID ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzWorkId, "Initial (Null): " + m);
			});

			tag.MusicBrainzWorkId = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.MusicBrainzWorkId, "Value Set (!Null): " + m);
			});

			tag.MusicBrainzWorkId = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzWorkId, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestMusicBrainzDiscID ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzDiscId, "Initial (Null): " + m);
			});

			tag.MusicBrainzDiscId = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.MusicBrainzDiscId, "Value Set (!Null): " + m);
			});

			tag.MusicBrainzDiscId = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzDiscId, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestMusicIPPUID ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.MusicIpId, "Initial (Null): " + m);
			});

			tag.MusicIpId = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.MusicIpId, "Value Set (!Null): " + m);
			});

			tag.MusicIpId = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.MusicIpId, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestAmazonID ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.AmazonId, "Initial (Null): " + m);
			});

			tag.AmazonId = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.AmazonId, "Value Set (!Null): " + m);
			});

			tag.AmazonId = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.AmazonId, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestMusicBrainzReleaseStatus ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzReleaseStatus, "Initial (Null): " + m);
			});

			tag.MusicBrainzReleaseStatus = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.MusicBrainzReleaseStatus, "Value Set (!Null): " + m);
			});

			tag.MusicBrainzReleaseStatus = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzReleaseStatus, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestMusicBrainzReleaseType ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzReleaseType, "Initial (Null): " + m);
			});

			tag.MusicBrainzReleaseType = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.MusicBrainzReleaseType, "Value Set (!Null): " + m);
			});

			tag.MusicBrainzReleaseType = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzReleaseType, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestMusicBrainzReleaseCountry ()
		{
			Tag tag = new Tag ();

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzReleaseCountry, "Initial (Null): " + m);
			});

			tag.MusicBrainzReleaseCountry = val_sing;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
				Assert.AreEqual (val_sing, t.MusicBrainzReleaseCountry, "Value Set (!Null): " + m);
			});

			tag.MusicBrainzReleaseCountry = string.Empty;

			TagTestWithSave (ref tag, delegate (Tag t, string m) {
				Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
				Assert.IsNull (t.MusicBrainzReleaseCountry, "Value Cleared (Null): " + m);
			});
		}

		[TestMethod]
		public void TestClear ()
		{
			Tag tag = new Tag {
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

		delegate void TagTestFunc (Tag tag, string msg);

		void TagTestWithSave (ref Tag tag, TagTestFunc testFunc)
		{
			testFunc (tag, "Before Save");
			tag = new Tag (tag.Render ());
			testFunc (tag, "After Save");
		}
	}
}
