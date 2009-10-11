using System;
using NUnit.Framework;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.Jpeg;
using TagLib.Exif;
using TagLib.Xmp;

namespace TagLib.Tests.FileFormats
{
    [TestFixture]
    public class JpegFormatTest
    {
		private static string sample_file = "samples/sample.jpg";
        private static string tmp_file = "samples/tmpwrite.jpg";
		private File file;

        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create(sample_file);
        }

		[Test]
		public void TestJpegRead()
		{
			Assert.IsTrue (file is Jpeg.File);

			Assert.AreEqual (TagTypes.JpegComment | TagTypes.TiffIFD | TagTypes.Exif | TagTypes.XMP | TagTypes.Thumbnail , file.TagTypes);
			Assert.AreEqual (TagTypes.JpegComment | TagTypes.TiffIFD | TagTypes.Exif | TagTypes.XMP | TagTypes.Thumbnail , file.TagTypesOnDisk);


			JpegCommentTag tag = file.GetTag (TagTypes.JpegComment) as JpegCommentTag;
			Assert.IsFalse (tag == null);
			Assert.AreEqual ("Test Comment", tag.Value);
		}

		[Test]
		public void TestExif()
		{
			ExifTag tag = file.GetTag (TagTypes.Exif) as ExifTag;
			Assert.IsFalse (tag == null);

			{
				var entry = tag.GetEntry (IFDEntryTag.ExposureTime) as RationalIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual (0.008, entry.Value);
			}
			{
				var entry = tag.GetEntry (IFDEntryTag.FNumber) as RationalIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual (3.2, entry.Value);
			}
			{
				var entry = tag.GetEntry (IFDEntryTag.ISOSpeedRatings) as ShortIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual (100, entry.Value);
			}
		}

		[Test]
		public void TestXmp()
		{
			XmpTag tag = file.GetTag (TagTypes.XMP) as XmpTag;
			Assert.IsFalse (tag == null);

			TestBagNode (tag, XmpTag.DC_NS, "subject", new string[] {"keyword1", "keyword2", "keyword3"});
			TestAltNode (tag, XmpTag.DC_NS, "description", new string[] {"Sample Image"});
		}

		[Test]
		public void WriteExif ()
		{
			StandardExifTests.WriteTags (sample_file, tmp_file);
		}

		private void TestBagNode (XmpTag tag, string ns, string name, string[] values)
		{
			var node = tag.FindNode (ns, name);
			Assert.IsFalse (node == null);
			Assert.AreEqual (XmpNodeType.Bag, node.Type);
			Assert.AreEqual (values.Length, node.Children.Count);

			int i = 0;
			foreach (var child_node in node.Children) {
				Assert.AreEqual (values[i], child_node.Value);
				Assert.AreEqual (0, child_node.Children.Count);
				i++;
			}
		}

		private void TestAltNode (XmpTag tag, string ns, string name, string[] values)
		{
			var node = tag.FindNode (ns, name);
			Assert.IsFalse (node == null);
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
}
