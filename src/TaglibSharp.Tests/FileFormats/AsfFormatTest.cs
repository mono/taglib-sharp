using System;
using NUnit.Framework;
using TagLib;

namespace TaglibSharp.Tests.FileFormats
{
	[TestFixture]
	public class AsfFormatTest : IFormatTest
	{
		static readonly string sample_file = TestPath.Samples + "sample.wma";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite.wma";
		File file;

		[OneTimeSetUp]
		public void Init ()
		{
			file = File.Create (sample_file);
		}

		[Test]
		public void ReadAudioProperties ()
		{
			Assert.AreEqual (96, file.Properties.AudioBitrate);
			Assert.AreEqual (2, file.Properties.AudioChannels);
			Assert.AreEqual (44100, file.Properties.AudioSampleRate);
			// NOTE, with .net core it keeps the decimal places. So, for now, we round to match .net behavior
			Assert.AreEqual (4153, Math.Round(file.Properties.Duration.TotalMilliseconds));
			Assert.AreEqual (MediaTypes.Audio, file.Properties.MediaTypes);
		}

		[Test]
		public void ReadTags ()
		{
			Assert.AreEqual ("WMA album", file.Tag.Album);
			Assert.AreEqual ("Dan Drake", file.Tag.FirstAlbumArtist);
			Assert.AreEqual ("WMA artist", file.Tag.FirstPerformer);
			Assert.AreEqual ("WMA comment", file.Tag.Description);
			Assert.AreEqual ("Brit Pop", file.Tag.FirstGenre);
			Assert.AreEqual ("WMA title", file.Tag.Title);
			Assert.AreEqual (5, file.Tag.Track);
			Assert.AreEqual (2005, file.Tag.Year);
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
		[Ignore ("PictureLazy not supported yet")]
		public void WriteStandardPicturesLazy ()
		{
			StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy);
		}

		[Test]
		public void WriteExtendedTags ()
		{
			ExtendedTests.WriteExtendedTags (sample_file, tmp_file);
		}

		[Test]
		public void TestCorruptionResistance ()
		{
			StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.wma");
		}
	}
}
