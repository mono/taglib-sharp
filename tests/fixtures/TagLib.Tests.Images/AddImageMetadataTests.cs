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
	}
}
