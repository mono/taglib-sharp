using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class MpcFormatTest : TestFixtureBase, IFormatTest
{
	static readonly string sample_file = TestPath.Samples + "sample.mpc";
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
		Assert.AreEqual ("MPC album", file.Tag.Album);
		Assert.AreEqual ("MPC artist", file.Tag.FirstPerformer);
		Assert.AreEqual ("MPC comment", file.Tag.Comment);
		Assert.AreEqual ("Acid Punk", file.Tag.FirstGenre);
		Assert.AreEqual ("MPC title", file.Tag.Title);
		Assert.AreEqual (6u, file.Tag.Track);
		Assert.AreEqual (7u, file.Tag.TrackCount);
		Assert.AreEqual (1234u, file.Tag.Year);
	}

	[TestMethod]
	public void WriteStandardTags ()
	{
		var tmp_file = CreateTempFile(sample_file, "tmpwrite.mpc");
		StandardTests.WriteStandardTags (sample_file, tmp_file);
	}

	[TestMethod]
	public void WriteStandardPictures ()
	{
		var tmp_file = CreateTempFile(sample_file, "tmpwrite.mpc");
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.None, StandardTests.TestTagLevel.Normal);
	}

	[TestMethod]
	[Ignore ("PictureLazy not supported yet")]
	public void WriteStandardPicturesLazy ()
	{
		var tmp_file = CreateTempFile(sample_file, "tmpwrite.mpc");
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy, StandardTests.TestTagLevel.Normal);
	}

	[TestMethod]
	public void TestCorruptionResistance ()
	{
		StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.mpc");
	}
}
