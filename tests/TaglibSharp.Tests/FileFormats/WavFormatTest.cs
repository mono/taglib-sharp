using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class WavFormatTest : IFormatTest
{
	static readonly string sample_file = TestPath.Samples + "sample.wav";
	static readonly string sample_picture = TestPath.Samples + "sample_gimp.gif";
	static readonly string sample_other = TestPath.Samples + "apple_tags.m4a";
	static readonly string tmp_file = TestPath.SamplesTmp + "tmpwrite.wav";
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
		Assert.AreEqual (2000, file.Properties.Duration.TotalMilliseconds);
		Assert.AreEqual (16, file.Properties.BitsPerSample);
		Assert.AreEqual (706, file.Properties.AudioBitrate);
		Assert.AreEqual (1, file.Properties.AudioChannels);
	}

	[TestMethod]
	public void ReadTags ()
	{
		Assert.AreEqual ("Artist", file.Tag.FirstPerformer);
		Assert.AreEqual ("yepa", file.Tag.Comment);
		Assert.AreEqual ("Genre", file.Tag.FirstGenre);
		Assert.AreEqual ("Album", file.Tag.Album);
		Assert.AreEqual ("Title", file.Tag.Title);
		Assert.AreEqual (2009u, file.Tag.Year);
		Assert.IsNull (file.Tag.FirstComposer);
		Assert.IsNull (file.Tag.Conductor);
		Assert.IsNull (file.Tag.Copyright);
	}

	[TestMethod]
	public void ReadPictures ()
	{
		var pics = file.Tag.Pictures;
		Assert.AreEqual (PictureType.FrontCover, pics[0].Type);
		Assert.AreEqual ("image/jpeg", pics[0].MimeType);
		Assert.AreEqual (10210, pics[0].Data.Count);
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
	public void WriteStandardTags ()
	{
		StandardTests.WriteStandardTags (sample_file, tmp_file, StandardTests.TestTagLevel.Medium);
	}

	[TestMethod]
	public void RemoveStandardTags ()
	{
		StandardTests.RemoveStandardTags (sample_file, tmp_file);
	}

	[TestMethod]
	public void TestCorruptionResistance ()
	{
		StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.mkv");
	}
}
