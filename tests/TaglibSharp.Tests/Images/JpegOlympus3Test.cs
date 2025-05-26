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
	public class JpegOlympus3Test
	{
		static readonly string sample_file = TestPath.Samples + "sample_olympus3.jpg";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite_olympus3.jpg";

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

			ClassicAssert.AreEqual ("OLYMPUS IMAGING CORP.  ", tag.Make);
			ClassicAssert.AreEqual ("E-410           ", tag.Model);
			ClassicAssert.AreEqual (100, tag.ISOSpeedRatings);
			ClassicAssert.AreEqual (1.0d / 125.0d, tag.ExposureTime);
			ClassicAssert.AreEqual (6.3d, tag.FNumber);
			ClassicAssert.AreEqual (42.0d, tag.FocalLength);
			ClassicAssert.AreEqual (new DateTime (2009, 04, 11, 19, 45, 42), tag.DateTime);
			ClassicAssert.AreEqual (new DateTime (2009, 04, 11, 19, 45, 42), tag.DateTimeDigitized);
			ClassicAssert.AreEqual (new DateTime (2009, 04, 11, 19, 45, 42), tag.DateTimeOriginal);
		}


		public void CheckMakerNote (File file)
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			ClassicAssert.IsNotNull (tag, "tag");

			var makernote_ifd =
				tag.ExifIFD.GetEntry (0, (ushort)ExifEntryTag.MakerNote) as MakernoteIFDEntry;

			ClassicAssert.IsNotNull (makernote_ifd, "makernote ifd");
			ClassicAssert.AreEqual (MakernoteType.Olympus2, makernote_ifd.MakernoteType);

			var structure = makernote_ifd.Structure;
			ClassicAssert.IsNotNull (structure, "structure");
			/*{
				var entry = structure.GetEntry (0, 0x01) as UndefinedIFDEntry;
				ClassicAssert.IsNotNull (entry);
				ByteVector read_bytes = entry.Data;
				ByteVector expected_bytes = new ByteVector (new byte [] {48, 50, 49, 48});

				ClassicAssert.AreEqual (expected_bytes.Count, read_bytes.Count);
				for (int i = 0; i < expected_bytes.Count; i++)
					ClassicAssert.AreEqual (expected_bytes[i], read_bytes[i]);
			}
			{
				var entry = structure.GetEntry (0, 0x04) as StringIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x04");
				ClassicAssert.AreEqual ("FINE   ", entry.Value);
			}
			{
				var entry = structure.GetEntry (0, 0x08) as StringIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x08");
				ClassicAssert.AreEqual ("NORMAL      ", entry.Value);
			}
			{
				var entry = structure.GetEntry (0, 0x92) as SShortIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x92");
				ClassicAssert.AreEqual (0, entry.Value);
			}
			{
				var entry = structure.GetEntry (0, 0x9A) as RationalArrayIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x9A");
				var values = entry.Values;

				ClassicAssert.IsNotNull (values, "values of entry 0x9A");
				ClassicAssert.AreEqual (2, values.Length);
				ClassicAssert.AreEqual (78.0d/10.0d, (double) values[0]);
				ClassicAssert.AreEqual (78.0d/10.0d, (double) values[1]);
			}*/
		}

		public void CheckProperties (File file)
		{
			ClassicAssert.AreEqual (3648, file.Properties.PhotoWidth);
			ClassicAssert.AreEqual (2736, file.Properties.PhotoHeight);
			ClassicAssert.AreEqual (96, file.Properties.PhotoQuality);
		}
	}
}
