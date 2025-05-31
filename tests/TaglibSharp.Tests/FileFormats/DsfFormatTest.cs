using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class DsfFormatTest : TestFixtureBase, IFormatTest
{
	static readonly string sample_file = TestPath.Samples + "sample.dsf";
	static File file;

	[ClassInitialize]
	public static void Init (TestContext testContext)
	{
		file = File.Create (sample_file);
	}

	[TestMethod]
	public void ReadAudioProperties ()
	{
		Assert.AreEqual (2822400, file.Properties.AudioSampleRate);
	}

	[TestMethod]
	public void ReadTags ()
	{
		Assert.AreEqual ("Dsf Album", file.Tag.Album);
		Assert.AreEqual ("Dsf Artist", file.Tag.FirstPerformer);
		Assert.AreEqual ("Dsf Comment", file.Tag.Comment);
		Assert.AreEqual ("Rock", file.Tag.FirstGenre);
		Assert.AreEqual ("Dsf Title", file.Tag.Title);
		Assert.AreEqual (1u, file.Tag.Track);
		Assert.AreEqual (2016u, file.Tag.Year);
	}

	[TestMethod]
	public void WriteStandardTags ()
	{
		var tmp_file = CreateTempFile(sample_file, "tmpwrite.dsf");
		StandardTests.WriteStandardTags (sample_file, tmp_file);
	}

	[TestMethod]
	public void WriteStandardPictures ()
	{
		var tmp_file = CreateTempFile(sample_file, "tmpwrite.dsf");
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.None);
	}

	[TestMethod]
	public void WriteStandardPicturesLazy ()
	{
		var tmp_file = CreateTempFile(sample_file, "tmpwrite.dsf");
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy);
	}

	[TestMethod]
	public void TestCorruptionResistance ()
	{
		StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.dsf");
	}
}
