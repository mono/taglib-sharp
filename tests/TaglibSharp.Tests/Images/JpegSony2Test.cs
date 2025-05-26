using NUnit.Framework;
using System;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using File = TagLib.File;

namespace TaglibSharp.Tests.Images
{
	[TestFixture]
	public class JpegSony2Test
	{
		static readonly string sample_file = TestPath.Samples + "sample_sony2.jpg";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite_sony2.jpg";

		readonly TagTypes contained_types = TagTypes.TiffIFD;

		File file;

		[OneTimeSetUp]
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
		public void MakernoteRead ()
		{
			CheckMakerNote (file);
		}

		[Test]
		public void Rewrite ()
		{
			var tmp = Utils.CreateTmpFile (sample_file, tmp_file);
			tmp.Save ();

			tmp = File.Create (tmp_file);

			CheckTags (tmp);
			CheckExif (tmp);
			CheckMakerNote (tmp);
			CheckProperties (tmp);
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

		[Test]
		public void AddXMP1 ()
		{
			AddImageMetadataTests.AddXMPTest1 (sample_file, tmp_file, false);
		}

		[Test]
		public void AddXMP2 ()
		{
			AddImageMetadataTests.AddXMPTest2 (sample_file, tmp_file, false);
		}

		public void CheckTags (File file)
		{
			ClassicAssert.IsTrue (file is TagLib.Jpeg.File, "not a Jpeg file");

			ClassicAssert.AreEqual (contained_types, file.TagTypes);
			ClassicAssert.AreEqual (contained_types, file.TagTypesOnDisk);
		}

		public void CheckExif (File file)
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;

			ClassicAssert.IsNotNull (tag, "tag");

			var exif_ifd = tag.Structure.GetEntry (0, IFDEntryTag.ExifIFD) as SubIFDEntry;
			ClassicAssert.IsNotNull (exif_ifd, "Exif IFD");

			ClassicAssert.AreEqual ("SONY ", tag.Make);
			ClassicAssert.AreEqual ("DSLR-A700", tag.Model);
			ClassicAssert.AreEqual (400, tag.ISOSpeedRatings, "ISOSpeedRatings");
			ClassicAssert.AreEqual (1.0d / 125.0d, tag.ExposureTime);
			ClassicAssert.AreEqual (5.6d, tag.FNumber);
			ClassicAssert.AreEqual (70.0d, tag.FocalLength);
			ClassicAssert.AreEqual (new DateTime (2009, 11, 06, 20, 56, 07), tag.DateTime);
			ClassicAssert.AreEqual (new DateTime (2009, 11, 06, 20, 56, 07), tag.DateTimeDigitized);
			ClassicAssert.AreEqual (new DateTime (2009, 11, 06, 20, 56, 07), tag.DateTimeOriginal);
		}

		public void CheckMakerNote (File file)
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			ClassicAssert.IsNotNull (tag, "tag");

			var makernote_ifd =
				tag.ExifIFD.GetEntry (0, (ushort)ExifEntryTag.MakerNote) as MakernoteIFDEntry;

			ClassicAssert.IsNotNull (makernote_ifd, "makernote ifd");
			ClassicAssert.AreEqual (MakernoteType.Sony, makernote_ifd.MakernoteType);

			var structure = makernote_ifd.Structure;
			ClassicAssert.IsNotNull (structure, "structure");
			//Tag info from http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Sony.html
			//0x0102: image quality
			{
				var entry = structure.GetEntry (0, 0x0102) as LongIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x0102");
				ClassicAssert.AreEqual (5, entry.Value);
			}
			{
				var entry = structure.GetEntry (0, 0x0104) as SRationalIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x0115");
				ClassicAssert.AreEqual (0.0d, (double)entry.Value);
			}
			//0x0115: white balance
			{
				var entry = structure.GetEntry (0, 0x0115) as LongIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x0115");
				ClassicAssert.AreEqual (80, entry.Value);
			}
			//0xb026: image stabilizer
			{
				var entry = structure.GetEntry (0, 0xb026) as LongIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0xb026");
				ClassicAssert.AreEqual (1, entry.Value);
			}
		}

		public void CheckProperties (File file)
		{
			ClassicAssert.AreEqual (4272, file.Properties.PhotoWidth);
			ClassicAssert.AreEqual (2848, file.Properties.PhotoHeight);
			ClassicAssert.AreEqual (99, file.Properties.PhotoQuality);
		}
	}
}
