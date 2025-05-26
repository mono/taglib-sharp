using NUnit.Framework;
using System;
using TagLib;
using TagLib.IFD;
using TagLib.Xmp;

using File = TagLib.File;

namespace TaglibSharp.Tests.Images
{
	public static class AddImageMetadataTests
	{
		public static string test_comment = "This is a TagLib# &Test?Comment%$@_ ";
		public static readonly DateTime date_time = new DateTime (2009, 10, 15, 12, 12, 59);

		public static readonly string[] keywords = { "keyword 1", "§$&§%", "99 dsf" };

		public static void AddExifTest (string sample_file, string tmp_file, bool contains_exif)
		{
			var file = Utils.CreateTmpFile (sample_file, tmp_file);
			IFDTag exif_tag;

			if (!contains_exif) {
				exif_tag = file.GetTag (TagTypes.TiffIFD, false) as IFDTag;
				ClassicAssert.IsNull (exif_tag, "Tiff Tag contained");
			}

			exif_tag = file.GetTag (TagTypes.TiffIFD, true) as IFDTag;
			ClassicAssert.IsNotNull (exif_tag, "Tiff Tag not created");

			exif_tag.Comment = test_comment;
			exif_tag.DateTime = date_time;
			exif_tag.DateTimeDigitized = date_time;

			ClassicAssert.AreEqual (test_comment, exif_tag.Comment);
			ClassicAssert.AreEqual (date_time, exif_tag.DateTime);
			ClassicAssert.AreEqual (date_time, exif_tag.DateTimeDigitized);
			ClassicAssert.AreEqual (date_time, exif_tag.DateTimeOriginal);

			// Store and reload file
			file.Save ();
			file = File.Create (tmp_file);

			exif_tag = file.GetTag (TagTypes.TiffIFD, false) as IFDTag;
			ClassicAssert.IsNotNull (exif_tag, "Tiff Tag not read");

			ClassicAssert.AreEqual (test_comment, exif_tag.Comment);
			ClassicAssert.AreEqual (date_time, exif_tag.DateTime);
			ClassicAssert.AreEqual (date_time, exif_tag.DateTimeDigitized);
			ClassicAssert.AreEqual (date_time, exif_tag.DateTimeOriginal);
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

		public static void AddXMPTest1 (string sample_file, string tmp_file, bool contains_xmp)
		{
			var file = Utils.CreateTmpFile (sample_file, tmp_file);
			XmpTag xmp_tag;

			if (!contains_xmp) {
				xmp_tag = file.GetTag (TagTypes.XMP, false) as XmpTag;
				ClassicAssert.IsNull (xmp_tag, "XMP Tag contained");
			}

			xmp_tag = file.GetTag (TagTypes.XMP, true) as XmpTag;
			ClassicAssert.IsNotNull (xmp_tag, "XMP Tag not created");

			xmp_tag.Keywords = keywords;
			xmp_tag.Comment = test_comment;
			xmp_tag.Software = null;

			ClassicAssert.AreEqual (keywords, xmp_tag.Keywords);
			ClassicAssert.AreEqual (test_comment, xmp_tag.Comment);
			ClassicAssert.AreEqual (null, xmp_tag.Software);

			// Store and reload file
			file.Save ();
			file = File.Create (tmp_file);

			xmp_tag = file.GetTag (TagTypes.XMP, false) as XmpTag;
			ClassicAssert.IsNotNull (xmp_tag, "XMP Tag not read");

			ClassicAssert.AreEqual (keywords, xmp_tag.Keywords);
			ClassicAssert.AreEqual (test_comment, xmp_tag.Comment);
			ClassicAssert.AreEqual (null, xmp_tag.Software);
		}

		public static void AddXMPTest2 (string sample_file, string tmp_file, bool contains_xmp)
		{
			var file = Utils.CreateTmpFile (sample_file, tmp_file);
			XmpTag xmp_tag;

			if (!contains_xmp) {
				xmp_tag = file.GetTag (TagTypes.XMP, false) as XmpTag;
				ClassicAssert.IsNull (xmp_tag, "XMP Tag contained");
			}

			xmp_tag = file.GetTag (TagTypes.XMP, true) as XmpTag;
			ClassicAssert.IsNotNull (xmp_tag, "XMP Tag not created");

			xmp_tag.Keywords = null;
			xmp_tag.Comment = null;
			xmp_tag.Software = test_comment;

			ClassicAssert.AreEqual (new string[] { }, xmp_tag.Keywords);
			ClassicAssert.AreEqual (null, xmp_tag.Comment);
			ClassicAssert.AreEqual (test_comment, xmp_tag.Software);

			// Store and reload file
			file.Save ();
			file = File.Create (tmp_file);

			xmp_tag = file.GetTag (TagTypes.XMP, false) as XmpTag;
			ClassicAssert.IsNotNull (xmp_tag, "XMP Tag not read");

			ClassicAssert.AreEqual (new string[] { }, xmp_tag.Keywords);
			ClassicAssert.AreEqual (null, xmp_tag.Comment);
			ClassicAssert.AreEqual (test_comment, xmp_tag.Software);
		}

		public static void AddAllTest (string sample_file, string tmp_file)
		{
			var file = Utils.CreateTmpFile (sample_file, tmp_file) as TagLib.Image.File;

			ClassicAssert.IsNotNull (file, "file");

			// ensure all tags are present
			file.GetTag (TagTypes.XMP, true);
			file.GetTag (TagTypes.TiffIFD, true);

			file.ImageTag.Comment = test_comment;
			file.ImageTag.Keywords = keywords;
			file.ImageTag.Rating = 4;
			file.ImageTag.DateTime = date_time;
			file.ImageTag.Latitude = 3.0;
			file.ImageTag.Longitude = 3.0;
			file.ImageTag.Altitude = 3.0;

			ClassicAssert.AreEqual (test_comment, file.ImageTag.Comment);
			ClassicAssert.AreEqual (keywords, file.ImageTag.Keywords);
			ClassicAssert.AreEqual (4, file.ImageTag.Rating);
			ClassicAssert.AreEqual (date_time, file.ImageTag.DateTime);
			ClassicAssert.AreEqual (3.0, file.ImageTag.Latitude);
			ClassicAssert.AreEqual (3.0, file.ImageTag.Longitude);
			ClassicAssert.AreEqual (3.0, file.ImageTag.Altitude);

			// Store and reload file
			file.Save ();
			file = File.Create (tmp_file) as TagLib.Image.File;

			ClassicAssert.IsNotNull (file, "tmp file");

			ClassicAssert.AreEqual (test_comment, file.ImageTag.Comment);
			ClassicAssert.AreEqual (keywords, file.ImageTag.Keywords);
			ClassicAssert.AreEqual (4, file.ImageTag.Rating);
			ClassicAssert.AreEqual (date_time, file.ImageTag.DateTime);
			ClassicAssert.AreEqual (3.0, file.ImageTag.Latitude);
			ClassicAssert.AreEqual (3.0, file.ImageTag.Longitude);
			ClassicAssert.AreEqual (3.0, file.ImageTag.Altitude);

			var xmp_tag = file.GetTag (TagTypes.XMP, false) as XmpTag;
			ClassicAssert.IsNotNull (xmp_tag, "XMP Tag not read");
			ClassicAssert.AreEqual (test_comment, xmp_tag.Comment);
			ClassicAssert.AreEqual (keywords, xmp_tag.Keywords);
			ClassicAssert.AreEqual (4, xmp_tag.Rating);

			var ifd_tag = file.GetTag (TagTypes.TiffIFD, false) as IFDTag;
			ClassicAssert.IsNotNull (ifd_tag, "Tiff Tag not read");
			ClassicAssert.AreEqual (test_comment, ifd_tag.Comment);
			ClassicAssert.AreEqual (date_time, ifd_tag.DateTime);
			ClassicAssert.AreEqual (3.0, ifd_tag.Latitude);
			ClassicAssert.AreEqual (3.0, ifd_tag.Longitude);
			ClassicAssert.AreEqual (3.0, ifd_tag.Altitude);
		}

		static void AddGPSTest (string sample_file, string tmp_file, bool contains_tiff, double latitude, double longitude, double altitude)
		{
			var file = Utils.CreateTmpFile (sample_file, tmp_file);
			IFDTag ifd;

			if (!contains_tiff) {
				ifd = file.GetTag (TagTypes.TiffIFD, false) as IFDTag;
				ClassicAssert.IsNull (ifd, "Tiff IFD not contained");
			}

			ifd = file.GetTag (TagTypes.TiffIFD, true) as IFDTag;
			ClassicAssert.IsNotNull (ifd, "Tiff IFD not created");

			ifd.Latitude = latitude;
			ifd.Longitude = longitude;
			ifd.Altitude = altitude;

			ClassicAssert.IsNotNull (ifd.Latitude, "Latitude");
			ClassicAssert.IsNotNull (ifd.Longitude, "Longitude");
			ClassicAssert.IsNotNull (ifd.Altitude, "Altitude");
			ClassicAssertEqualDouble (latitude, ifd.Latitude.Value, 0.00000001);
			ClassicAssertEqualDouble (longitude, ifd.Longitude.Value, 0.00000001);
			ClassicAssertEqualDouble (altitude, ifd.Altitude.Value, 0.00000001);

			// Store and reload file
			file.Save ();
			file = File.Create (tmp_file);

			ifd = file.GetTag (TagTypes.TiffIFD, false) as IFDTag;
			ClassicAssert.IsNotNull (ifd, "Tiff IFD not read");

			ClassicAssert.IsNotNull (ifd.Latitude, "Latitude");
			ClassicAssert.IsNotNull (ifd.Longitude, "Longitude");
			ClassicAssert.IsNotNull (ifd.Altitude, "Altitude");
			ClassicAssertEqualDouble (latitude, ifd.Latitude.Value, 0.00000001);
			ClassicAssertEqualDouble (longitude, ifd.Longitude.Value, 0.00000001);
			ClassicAssertEqualDouble (altitude, ifd.Altitude.Value, 0.00000001);
		}

		static void ClassicAssertEqualDouble (double d1, double d2, double acc)
		{
			ClassicAssert.Less (d1 - acc, d2);
			ClassicAssert.Greater (d1 + acc, d2);
		}
	}
}
