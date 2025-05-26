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
	public class JpegNikon2Test
	{
		static readonly string sample_file = TestPath.Samples + "sample_nikon2.jpg";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite_nikon2.jpg";

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
			Assert.IsTrue (file is TagLib.Jpeg.File, "not a Jpeg file");

			Assert.AreEqual (contained_types, file.TagTypes);
			Assert.AreEqual (contained_types, file.TagTypesOnDisk);
		}

		public void CheckExif (File file)
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;

			Assert.IsNotNull (tag, "tag");

			var exif_ifd = tag.Structure.GetEntry (0, IFDEntryTag.ExifIFD) as SubIFDEntry;
			Assert.IsNotNull (exif_ifd, "Exif IFD");

			Assert.AreEqual ("NIKON CORPORATION", tag.Make);
			Assert.AreEqual ("NIKON D70s", tag.Model);
			Assert.AreEqual (null, tag.ISOSpeedRatings, "ISOSpeedRatings");
			Assert.AreEqual (1.0d / 180.0d, tag.ExposureTime);
			Assert.AreEqual (6.7d, tag.FNumber);
			Assert.AreEqual (50.0d, tag.FocalLength);
			Assert.AreEqual (new DateTime (2007, 01, 01, 12, 19, 34), tag.DateTime);
			Assert.AreEqual (new DateTime (2007, 01, 01, 13, 19, 34), tag.DateTimeDigitized);
			Assert.AreEqual (new DateTime (2007, 01, 01, 12, 19, 34), tag.DateTimeOriginal);
		}


		public void CheckMakerNote (File file)
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			Assert.IsNotNull (tag, "tag");

			var makernote_ifd =
				tag.ExifIFD.GetEntry (0, (ushort)ExifEntryTag.MakerNote) as MakernoteIFDEntry;

			Assert.IsNotNull (makernote_ifd, "makernote ifd");
			Assert.AreEqual (MakernoteType.Nikon3, makernote_ifd.MakernoteType);

			var structure = makernote_ifd.Structure;
			Assert.IsNotNull (structure, "structure");
			{
				var entry = structure.GetEntry (0, 0x01) as UndefinedIFDEntry;
				Assert.IsNotNull (entry);
				var read_bytes = entry.Data;
				var expected_bytes = new ByteVector (new byte[] { 48, 50, 49, 48 });

				Assert.AreEqual (expected_bytes.Count, read_bytes.Count);
				for (int i = 0; i < expected_bytes.Count; i++)
					Assert.AreEqual (expected_bytes[i], read_bytes[i]);
			}
			{
				var entry = structure.GetEntry (0, 0x04) as StringIFDEntry;
				Assert.IsNotNull (entry, "entry 0x04");
				Assert.AreEqual ("FINE   ", entry.Value);
			}
			{
				var entry = structure.GetEntry (0, 0x08) as StringIFDEntry;
				Assert.IsNotNull (entry, "entry 0x08");
				Assert.AreEqual ("NORMAL      ", entry.Value);
			}
			{
				var entry = structure.GetEntry (0, 0x92) as SShortIFDEntry;
				Assert.IsNotNull (entry, "entry 0x92");
				Assert.AreEqual (0, entry.Value);
			}
			{
				var entry = structure.GetEntry (0, 0x9A) as RationalArrayIFDEntry;
				Assert.IsNotNull (entry, "entry 0x9A");
				var values = entry.Values;

				Assert.IsNotNull (values, "values of entry 0x9A");
				Assert.AreEqual (2, values.Length);
				Assert.AreEqual (78.0d / 10.0d, (double)values[0]);
				Assert.AreEqual (78.0d / 10.0d, (double)values[1]);
			}
		}

		public void CheckXMP (File file)
		{
			string[] keywords = {
				"Nikon D70s",
				"Food",
				"2007",
				"2006",
				"Neujahr",
				"Sylvester"
			};

			var tag = file.GetTag (TagTypes.XMP) as XmpTag;

			Assert.IsNotNull (tag, "tag");

			Assert.AreEqual (keywords, tag.Keywords);
		}

		public void CheckProperties (File file)
		{
			Assert.AreEqual (3008, file.Properties.PhotoWidth);
			Assert.AreEqual (2000, file.Properties.PhotoHeight);
			Assert.AreEqual (96, file.Properties.PhotoQuality);
		}
	}
}
