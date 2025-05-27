using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using TagLib.Xmp;

using File = TagLib.File;

namespace TaglibSharp.Tests.Images;

[TestClass]
public class JpegOlympus2Test
{
	static readonly string sample_file = TestPath.Samples + "sample_olympus2.jpg";
	static readonly string tmp_file = TestPath.SamplesTmp + "tmpwrite_olympus2.jpg";

	readonly TagTypes contained_types = TagTypes.TiffIFD | TagTypes.XMP;

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
	public void XMPRead ()
	{
		CheckXMP (file);
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
		CheckXMP (tmp);
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
		AddImageMetadataTests.AddXMPTest1 (sample_file, tmp_file, true);
	}

	[TestMethod]
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

		Assert.AreEqual ("OLYMPUS CORPORATION", tag.Make);
		Assert.AreEqual ("C5060WZ", tag.Model);
		Assert.AreEqual (100u, tag.ISOSpeedRatings);
		Assert.AreEqual (1.0d / 250.0d, tag.ExposureTime);
		Assert.AreEqual (4.8d, tag.FNumber);
		Assert.AreEqual (22.9d, tag.FocalLength);
		Assert.AreEqual (new DateTime (2009, 03, 02, 17, 05, 32), tag.DateTime);
		Assert.AreEqual (new DateTime (2009, 03, 02, 17, 05, 32), tag.DateTimeDigitized);
		Assert.AreEqual (new DateTime (2009, 03, 02, 17, 05, 32), tag.DateTimeOriginal);
	}


	public void CheckMakerNote (File file)
	{
		var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
		Assert.IsNotNull (tag, "tag");

		var makernote_ifd =
			tag.ExifIFD.GetEntry (0, (ushort)ExifEntryTag.MakerNote) as MakernoteIFDEntry;

		Assert.IsNotNull (makernote_ifd, "makernote ifd");
		Assert.AreEqual (MakernoteType.Olympus1, makernote_ifd.MakernoteType);

		var structure = makernote_ifd.Structure;
		Assert.IsNotNull (structure, "structure");
		{
			var entry = structure.GetEntry (0, 0x0200) as LongArrayIFDEntry;
			Assert.IsNotNull (entry, "entry 0x0200");
			uint[] values = entry.Values;

			Assert.IsNotNull (values, "values of entry 0x0200");
			Assert.AreEqual (3, values.Length);
			Assert.AreEqual (0u, values[0]);
			Assert.AreEqual (0u, values[1]);
			Assert.AreEqual (0u, values[2]);
		}
		{
			var entry = structure.GetEntry (0, 0x0204) as RationalIFDEntry;
			Assert.IsNotNull (entry, "entry 0x0204");
			Assert.AreEqual (100.0d / 100.0d, (double)entry.Value);
		}
		{
			var entry = structure.GetEntry (0, 0x0207) as StringIFDEntry;
			Assert.IsNotNull (entry, "entry 0x0207");
			Assert.AreEqual ("SX756", entry.Value);
		}
	}

	public void CheckXMP (File file)
	{
		var tag = file.GetTag (TagTypes.XMP) as XmpTag;

		Assert.IsNotNull (tag, "tag");

		CollectionAssert.AreEqual (new string[] { }, tag.Keywords);
		Assert.AreEqual ("OLYMPUS CORPORATION", tag.Make);
		Assert.AreEqual ("C5060WZ", tag.Model);
		Assert.AreEqual ("Adobe Photoshop Elements 4.0", tag.Software);
	}

	public void CheckProperties (File file)
	{
		Assert.AreEqual (1280, file.Properties.PhotoWidth);
		Assert.AreEqual (958, file.Properties.PhotoHeight);
		Assert.AreEqual (69, file.Properties.PhotoQuality);
	}
}
