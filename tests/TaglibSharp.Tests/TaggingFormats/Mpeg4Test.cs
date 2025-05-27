
namespace TaglibSharp.Tests.TaggingFormats;

[TestClass]
public class Mpeg4Test
{
	static readonly string val_sing =
		"01234567890123456789012345678901234567890123456789";

	static readonly string[] val_mult = {"A123456789",
		"B123456789", "C123456789", "D123456789", "E123456789"};

	static readonly string[] val_gnre = {"Rap",
		"Jazz", "Non-Genre", "Blues"};

	[TestMethod]
	public void TestTitle ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.Title, "Initial (Null): " + m);
		});

		file.Tag.Title = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.Title, "Value Set (!Null): " + m);
		});

		file.Tag.Title = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.Title, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestPerformers ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.AreEqual (0, t.Performers.Length, "Initial (Zero): " + m);
		});

		file.Tag.Performers = val_mult;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_mult.Length, t.Performers.Length, "Value Set: " + m);
			for (int i = 0; i < val_mult.Length; i++) {
				Assert.AreEqual (val_mult[i], t.Performers[i], "Value Set: " + m);
			}
		});

		file.Tag.Performers = new string[0];

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.AreEqual (0, t.Performers.Length, "Value Cleared (Zero): " + m);
		});
	}

	[TestMethod]
	public void TestAlbumArtists ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.AreEqual (0, t.AlbumArtists.Length, "Initial (Zero): " + m);
		});

		file.Tag.AlbumArtists = val_mult;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_mult.Length, t.AlbumArtists.Length, "Value Set: " + m);
			for (int i = 0; i < val_mult.Length; i++) {
				Assert.AreEqual (val_mult[i], t.AlbumArtists[i], "Value Set: " + m);
			}
		});

		file.Tag.AlbumArtists = new string[0];

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.AreEqual (0, t.AlbumArtists.Length, "Value Cleared (Zero): " + m);
		});
	}

	[TestMethod]
	public void TestComposers ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.AreEqual (0, t.Composers.Length, "Initial (Zero): " + m);
		});

		file.Tag.Composers = val_mult;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_mult.Length, t.Composers.Length, "Value Set: " + m);
			for (int i = 0; i < val_mult.Length; i++) {
				Assert.AreEqual (val_mult[i], t.Composers[i], "Value Set: " + m);
			}
		});

		file.Tag.Composers = new string[0];

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.AreEqual (0, t.Composers.Length, "Value Cleared (Zero): " + m);
		});
	}

	[TestMethod]
	public void TestAlbum ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.Album, "Initial (Null): " + m);
		});

		file.Tag.Album = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.Album, "Value Set (!Null): " + m);
		});

		file.Tag.Album = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.Album, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestComment ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.Comment, "Initial (Null): " + m);
		});

		file.Tag.Comment = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.Comment, "Value Set (!Null): " + m);
		});

		file.Tag.Comment = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.Comment, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestGenres ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.AreEqual (0, t.Genres.Length, "Initial (Zero): " + m);
		});

		file.Tag.Genres = val_gnre;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_gnre.Length, t.Genres.Length, "Value Set: " + m);
			for (int i = 0; i < val_gnre.Length; i++) {
				Assert.AreEqual (val_gnre[i], t.Genres[i], "Value Set: " + m);
			}
		});

		file.Tag.Genres = val_mult;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_mult.Length, t.Genres.Length, "Value Set: " + m);
			for (int i = 0; i < val_mult.Length; i++) {
				Assert.AreEqual (val_mult[i], t.Genres[i], "Value Set: " + m);
			}
		});

		file.Tag.Genres = new string[0];

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.AreEqual (0, t.Genres.Length, "Value Cleared (Zero): " + m);
		});
	}

	[TestMethod]
	public void TestYear ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.AreEqual (0u, t.Year, "Initial (Zero): " + m);
		});

		file.Tag.Year = 1999;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (1999u, t.Year, "Value Set: " + m);
		});

		file.Tag.Year = 0;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.AreEqual (0u, t.Year, "Value Cleared (Zero): " + m);
		});
	}

	[TestMethod]
	public void TestTrack ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.AreEqual (0u, t.Track, "Initial (Zero): " + m);
		});

		file.Tag.Track = 199;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (199u, t.Track, "Value Set: " + m);
		});

		file.Tag.Track = 0;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.AreEqual (0u, t.Track, "Value Cleared (Zero): " + m);
		});
	}

	[TestMethod]
	public void TestTrackCount ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.AreEqual (0u, t.TrackCount, "Initial (Zero): " + m);
		});

		file.Tag.TrackCount = 199;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (199u, t.TrackCount, "Value Set: " + m);
		});

		file.Tag.TrackCount = 0;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.AreEqual (0u, t.TrackCount, "Value Cleared (Zero): " + m);
		});
	}

	[TestMethod]
	public void TestDisc ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.AreEqual (0u, t.Disc, "Initial (Zero): " + m);
		});

		file.Tag.Disc = 199;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (199u, t.Disc, "Value Set: " + m);
		});

		file.Tag.Disc = 0;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.AreEqual (0u, t.Disc, "Value Cleared (Zero): " + m);
		});
	}

	[TestMethod]
	public void TestDiscCount ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.AreEqual (0u, t.DiscCount, "Initial (Zero): " + m);
		});

		file.Tag.DiscCount = 199;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (199u, t.DiscCount, "Value Set: " + m);
		});

		file.Tag.DiscCount = 0;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.AreEqual (0u, t.DiscCount, "Value Cleared (Zero): " + m);
		});
	}

	[TestMethod]
	public void TestLyrics ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.Lyrics, "Initial (Null): " + m);
		});

		file.Tag.Lyrics = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.Lyrics, "Value Set (!Null): " + m);
		});

		file.Tag.Lyrics = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.Lyrics, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestGrouping ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.Grouping, "Initial (Null): " + m);
		});

		file.Tag.Grouping = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.Grouping, "Value Set (!Null): " + m);
		});

		file.Tag.Grouping = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.Grouping, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestBeatsPerMinute ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.AreEqual (0u, t.BeatsPerMinute, "Initial (Zero): " + m);
		});

		file.Tag.BeatsPerMinute = 199;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (199u, t.BeatsPerMinute, "Value Set: " + m);
		});

		file.Tag.BeatsPerMinute = 0;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.AreEqual (0u, t.BeatsPerMinute, "Value Cleared (Zero): " + m);
		});
	}

	[TestMethod]
	public void TestConductor ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.Conductor, "Initial (Null): " + m);
		});

		file.Tag.Conductor = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.Conductor, "Value Set (!Null): " + m);
		});

		file.Tag.Conductor = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.Conductor, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestCopyright ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.Copyright, "Initial (Null): " + m);
		});

		file.Tag.Copyright = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.Copyright, "Value Set (!Null): " + m);
		});

		file.Tag.Copyright = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.Copyright, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestPictures ()
	{
		var file = CreateFile (out var abst);

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

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.AreEqual (0, t.Pictures.Length, "Initial (Zero): " + m);
		});

		file.Tag.Pictures = pictures;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (pictures.Length, t.Pictures.Length, "Value Set: " + m);
			for (int i = 0; i < pictures.Length; i++) {
				string msg = "Value " + i + "Set: " + m;
				Assert.AreEqual (pictures[i].Data, t.Pictures[i].Data, msg);
				Assert.AreEqual (PictureType.FrontCover, t.Pictures[i].Type, msg);
				Assert.AreEqual (i % 2 == 0 ? "cover.png" : "cover.jpg", t.Pictures[i].Description, msg);
				Assert.AreEqual (pictures[i].MimeType, t.Pictures[i].MimeType, msg);
			}
		});

		file.Tag.Pictures = new Picture[0];

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.AreEqual (0, t.Pictures.Length, "Value Cleared (Zero): " + m);
		});
	}

	[TestMethod]
	public void TestMusicBrainzArtistID ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzArtistId, "Initial (Null): " + m);
		});

		file.Tag.MusicBrainzArtistId = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.MusicBrainzArtistId, "Value Set (!Null): " + m);
		});

		file.Tag.MusicBrainzArtistId = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzArtistId, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestMusicBrainzReleaseID ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzReleaseId, "Initial (Null): " + m);
		});

		file.Tag.MusicBrainzReleaseId = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.MusicBrainzReleaseId, "Value Set (!Null): " + m);
		});

		file.Tag.MusicBrainzReleaseId = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzReleaseId, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestMusicBrainzReleaseGroupID ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzReleaseGroupId, "Initial (Null): " + m);
		});

		file.Tag.MusicBrainzReleaseGroupId = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.MusicBrainzReleaseGroupId, "Value Set (!Null): " + m);
		});

		file.Tag.MusicBrainzReleaseGroupId = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzReleaseGroupId, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestMusicBrainzReleaseArtistID ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzReleaseArtistId, "Initial (Null): " + m);
		});

		file.Tag.MusicBrainzReleaseArtistId = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.MusicBrainzReleaseArtistId, "Value Set (!Null): " + m);
		});

		file.Tag.MusicBrainzReleaseArtistId = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzReleaseArtistId, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestMusicBrainzTrackID ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzTrackId, "Initial (Null): " + m);
		});

		file.Tag.MusicBrainzTrackId = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.MusicBrainzTrackId, "Value Set (!Null): " + m);
		});

		file.Tag.MusicBrainzTrackId = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzTrackId, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestMusicBrainzRecordingID ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzRecordingId, "Initial (Null): " + m);
		});

		file.Tag.MusicBrainzRecordingId = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.MusicBrainzRecordingId, "Value Set (!Null): " + m);
		});

		file.Tag.MusicBrainzRecordingId = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzRecordingId, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestMusicBrainzWorkID ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzWorkId, "Initial (Null): " + m);
		});

		file.Tag.MusicBrainzWorkId = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.MusicBrainzWorkId, "Value Set (!Null): " + m);
		});

		file.Tag.MusicBrainzWorkId = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzWorkId, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestMusicBrainzDiscID ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzDiscId, "Initial (Null): " + m);
		});

		file.Tag.MusicBrainzDiscId = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.MusicBrainzDiscId, "Value Set (!Null): " + m);
		});

		file.Tag.MusicBrainzDiscId = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzDiscId, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestMusicIPPUID ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.MusicIpId, "Initial (Null): " + m);
		});

		file.Tag.MusicIpId = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.MusicIpId, "Value Set (!Null): " + m);
		});

		file.Tag.MusicIpId = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.MusicIpId, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestAmazonID ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.AmazonId, "Initial (Null): " + m);
		});

		file.Tag.AmazonId = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.AmazonId, "Value Set (!Null): " + m);
		});

		file.Tag.AmazonId = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.AmazonId, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestMusicBrainzReleaseStatus ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzReleaseStatus, "Initial (Null): " + m);
		});

		file.Tag.MusicBrainzReleaseStatus = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.MusicBrainzReleaseStatus, "Value Set (!Null): " + m);
		});

		file.Tag.MusicBrainzReleaseStatus = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzReleaseStatus, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestMusicBrainzReleaseType ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzReleaseType, "Initial (Null): " + m);
		});

		file.Tag.MusicBrainzReleaseType = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.MusicBrainzReleaseType, "Value Set (!Null): " + m);
		});

		file.Tag.MusicBrainzReleaseType = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzReleaseType, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestMusicBrainzReleaseCountry ()
	{
		var file = CreateFile (out var abst);

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Initial (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzReleaseCountry, "Initial (Null): " + m);
		});

		file.Tag.MusicBrainzReleaseCountry = val_sing;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsFalse (t.IsEmpty, "Value Set (!IsEmpty): " + m);
			Assert.AreEqual (val_sing, t.MusicBrainzReleaseCountry, "Value Set (!Null): " + m);
		});

		file.Tag.MusicBrainzReleaseCountry = string.Empty;

		TagTestWithSave (ref file, abst, delegate (Tag t, string m) {
			Assert.IsTrue (t.IsEmpty, "Value Cleared (IsEmpty): " + m);
			Assert.IsNull (t.MusicBrainzReleaseCountry, "Value Cleared (Null): " + m);
		});
	}

	[TestMethod]
	public void TestClear ()
	{
		var file = CreateFile (out _);

		file.Tag.Title = "A";
		file.Tag.Performers = new[] { "B" };
		file.Tag.AlbumArtists = new[] { "C" };
		file.Tag.Composers = new[] { "D" };
		file.Tag.Album = "E";
		file.Tag.Comment = "F";
		file.Tag.Genres = new[] { "Blues" };
		file.Tag.Year = 123;
		file.Tag.Track = 234;
		file.Tag.TrackCount = 234;
		file.Tag.Disc = 234;
		file.Tag.DiscCount = 234;
		file.Tag.Lyrics = "G";
		file.Tag.Grouping = "H";
		file.Tag.BeatsPerMinute = 234;
		file.Tag.Conductor = "I";
		file.Tag.Copyright = "J";
		file.Tag.Pictures = new[] { new Picture (TestPath.Covers + "sample_a.png") };

		Assert.IsFalse (file.Tag.IsEmpty, "Should be full.");
		file.Tag.Clear ();

		Assert.IsNull (file.Tag.Title, "Title");
		Assert.AreEqual (0, file.Tag.Performers.Length, "Performers");
		Assert.AreEqual (0, file.Tag.AlbumArtists.Length, "AlbumArtists");
		Assert.AreEqual (0, file.Tag.Composers.Length, "Composers");
		Assert.IsNull (file.Tag.Album, "Album");
		Assert.IsNull (file.Tag.Comment, "Comment");
		Assert.AreEqual (0, file.Tag.Genres.Length, "Genres");
		Assert.AreEqual (0u, file.Tag.Year, "Year");
		Assert.AreEqual (0u, file.Tag.Track, "Track");
		Assert.AreEqual (0u, file.Tag.TrackCount, "TrackCount");
		Assert.AreEqual (0u, file.Tag.Disc, "Disc");
		Assert.AreEqual (0u, file.Tag.DiscCount, "DiscCount");
		Assert.IsNull (file.Tag.Lyrics, "Lyrics");
		Assert.IsNull (file.Tag.Comment, "Comment");
		Assert.AreEqual (0u, file.Tag.BeatsPerMinute, "BeatsPerMinute");
		Assert.IsNull (file.Tag.Conductor, "Conductor");
		Assert.IsNull (file.Tag.Copyright, "Copyright");
		Assert.AreEqual (0, file.Tag.Pictures.Length, "Pictures");
		Assert.IsTrue (file.Tag.IsEmpty, "Should be empty.");
	}

	TagLib.Mpeg4.File CreateFile (out MemoryFileAbstraction abst)
	{
		byte[] data = {
			0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70,
			0x6d, 0x70, 0x34, 0x32, 0x00, 0x00, 0x00, 0x00,
			0x6d, 0x70, 0x34, 0x32, 0x69, 0x73, 0x6f, 0x6d,
			0x00, 0x00, 0x00, 0x08, 0x6d, 0x6f, 0x6f, 0x76
		};

		abst = new MemoryFileAbstraction (0xffff, data);
		return new TagLib.Mpeg4.File (abst, ReadStyle.None);
	}

	delegate void TagTestFunc (Tag tag, string msg);

	void TagTestWithSave (ref TagLib.Mpeg4.File file, MemoryFileAbstraction abst, TagTestFunc testFunc)
	{
		testFunc (file.GetTag (TagTypes.Apple), "Before Save");
		file.Save ();
		//			Console.WriteLine ();
		//			Extras.DumpHex ((abst.ReadStream as System.IO.MemoryStream).ToArray ());
		file = new TagLib.Mpeg4.File (abst, ReadStyle.None);
		testFunc (file.GetTag (TagTypes.Apple), "After Save");
	}
}
