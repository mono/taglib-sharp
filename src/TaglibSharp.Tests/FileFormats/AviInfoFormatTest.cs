using System;
using NUnit.Framework;
using TagLib;

namespace TaglibSharp.Tests.FileFormats
{
	[TestFixture]
	public class AviInfoFormatTest : IFormatTest
	{
		static readonly string sample_file = TestPath.Samples + "sampleinfo.avi";
		static readonly string tmp_file = TestPath.Samples + "tmpwriteinfo.avi";
		File file;

		[OneTimeSetUp]
		public void Init ()
		{
			file = File.Create (sample_file);
		}

		[Test]
		public void ReadAudioProperties ()
		{
			Assert.AreEqual (0, file.Properties.AudioSampleRate);
			Assert.AreEqual (0, file.Properties.Duration.Seconds);
		}

		[Test]
		public void ReadTags ()
		{
			Assert.AreEqual ("13:01:14:21", file.Tag.Timecode);
			Assert.AreEqual ("30", file.Tag.TimecodeFrequency);
			Assert.AreEqual (new DateTime(2020, 10, 6, 14, 1, 21), file.Tag.DateTagged);
			Assert.AreEqual ("Qualisys Tracker Manager 2020.3 (build 5882)", file.Tag.Software);
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
