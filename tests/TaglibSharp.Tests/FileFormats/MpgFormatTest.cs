using NUnit.Framework;
using System;
using TagLib;

namespace TaglibSharp.Tests.FileFormats
{
	[TestFixture]
	public class MpgFormatTest : IFormatTest
	{
		static readonly string sample_file = TestPath.Samples + "Turning Lime.mpg";
		static readonly string sample_picture = TestPath.Samples + "sample_gimp.gif";
		static readonly string sample_other = TestPath.Samples + "apple_tags.m4a";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite.mpg";
		File file;


		[OneTimeSetUp]
		public void Init ()
		{
			file = File.Create (sample_file);
		}


		[Test]
		public void ReadAudioProperties ()
		{
			Assert.AreEqual (128, file.Properties.AudioBitrate);
			Assert.AreEqual (2, file.Properties.AudioChannels);
			Assert.AreEqual (44100, file.Properties.AudioSampleRate);
			// NOTE, with .net core it keeps the decimal places. So, for now, we round to match .net behavior
			Assert.AreEqual (1391, Math.Round (file.Properties.Duration.TotalMilliseconds));

		}

		[Test]
		public void ReadVideoProperties ()
		{
			Assert.AreEqual (480, file.Properties.VideoHeight);
			Assert.AreEqual (640, file.Properties.VideoWidth);
		}


		[Test]
		public void ReadTags ()
		{
			Assert.IsTrue (file.Tag.IsEmpty);
		}


		[Test]
		public void WritePictures ()
		{
			if (System.IO.File.Exists (tmp_file))
				System.IO.File.Delete (tmp_file);

			File file;
			System.IO.File.Copy (sample_file, tmp_file);
			file = File.Create (tmp_file);
			Assert.NotNull (file);

			var pics = file.Tag.Pictures;
			Assert.AreEqual (0, pics.Length);

			// Insert new picture
			Array.Resize (ref pics, 3);
			pics[0] = new Picture (sample_picture) {
				Type = PictureType.BackCover,
				Description = "TEST description 1"
			};
			pics[1] = new Picture (sample_other) {
				Description = "TEST description 2"
			};
			pics[2] = new Picture (sample_picture) {
				Type = PictureType.Other,
				Description = "TEST description 3"
			};
			file.Tag.Pictures = pics;

			file.Save ();

			// Read back the Matroska-specific tags
			file = File.Create (tmp_file);
			Assert.NotNull (file);
			pics = file.Tag.Pictures;

			Assert.AreEqual (3, pics.Length);

			// Filename has been changed to keep the PictureType information
			Assert.AreEqual (PictureType.BackCover, pics[0].Type);
			Assert.IsNull (pics[0].Filename);
			Assert.AreEqual ("TEST description 1", pics[0].Description);
			Assert.AreEqual ("image/gif", pics[0].MimeType);
			Assert.AreEqual (73, pics[0].Data.Count);

			Assert.IsNull (pics[1].Filename);
			Assert.AreEqual ("TEST description 2", pics[1].Description);
			Assert.AreEqual ("audio/mp4", pics[1].MimeType);
			Assert.AreEqual (PictureType.NotAPicture, pics[1].Type);
			Assert.AreEqual (102400, pics[1].Data.Count);

			Assert.AreEqual (PictureType.Other, pics[2].Type);
			Assert.IsNull (pics[2].Filename);
			Assert.AreEqual ("TEST description 3", pics[2].Description);
			Assert.AreEqual ("image/gif", pics[2].MimeType);
			Assert.AreEqual (73, pics[2].Data.Count);
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
		public void RemoveStandardTags ()
		{
			StandardTests.RemoveStandardTags (sample_file, tmp_file);
		}

		[Test]
		public void TestCorruptionResistance ()
		{
			StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.mpg");
		}
	}
}
