using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using TagLib.Jpeg;
using TagLib.Xmp;

using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class JpegFormatTest
{
	static readonly string sample_file = TestPath.Samples + "sample.jpg";
	static TagLib.Image.File file;

	readonly TagTypes contained_types =
			TagTypes.JpegComment |
			TagTypes.TiffIFD |
			TagTypes.XMP;

	[ClassInitialize]
	public static void Init (TestContext testContext)
	{
		file = File.Create (sample_file) as TagLib.Image.File;
	}

	[TestMethod]
	public void TestJpegRead ()
	{
		Assert.IsTrue (file is TagLib.Jpeg.File);

		Assert.AreEqual (contained_types, file.TagTypes);
		Assert.AreEqual (contained_types, file.TagTypesOnDisk);

		Assert.IsNotNull (file.Properties, "properties");
		Assert.AreEqual (7, file.Properties.PhotoHeight);
		Assert.AreEqual (10, file.Properties.PhotoWidth);
		Assert.AreEqual (90, file.Properties.PhotoQuality);

		var tag = file.GetTag (TagTypes.JpegComment) as JpegCommentTag;
		Assert.IsNotNull (tag);
		Assert.AreEqual ("Test Comment", tag.Value);
	}

	[TestMethod]
	public void TestExif ()
	{
		var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
		Assert.IsNotNull (tag);

		var exif_ifd = tag.Structure.GetEntry (0, IFDEntryTag.ExifIFD) as SubIFDEntry;
		Assert.IsNotNull (exif_ifd);
		var exif_tag = exif_ifd.Structure;

		{
			var entry = exif_tag.GetEntry (0, (ushort)ExifEntryTag.ExposureTime) as RationalIFDEntry;
			Assert.IsNotNull (entry);
			Assert.AreEqual (0.008, (double)entry.Value);
		}
		{
			var entry = exif_tag.GetEntry (0, (ushort)ExifEntryTag.FNumber) as RationalIFDEntry;
			Assert.IsNotNull (entry);
			Assert.AreEqual (3.2, (double)entry.Value);
		}
		{
			var entry = exif_tag.GetEntry (0, (ushort)ExifEntryTag.ISOSpeedRatings) as ShortIFDEntry;
			Assert.IsNotNull (entry);
			Assert.AreEqual (100, entry.Value);
		}
	}

	[TestMethod]
	public void TestXmp ()
	{
		var tag = file.GetTag (TagTypes.XMP) as XmpTag;
		Assert.IsNotNull (tag);

		TestBagNode (tag, XmpTag.DC_NS, "subject", new[] { "keyword1", "keyword2", "keyword3" });
		TestAltNode (tag, XmpTag.DC_NS, "description", new[] { "Sample Image" });
	}

	[TestMethod]
	public void TestConstructor1 ()
	{
		var file = new TagLib.Jpeg.File (sample_file);
		Assert.IsNotNull (file.ImageTag, "ImageTag");
		Assert.AreEqual (contained_types, file.TagTypes);

		Assert.IsNotNull (file.Properties, "properties");
	}

	[TestMethod]
	public void TestConstructor2 ()
	{
		var file = new TagLib.Jpeg.File (sample_file, ReadStyle.None);
		Assert.IsNotNull (file.ImageTag, "ImageTag");
		Assert.AreEqual (contained_types, file.TagTypes);

		Assert.IsNull (file.Properties, "properties");
	}

	[TestMethod]
	public void TestConstructor3 ()
	{
		var file = new TagLib.Jpeg.File (new File.LocalFileAbstraction (sample_file), ReadStyle.None);
		Assert.IsNotNull (file.ImageTag, "ImageTag");
		Assert.AreEqual (contained_types, file.TagTypes);

		Assert.IsNull (file.Properties, "properties");
	}

	void TestBagNode (XmpTag tag, string ns, string name, string[] values)
	{
		var node = tag.FindNode (ns, name);
		Assert.IsNotNull (node);
		Assert.AreEqual (XmpNodeType.Bag, node.Type);
		Assert.AreEqual (values.Length, node.Children.Count);

		int i = 0;
		foreach (var child_node in node.Children) {
			Assert.AreEqual (values[i], child_node.Value);
			Assert.AreEqual (0, child_node.Children.Count);
			i++;
		}
	}

	void TestAltNode (XmpTag tag, string ns, string name, string[] values)
	{
		var node = tag.FindNode (ns, name);
		Assert.IsNotNull (node);
		Assert.AreEqual (XmpNodeType.Alt, node.Type);
		Assert.AreEqual (values.Length, node.Children.Count);

		int i = 0;
		foreach (var child_node in node.Children) {
			Assert.AreEqual (values[i], child_node.Value);
			Assert.AreEqual (0, child_node.Children.Count);
			i++;
		}
	}
}
