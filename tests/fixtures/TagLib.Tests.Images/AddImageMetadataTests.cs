using System;
using NUnit.Framework;

using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;

namespace TagLib.Tests.Images
{
    public static class AddImageMetadataTests
    {
		public static string test_comment = "This is a TagLib# &Test?Comment%$@_ ";
		public static readonly DateTime date_time = new DateTime (2009, 10, 15, 12, 12, 59);

		public static void AddExifTest (string sample_file, string tmp_file, bool contains_exif)
		{
			File file = Utils.CreateTmpFile (sample_file, tmp_file);
			IFDTag exif_tag;

			if (! contains_exif) {
				exif_tag = file.GetTag (TagTypes.TiffIFD, false) as IFDTag;
				Assert.IsTrue (exif_tag == null);
			}

			exif_tag = file.GetTag (TagTypes.TiffIFD, true) as IFDTag;
			Assert.IsFalse (exif_tag == null);

			exif_tag.Comment = test_comment;
			exif_tag.DateTime = date_time;
			exif_tag.DateTimeDigitized = date_time;

			Assert.AreEqual (test_comment, exif_tag.Comment);
			Assert.AreEqual (date_time, exif_tag.DateTime);
			Assert.AreEqual (date_time, exif_tag.DateTimeDigitized);
			Assert.AreEqual (date_time, exif_tag.DateTimeOriginal);

			// Store and reload file
			file.Save ();
			file = File.Create (tmp_file);

			exif_tag = file.GetTag (TagTypes.TiffIFD, false) as IFDTag;
			Assert.IsFalse (exif_tag == null);

			Assert.AreEqual (test_comment, exif_tag.Comment);
			Assert.AreEqual (date_time, exif_tag.DateTime);
			Assert.AreEqual (date_time, exif_tag.DateTimeDigitized);
			Assert.AreEqual (date_time, exif_tag.DateTimeOriginal);

		}

		public static void AddGPSTest (string sample_file, string tmp_file, bool contains_tiff)
		{
			AddGPSTest (sample_file, tmp_file, contains_tiff, +53.231d, +8.98712d);
			AddGPSTest (sample_file, tmp_file, contains_tiff, -21.342d, +88.18232d);
			AddGPSTest (sample_file, tmp_file, contains_tiff, +75.12931d, +168.19823d);
			AddGPSTest (sample_file, tmp_file, contains_tiff, -42.1023d, +113.12432d);
		}

		private static void AddGPSTest (string sample_file, string tmp_file, bool contains_tiff, double latitude, double longitude)
		{
			File file = Utils.CreateTmpFile (sample_file, tmp_file);
			IFDTag ifd;

			if (! contains_tiff) {
				ifd = file.GetTag (TagTypes.TiffIFD, false) as IFDTag;
				Assert.IsTrue (ifd == null);
			}

			ifd = file.GetTag (TagTypes.TiffIFD, true) as IFDTag;
			Assert.IsFalse (ifd == null);

			ifd.Latitude = latitude;
			ifd.Longitude = longitude;

			AssertEqualDouble (latitude, ifd.Latitude, 0.00000001);
			AssertEqualDouble (longitude, ifd.Longitude, 0.00000001);

			// Store and reload file
			file.Save ();
			file = File.Create (tmp_file);

			ifd = file.GetTag (TagTypes.TiffIFD, false) as IFDTag;
			Assert.IsFalse (ifd == null);

			AssertEqualDouble (latitude, ifd.Latitude, 0.00000001);
			AssertEqualDouble (longitude, ifd.Longitude, 0.00000001);
		}

		private static void AssertEqualDouble (double d1, double d2, double acc)
		{
			Assert.Less (d1 - acc, d2);
			Assert.Greater (d1 + acc, d2);
		}
	}
}
