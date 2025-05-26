using NUnit.Framework;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using TagLib.Jpeg;
using TagLib.Xmp;
using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats
{
	[TestFixture]
	public class JpegFormatTest
	{
		static readonly string sample_file = TestPath.Samples + "sample.jpg";
		TagLib.Image.File file;

		readonly TagTypes contained_types =
				TagTypes.JpegComment |
				TagTypes.TiffIFD |
				TagTypes.XMP;

		[OneTimeSetUp]
		public void Init ()
		{
			file = File.Create (sample_file) as TagLib.Image.File;
		}

		[Test]
		public void TestJpegRead ()
		{
			ClassicAssert.IsTrue (file is TagLib.Jpeg.File);

			ClassicAssert.AreEqual (contained_types, file.TagTypes);
			ClassicAssert.AreEqual (contained_types, file.TagTypesOnDisk);

			ClassicAssert.IsNotNull (file.Properties, "properties");
			ClassicAssert.AreEqual (7, file.Properties.PhotoHeight);
			ClassicAssert.AreEqual (10, file.Properties.PhotoWidth);
			ClassicAssert.AreEqual (90, file.Properties.PhotoQuality);

			var tag = file.GetTag (TagTypes.JpegComment) as JpegCommentTag;
			ClassicAssert.IsFalse (tag == null);
			ClassicAssert.AreEqual ("Test Comment", tag.Value);
		}

		[Test]
		public void TestExif ()
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			ClassicAssert.IsFalse (tag == null);

			var exif_ifd = tag.Structure.GetEntry (0, IFDEntryTag.ExifIFD) as SubIFDEntry;
			ClassicAssert.IsFalse (exif_ifd == null);
			var exif_tag = exif_ifd.Structure;

			{
				var entry = exif_tag.GetEntry (0, (ushort)ExifEntryTag.ExposureTime) as RationalIFDEntry;
				ClassicAssert.IsFalse (entry == null);
				ClassicAssert.AreEqual (0.008, (double)entry.Value);
			}
			{
				var entry = exif_tag.GetEntry (0, (ushort)ExifEntryTag.FNumber) as RationalIFDEntry;
				ClassicAssert.IsFalse (entry == null);
				ClassicAssert.AreEqual (3.2, (double)entry.Value);
			}
			{
				var entry = exif_tag.GetEntry (0, (ushort)ExifEntryTag.ISOSpeedRatings) as ShortIFDEntry;
				ClassicAssert.IsFalse (entry == null);
				ClassicAssert.AreEqual (100, entry.Value);
			}
		}

		[Test]
		public void TestXmp ()
		{
			var tag = file.GetTag (TagTypes.XMP) as XmpTag;
			ClassicAssert.IsFalse (tag == null);

			TestBagNode (tag, XmpTag.DC_NS, "subject", new[] { "keyword1", "keyword2", "keyword3" });
			TestAltNode (tag, XmpTag.DC_NS, "description", new[] { "Sample Image" });
		}

		[Test]
		public void TestConstructor1 ()
		{
			var file = new TagLib.Jpeg.File (sample_file);
			ClassicAssert.IsNotNull (file.ImageTag, "ImageTag");
			ClassicAssert.AreEqual (contained_types, file.TagTypes);

			ClassicAssert.IsNotNull (file.Properties, "properties");
		}

		[Test]
		public void TestConstructor2 ()
		{
			var file = new TagLib.Jpeg.File (sample_file, ReadStyle.None);
			ClassicAssert.IsNotNull (file.ImageTag, "ImageTag");
			ClassicAssert.AreEqual (contained_types, file.TagTypes);

			ClassicAssert.IsNull (file.Properties, "properties");
		}

		[Test]
		public void TestConstructor3 ()
		{
			var file = new TagLib.Jpeg.File (new File.LocalFileAbstraction (sample_file), ReadStyle.None);
			ClassicAssert.IsNotNull (file.ImageTag, "ImageTag");
			ClassicAssert.AreEqual (contained_types, file.TagTypes);

			ClassicAssert.IsNull (file.Properties, "properties");
		}

		void TestBagNode (XmpTag tag, string ns, string name, string[] values)
		{
			var node = tag.FindNode (ns, name);
			ClassicAssert.IsFalse (node == null);
			ClassicAssert.AreEqual (XmpNodeType.Bag, node.Type);
			ClassicAssert.AreEqual (values.Length, node.Children.Count);

			int i = 0;
			foreach (var child_node in node.Children) {
				ClassicAssert.AreEqual (values[i], child_node.Value);
				ClassicAssert.AreEqual (0, child_node.Children.Count);
				i++;
			}
		}

		void TestAltNode (XmpTag tag, string ns, string name, string[] values)
		{
			var node = tag.FindNode (ns, name);
			ClassicAssert.IsFalse (node == null);
			ClassicAssert.AreEqual (XmpNodeType.Alt, node.Type);
			ClassicAssert.AreEqual (values.Length, node.Children.Count);

			int i = 0;
			foreach (var child_node in node.Children) {
				ClassicAssert.AreEqual (values[i], child_node.Value);
				ClassicAssert.AreEqual (0, child_node.Children.Count);
				i++;
			}
		}
	}
}
