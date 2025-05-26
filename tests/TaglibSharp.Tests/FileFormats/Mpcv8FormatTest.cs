using NUnit.Framework;
using TagLib;

using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats
{
	[TestFixture]
	public class MpcV8FormatTest : IFormatTest
	{
		static readonly string sample_file = TestPath.Samples + "sample_v8.mpc";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite_v8.mpc";
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
			ClassicAssert.AreEqual ("Mpc Album", file.Tag.Album);
			ClassicAssert.AreEqual ("Mpc Artist", file.Tag.FirstPerformer);
			ClassicAssert.AreEqual ("Mpc Comment", file.Tag.Comment);
			ClassicAssert.AreEqual ("Pop", file.Tag.FirstGenre);
			ClassicAssert.AreEqual ("Mpc Title", file.Tag.Title);
			ClassicAssert.AreEqual (1, file.Tag.Track);
			ClassicAssert.AreEqual (10, file.Tag.TrackCount);
			ClassicAssert.AreEqual (2016, file.Tag.Year);
		}

		[Test]
		public void WriteStandardTags ()
		{
			StandardTests.WriteStandardTags (sample_file, tmp_file);
		}


		[Test]
		public void WriteStandardPictures ()
		{
			StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.None, StandardTests.TestTagLevel.Normal);
		}

		[Test]
		[Ignore ("PictureLazy not supported yet")]
		public void WriteStandardPicturesLazy ()
		{
			StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy, StandardTests.TestTagLevel.Normal);
		}


		[Test]
		public void TestCorruptionResistance ()
		{
			StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.mpc");
		}
	}
}
