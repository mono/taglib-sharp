using NUnit.Framework;
using System;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using TagLib.Xmp;
using File = TagLib.File;

namespace TaglibSharp.Tests.Images
{
	[TestFixture]
	public class JpegOlympus2Test
	{
		static readonly string sample_file = TestPath.Samples + "sample_olympus2.jpg";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite_olympus2.jpg";

		readonly TagTypes contained_types = TagTypes.TiffIFD | TagTypes.XMP;

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
		public void XMPRead ()
		{
			CheckXMP (file);
		}

		[Test]
		public void PropertiesRead ()
		{
			CheckProperties (file);
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
			CheckXMP (tmp);
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
			AddImageMetadataTests.AddXMPTest1 (sample_file, tmp_file, true);
		}

		[Test]
		public void AddXMP2 ()
		{
			AddImageMetadataTests.AddXMPTest2 (sample_file, tmp_file, true);
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

			ClassicAssert.AreEqual ("OLYMPUS CORPORATION", tag.Make);
			ClassicAssert.AreEqual ("C5060WZ", tag.Model);
			ClassicAssert.AreEqual (100, tag.ISOSpeedRatings);
			ClassicAssert.AreEqual (1.0d / 250.0d, tag.ExposureTime);
			ClassicAssert.AreEqual (4.8d, tag.FNumber);
			ClassicAssert.AreEqual (22.9d, tag.FocalLength);
			ClassicAssert.AreEqual (new DateTime (2009, 03, 02, 17, 05, 32), tag.DateTime);
			ClassicAssert.AreEqual (new DateTime (2009, 03, 02, 17, 05, 32), tag.DateTimeDigitized);
			ClassicAssert.AreEqual (new DateTime (2009, 03, 02, 17, 05, 32), tag.DateTimeOriginal);
		}


		public void CheckMakerNote (File file)
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			ClassicAssert.IsNotNull (tag, "tag");

			var makernote_ifd =
				tag.ExifIFD.GetEntry (0, (ushort)ExifEntryTag.MakerNote) as MakernoteIFDEntry;

			ClassicAssert.IsNotNull (makernote_ifd, "makernote ifd");
			ClassicAssert.AreEqual (MakernoteType.Olympus1, makernote_ifd.MakernoteType);

			var structure = makernote_ifd.Structure;
			ClassicAssert.IsNotNull (structure, "structure");
			{
				var entry = structure.GetEntry (0, 0x0200) as LongArrayIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x0200");
				uint[] values = entry.Values;

				ClassicAssert.IsNotNull (values, "values of entry 0x0200");
				ClassicAssert.AreEqual (3, values.Length);
				ClassicAssert.AreEqual (0, values[0]);
				ClassicAssert.AreEqual (0, values[1]);
				ClassicAssert.AreEqual (0, values[2]);
			}
			{
				var entry = structure.GetEntry (0, 0x0204) as RationalIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x0204");
				ClassicAssert.AreEqual (100.0d / 100.0d, (double)entry.Value);
			}
			{
				var entry = structure.GetEntry (0, 0x0207) as StringIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x0207");
				ClassicAssert.AreEqual ("SX756", entry.Value);
			}
		}

		public void CheckXMP (File file)
		{
			var tag = file.GetTag (TagTypes.XMP) as XmpTag;

			ClassicAssert.IsNotNull (tag, "tag");

			ClassicAssert.AreEqual (new string[] { }, tag.Keywords);
			ClassicAssert.AreEqual ("OLYMPUS CORPORATION", tag.Make);
			ClassicAssert.AreEqual ("C5060WZ", tag.Model);
			ClassicAssert.AreEqual ("Adobe Photoshop Elements 4.0", tag.Software);
		}

		public void CheckProperties (File file)
		{
			ClassicAssert.AreEqual (1280, file.Properties.PhotoWidth);
			ClassicAssert.AreEqual (958, file.Properties.PhotoHeight);
			ClassicAssert.AreEqual (69, file.Properties.PhotoQuality);
		}
	}
}
