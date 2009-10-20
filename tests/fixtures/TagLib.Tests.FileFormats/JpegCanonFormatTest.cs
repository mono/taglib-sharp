using System;
using NUnit.Framework;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.Jpeg;
using TagLib.Xmp;

namespace TagLib.Tests.FileFormats
{
    [TestFixture]
    public class JpegCanonFormatTest
    {
		private static string sample_file = "samples/sample_canon_zoombrowser.jpg";

		private Image.File file;

		private TagTypes contained_types = TagTypes.TiffIFD;

        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create(sample_file) as Image.File;
        }

		[Test]
		public void TestJpegRead()
		{
			Assert.IsTrue (file is Jpeg.File);

			Assert.AreEqual (contained_types, file.TagTypes);
			Assert.AreEqual (contained_types, file.TagTypesOnDisk);
		}

		[Test]
		public void TestExif()
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			Assert.IsFalse (tag == null);

			var exif_ifd = tag.Structure.GetEntry(0, IFDEntryTag.ExifIFD) as SubIFDEntry;
			Assert.IsFalse (exif_ifd == null);
			var exif_tag = exif_ifd.Structure;

			Assert.AreEqual ("Canon", tag.Make);
			Assert.AreEqual ("Canon EOS 400D DIGITAL", tag.Model);
			Assert.AreEqual (400, tag.ISOSpeedRatings);
			Assert.AreEqual (1.0d/200.0d, tag.ExposureTime);
			Assert.AreEqual (6.3d, tag.FNumber);
			Assert.AreEqual (180.0d, tag.FocalLength);
			Assert.AreEqual ("%test comment%", tag.Comment);
			Assert.AreEqual (new DateTime (2009, 08, 09, 19, 12, 44), tag.DateTime);
			Assert.AreEqual (new DateTime (2009, 08, 09, 19, 12, 44), tag.DateTimeDigitized);
			Assert.AreEqual (new DateTime (2009, 08, 09, 19, 12, 44), tag.DateTimeOriginal);
		}
	}
}
