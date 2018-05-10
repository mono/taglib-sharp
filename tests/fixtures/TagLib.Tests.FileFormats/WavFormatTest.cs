using System;
using NUnit.Framework;

namespace TagLib.Tests.FileFormats
{   
	[TestFixture]
	public class WavFormatTest : IFormatTest
	{
		private static string sample_file = TestPath.Samples + "sample.wav";
		private static string sample_picture = TestPath.Samples + "sample_gimp.gif";
		private static string sample_other = TestPath.Samples + "apple_tags.m4a";
		private static string tmp_file = TestPath.Samples + "tmpwrite.wav";
		private File file;
		

		[OneTimeSetUp]
		public void Init()
		{
			file = File.Create(sample_file);
		}
	

		[Test]
		public void ReadAudioProperties()
		{
			Assert.AreEqual(44100, file.Properties.AudioSampleRate);
			Assert.AreEqual(2000, file.Properties.Duration.TotalMilliseconds);
			Assert.AreEqual(16, file.Properties.BitsPerSample);
			Assert.AreEqual(706, file.Properties.AudioBitrate);
			Assert.AreEqual(1, file.Properties.AudioChannels);
		}


		[Test]
		public void ReadTags()
		{
			Assert.AreEqual("Artist", file.Tag.FirstPerformer);
			Assert.AreEqual("yepa", file.Tag.Comment);
			Assert.AreEqual("Genre", file.Tag.FirstGenre);
			Assert.AreEqual("Album", file.Tag.Album);
			Assert.AreEqual("Title", file.Tag.Title);
			Assert.AreEqual(2009, file.Tag.Year);
			Assert.IsNull(file.Tag.FirstComposer);
			Assert.IsNull(file.Tag.Conductor);
			Assert.IsNull(file.Tag.Copyright);
		}


		[Test]
		public void ReadPictures()
		{
			var pics = file.Tag.Pictures;
			Assert.AreEqual(PictureType.FrontCover, pics[0].Type);
			Assert.AreEqual("image/jpeg", pics[0].MimeType);
			Assert.AreEqual(10210, pics[0].Data.Count);
		}

		[Test]
		public void WritePictures()
		{
			if (System.IO.File.Exists(tmp_file))
				System.IO.File.Delete(tmp_file);
			File file = null;
			try
			{
				System.IO.File.Copy(sample_file, tmp_file);
				file = File.Create(tmp_file);
			}
			finally { }
			Assert.NotNull(file);

			var pics = file.Tag.Pictures;
			Assert.AreEqual(1, pics.Length);

			// Insert new picture
			Array.Resize(ref pics, 4);
			pics[0].Description = "TEST description 0";
			pics[1] = new Picture(sample_picture);
			pics[1].Type = PictureType.BackCover;
			pics[1].Description = "TEST description 1";
			pics[2] = new Picture(sample_other);
			pics[2].Description = "TEST description 2";
			pics[3] = new Picture(sample_picture);
			pics[3].Type = PictureType.Other;
			pics[3].Description = "TEST description 3";
			file.Tag.Pictures = pics;

			file.Save();

			// Read back the Matroska-specific tags 
			file = File.Create(tmp_file);
			Assert.NotNull(file);
			pics = file.Tag.Pictures;

			Assert.AreEqual(4, pics.Length);

			Assert.AreEqual("image/jpeg", pics[0].MimeType);
			Assert.AreEqual(PictureType.FrontCover, pics[0].Type);
			Assert.AreEqual(10210, pics[0].Data.Count);

			// Filename has been changed to keep the PictureType information
			Assert.AreEqual(PictureType.BackCover, pics[1].Type);
			Assert.AreEqual("TEST description 1", pics[1].Description);
			Assert.AreEqual("image/gif", pics[1].MimeType);
			Assert.AreEqual(73, pics[1].Data.Count);

			Assert.AreEqual("TEST description 2", pics[2].Description);
			Assert.AreEqual("audio/mp4", pics[2].MimeType);
			Assert.AreEqual(PictureType.NotAPicture, pics[2].Type);
			Assert.AreEqual(102400, pics[2].Data.Count);

			Assert.AreEqual(PictureType.Other, pics[3].Type);
			Assert.AreEqual("TEST description 3", pics[3].Description);
			Assert.AreEqual("image/gif", pics[3].MimeType);
			Assert.AreEqual(73, pics[3].Data.Count);
		}


		[Test]
		public void WriteStandardTags ()
		{
			StandardTests.WriteStandardTags (sample_file, tmp_file, StandardTests.TestTagLevel.Medium);
		}

		[Test]
		public void RemoveStandardTags()
		{
			StandardTests.RemoveStandardTags(sample_file, tmp_file);
		}

		[Test]
		public void TestCorruptionResistance()
		{
			StandardTests.TestCorruptionResistance (TestPath.Samples + "corrupt/a.mkv");
		}
	}
}
