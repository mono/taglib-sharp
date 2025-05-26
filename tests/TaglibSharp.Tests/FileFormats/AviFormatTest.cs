using NUnit.Framework;
using TagLib;

using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats
{
	[TestFixture]
	public class AviFormatTest : IFormatTest
	{
		static readonly string sample_file = TestPath.Samples + "sample.avi";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite.avi";
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
			ClassicAssert.AreEqual ("Avi album", file.Tag.Album);
			ClassicAssert.AreEqual ("Dan Drake", file.Tag.FirstAlbumArtist);
			ClassicAssert.AreEqual ("AVI artist", file.Tag.FirstPerformer);
			ClassicAssert.AreEqual ("AVI comment", file.Tag.Comment);
			ClassicAssert.AreEqual ("Brit Pop", file.Tag.FirstGenre);
			ClassicAssert.AreEqual ("AVI title", file.Tag.Title);
			ClassicAssert.AreEqual (5, file.Tag.Track);
			ClassicAssert.AreEqual (2005, file.Tag.Year);
		}

		[Test]
		public void WriteStandardTags ()
		{
			StandardTests.WriteStandardTags (sample_file, tmp_file, StandardTests.TestTagLevel.Medium);
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
		public void WriteStandardTagsID3v2 ()
		{
			StandardTests.WriteStandardTags (sample_file, tmp_file, StandardTests.TestTagLevel.Medium, TagTypes.Id3v2);
		}

		[Test]
		public void TestCorruptionResistance ()
		{
			StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.avi");
		}
	}
}
