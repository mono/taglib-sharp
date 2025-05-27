using TagLib.Mpeg4;

using File = TagLib.Mpeg4.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class M4aFormatTest : IFormatTest
{
	class Mpeg4TestFile : File
	{
		public Mpeg4TestFile (string path) : base (path)
		{

		}

		public new List<IsoUserDataBox> UdtaBoxes => base.UdtaBoxes;
	}

	static readonly string sample_file = TestPath.Samples + "sample.m4a";
	static readonly string sample_file_rg = TestPath.Samples + "sample_replaygain.m4a";
	static readonly string tmp_file = TestPath.SamplesTmp + "tmpwrite.m4a";
	static readonly string aac_broken_tags = TestPath.Samples + "bgo_658920.m4a";
	static TagLib.File file;

	[ClassInitialize]
	public static void Init (TestContext testContext)
	{
		file = TagLib.File.Create (sample_file);
	}

	[TestMethod]
	public void AppleTags_MoreTests ()
	{
		// This tests that a 'text' atom inside an 'stsd' atom is parsed correctly
		// We just ensure that this does not throw an exception. I don't know how to
		// verify the content is correct.
		TagLib.File.Create (TestPath.Samples + "apple_tags.m4a");
	}


	[TestMethod]
	public void bgo_676934 ()
	{
		// This file contains an atom which says its 800MB in size
		var file = TagLib.File.Create (TestPath.Samples + "bgo_676934.m4a");
		Assert.IsTrue (file.CorruptionReasons.Any (), "#1");
	}


	[TestMethod]
	public void bgo_701689 ()
	{
		// This file contains a musicbrainz recording id "883821fc-9bbc-4e04-be79-b4b12c4c4a4e"
		// This case also handles bgo #701690 as a proper value for the tag must be returned
		var file = TagLib.File.Create (TestPath.Samples + "bgo_701689.m4a");
		Assert.AreEqual ("883821fc-9bbc-4e04-be79-b4b12c4c4a4e", file.Tag.MusicBrainzRecordingId, "#1");
	}

	[TestMethod]
	public void ReadAppleAacTags ()
	{
		var file = new Mpeg4TestFile (aac_broken_tags);
		Assert.AreEqual (2, file.UdtaBoxes.Count, "#1");

		var first = file.UdtaBoxes[0];
		Assert.AreEqual (1, first.Children.Count (), "#2");

		Assert.IsInstanceOfType<AppleAdditionalInfoBox> (first.Children.First ());
		var child = (AppleAdditionalInfoBox)first.Children.First ();
		Assert.AreEqual ((ReadOnlyByteVector)"name", child.BoxType, "#3");
		Assert.AreEqual (0, child.Data.Count, "#4");
	}

	[TestMethod]
	public void ReadAudioProperties ()
	{
		StandardTests.ReadAudioProperties (file);
	}

	[TestMethod]
	public void ReadTags ()
	{
		Assert.AreEqual ("M4A album", file.Tag.Album);
		Assert.AreEqual ("M4A artist", file.Tag.FirstPerformer);
		Assert.AreEqual ("M4A comment", file.Tag.Comment);
		Assert.AreEqual ("Acid Punk", file.Tag.FirstGenre);
		Assert.AreEqual ("M4A title", file.Tag.Title);
		Assert.AreEqual (6u, file.Tag.Track);
		//Assert.AreEqual(7u, file.Tag.TrackCount);
		Assert.AreEqual (1234u, file.Tag.Year);
	}

	[TestMethod]
	public void ReadReplayGain()
	{
		var fileWithRg = TagLib.File.Create (sample_file_rg);
		Assert.AreEqual(-1.43, fileWithRg.Tag.ReplayGainTrackGain, 0.01);
	}

	[TestMethod]
	public void WriteStandardTags ()
	{
		StandardTests.WriteStandardTags (sample_file, tmp_file, StandardTests.TestTagLevel.Medium);
	}

	[TestMethod]
	public void WriteExtendedTags ()
	{
		ExtendedTests.WriteExtendedTags (sample_file, tmp_file);
	}

	[TestMethod]
	public void TestCorruptionResistance ()
	{
		StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.m4a");
	}
}
