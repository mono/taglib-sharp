
namespace TaglibSharp.Tests.TaggingFormats;

[TestClass]
public class Id3V1Test
{
	[TestMethod]
	public void TestTitle ()
	{
		var tag = new TagLib.Id3v1.Tag ();

		Assert.IsTrue (tag.IsEmpty, "Initially empty");
		Assert.IsNull (tag.Title, "Initially null");

		var rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.IsNull (tag.Title, "Still null");

		tag.Title = "01234567890123456789012345678901234567890123456789";
		Assert.IsFalse (tag.IsEmpty, "Not empty");
		Assert.AreEqual ("01234567890123456789012345678901234567890123456789", tag.Title);

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsFalse (tag.IsEmpty, "Still not empty");
		Assert.AreEqual ("012345678901234567890123456789", tag.Title);

		tag.Title = string.Empty;
		Assert.IsTrue (tag.IsEmpty, "Again empty");
		Assert.IsNull (tag.Title, "Again null");

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.IsNull (tag.Title, "Still null");
	}

	[TestMethod]
	public void TestPerformers ()
	{
		var tag = new TagLib.Id3v1.Tag ();

		Assert.IsTrue (tag.IsEmpty, "Initially empty");
		Assert.AreEqual (0, tag.Performers.Length, "Initially empty");

		var rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.AreEqual (0, tag.Performers.Length, "Still empty");

		tag.Performers = new[] { "A123456789", "B123456789", "C123456789", "D123456789", "E123456789" };
		Assert.IsFalse (tag.IsEmpty, "Not empty");
		Assert.AreEqual ("A123456789; B123456789; C123456789; D123456789; E123456789", tag.JoinedPerformers);

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsFalse (tag.IsEmpty, "Still not empty");
		Assert.AreEqual ("A123456789; B123456789; C1234567", tag.JoinedPerformers);

		tag.Performers = new string[0];
		Assert.IsTrue (tag.IsEmpty, "Again empty");
		Assert.AreEqual (0, tag.Performers.Length, "Again empty");

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.AreEqual (0, tag.Performers.Length, "Still empty");
	}

	[TestMethod]
	public void TestAlbum ()
	{
		var tag = new TagLib.Id3v1.Tag ();

		Assert.IsTrue (tag.IsEmpty, "Initially empty");
		Assert.IsNull (tag.Album, "Initially null");

		var rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.IsNull (tag.Album, "Still null");

		tag.Album = "01234567890123456789012345678901234567890123456789";
		Assert.IsFalse (tag.IsEmpty, "Not empty");
		Assert.AreEqual ("01234567890123456789012345678901234567890123456789", tag.Album);

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsFalse (tag.IsEmpty, "Still not empty");
		Assert.AreEqual ("012345678901234567890123456789", tag.Album);

		tag.Album = string.Empty;
		Assert.IsTrue (tag.IsEmpty, "Again empty");
		Assert.IsNull (tag.Album, "Again null");

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.IsNull (tag.Album, "Still null");
	}

	[TestMethod]
	public void TestYear ()
	{
		var tag = new TagLib.Id3v1.Tag ();

		Assert.IsTrue (tag.IsEmpty, "Initially empty");
		Assert.AreEqual (0u, tag.Year, "Initially zero");

		var rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.AreEqual (0u, tag.Year, "Still zero");

		tag.Year = 1999;
		Assert.IsFalse (tag.IsEmpty, "Not empty");
		Assert.AreEqual (1999u, tag.Year);

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsFalse (tag.IsEmpty, "Still not empty");
		Assert.AreEqual (1999u, tag.Year);

		tag.Year = 20000;
		Assert.IsTrue (tag.IsEmpty, "Again empty");
		Assert.AreEqual (0u, tag.Year, "Again zero");

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.AreEqual (0u, tag.Year, "Still zero");
	}

