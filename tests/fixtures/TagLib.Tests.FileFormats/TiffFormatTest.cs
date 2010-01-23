using System;
using System.Collections.Generic;
using NUnit.Framework;
using TagLib;
using TagLib.Tiff;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using TagLib.Xmp;

namespace TagLib.Tests.FileFormats
{
    [TestFixture]
    public class TiffFormatTest
    {
		private static string sample_file = "samples/sample.tiff";
		private static string tmp_file = "samples/tmpwrite.tiff";
		private Image.File file;

        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create(sample_file) as Image.File;
        }

		[Test]
		public void TestConvenienceAPI()
		{
			var tag = file.ImageTag;
			Assert.IsFalse (tag == null);
			Assert.AreEqual ("Canon", tag.Make);
		}

		[Test]
		public void TestConvenienceAPIXMP()
		{
			var tag = file.GetTag (TagTypes.XMP) as XmpTag;
			Assert.IsFalse (tag == null);
			Assert.AreEqual ("Canon", tag.Make);
		}

		[Test]
		public void TestConvenienceAPITIFF()
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			Assert.IsFalse (tag == null);
			Assert.AreEqual ("Canon", tag.Make);
		}

		/// <summary>
		///    This tests the internal IDF parsing structure. For normal usage,
		///    you do not need to use these methods. Check the convenience API
		///    for easier usage.
		/// </summary>
		[Test]
        public void TestIFDInternals()
        {
			Assert.IsTrue (file is Tiff.File);
			Assert.AreEqual (10, file.Properties.PhotoHeight);
			Assert.AreEqual (10, file.Properties.PhotoWidth);

			Assert.AreEqual (TagTypes.TiffIFD | TagTypes.XMP, file.TagTypes);

			IFDTag tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			Assert.IsFalse (tag == null);

			Assert.AreEqual (tag.Structure.Directories.Length, 1);
			Assert.AreEqual (tag.Structure.Directories [0].Count, 24);

			// The tests below validate if the obtained data matches what tiffdump parsed.
			IFDEntry [] entries = new List<IFDEntry> (tag.Structure.Directories [0].Values).ToArray ();
			{
				var entry = entries [0] as LongIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.NewSubfileType, entry.Tag);
				Assert.AreEqual (0, entry.Value);
			}
			{
				var entry = entries [1] as ShortIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.ImageWidth, entry.Tag);
				Assert.AreEqual (10, entry.Value);
			}
			{
				var entry = entries [2] as ShortIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.ImageLength, entry.Tag);
				Assert.AreEqual (10, entry.Value);
			}
			{
				var entry = entries [3] as ShortArrayIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.BitsPerSample, entry.Tag);
				Assert.AreEqual (3, entry.Values.Length);
				Assert.AreEqual (8, entry.Values [0]);
				Assert.AreEqual (8, entry.Values [1]);
				Assert.AreEqual (8, entry.Values [2]);
			}
			{
				var entry = entries [4] as ShortIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.Compression, entry.Tag);
				Assert.AreEqual (5, entry.Value);
			}
			{
				var entry = entries [5] as ShortIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.PhotometricInterpretation, entry.Tag);
				Assert.AreEqual (2, entry.Value);
			}
			{
				var entry = entries [6] as StringIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.DocumentName, entry.Tag);
				Assert.AreEqual ("/home/ruben/Desktop/test.tiff", entry.Value);
			}
			{
				var entry = entries [7] as StringIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.ImageDescription, entry.Tag);
				Assert.AreEqual ("Created with GIMP", entry.Value);
			}
			{
				var entry = entries [8] as StringIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.Make, entry.Tag);
				Assert.AreEqual ("Canon", entry.Value);
			}
			{
				var entry = entries [9] as StringIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.Model, entry.Tag);
				Assert.AreEqual ("Canon DIGITAL IXUS 850 IS", entry.Value);
			}
			{
				var entry = entries [10] as LongIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.StripOffsets, entry.Tag);
				Assert.AreEqual (6980, entry.Value);
			}
			{
				var entry = entries [11] as ShortIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.Orientation, entry.Tag);
				Assert.AreEqual (1, entry.Value);
			}
			{
				var entry = entries [12] as ShortIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.SamplesPerPixel, entry.Tag);
				Assert.AreEqual (3, entry.Value);
			}
			{
				var entry = entries [13] as ShortIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.RowsPerStrip, entry.Tag);
				Assert.AreEqual (64, entry.Value);
			}
			{
				var entry = entries [14] as LongIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.StripByteCounts, entry.Tag);
				Assert.AreEqual (49, entry.Value);
			}
			{
				var entry = entries [15] as RationalIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.XResolution, entry.Tag);
				Assert.AreEqual (72, (int) entry.Value);
			}
			{
				var entry = entries [16] as RationalIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.YResolution, entry.Tag);
				Assert.AreEqual (72, (int) entry.Value);
			}
			{
				var entry = entries [17] as ShortIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.PlanarConfiguration, entry.Tag);
				Assert.AreEqual (1, entry.Value);
			}
			{
				var entry = entries [18] as ShortIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.ResolutionUnit, entry.Tag);
				Assert.AreEqual (2, entry.Value);
			}
			{
				var entry = entries [19] as StringIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.Software, entry.Tag);
				Assert.AreEqual ("CHDK ver. 0.9.8-782", entry.Value);
			}
			{
				var entry = entries [20] as StringIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.DateTime, entry.Tag);
				Assert.AreEqual ("2009:07:05 19:33:52", entry.Value);
			}
			{
				var entry = entries [21] as ShortIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.Predictor, entry.Tag);
				Assert.AreEqual (2, entry.Value);
			}
			{
				var entry = entries [22] as ByteVectorIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.XMP, entry.Tag);
				Assert.AreEqual (6285, entry.Data.Data.Length);
			}
			{
				var entry = entries [23] as SubIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.ExifIFD, entry.Tag);
			}
		}
	}
}
