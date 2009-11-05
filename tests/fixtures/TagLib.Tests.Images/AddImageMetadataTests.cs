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
				Assert.IsNull (exif_tag, "Tiff Tag contained");
			}

			exif_tag = file.GetTag (TagTypes.TiffIFD, true) as IFDTag;
			Assert.IsNotNull (exif_tag, "Tiff Tag not created");

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
			Assert.IsNotNull (exif_tag, "Tiff Tag not read");

			Assert.AreEqual (test_comment, exif_tag.Comment);
			Assert.AreEqual (date_time, exif_tag.DateTime);
			Assert.AreEqual (date_time, exif_tag.DateTimeDigitized);
			Assert.AreEqual (date_time, exif_tag.DateTimeOriginal);

		}

		public static void AddGPSTest (string sample_file, string tmp_file, bool contains_tiff)
		{
			AddGPSTest (sample_file, tmp_file, contains_tiff, +53.231d, +168.19823d, 40.0d);
			AddGPSTest (sample_file, tmp_file, contains_tiff, -21.342d, +88.18232d, -39.0d);
			AddGPSTest (sample_file, tmp_file, contains_tiff, +75.12931d, -8.98712d, -10.0d);
			AddGPSTest (sample_file, tmp_file, contains_tiff, -42.1023d, -113.12432d, 1920.0d);
			AddGPSTest (sample_file, tmp_file, contains_tiff, -87.23d, +23.9743d, 0.0000123d);
			AddGPSTest (sample_file, tmp_file, contains_tiff, +72.123d, +17.432d, -0.0000089d);
		}

		private static void AddGPSTest (string sample_file, string tmp_file, bool contains_tiff, double latitude, double longitude, double altitude)
		{
			File file = Utils.CreateTmpFile (sample_file, tmp_file);
			IFDTag ifd;

			if (! contains_tiff) {
				ifd = file.GetTag (TagTypes.TiffIFD, false) as IFDTag;
				Assert.IsNull (ifd, "Tiff IFD not contained");
			}

			ifd = file.GetTag (TagTypes.TiffIFD, true) as IFDTag;
			Assert.IsNotNull (ifd, "Tiff IFD not created");

			ifd.Latitude = latitude;
			ifd.Longitude = longitude;
			ifd.Altitude = altitude;

			Assert.IsNotNull (ifd.Latitude, "Latitude");
			Assert.IsNotNull (ifd.Longitude, "Longitude");
			Assert.IsNotNull (ifd.Altitude, "Altitude");
			AssertEqualDouble (latitude, ifd.Latitude.Value, 0.00000001);
			AssertEqualDouble (longitude, ifd.Longitude.Value, 0.00000001);
			AssertEqualDouble (altitude, ifd.Altitude.Value, 0.00000001);

			// Store and reload file
			file.Save ();
			file = File.Create (tmp_file);

			ifd = file.GetTag (TagTypes.TiffIFD, false) as IFDTag;
			Assert.IsNotNull (ifd, "Tiff IFD not read");

			Assert.IsNotNull (ifd.Latitude, "Latitude");
			Assert.IsNotNull (ifd.Longitude, "Longitude");
			Assert.IsNotNull (ifd.Altitude, "Altitude");
			AssertEqualDouble (latitude, ifd.Latitude.Value, 0.00000001);
			AssertEqualDouble (longitude, ifd.Longitude.Value, 0.00000001);
			AssertEqualDouble (altitude, ifd.Altitude.Value, 0.00000001);
		}

		private static void AssertEqualDouble (double d1, double d2, double acc)
		{
			Assert.Less (d1 - acc, d2);
			Assert.Greater (d1 + acc, d2);
		}
	}
}
