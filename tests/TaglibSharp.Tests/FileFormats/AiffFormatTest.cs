using NUnit.Framework;
using TagLib;

using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats
{
	[TestFixture]
	public class AiffFormatTest : IFormatTest
	{
		static readonly string sample_file = TestPath.Samples + "sample.aif";
		static readonly string corrupt_file = TestPath.Samples + "corrupta.aif";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite.aif";
		File file;

		[OneTimeSetUp]
		public void Init ()
		{
			file = File.Create (sample_file);
		}

		[Test]
		public void ReadAudioProperties ()
		{
			ClassicAssert.AreEqual (44100, file.Properties.AudioSampleRate);
			ClassicAssert.AreEqual (2, file.Properties.Duration.Seconds);
		}

		[Test]
		public void ReadTags ()
		{
			ClassicAssert.AreEqual ("Aiff Album", file.Tag.Album);
			ClassicAssert.AreEqual ("Aiff Artist", file.Tag.FirstPerformer);
			ClassicAssert.AreEqual ("Aiff Comment", file.Tag.Comment);
			ClassicAssert.AreEqual ("Blues", file.Tag.FirstGenre);
			ClassicAssert.AreEqual ("Aiff Title", file.Tag.Title);
			ClassicAssert.AreEqual (5, file.Tag.Track);
			ClassicAssert.AreEqual (10, file.Tag.TrackCount);

			// sample.aif contains a TDAT (and no TYER) with 2009 in it, but TDAT
			// is supposed to contain MMDD - so the following should not be equal
			ClassicAssert.AreNotEqual (2009, file.Tag.Year);
		}

		[Test]
		public void WriteStandardTags ()
		{
			StandardTests.WriteStandardTags (sample_file, tmp_file);
		}

		[Test]
		public void WriteExtendedTags ()
		{
			ExtendedTests.WriteExtendedTags (sample_file, tmp_file);
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
			StandardTests.TestCorruptionResistance (corrupt_file);
		}
	}
}
