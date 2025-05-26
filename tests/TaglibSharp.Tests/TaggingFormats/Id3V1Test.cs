using NUnit.Framework;

namespace TaglibSharp.Tests.TaggingFormats
{
	[TestFixture]
	public class Id3V1Test
	{
		[Test]
		public void TestTitle ()
		{
			var tag = new TagLib.Id3v1.Tag ();

			ClassicAssert.IsTrue (tag.IsEmpty, "Initially empty");
			ClassicAssert.IsNull (tag.Title, "Initially null");

			var rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.IsNull (tag.Title, "Still null");

			tag.Title = "01234567890123456789012345678901234567890123456789";
			ClassicAssert.IsFalse (tag.IsEmpty, "Not empty");
			ClassicAssert.AreEqual ("01234567890123456789012345678901234567890123456789", tag.Title);

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsFalse (tag.IsEmpty, "Still not empty");
			ClassicAssert.AreEqual ("012345678901234567890123456789", tag.Title);

			tag.Title = string.Empty;
			ClassicAssert.IsTrue (tag.IsEmpty, "Again empty");
			ClassicAssert.IsNull (tag.Title, "Again null");

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.IsNull (tag.Title, "Still null");
		}

		[Test]
		public void TestPerformers ()
		{
			var tag = new TagLib.Id3v1.Tag ();

			ClassicAssert.IsTrue (tag.IsEmpty, "Initially empty");
			ClassicAssert.AreEqual (0, tag.Performers.Length, "Initially empty");

			var rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.AreEqual (0, tag.Performers.Length, "Still empty");

			tag.Performers = new[] { "A123456789", "B123456789", "C123456789", "D123456789", "E123456789" };
			ClassicAssert.IsFalse (tag.IsEmpty, "Not empty");
			ClassicAssert.AreEqual ("A123456789; B123456789; C123456789; D123456789; E123456789", tag.JoinedPerformers);

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsFalse (tag.IsEmpty, "Still not empty");
			ClassicAssert.AreEqual ("A123456789; B123456789; C1234567", tag.JoinedPerformers);

			tag.Performers = new string[0];
			ClassicAssert.IsTrue (tag.IsEmpty, "Again empty");
			ClassicAssert.AreEqual (0, tag.Performers.Length, "Again empty");

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.AreEqual (0, tag.Performers.Length, "Still empty");
		}

		[Test]
		public void TestAlbum ()
		{
			var tag = new TagLib.Id3v1.Tag ();

			ClassicAssert.IsTrue (tag.IsEmpty, "Initially empty");
			ClassicAssert.IsNull (tag.Album, "Initially null");

			var rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.IsNull (tag.Album, "Still null");

			tag.Album = "01234567890123456789012345678901234567890123456789";
			ClassicAssert.IsFalse (tag.IsEmpty, "Not empty");
			ClassicAssert.AreEqual ("01234567890123456789012345678901234567890123456789", tag.Album);

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsFalse (tag.IsEmpty, "Still not empty");
			ClassicAssert.AreEqual ("012345678901234567890123456789", tag.Album);

			tag.Album = string.Empty;
			ClassicAssert.IsTrue (tag.IsEmpty, "Again empty");
			ClassicAssert.IsNull (tag.Album, "Again null");

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.IsNull (tag.Album, "Still null");
		}

		[Test]
		public void TestYear ()
		{
			var tag = new TagLib.Id3v1.Tag ();

			ClassicAssert.IsTrue (tag.IsEmpty, "Initially empty");
			ClassicAssert.AreEqual (0, tag.Year, "Initially zero");

			var rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.AreEqual (0, tag.Year, "Still zero");

			tag.Year = 1999;
			ClassicAssert.IsFalse (tag.IsEmpty, "Not empty");
			ClassicAssert.AreEqual (1999, tag.Year);

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsFalse (tag.IsEmpty, "Still not empty");
			ClassicAssert.AreEqual (1999, tag.Year);

			tag.Year = 20000;
			ClassicAssert.IsTrue (tag.IsEmpty, "Again empty");
			ClassicAssert.AreEqual (0, tag.Year, "Again zero");

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.AreEqual (0, tag.Year, "Still zero");
		}

