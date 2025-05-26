using NUnit.Framework;
using TagLib;

using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats
{
	[TestFixture]
	public class Id3BothFormatTest : IFormatTest
	{
		static readonly string sample_file = TestPath.Samples + "sample_both.mp3";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite_both.mp3";
		File file;

		[OneTimeSetUp]
		public void Init ()
		{
			file = File.Create (sample_file);
		}

		[Test]
		public void ReadAudioProperties ()
		{
			StandardTests.ReadAudioProperties (file);
		}

		[Test]
		public void ReadTags ()
		{
			ClassicAssert.AreEqual ("MP3 album v2", file.Tag.Album);
			ClassicAssert.AreEqual ("MP3 artist", file.Tag.FirstPerformer);
			ClassicAssert.AreEqual ("MP3 comment v2", file.Tag.Comment);
			ClassicAssert.AreEqual ("Acid Punk", file.Tag.FirstGenre);
			ClassicAssert.AreEqual ("MP3 title v2", file.Tag.Title);
			ClassicAssert.AreEqual (6, file.Tag.Track);
			ClassicAssert.AreEqual (7, file.Tag.TrackCount);
			ClassicAssert.AreEqual (1234, file.Tag.Year);
		}

		[Test]
		public void FirstTag ()
		{
			ClassicAssert.AreEqual ("MP3 title v2", file.GetTag (TagTypes.Id3v2).Title);
			ClassicAssert.AreEqual ("MP3 album v2", file.GetTag (TagTypes.Id3v2).Album);
			ClassicAssert.AreEqual ("MP3 comment v2", file.GetTag (TagTypes.Id3v2).Comment);
			ClassicAssert.AreEqual (1234, (int)file.GetTag (TagTypes.Id3v2).Year);
			ClassicAssert.AreEqual (6, (int)file.GetTag (TagTypes.Id3v2).Track);
			ClassicAssert.AreEqual (7, (int)file.GetTag (TagTypes.Id3v2).TrackCount);
		}

		[Test]
		public void SecondTag ()
		{
			ClassicAssert.AreEqual ("MP3 title", file.GetTag (TagTypes.Id3v1).Title);
			ClassicAssert.AreEqual ("MP3 album", file.GetTag (TagTypes.Id3v1).Album);
			ClassicAssert.AreEqual ("MP3 comment", file.GetTag (TagTypes.Id3v1).Comment);
			ClassicAssert.AreEqual ("MP3 artist", file.GetTag (TagTypes.Id3v1).FirstPerformer);
			ClassicAssert.AreEqual (1235, (int)file.GetTag (TagTypes.Id3v1).Year);
			ClassicAssert.AreEqual (6, (int)file.GetTag (TagTypes.Id3v1).Track);
			ClassicAssert.AreEqual (0, (int)file.GetTag (TagTypes.Id3v1).TrackCount);
		}

		[Test]
		public void WriteStandardTags ()
		{
			StandardTests.WriteStandardTags (sample_file, tmp_file);
		}

		[Test]
		public void WriteStandardPictures ()
		{
			StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.None);
		}

		[Test]
		public void WriteStandardPicturesLazy ()
		{
			StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy);
		}

		[Test]
		public void TestCorruptionResistance ()
		{
			StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.mp3");
		}

		[Test]
		public void TestRemoveTags ()
		{
			string file_name = TestPath.Samples + "remove_tags.mp3";
			ByteVector.UseBrokenLatin1Behavior = true;
			var file = File.Create (file_name);
			ClassicAssert.AreEqual (TagTypes.Id3v1 | TagTypes.Id3v2 | TagTypes.Ape, file.TagTypesOnDisk);

			file.RemoveTags (TagTypes.Id3v1);
			ClassicAssert.AreEqual (TagTypes.Id3v2 | TagTypes.Ape, file.TagTypes);

			file = File.Create (file_name);
			file.RemoveTags (TagTypes.Id3v2);
			ClassicAssert.AreEqual (TagTypes.Id3v1 | TagTypes.Ape, file.TagTypes);

			file = File.Create (file_name);
			file.RemoveTags (TagTypes.Ape);
			ClassicAssert.AreEqual (TagTypes.Id3v1 | TagTypes.Id3v2, file.TagTypes);

			file = File.Create (file_name);
			file.RemoveTags (TagTypes.Xiph);
			ClassicAssert.AreEqual (TagTypes.Id3v1 | TagTypes.Id3v2 | TagTypes.Ape, file.TagTypes);

			file = File.Create (file_name);
			file.RemoveTags (TagTypes.AllTags);
			ClassicAssert.AreEqual (TagTypes.None, file.TagTypes);
		}

		[Test]
		public void TestCreateId3Tags ()
		{
			string tempFile = TestPath.Samples + "tmpwrite_sample_createid3tags.mp3";

			System.IO.File.Copy (sample_file, tempFile, true);

			// Remove All Tags first
			var file = File.Create (tempFile);
			file.RemoveTags (TagTypes.AllTags);
			file.Save ();

			// No TagTypes should exist
			TagLib.Mpeg.AudioFile.CreateID3Tags = false;
			file = File.Create (tempFile);
			ClassicAssert.AreEqual (TagTypes.None, file.TagTypes);
			file.Save ();

			// Empty TagTypes should be created
			TagLib.Mpeg.AudioFile.CreateID3Tags = true;
			file = File.Create (tempFile);
			ClassicAssert.AreEqual (TagTypes.Id3v1 | TagTypes.Id3v2, file.TagTypes);
		}
	}
}
