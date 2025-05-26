using NUnit.Framework;
using TagLib;

using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats
{
	[TestFixture]
	public class DsfFormatTest : IFormatTest
	{
		static readonly string sample_file = TestPath.Samples + "sample.dsf";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite.dsf";
		File file;

		[OneTimeSetUp]
		public void Init ()
		{
			file = File.Create (sample_file);
		}

		[Test]
		public void ReadAudioProperties ()
		{
			ClassicAssert.AreEqual (2822400, file.Properties.AudioSampleRate);
		}

		[Test]
		public void ReadTags ()
		{
			ClassicAssert.AreEqual ("Dsf Album", file.Tag.Album);
			ClassicAssert.AreEqual ("Dsf Artist", file.Tag.FirstPerformer);
			ClassicAssert.AreEqual ("Dsf Comment", file.Tag.Comment);
			ClassicAssert.AreEqual ("Rock", file.Tag.FirstGenre);
			ClassicAssert.AreEqual ("Dsf Title", file.Tag.Title);
			ClassicAssert.AreEqual (1, file.Tag.Track);
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
			StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.dsf");
		}
	}
}
