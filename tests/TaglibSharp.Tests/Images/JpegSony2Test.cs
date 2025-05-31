using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;

using File = TagLib.File;

namespace TaglibSharp.Tests.Images;

[TestClass]
public class JpegSony2Test : TestFixtureBase
{
	static readonly string sample_file = TestPath.Samples + "sample_sony2.jpg";
	// Use only the filename part
	static readonly string tmp_filename = "tmpwrite_sony2.jpg";

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
	public void Rewrite ()
	{
		var tmp = TestFileHelper.CreateTempFile(this, sample_file, tmp_filename);
		tmp.Save ();

		// Load the file from the temporary path
		tmp = TestFileHelper.LoadTempFile(this, sample_file, tmp_filename);

		CheckTags (tmp);
		CheckExif (tmp);
		CheckMakerNote (tmp);
		CheckProperties (tmp);
	}

	[TestMethod]
	public void AddExif ()
	{
		AddImageMetadataTests.AddExifTest (this, sample_file, tmp_filename, true);
	}

	[TestMethod]
	public void AddGPS ()
	{
		AddImageMetadataTests.AddGPSTest (this, sample_file, tmp_filename, true);
	}

	[TestMethod]
	public void AddXMP1 ()
	{
		AddImageMetadataTests.AddXMPTest1 (this, sample_file, tmp_filename, false);
	}

	[TestMethod]
	public void AddXMP2 ()
	{
		AddImageMetadataTests.AddXMPTest2 (this, sample_file, tmp_filename, false);
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

		Assert.AreEqual ("SONY ", tag.Make);
		Assert.AreEqual ("DSLR-A700", tag.Model);
		Assert.AreEqual (400u, tag.ISOSpeedRatings, "ISOSpeedRatings");
		Assert.AreEqual (1.0d / 125.0d, tag.ExposureTime);
		Assert.AreEqual (5.6d, tag.FNumber);
		Assert.AreEqual (70.0d, tag.FocalLength);
		Assert.AreEqual (new DateTime (2009, 11, 06, 20, 56, 07), tag.DateTime);
		Assert.AreEqual (new DateTime (2009, 11, 06, 20, 56, 07), tag.DateTimeDigitized);
		Assert.AreEqual (new DateTime (2009, 11, 06, 20, 56, 07), tag.DateTimeOriginal);
	}

	public void CheckMakerNote (File file)
	{
		var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
		Assert.IsNotNull (tag, "tag");

		var makernote_ifd =
			tag.ExifIFD.GetEntry (0, (ushort)ExifEntryTag.MakerNote) as MakernoteIFDEntry;

		Assert.IsNotNull (makernote_ifd, "makernote ifd");
		Assert.AreEqual (MakernoteType.Sony, makernote_ifd.MakernoteType);

		var structure = makernote_ifd.Structure;
		Assert.IsNotNull (structure, "structure");
		//Tag info from http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Sony.html
		//0x0102: image quality
		{
			var entry = structure.GetEntry (0, 0x0102) as LongIFDEntry;
			Assert.IsNotNull (entry, "entry 0x0102");
			Assert.AreEqual (5u, entry.Value);
		}
		{
			var entry = structure.GetEntry (0, 0x0104) as SRationalIFDEntry;
			Assert.IsNotNull (entry, "entry 0x0115");
			Assert.AreEqual (0.0d, (double)entry.Value);
		}
		//0x0115: white balance
		{
			var entry = structure.GetEntry (0, 0x0115) as LongIFDEntry;
			Assert.IsNotNull (entry, "entry 0x0115");
			Assert.AreEqual (80u, entry.Value);
		}
		//0xb026: image stabilizer
		{
			var entry = structure.GetEntry (0, 0xb026) as LongIFDEntry;
			Assert.IsNotNull (entry, "entry 0xb026");
			Assert.AreEqual (1u, entry.Value);
		}
	}

	public void CheckProperties (File file)
	{
		Assert.AreEqual (4272, file.Properties.PhotoWidth);
		Assert.AreEqual (2848, file.Properties.PhotoHeight);
		Assert.AreEqual (99, file.Properties.PhotoQuality);
	}
}
