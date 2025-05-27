using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class Id3V1FormatTest : IFormatTest
{
	static readonly string sample_file = TestPath.Samples + "sample_v1_only.mp3";
	static readonly string tmp_file = TestPath.SamplesTmp + "tmpwrite_v1_only.mp3";
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
		Assert.AreEqual (1234u, file.Tag.Year);
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
	public void WriteStandardPicturesLazy ()
	{
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy);
	}


	[TestMethod]
	public void TestCorruptionResistance ()
	{
	}
}
