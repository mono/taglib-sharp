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
	public class JpegNikon3Test
	{
		static readonly string sample_file = TestPath.Samples + "sample_nikon3.jpg";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite_nikon3.jpg";

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

			ClassicAssert.AreEqual ("NIKON CORPORATION", tag.Make);
			ClassicAssert.AreEqual ("NIKON D90", tag.Model);
			ClassicAssert.AreEqual (200, tag.ISOSpeedRatings, "ISOSpeedRatings");
			ClassicAssert.AreEqual (1.0d / 15.0d, tag.ExposureTime);
			ClassicAssert.AreEqual (5.6d, tag.FNumber);
			ClassicAssert.AreEqual (200.0d, tag.FocalLength);
			ClassicAssert.AreEqual (new DateTime (2009, 10, 21, 18, 55, 53), tag.DateTime);
			ClassicAssert.AreEqual (new DateTime (2009, 10, 21, 18, 55, 53), tag.DateTimeDigitized);
			ClassicAssert.AreEqual (new DateTime (2009, 10, 21, 18, 55, 53), tag.DateTimeOriginal);
		}


		public void CheckMakerNote (File file)
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			ClassicAssert.IsNotNull (tag, "tag");

			var makernote_ifd =
				tag.ExifIFD.GetEntry (0, (ushort)ExifEntryTag.MakerNote) as MakernoteIFDEntry;

			ClassicAssert.IsNotNull (makernote_ifd, "makernote ifd");
			ClassicAssert.AreEqual (MakernoteType.Nikon3, makernote_ifd.MakernoteType);

			var structure = makernote_ifd.Structure;
			ClassicAssert.IsNotNull (structure, "structure");
			{
				var entry = structure.GetEntry (0, 0x01) as UndefinedIFDEntry;
				ClassicAssert.IsNotNull (entry);
				var read_bytes = entry.Data;
				var expected_bytes = new ByteVector (new byte[] { 48, 50, 49, 48 });

				ClassicAssert.AreEqual (expected_bytes.Count, read_bytes.Count);
				for (int i = 0; i < expected_bytes.Count; i++)
					ClassicAssert.AreEqual (expected_bytes[i], read_bytes[i]);
			}
			{
				var entry = structure.GetEntry (0, 0x05) as StringIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x05");
				ClassicAssert.AreEqual ("AUTO        ", entry.Value);
			}
			{
				var entry = structure.GetEntry (0, 0x09) as StringIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x09");
				ClassicAssert.AreEqual ("                   ", entry.Value);
			}
			{
				var entry = structure.GetEntry (0, 0x0B) as SShortArrayIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x0B");
				var values = entry.Values;

				ClassicAssert.IsNotNull (values, "values of entry 0x0B");
				ClassicAssert.AreEqual (2, values.Length);
				ClassicAssert.AreEqual (0, values[0]);
				ClassicAssert.AreEqual (0, values[1]);
			}
			{
				var entry = structure.GetEntry (0, 0x84) as RationalArrayIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x84");
				var values = entry.Values;

				ClassicAssert.IsNotNull (values, "values of entry 0x84");
				ClassicAssert.AreEqual (4, values.Length);
				ClassicAssert.AreEqual (180.0d / 10.0d, (double)values[0]);
				ClassicAssert.AreEqual (2000.0d / 10.0d, (double)values[1]);
				ClassicAssert.AreEqual (35.0d / 10.0d, (double)values[2]);
				ClassicAssert.AreEqual (56.0d / 10.0d, (double)values[3]);
			}
		}

		public void CheckProperties (File file)
		{
			ClassicAssert.AreEqual (4288, file.Properties.PhotoWidth);
			ClassicAssert.AreEqual (2848, file.Properties.PhotoHeight);
			ClassicAssert.AreEqual (98, file.Properties.PhotoQuality);
		}
	}
}
