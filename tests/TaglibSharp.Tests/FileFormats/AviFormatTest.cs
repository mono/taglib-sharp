using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class AviFormatTest : IFormatTest
{
	static readonly string sample_file = TestPath.Samples + "sample.avi";
	static readonly string tmp_file = TestPath.SamplesTmp + "tmpwrite.avi";
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
		Assert.AreEqual ("Avi album", file.Tag.Album);
		Assert.AreEqual ("Dan Drake", file.Tag.FirstAlbumArtist);
		Assert.AreEqual ("AVI artist", file.Tag.FirstPerformer);
		Assert.AreEqual ("AVI comment", file.Tag.Comment);
		Assert.AreEqual ("Brit Pop", file.Tag.FirstGenre);
		Assert.AreEqual ("AVI title", file.Tag.Title);
		Assert.AreEqual (5u, file.Tag.Track);
		Assert.AreEqual (2005u, file.Tag.Year);
	}

	[TestMethod]
	public void WriteStandardTags ()
	{
		StandardTests.WriteStandardTags (sample_file, tmp_file, StandardTests.TestTagLevel.Medium);
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
	public void WriteStandardTagsID3v2 ()
	{
		StandardTests.WriteStandardTags (sample_file, tmp_file, StandardTests.TestTagLevel.Medium, TagTypes.Id3v2);
	}

	[TestMethod]
	public void TestCorruptionResistance ()
	{
		StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.avi");
	}
}
