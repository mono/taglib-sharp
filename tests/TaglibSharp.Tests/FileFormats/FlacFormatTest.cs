using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class FlacFormatTest : IFormatTest
{
	static readonly string sample_file = TestPath.Samples + "sample.flac";
	static readonly string tmp_file = TestPath.SamplesTmp + "tmpwrite.flac";
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
		Assert.AreEqual ("FLAC album", file.Tag.Album);
		Assert.AreEqual ("FLAC artist", file.Tag.FirstPerformer);
		Assert.AreEqual ("FLAC comment", file.Tag.Description);
		Assert.AreEqual ("Acid Punk", file.Tag.FirstGenre);
		Assert.AreEqual ("FLAC title", file.Tag.Title);
		Assert.AreEqual (6u, file.Tag.Track);
		Assert.AreEqual (7u, file.Tag.TrackCount);
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
	[Ignore ("PictureLazy not supported yet")]
	public void WriteStandardPicturesLazy ()
	{
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy);
	}


	[TestMethod]
	public void TestGetTagType ()
	{
		try {
			file.GetTag (TagTypes.Id3v2);
		} catch (System.NullReferenceException) {
			Assert.Fail ("Should not throw System.NullReferenceException calling file.GetTag method: http://bugzilla.gnome.org/show_bug.cgi?id=572380");
		}
	}

	[TestMethod]
	public void TestCorruptionResistance ()
	{
		StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.flac");
	}

	[TestMethod]
	public void ReplayGainTest ()
	{
		string inFile = TestPath.Samples + "sample_replaygain.flac";
		string tempFile = TestPath.Samples + "tmpwrite_sample_replaygain.flac";

		var rgFile = File.Create (inFile);
		Assert.AreEqual (1.8d, rgFile.Tag.ReplayGainTrackGain);
		Assert.AreEqual (0.462341d, rgFile.Tag.ReplayGainTrackPeak);
		Assert.AreEqual (2.8d, rgFile.Tag.ReplayGainAlbumGain);
		Assert.AreEqual (0.562341d, rgFile.Tag.ReplayGainAlbumPeak);
		rgFile.Dispose ();

		System.IO.File.Copy (inFile, tempFile, true);

		rgFile = File.Create (tempFile);
		rgFile.Tag.ReplayGainTrackGain = -1;
		rgFile.Tag.ReplayGainTrackPeak = 1;
		rgFile.Tag.ReplayGainAlbumGain = 2;
		rgFile.Tag.ReplayGainAlbumPeak = 0;
		rgFile.Save ();
		rgFile.Dispose ();

		rgFile = File.Create (tempFile);
		Assert.AreEqual (-1d, rgFile.Tag.ReplayGainTrackGain);
		Assert.AreEqual (1d, rgFile.Tag.ReplayGainTrackPeak);
		Assert.AreEqual (2d, rgFile.Tag.ReplayGainAlbumGain);
		Assert.AreEqual (0d, rgFile.Tag.ReplayGainAlbumPeak);
		rgFile.Tag.ReplayGainTrackGain = double.NaN;
		rgFile.Tag.ReplayGainTrackPeak = double.NaN;
		rgFile.Tag.ReplayGainAlbumGain = double.NaN;
		rgFile.Tag.ReplayGainAlbumPeak = double.NaN;
		rgFile.Save ();
		rgFile.Dispose ();

		rgFile = File.Create (tempFile);
		Assert.AreEqual (double.NaN, rgFile.Tag.ReplayGainTrackGain);
		Assert.AreEqual (double.NaN, rgFile.Tag.ReplayGainTrackPeak);
		Assert.AreEqual (double.NaN, rgFile.Tag.ReplayGainAlbumGain);
		Assert.AreEqual (double.NaN, rgFile.Tag.ReplayGainAlbumPeak);
		rgFile.Dispose ();

		System.IO.File.Delete (tempFile);
	}
}
