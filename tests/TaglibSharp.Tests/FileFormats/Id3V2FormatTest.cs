using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class Id3V2FormatTest : IFormatTest
{
	static readonly string sample_file = TestPath.Samples + "sample_v2_only.mp3";
	static readonly string corrupt_file = TestPath.Samples + "corrupt/null_title_v2.mp3";
	static readonly string tmp_file = TestPath.SamplesTmp + "tmpwrite_v2_only.mp3";
	static readonly string ext_header_file = TestPath.Samples + "sample_v2_3_ext_header.mp3";
	static File file;

	[ClassInitialize]
	public static void Init (TestContext testContext)
	{
		file = File.Create (sample_file);
	}

	[TestMethod]
	public void ReadAudioProperties ()
	{
		Assert.AreEqual (44100, file.Properties.AudioSampleRate);
		Assert.AreEqual (1, file.Properties.Duration.Seconds);
	}

	[TestMethod]
	public void ReadTags ()
	{
		Assert.AreEqual ("MP3 album", file.Tag.Album);
		Assert.AreEqual ("MP3 artist", file.Tag.FirstPerformer);
		Assert.AreEqual ("MP3 comment", file.Tag.Comment);
		Assert.AreEqual ("Acid Punk", file.Tag.FirstGenre);
		Assert.AreEqual ("MP3 title", file.Tag.Title);
		Assert.AreEqual (6u, file.Tag.Track);
		Assert.AreEqual (7u, file.Tag.TrackCount);
		Assert.AreEqual (1234u, file.Tag.Year);
	}

	[TestMethod]
	public void MultiGenresTest ()
	{
		string inFile = TestPath.Samples + "sample.mp3";
		string tempFile = TestPath.Samples + "tmpwrite.mp3";

		var rgFile = File.Create (inFile);
		var tag = rgFile.Tag;
		var genres = tag.Genres;

		Assert.AreEqual (3, genres.Length);
		Assert.AreEqual ("Genre 1", genres[0]);
		Assert.AreEqual ("Genre 2", genres[1]);
		Assert.AreEqual ("Genre 3", genres[2]);

		rgFile.Dispose ();
		System.IO.File.Delete (tempFile);
	}


	[TestMethod]
	public void WriteStandardTags ()
	{
		StandardTests.WriteStandardTags (sample_file, tmp_file);
	}


	[TestMethod]
	public void WriteStandardPictures ()
	{
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.None);
	}

	[TestMethod]
	public void WriteExtendedTags ()
	{
		ExtendedTests.WriteExtendedTags (sample_file, tmp_file);
	}

	[TestMethod] // http://bugzilla.gnome.org/show_bug.cgi?id=558123
	public void TestTruncateOnNull ()
	{
		if (System.IO.File.Exists (tmp_file)) {
			System.IO.File.Delete (tmp_file);
		}

		System.IO.File.Copy (corrupt_file, tmp_file);
		var tmp = File.Create (tmp_file);

		Assert.AreEqual ("T", tmp.Tag.Title);
	}

	[TestMethod]
	public void TestCorruptionResistance ()
	{
	}

	[TestMethod]
	public void TestExtendedHeaderSize ()
	{
		// bgo#604488
		var file = File.Create (ext_header_file);
		Assert.AreEqual ("Title v2", file.Tag.Title);
	}

	[TestMethod]
	public void URLLinkFrameTest ()
	{
		string tempFile = TestPath.SamplesTmp + "tmpwrite_sample_v2_only.mp3";
		Directory.CreateDirectory (Path.GetDirectoryName (tempFile));

        System.IO.File.Copy (sample_file, tempFile, true);

		var urlLinkFile = File.Create (tempFile);
		var id3v2tag = urlLinkFile.GetTag (TagTypes.Id3v2) as TagLib.Id3v2.Tag;
		id3v2tag.SetTextFrame ("WCOM", "www.commercial.com");
		id3v2tag.SetTextFrame ("WCOP", "www.copyright.com");
		id3v2tag.SetTextFrame ("WOAF", "www.official-audio.com");
		id3v2tag.SetTextFrame ("WOAR", "www.official-artist.com");
		id3v2tag.SetTextFrame ("WOAS", "www.official-audio-source.com");
		id3v2tag.SetTextFrame ("WORS", "www.official-internet-radio.com");
		id3v2tag.SetTextFrame ("WPAY", "www.payment.com");
		id3v2tag.SetTextFrame ("WPUB", "www.official-publisher.com");
		urlLinkFile.Save ();
		urlLinkFile.Dispose ();

		urlLinkFile = File.Create (tempFile);
		id3v2tag = urlLinkFile.GetTag (TagTypes.Id3v2) as TagLib.Id3v2.Tag;
		Assert.AreEqual ("www.commercial.com", id3v2tag.GetTextAsString ("WCOM"));
		Assert.AreEqual ("www.copyright.com", id3v2tag.GetTextAsString ("WCOP"));
		Assert.AreEqual ("www.official-audio.com", id3v2tag.GetTextAsString ("WOAF"));
		Assert.AreEqual ("www.official-artist.com", id3v2tag.GetTextAsString ("WOAR"));
		Assert.AreEqual ("www.official-audio-source.com", id3v2tag.GetTextAsString ("WOAS"));
		Assert.AreEqual ("www.official-internet-radio.com", id3v2tag.GetTextAsString ("WORS"));
		Assert.AreEqual ("www.payment.com", id3v2tag.GetTextAsString ("WPAY"));
		Assert.AreEqual ("www.official-publisher.com", id3v2tag.GetTextAsString ("WPUB"));
		urlLinkFile.Dispose ();

		System.IO.File.Delete (tempFile);
	}

	/// <summary>
	/// If we construct a new Id3v2 tag, then try to copy that onto a File.Tag
	/// We observe that simple text frames are copied, but APIC and GEOB aren't
	/// </summary>
	[TestMethod]
	public void TestPicturesAreCopied ()
	{
		string tempFile = TestPath.SamplesTmp + "tmpwrite_sample_v2_only.mp3";
        Directory.CreateDirectory (Path.GetDirectoryName (tempFile));

        System.IO.File.Copy (sample_file, tempFile, true);

		// Put a picture on the starting file
		File file = TagLib.File.Create (tempFile);
		var picture = new Picture (TestPath.Samples + "sample_gimp.gif") {
			Type = PictureType.BackCover,
			Description = "TEST description 1"
		};
		file.Tag.Pictures = new[] { picture };
		file.Save ();

		Assert.AreEqual (1, file.Tag.Pictures.Count (), "File should start with 1 picture");

		// Now construct a new tag with a title, APIC and GEOB frame

		var tag = new TagLib.Id3v2.Tag();
		tag.Title = "FOOBAR";

		// single red dot (1x1 px red image) APIC frame found in wild
		var redDot = new byte[] { 65, 80, 73, 67, 0, 0, 0, 155, 0, 0, 0, 105, 109, 97, 103, 101, 47, 112, 110, 103, 0, 3, 0, 137, 80, 78,
			71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 1, 0, 0, 0, 1, 8, 2, 0, 0, 0, 144, 119, 83, 222, 0, 0, 0, 4, 103, 65,
			77, 65, 0, 0, 177, 143, 11, 252, 97, 5, 0, 0, 0, 9, 112, 72, 89, 115, 0, 0, 11, 18, 0, 0, 11, 18, 1, 210, 221, 126, 252, 0, 0, 0,
			24, 116, 69, 88, 116, 83, 111, 102, 116, 119, 97, 114, 101, 0, 112, 97, 105, 110, 116, 46, 110, 101, 116, 32, 52, 46, 49, 46, 53,
			100, 71, 88, 82, 0, 0, 0, 12, 73, 68, 65, 84, 24, 87, 99, 248, 47, 162, 0, 0, 3, 73, 1, 52, 163, 224, 5, 179, 0, 0, 0, 0, 73, 69,
			78, 68, 174, 66, 96, 130 };
		var pictureFrame = new TagLib.Id3v2.AttachmentFrame (redDot, 3);

		var geobFrame = new TagLib.Id3v2.AttachmentFrame ();
		geobFrame.MimeType = "plain/text";
		geobFrame.Description = "random";
		geobFrame.Type = PictureType.NotAPicture;
		geobFrame.Data = "random text in geob";

		tag.AddFrame (pictureFrame);
		tag.AddFrame (geobFrame);

		tag.CopyTo (file.Tag, false);

		Assert.AreEqual ("MP3 title", file.Tag.Title, "Title shouldn't be copied if overwrite=false");
		Assert.AreEqual (1, file.Tag.Pictures.Count (), "GEOB/APIC frames shouldn't be copied if overwrite=false");

		tag.CopyTo (file.Tag, true);

		Assert.AreEqual (tag.Title, file.Tag.Title, "Title wasn't copied");
		Assert.AreEqual (tag.Pictures.Count (), file.Tag.Pictures.Count (), "GEOB/APIC frames weren't copied");
	}
}
