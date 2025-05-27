using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class AiffFormatTest : IFormatTest
{
	static readonly string sample_file = TestPath.Samples + "sample.aif";
	static readonly string corrupt_file = TestPath.Samples + "corrupta.aif";
	static readonly string tmp_file = TestPath.SamplesTmp + "tmpwrite.aif";
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
		Assert.AreEqual (2, file.Properties.Duration.Seconds);
	}

	[TestMethod]
	public void ReadTags ()
	{
		Assert.AreEqual ("Aiff Album", file.Tag.Album);
		Assert.AreEqual ("Aiff Artist", file.Tag.FirstPerformer);
		Assert.AreEqual ("Aiff Comment", file.Tag.Comment);
		Assert.AreEqual ("Blues", file.Tag.FirstGenre);
		Assert.AreEqual ("Aiff Title", file.Tag.Title);
		Assert.AreEqual (5u, file.Tag.Track);
		Assert.AreEqual (10u, file.Tag.TrackCount);

		// sample.aif contains a TDAT (and no TYER) with 2009 in it, but TDAT
		// is supposed to contain MMDD - so the following should not be equal
		Assert.AreNotEqual (2009u, file.Tag.Year);
	}

	[TestMethod]
	public void WriteStandardTags ()
	{
		StandardTests.WriteStandardTags (sample_file, tmp_file);
	}

	[TestMethod]
	public void WriteExtendedTags ()
	{
		ExtendedTests.WriteExtendedTags (sample_file, tmp_file);
	}

	[TestMethod]
	public void WriteStandardPictures ()
	{
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.None);
	}

	[TestMethod]
	public void WriteStandardPicturesLazy ()
	{
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy);
	}

	[TestMethod]
	public void TestCorruptionResistance ()
	{
		StandardTests.TestCorruptionResistance (corrupt_file);
	}
}
