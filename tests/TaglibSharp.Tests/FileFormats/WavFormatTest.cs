using NUnit.Framework;
using TagLib;

using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats
{
	[TestFixture]
	public class WavFormatTest : IFormatTest
	{
		static readonly string sample_file = TestPath.Samples + "sample.wav";
		static readonly string sample_picture = TestPath.Samples + "sample_gimp.gif";
		static readonly string sample_other = TestPath.Samples + "apple_tags.m4a";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite.wav";
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
			ClassicAssert.AreEqual (2000, file.Properties.Duration.TotalMilliseconds);
			ClassicAssert.AreEqual (16, file.Properties.BitsPerSample);
			ClassicAssert.AreEqual (706, file.Properties.AudioBitrate);
			ClassicAssert.AreEqual (1, file.Properties.AudioChannels);
		}

		[Test]
		public void ReadTags ()
		{
			ClassicAssert.AreEqual ("Artist", file.Tag.FirstPerformer);
			ClassicAssert.AreEqual ("yepa", file.Tag.Comment);
			ClassicAssert.AreEqual ("Genre", file.Tag.FirstGenre);
			ClassicAssert.AreEqual ("Album", file.Tag.Album);
			ClassicAssert.AreEqual ("Title", file.Tag.Title);
			ClassicAssert.AreEqual (2009, file.Tag.Year);
			ClassicAssert.IsNull (file.Tag.FirstComposer);
			ClassicAssert.IsNull (file.Tag.Conductor);
			ClassicAssert.IsNull (file.Tag.Copyright);
		}

		[Test]
		public void ReadPictures ()
		{
			var pics = file.Tag.Pictures;
			ClassicAssert.AreEqual (PictureType.FrontCover, pics[0].Type);
			ClassicAssert.AreEqual ("image/jpeg", pics[0].MimeType);
			ClassicAssert.AreEqual (10210, pics[0].Data.Count);
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
		public void WriteStandardTags ()
		{
			StandardTests.WriteStandardTags (sample_file, tmp_file, StandardTests.TestTagLevel.Medium);
		}

		[Test]
		public void RemoveStandardTags ()
		{
			StandardTests.RemoveStandardTags (sample_file, tmp_file);
		}

		[Test]
		public void TestCorruptionResistance ()
		{
			StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.mkv");
		}
	}
}
