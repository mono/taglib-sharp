using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using TagLib.Jpeg;
using TagLib.Xmp;

using File = TagLib.File;

namespace TaglibSharp.Tests.Images;

[TestClass]
public class JpegTangledTest : TestFixtureBase
{
	static readonly int count = 6;

	static readonly string sample_file = TestPath.Samples + "sample_tangled{0}.jpg";
	// Keep just the filename pattern for temp files
	static readonly string tmp_filename = "tmpwrite_tangled{0}.jpg";

	static readonly TagTypes[] contained_types = {
			TagTypes.JpegComment | TagTypes.TiffIFD | TagTypes.XMP,
			TagTypes.JpegComment | TagTypes.TiffIFD,
			TagTypes.JpegComment | TagTypes.TiffIFD | TagTypes.XMP,
			TagTypes.JpegComment | TagTypes.XMP,
			TagTypes.JpegComment | TagTypes.XMP,
			TagTypes.JpegComment
	};

	static File[] files;

	static string GetSampleFilename (int i)
	{
		return string.Format (sample_file, i + 1);
	}

	string GetTmpFilename (int i)
	{
		return string.Format (tmp_filename, i + 1);
	}

	[ClassInitialize]
	public static void Init (TestContext testContext)
	{
		files = new File[count];

		for (var i = 0; i < count; i++)
			files[i] = File.Create (GetSampleFilename (i));
	}

	[TestMethod]
	public void JpegRead ()
	{
		for (int i = 0; i < count; i++)
			CheckTags (files[i], i);
	}

	[TestMethod]
	public void ExifRead ()
	{
		for (int i = 0; i < count; i++)
			if ((TagTypes.TiffIFD & contained_types[i]) != 0)
				CheckExif (files[i], i);
	}

	[TestMethod]
	public void XmpRead ()
	{
		for (int i = 0; i < count; i++)
			if ((TagTypes.XMP & contained_types[i]) != 0)
				CheckXmp (files[i], i);
	}

	[TestMethod]
	public void JpegCommentRead ()
	{
		for (int i = 0; i < count; i++)
			if ((TagTypes.JpegComment & contained_types[i]) != 0)
				CheckJpegComment (files[i], i);
	}

	[TestMethod]
	public void Rewrite ()
	{
		for (int i = 0; i < count; i++) {
			var tmp = TestFileHelper.CreateTempFile(this, GetSampleFilename(i), GetTmpFilename(i));
			tmp.Save();

			// Load the file from the temporary path
			tmp = TestFileHelper.LoadTempFile(this, GetSampleFilename(i), GetTmpFilename(i));

			if ((TagTypes.TiffIFD & contained_types[i]) != 0)
				CheckExif (tmp, i);

			if ((TagTypes.XMP & contained_types[i]) != 0)
				CheckXmp (tmp, i);

			if ((TagTypes.JpegComment & contained_types[i]) != 0)
				CheckJpegComment (tmp, i);
		}
	}

	[TestMethod]
	public void AddExif ()
	{
		for (int i = 0; i < count; i++)
			AddImageMetadataTests.AddExifTest (this, GetSampleFilename (i),
											   GetTmpFilename (i),
											   (TagTypes.TiffIFD & contained_types[i]) != 0);
	}

	[TestMethod]
	public void AddGPS ()
	{
		for (int i = 0; i < count; i++)
			AddImageMetadataTests.AddGPSTest (this, GetSampleFilename (i),
											  GetTmpFilename (i),
											  (TagTypes.TiffIFD & contained_types[i]) != 0);
	}

	[TestMethod]
	public void AddXMP1 ()
	{
		for (int i = 0; i < count; i++)
			AddImageMetadataTests.AddXMPTest1 (this, GetSampleFilename (i),
											  GetTmpFilename (i),
											  (TagTypes.XMP & contained_types[i]) != 0);
	}

	[TestMethod]
	public void AddXMP2 ()
	{
		for (int i = 0; i < count; i++)
			AddImageMetadataTests.AddXMPTest2 (this, GetSampleFilename (i),
											  GetTmpFilename (i),
											  (TagTypes.XMP & contained_types[i]) != 0);
	}

	public void CheckTags (File file, int i)
	{
		Assert.IsTrue (file is TagLib.Jpeg.File, $"not a Jpeg file: index {i}");

		Assert.AreEqual (contained_types[i], file.TagTypes, $"index {i}");
		Assert.AreEqual (contained_types[i], file.TagTypesOnDisk, $"index {i}");
	}

	public void CheckExif (File file, int i)
	{
		var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
		Assert.IsNotNull (tag, $"Tiff Tag not contained: index {i}");

		var exif_ifd = tag.Structure.GetEntry (0, IFDEntryTag.ExifIFD) as SubIFDEntry;
		Assert.IsNotNull (exif_ifd, $"Exif SubIFD not contained: index {i}");

		Assert.AreEqual ("test comment", tag.Comment, $"index {i}");
	}

	public void CheckXmp (File file, int i)
	{
		var tag = file.GetTag (TagTypes.XMP) as XmpTag;
		Assert.IsNotNull (tag, $"XMP Tag not contained: index {i}");

		Assert.AreEqual ("test description", tag.Comment);
	}

	public void CheckJpegComment (File file, int i)
	{
		var tag = file.GetTag (TagTypes.JpegComment) as JpegCommentTag;
		Assert.IsNotNull (tag, $"JpegTag Tag not contained: index {i}");

		Assert.AreEqual ("Created with GIMP", tag.Comment, $"index {i}");
	}
}