	[TestMethod]
	public void TestComment ()
	{
		var tag = new TagLib.Id3v1.Tag ();

		Assert.IsTrue (tag.IsEmpty, "Initially empty");
		Assert.IsNull (tag.Comment, "Initially null");

		var rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.IsNull (tag.Comment, "Still null");

		tag.Comment = "01234567890123456789012345678901234567890123456789";
		Assert.IsFalse (tag.IsEmpty, "Not empty");
		Assert.AreEqual ("01234567890123456789012345678901234567890123456789", tag.Comment);

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsFalse (tag.IsEmpty, "Still not empty");
		Assert.AreEqual ("0123456789012345678901234567", tag.Comment);

		tag.Comment = string.Empty;
		Assert.IsTrue (tag.IsEmpty, "Again empty");
		Assert.IsNull (tag.Comment, "Again null");

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.IsNull (tag.Comment, "Still null");
	}

	[TestMethod]
	public void TestTrack ()
	{
		var tag = new TagLib.Id3v1.Tag ();

		Assert.IsTrue (tag.IsEmpty, "Initially empty");
		Assert.AreEqual (0u, tag.Track, "Initially zero");

		var rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.AreEqual (0u, tag.Track, "Still zero");

		tag.Track = 123;
		Assert.IsFalse (tag.IsEmpty, "Not empty");
		Assert.AreEqual (123u, tag.Track);

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsFalse (tag.IsEmpty, "Still not empty");
		Assert.AreEqual (123u, tag.Track);

		tag.Track = 0;
		Assert.IsTrue (tag.IsEmpty, "Again empty");
		Assert.AreEqual (0u, tag.Track, "Again zero");

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.AreEqual (0u, tag.Track, "Still zero");
	}

	[TestMethod]
	public void TestGenres ()
	{
		var tag = new TagLib.Id3v1.Tag ();

		Assert.IsTrue (tag.IsEmpty, "Initially empty");
		Assert.AreEqual (0, tag.Genres.Length, "Initially empty");

		var rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.AreEqual (0, tag.Genres.Length, "Still empty");

		tag.Genres = new[] { "Rap", "Jazz", "Non-Genre", "Blues" };
		Assert.IsFalse (tag.IsEmpty, "Not empty");
		Assert.AreEqual ("Rap", tag.JoinedGenres);

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsFalse (tag.IsEmpty, "Still not empty");
		Assert.AreEqual ("Rap", tag.JoinedGenres);

		tag.Genres = new[] { "Non-Genre" };
		Assert.IsTrue (tag.IsEmpty, "Surprisingly empty");
		Assert.AreEqual (0, tag.Genres.Length, "Surprisingly empty");

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.AreEqual (0, tag.Genres.Length, "Still empty");

		tag.Genres = new string[0];
		Assert.IsTrue (tag.IsEmpty, "Again empty");
		Assert.AreEqual (0, tag.Genres.Length, "Again empty");

		rendered = tag.Render ();
		tag = new TagLib.Id3v1.Tag (rendered);
		Assert.IsTrue (tag.IsEmpty, "Still empty");
		Assert.AreEqual (0, tag.Genres.Length, "Still empty");
	}

	[TestMethod]
	public void TestClear ()
	{
		var tag = new TagLib.Id3v1.Tag {
			Title = "A",
			Performers = new[] { "B" },
			Album = "C",
			Year = 123,
			Comment = "D",
			Track = 234,
			Genres = new[] { "Blues" }
		};

		Assert.IsFalse (tag.IsEmpty, "Should be full.");
		tag.Clear ();
		Assert.IsNull (tag.Title, "Title");
		Assert.AreEqual (0, tag.Performers.Length, "Performers");
		Assert.IsNull (tag.Album, "Album");
		Assert.AreEqual (0u, tag.Year, "Year");
		Assert.IsNull (tag.Comment, "Comment");
		Assert.AreEqual (0u, tag.Track, "Track");
		Assert.AreEqual (0, tag.Genres.Length, "Genres");
		Assert.IsTrue (tag.IsEmpty, "Should be empty.");
	}

	[TestMethod]
	public void TestRender ()
	{
		var rendered = new TagLib.Id3v1.Tag ().Render ();
		Assert.AreEqual (128, rendered.Count);
		Assert.IsTrue (rendered.StartsWith (TagLib.Id3v1.Tag.FileIdentifier));
	}
}
