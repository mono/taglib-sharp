using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class OggFormatTest : TestFixtureBase, IFormatTest
{
	static readonly string sample_file = TestPath.Samples + "sample.ogg";
	static File file;

	[ClassInitialize]
	public static void Init (TestContext testContext)
	{
		file = File.Create (sample_file);
	}

	[TestMethod]
	public void ReadAudioProperties ()
	{
		StandardTests.ReadAudioProperties (file);
	}

	[TestMethod]
	public void ReadTags ()
	{
		Assert.AreEqual ("OGG album", file.Tag.Album);
		Assert.AreEqual ("OGG artist", file.Tag.FirstPerformer);
		Assert.AreEqual ("OGG comment", file.Tag.Comment);
		Assert.AreEqual ("Acid Punk", file.Tag.FirstGenre);
		Assert.AreEqual ("OGG title", file.Tag.Title);
		Assert.AreEqual (6u, file.Tag.Track);
		Assert.AreEqual (7u, file.Tag.TrackCount);
		Assert.AreEqual (1234u, file.Tag.Year);
	}

	[TestMethod]
	public void WriteStandardTags ()
	{
		var tmp_file = CreateTempFile(sample_file, "tmpwrite.ogg");
		StandardTests.WriteStandardTags (sample_file, tmp_file, StandardTests.TestTagLevel.Medium);
	}

	[TestMethod]
	public void WriteExtendedTags ()
	{
		var tmp_file = CreateTempFile(sample_file, "tmpwrite.ogg");
		ExtendedTests.WriteExtendedTags (sample_file, tmp_file);
	}

	[TestMethod]
	public void WriteStandardPictures ()
	{
		var tmp_file = CreateTempFile(sample_file, "tmpwrite.ogg");
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.None);
	}

	[TestMethod]
	[Ignore ("PictureLazy not supported yet")]
	public void WriteStandardPicturesLazy ()
	{
		var tmp_file = CreateTempFile(sample_file, "tmpwrite.ogg");
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy);
	}


	[TestMethod]
	public void TestCorruptionResistance ()
	{
		StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.ogg");
		StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/missing_flag.ogg");
	}
}
