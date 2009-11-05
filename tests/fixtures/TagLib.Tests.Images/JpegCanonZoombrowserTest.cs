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
    public class JpegCanonZoombrowserFormatTest
    {
		private static string sample_file = "samples/sample_canon_zoombrowser.jpg";
		private static string tmp_file = "samples/tmpwrite_canon_zoombrowser.jpg";

		private TagTypes contained_types = TagTypes.TiffIFD;

		private File file;

		[TestFixtureSetUp]
		public void Init()
		{
			file = File.Create (sample_file) as Image.File;
		}

		[Test]
		public void JpegRead () {
			CheckTags (file);
		}

		[Test]
		public void ExifRead () {
			CheckExif (file);
		}

		[Test]
		public void MakernoteRead () {
			CheckMakerNote (file);
		}

		[Test]
		public void Rewrite () {
			File tmp = Utils.CreateTmpFile (sample_file, tmp_file);
			tmp.Save ();

			tmp = File.Create (tmp_file);

			CheckTags (tmp);
			CheckExif (tmp);
			CheckMakerNote (tmp);
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

		public void CheckTags (File file) {
			Assert.IsTrue (file is Jpeg.File, "not a Jpeg file");

			Assert.AreEqual (contained_types, file.TagTypes);
			Assert.AreEqual (contained_types, file.TagTypesOnDisk);
		}

		public void CheckExif (File file) {
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			Assert.IsNotNull (tag, "Tiff Tag not contained");

			var exif_ifd = tag.Structure.GetEntry(0, IFDEntryTag.ExifIFD) as SubIFDEntry;
			Assert.IsNotNull (exif_ifd, "Exif SubIFD not contained");

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


		public void CheckMakerNote (File file) {
			IFDTag tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			Assert.IsNotNull (tag, "Tiff Tag not contained");

			var makernote_ifd =
				tag.ExifIFD.GetEntry (0, (ushort) ExifEntryTag.MakerNote) as SubIFDEntry;

			Assert.IsNotNull (makernote_ifd, "Makernote SubIFD not contained");
			Assert.AreEqual (SubIFDType.CanonMakernote, makernote_ifd.SubIFDType);

			var structure = makernote_ifd.Structure;
			Assert.IsNotNull (structure, "Makernote IFD Structure not contained");
			/* TODO Check some Markenote entries */
		}
	}
}
