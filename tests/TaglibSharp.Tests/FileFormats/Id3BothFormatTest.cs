using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class Id3BothFormatTest : IFormatTest
{
	static readonly string sample_file = TestPath.Samples + "sample_both.mp3";
	static readonly string tmp_file = TestPath.SamplesTmp + "tmpwrite_both.mp3";
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
		Assert.AreEqual ("MP3 album v2", file.Tag.Album);
		Assert.AreEqual ("MP3 artist", file.Tag.FirstPerformer);
		Assert.AreEqual ("MP3 comment v2", file.Tag.Comment);
		Assert.AreEqual ("Acid Punk", file.Tag.FirstGenre);
		Assert.AreEqual ("MP3 title v2", file.Tag.Title);
		Assert.AreEqual (6u, file.Tag.Track);
		Assert.AreEqual (7u, file.Tag.TrackCount);
		Assert.AreEqual (1234u, file.Tag.Year);
	}

	[TestMethod]
	public void FirstTag ()
	{
		Assert.AreEqual ("MP3 title v2", file.GetTag (TagTypes.Id3v2).Title);
		Assert.AreEqual ("MP3 album v2", file.GetTag (TagTypes.Id3v2).Album);
		Assert.AreEqual ("MP3 comment v2", file.GetTag (TagTypes.Id3v2).Comment);
		Assert.AreEqual (1234, (int)file.GetTag (TagTypes.Id3v2).Year);
		Assert.AreEqual (6, (int)file.GetTag (TagTypes.Id3v2).Track);
		Assert.AreEqual (7, (int)file.GetTag (TagTypes.Id3v2).TrackCount);
	}

	[TestMethod]
	public void SecondTag ()
	{
		Assert.AreEqual ("MP3 title", file.GetTag (TagTypes.Id3v1).Title);
		Assert.AreEqual ("MP3 album", file.GetTag (TagTypes.Id3v1).Album);
		Assert.AreEqual ("MP3 comment", file.GetTag (TagTypes.Id3v1).Comment);
		Assert.AreEqual ("MP3 artist", file.GetTag (TagTypes.Id3v1).FirstPerformer);
		Assert.AreEqual (1235, (int)file.GetTag (TagTypes.Id3v1).Year);
		Assert.AreEqual (6, (int)file.GetTag (TagTypes.Id3v1).Track);
		Assert.AreEqual (0, (int)file.GetTag (TagTypes.Id3v1).TrackCount);
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
		StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.mp3");
	}

	[TestMethod]
	public void TestRemoveTags ()
	{
		string file_name = TestPath.Samples + "remove_tags.mp3";
		ByteVector.UseBrokenLatin1Behavior = true;
		var file = File.Create (file_name);
		Assert.AreEqual (TagTypes.Id3v1 | TagTypes.Id3v2 | TagTypes.Ape, file.TagTypesOnDisk);

		file.RemoveTags (TagTypes.Id3v1);
		Assert.AreEqual (TagTypes.Id3v2 | TagTypes.Ape, file.TagTypes);

		file = File.Create (file_name);
		file.RemoveTags (TagTypes.Id3v2);
		Assert.AreEqual (TagTypes.Id3v1 | TagTypes.Ape, file.TagTypes);

		file = File.Create (file_name);
		file.RemoveTags (TagTypes.Ape);
		Assert.AreEqual (TagTypes.Id3v1 | TagTypes.Id3v2, file.TagTypes);

		file = File.Create (file_name);
		file.RemoveTags (TagTypes.Xiph);
		Assert.AreEqual (TagTypes.Id3v1 | TagTypes.Id3v2 | TagTypes.Ape, file.TagTypes);

		file = File.Create (file_name);
		file.RemoveTags (TagTypes.AllTags);
		Assert.AreEqual (TagTypes.None, file.TagTypes);
	}

	[TestMethod]
	public void TestCreateId3Tags ()
	{
		string tempFile = TestPath.SamplesTmp + "tmpwrite_sample_createid3tags.mp3";
        Directory.CreateDirectory (Path.GetDirectoryName (tempFile));

        System.IO.File.Copy (sample_file, tempFile, true);

		// Remove All Tags first
		var file = File.Create (tempFile);
		file.RemoveTags (TagTypes.AllTags);
		file.Save ();

		// No TagTypes should exist
		TagLib.Mpeg.AudioFile.CreateID3Tags = false;
		file = File.Create (tempFile);
		Assert.AreEqual (TagTypes.None, file.TagTypes);
		file.Save ();

		// Empty TagTypes should be created
		TagLib.Mpeg.AudioFile.CreateID3Tags = true;
		file = File.Create (tempFile);
		Assert.AreEqual (TagTypes.Id3v1 | TagTypes.Id3v2, file.TagTypes);
	}
}
