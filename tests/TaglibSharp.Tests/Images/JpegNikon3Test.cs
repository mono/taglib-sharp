using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;

using File = TagLib.File;

namespace TaglibSharp.Tests.Images;

[TestClass]
public class JpegNikon3Test
{
	static readonly string sample_file = TestPath.Samples + "sample_nikon3.jpg";
	static readonly string tmp_file = TestPath.SamplesTmp + "tmpwrite_nikon3.jpg";

	readonly TagTypes contained_types = TagTypes.TiffIFD;

	static File file;

	[ClassInitialize]
	public static void Init (TestContext testContext)
	{
		file = File.Create (sample_file);
	}

	[TestMethod]
	public void JpegRead ()
	{
		CheckTags (file);
	}

	[TestMethod]
	public void ExifRead ()
	{
		CheckExif (file);
	}

	[TestMethod]
	public void MakernoteRead ()
	{
		CheckMakerNote (file);
	}

	[TestMethod]
	public void PropertiesRead ()
	{
		CheckProperties (file);
	}

	[TestMethod]
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

	[TestMethod]
	public void AddExif ()
	{
		AddImageMetadataTests.AddExifTest (sample_file, tmp_file, true);
	}

	[TestMethod]
	public void AddGPS ()
	{
		AddImageMetadataTests.AddGPSTest (sample_file, tmp_file, true);
	}

	[TestMethod]
	public void AddXMP1 ()
	{
		AddImageMetadataTests.AddXMPTest1 (sample_file, tmp_file, false);
	}

	[TestMethod]
	public void AddXMP2 ()
	{
		AddImageMetadataTests.AddXMPTest2 (sample_file, tmp_file, false);
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
		Assert.AreEqual ("NIKON D90", tag.Model);
		Assert.AreEqual (200u, tag.ISOSpeedRatings, "ISOSpeedRatings");
		Assert.AreEqual (1.0d / 15.0d, tag.ExposureTime);
		Assert.AreEqual (5.6d, tag.FNumber);
		Assert.AreEqual (200.0d, tag.FocalLength);
		Assert.AreEqual (new DateTime (2009, 10, 21, 18, 55, 53), tag.DateTime);
		Assert.AreEqual (new DateTime (2009, 10, 21, 18, 55, 53), tag.DateTimeDigitized);
		Assert.AreEqual (new DateTime (2009, 10, 21, 18, 55, 53), tag.DateTimeOriginal);
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
			var entry = structure.GetEntry (0, 0x05) as StringIFDEntry;
			Assert.IsNotNull (entry, "entry 0x05");
			Assert.AreEqual ("AUTO        ", entry.Value);
		}
		{
			var entry = structure.GetEntry (0, 0x09) as StringIFDEntry;
			Assert.IsNotNull (entry, "entry 0x09");
			Assert.AreEqual ("                   ", entry.Value);
		}
		{
			var entry = structure.GetEntry (0, 0x0B) as SShortArrayIFDEntry;
			Assert.IsNotNull (entry, "entry 0x0B");
			var values = entry.Values;

			Assert.IsNotNull (values, "values of entry 0x0B");
			Assert.AreEqual (2, values.Length);
			Assert.AreEqual (0, values[0]);
			Assert.AreEqual (0, values[1]);
		}
		{
			var entry = structure.GetEntry (0, 0x84) as RationalArrayIFDEntry;
			Assert.IsNotNull (entry, "entry 0x84");
			var values = entry.Values;

			Assert.IsNotNull (values, "values of entry 0x84");
			Assert.AreEqual (4, values.Length);
			Assert.AreEqual (180.0d / 10.0d, (double)values[0]);
			Assert.AreEqual (2000.0d / 10.0d, (double)values[1]);
			Assert.AreEqual (35.0d / 10.0d, (double)values[2]);
			Assert.AreEqual (56.0d / 10.0d, (double)values[3]);
		}
	}

	public void CheckProperties (File file)
	{
		Assert.AreEqual (4288, file.Properties.PhotoWidth);
		Assert.AreEqual (2848, file.Properties.PhotoHeight);
		Assert.AreEqual (98, file.Properties.PhotoQuality);
	}
}