		[Test]
		public void TestComment ()
		{
			var tag = new TagLib.Id3v1.Tag ();

			ClassicAssert.IsTrue (tag.IsEmpty, "Initially empty");
			ClassicAssert.IsNull (tag.Comment, "Initially null");

			var rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.IsNull (tag.Comment, "Still null");

			tag.Comment = "01234567890123456789012345678901234567890123456789";
			ClassicAssert.IsFalse (tag.IsEmpty, "Not empty");
			ClassicAssert.AreEqual ("01234567890123456789012345678901234567890123456789", tag.Comment);

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsFalse (tag.IsEmpty, "Still not empty");
			ClassicAssert.AreEqual ("0123456789012345678901234567", tag.Comment);

			tag.Comment = string.Empty;
			ClassicAssert.IsTrue (tag.IsEmpty, "Again empty");
			ClassicAssert.IsNull (tag.Comment, "Again null");

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.IsNull (tag.Comment, "Still null");
		}

		[Test]
		public void TestTrack ()
		{
			var tag = new TagLib.Id3v1.Tag ();

			ClassicAssert.IsTrue (tag.IsEmpty, "Initially empty");
			ClassicAssert.AreEqual (0, tag.Track, "Initially zero");

			var rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.AreEqual (0, tag.Track, "Still zero");

			tag.Track = 123;
			ClassicAssert.IsFalse (tag.IsEmpty, "Not empty");
			ClassicAssert.AreEqual (123, tag.Track);

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsFalse (tag.IsEmpty, "Still not empty");
			ClassicAssert.AreEqual (123, tag.Track);

			tag.Track = 0;
			ClassicAssert.IsTrue (tag.IsEmpty, "Again empty");
			ClassicAssert.AreEqual (0, tag.Track, "Again zero");

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.AreEqual (0, tag.Track, "Still zero");
		}

		[Test]
		public void TestGenres ()
		{
			var tag = new TagLib.Id3v1.Tag ();

			ClassicAssert.IsTrue (tag.IsEmpty, "Initially empty");
			ClassicAssert.AreEqual (0, tag.Genres.Length, "Initially empty");

			var rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.AreEqual (0, tag.Genres.Length, "Still empty");

			tag.Genres = new[] { "Rap", "Jazz", "Non-Genre", "Blues" };
			ClassicAssert.IsFalse (tag.IsEmpty, "Not empty");
			ClassicAssert.AreEqual ("Rap", tag.JoinedGenres);

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsFalse (tag.IsEmpty, "Still not empty");
			ClassicAssert.AreEqual ("Rap", tag.JoinedGenres);

			tag.Genres = new[] { "Non-Genre" };
			ClassicAssert.IsTrue (tag.IsEmpty, "Surprisingly empty");
			ClassicAssert.AreEqual (0, tag.Genres.Length, "Surprisingly empty");

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.AreEqual (0, tag.Genres.Length, "Still empty");

			tag.Genres = new string[0];
			ClassicAssert.IsTrue (tag.IsEmpty, "Again empty");
			ClassicAssert.AreEqual (0, tag.Genres.Length, "Again empty");

			rendered = tag.Render ();
			tag = new TagLib.Id3v1.Tag (rendered);
			ClassicAssert.IsTrue (tag.IsEmpty, "Still empty");
			ClassicAssert.AreEqual (0, tag.Genres.Length, "Still empty");
		}

		[Test]
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

			ClassicAssert.IsFalse (tag.IsEmpty, "Should be full.");
			tag.Clear ();
			ClassicAssert.IsNull (tag.Title, "Title");
			ClassicAssert.AreEqual (0, tag.Performers.Length, "Performers");
			ClassicAssert.IsNull (tag.Album, "Album");
			ClassicAssert.AreEqual (0, tag.Year, "Year");
			ClassicAssert.IsNull (tag.Comment, "Comment");
			ClassicAssert.AreEqual (0, tag.Track, "Track");
			ClassicAssert.AreEqual (0, tag.Genres.Length, "Genres");
			ClassicAssert.IsTrue (tag.IsEmpty, "Should be empty.");
		}

		[Test]
		public void TestRender ()
		{
			var rendered = new TagLib.Id3v1.Tag ().Render ();
			ClassicAssert.AreEqual (128, rendered.Count);
			ClassicAssert.IsTrue (rendered.StartsWith (TagLib.Id3v1.Tag.FileIdentifier));
		}
	}
}
