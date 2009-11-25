using System;
using NUnit.Framework;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.Jpeg;
using TagLib.Xmp;

namespace TagLib.Tests.Images
{
	[TestFixture]
	public class JpegSonyTest
	{
		private static string sample_file = "samples/sample_sonyalpha200.jpg";
		private static string tmp_file = "samples/tmpwrite_sonyalpha200.jpg";

		private TagTypes contained_types = TagTypes.TiffIFD;

		private File file;

		[TestFixtureSetUp]
		public void Init ()
		{
			file = File.Create (sample_file);
		}

		[Test]
		public void JpegRead ()
		{
			CheckTags (file);
		}

		[Test]
		public void ExifRead ()
		{
			CheckExif (file);
		}

		[Test]
		public void Rewrite ()
		{
			File tmp = Utils.CreateTmpFile (sample_file, tmp_file);
			tmp.Save ();

			tmp = File.Create (tmp_file);

			// does not run at the moment, since XMP writing is not implemented
			//CheckTags (tmp);
			CheckExif (tmp);
			// not supported for Sony so far
			//CheckMakerNote (tmp);
		}

		[Test]
		public void AddExif ()
		{
			AddImageMetadataTests.AddExifTest (sample_file, tmp_file, true);
		}

		[Test]
		public void AddGPS ()
		{
			AddImageMetadataTests.AddGPSTest (sample_file, tmp_file, true);
		}

		public void CheckTags (File file)
		{
			Assert.IsTrue (file is Jpeg.File, "not a Jpeg file");

			Assert.AreEqual (contained_types, file.TagTypes);
			Assert.AreEqual (contained_types, file.TagTypesOnDisk);
		}

		public void CheckExif (File file)
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;

			Assert.IsNotNull (tag, "tag");

			var exif_ifd = tag.Structure.GetEntry(0, IFDEntryTag.ExifIFD) as SubIFDEntry;
			Assert.IsNotNull (exif_ifd, "Exif IFD");

			Assert.AreEqual ("SONY ", tag.Make);
			Assert.AreEqual ("DSLR-A200", tag.Model);
			Assert.AreEqual (400, tag.ISOSpeedRatings, "ISOSpeedRatings");
			Assert.AreEqual (1.0d/60.0d, tag.ExposureTime);
			Assert.AreEqual (5.6d, tag.FNumber);
			Assert.AreEqual (35.0d, tag.FocalLength);
			Assert.AreEqual (new DateTime (2009, 11, 21, 12, 39, 39), tag.DateTime);
			Assert.AreEqual (new DateTime (2009, 11, 21, 12, 39, 39), tag.DateTimeDigitized);
			Assert.AreEqual (new DateTime (2009, 11, 21, 12, 39, 39), tag.DateTimeOriginal);
		}
	}
}
