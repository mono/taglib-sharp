using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class MpcV8FormatTest : IFormatTest
{
	static readonly string sample_file = TestPath.Samples + "sample_v8.mpc";
	static readonly string tmp_file = TestPath.SamplesTmp + "tmpwrite_v8.mpc";
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
		Assert.AreEqual ("Mpc Album", file.Tag.Album);
		Assert.AreEqual ("Mpc Artist", file.Tag.FirstPerformer);
		Assert.AreEqual ("Mpc Comment", file.Tag.Comment);
		Assert.AreEqual ("Pop", file.Tag.FirstGenre);
		Assert.AreEqual ("Mpc Title", file.Tag.Title);
		Assert.AreEqual (1u, file.Tag.Track);
		Assert.AreEqual (10u, file.Tag.TrackCount);
		Assert.AreEqual (2016u, file.Tag.Year);
	}

	[TestMethod]
	public void WriteStandardTags ()
	{
		StandardTests.WriteStandardTags (sample_file, tmp_file);
	}


	[TestMethod]
	public void WriteStandardPictures ()
	{
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.None, StandardTests.TestTagLevel.Normal);
	}

	[TestMethod]
	[Ignore ("PictureLazy not supported yet")]
	public void WriteStandardPicturesLazy ()
	{
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy, StandardTests.TestTagLevel.Normal);
	}


	[TestMethod]
	public void TestCorruptionResistance ()
	{
		StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.mpc");
	}
}
