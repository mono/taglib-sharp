using System;
using System.Collections.Generic;
using NUnit.Framework;
using TagLib;
using TagLib.Tiff;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.Xmp;
using TagLib.Exif;

namespace TagLib.Tests.FileFormats
{
    [TestFixture]
    public class TiffFormatTest
    {
		private static string sample_file = "samples/sample.tiff";
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

		[Test]
		public void TestConvenienceAPIExif()
		{
			var tag = file.GetTag (TagTypes.Exif) as ExifTag;
			Assert.IsFalse (tag == null);
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

			Assert.AreEqual (TagTypes.TiffIFD | TagTypes.XMP | TagTypes.Exif, file.TagTypes);

			IFDTag tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			Assert.IsFalse (tag == null);

			Assert.AreEqual (tag.Directories.Length, 1);
			Assert.AreEqual (tag.Directories [0].Count, 24);

			// The tests below validate if the obtained data matches what tiffdump parsed.
			IFDEntry [] entries = new List<IFDEntry> (tag.Directories [0].Values).ToArray ();
			{
				var entry = entries [0] as LongIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.NewSubFileType, entry.Tag);
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
				Assert.AreEqual (8, entry.Values [0].Value);
				Assert.AreEqual (8, entry.Values [1].Value);
				Assert.AreEqual (8, entry.Values [2].Value);
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
				Assert.AreEqual ((ushort) IFDEntryTag.PhotoMetricInterpretation, entry.Tag);
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
				var entry = entries [23] as LongIFDEntry;
				Assert.IsFalse (entry == null);
				Assert.AreEqual ((ushort) IFDEntryTag.ExifIFD, entry.Tag);
				Assert.AreEqual (6730, entry.Value);
			}
		}

		[Test]
		public void TestXMPRead()
		{
			Assert.AreEqual (TagTypes.TiffIFD | TagTypes.XMP | TagTypes.Exif, file.TagTypes);

			XmpTag tag = file.GetTag (TagTypes.XMP) as XmpTag;
			Assert.IsFalse (tag == null);

			TestNode (tag, XmpTag.EXIF_NS, "ExifVersion", "0221", 0);
			TestNode (tag, XmpTag.EXIF_NS, "DateTimeOriginal", "2009-07-05T19:33:52", 0);
			TestNode (tag, XmpTag.EXIF_NS, "ExposureTime", "1/30", 0);
			TestNode (tag, XmpTag.EXIF_NS, "FNumber", "28/10", 0);
			TestNode (tag, XmpTag.EXIF_NS, "ExposureProgram", "0", 0);
			TestNode (tag, XmpTag.EXIF_NS, "ShutterSpeedValue", "4906891/1000000", 0);
			TestNode (tag, XmpTag.EXIF_NS, "ApertureValue", "2970854/1000000", 0);
			TestNode (tag, XmpTag.EXIF_NS, "ExposureBiasValue", "-96/96", 0);
			TestNode (tag, XmpTag.EXIF_NS, "MaxApertureValue", "293/96", 0);
			TestNode (tag, XmpTag.EXIF_NS, "MeteringMode", "5", 0);
			TestNode (tag, XmpTag.EXIF_NS, "FocalLength", "4600/1000", 0);
			TestNode (tag, XmpTag.EXIF_NS, "FocalLengthIn35mmFilm", "27", 0);
			TestNode (tag, XmpTag.XAP_NS, "ModifyDate", "2009-07-05T19:33:52+03:00", 0);
			TestNode (tag, XmpTag.XAP_NS, "CreatorTool", "CHDK ver. 0.9.8-782", 0);
			TestNode (tag, XmpTag.XAP_NS, "Rating", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "Version", "3.7", 0);
			TestNode (tag, XmpTag.CRS_NS, "WhiteBalance", "Custom", 0);
			TestNode (tag, XmpTag.CRS_NS, "Temperature", "6550", 0);
			TestNode (tag, XmpTag.CRS_NS, "Tint", "+150", 0);
			TestNode (tag, XmpTag.CRS_NS, "Exposure", "0.00", 0);
			TestNode (tag, XmpTag.CRS_NS, "Shadows", "5", 0);
			TestNode (tag, XmpTag.CRS_NS, "Brightness", "+50", 0);
			TestNode (tag, XmpTag.CRS_NS, "Contrast", "+25", 0);
			TestNode (tag, XmpTag.CRS_NS, "Saturation", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "Sharpness", "25", 0);
			TestNode (tag, XmpTag.CRS_NS, "LuminanceSmoothing", "31", 0);
			TestNode (tag, XmpTag.CRS_NS, "ColorNoiseReduction", "19", 0);
			TestNode (tag, XmpTag.CRS_NS, "ChromaticAberrationR", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "ChromaticAberrationB", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "VignetteAmount", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "ShadowTint", "+100", 0);
			TestNode (tag, XmpTag.CRS_NS, "RedHue", "+33", 0);
			TestNode (tag, XmpTag.CRS_NS, "RedSaturation", "-33", 0);
			TestNode (tag, XmpTag.CRS_NS, "GreenHue", "-100", 0);
			TestNode (tag, XmpTag.CRS_NS, "GreenSaturation", "-100", 0);
			TestNode (tag, XmpTag.CRS_NS, "BlueHue", "+100", 0);
			TestNode (tag, XmpTag.CRS_NS, "BlueSaturation", "+33", 0);
			TestNode (tag, XmpTag.CRS_NS, "FillLight", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "Vibrance", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "HighlightRecovery", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "HueAdjustmentRed", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "HueAdjustmentOrange", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "HueAdjustmentYellow", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "HueAdjustmentGreen", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "HueAdjustmentAqua", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "HueAdjustmentBlue", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "HueAdjustmentPurple", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "HueAdjustmentMagenta", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SaturationAdjustmentRed", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SaturationAdjustmentOrange", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SaturationAdjustmentYellow", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SaturationAdjustmentGreen", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SaturationAdjustmentAqua", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SaturationAdjustmentBlue", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SaturationAdjustmentPurple", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SaturationAdjustmentMagenta", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "LuminanceAdjustmentRed", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "LuminanceAdjustmentOrange", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "LuminanceAdjustmentYellow", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "LuminanceAdjustmentGreen", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "LuminanceAdjustmentAqua", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "LuminanceAdjustmentBlue", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "LuminanceAdjustmentPurple", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "LuminanceAdjustmentMagenta", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SplitToningShadowHue", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SplitToningShadowSaturation", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SplitToningHighlightHue", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SplitToningHighlightSaturation", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "SplitToningBalance", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "ParametricShadows", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "ParametricDarks", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "ParametricLights", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "ParametricHighlights", "0", 0);
			TestNode (tag, XmpTag.CRS_NS, "ParametricShadowSplit", "25", 0);
			TestNode (tag, XmpTag.CRS_NS, "ParametricMidtoneSplit", "50", 0);
			TestNode (tag, XmpTag.CRS_NS, "ParametricHighlightSplit", "75", 0);
			TestNode (tag, XmpTag.CRS_NS, "ConvertToGrayscale", "False", 0);
			TestNode (tag, XmpTag.CRS_NS, "ToneCurveName", "Custom", 0);
			TestNode (tag, XmpTag.CRS_NS, "CameraProfile", "Embedded", 0);
			TestNode (tag, XmpTag.CRS_NS, "HasSettings", "True", 0);
			TestNode (tag, XmpTag.CRS_NS, "HasCrop", "False", 0);
			TestNode (tag, XmpTag.CRS_NS, "AlreadyApplied", "False", 0);
			TestNode (tag, XmpTag.TIFF_NS, "ImageWidth", "256", 0);
			TestNode (tag, XmpTag.TIFF_NS, "ImageLength", "192", 0);
			TestNode (tag, XmpTag.TIFF_NS, "Compression", "1", 0);
			TestNode (tag, XmpTag.TIFF_NS, "PhotometricInterpretation", "2", 0);
			TestNode (tag, XmpTag.TIFF_NS, "Orientation", "1", 0);
			TestNode (tag, XmpTag.TIFF_NS, "SamplesPerPixe", "3", 0);
			TestNode (tag, XmpTag.TIFF_NS, "PlanarConfiguration", "1", 0);
			TestNode (tag, XmpTag.TIFF_NS, "DateTime", "2009-07-05T19:33:52", 0);
			TestNode (tag, XmpTag.TIFF_NS, "Make", "Canon", 0);
			TestNode (tag, XmpTag.TIFF_NS, "Model", "Canon DIGITAL IXUS 850 IS", 0);
			TestNode (tag, XmpTag.TIFF_NS, "Software", "CHDK ver. 0.9.8-782", 0);
			TestNode (tag, XmpTag.EXIF_NS, "Flash", "", 5);
			TestNode (tag, XmpTag.EXIF_NS, "Fired", "False", 0);
			TestNode (tag, XmpTag.EXIF_NS, "Return", "0", 0);
			TestNode (tag, XmpTag.EXIF_NS, "Mode", "2", 0);
			TestNode (tag, XmpTag.EXIF_NS, "Function", "False", 0);
			TestNode (tag, XmpTag.EXIF_NS, "RedEyeMode", "False", 0);

			{
				var node = tag.FindNode (XmpTag.EXIF_NS, "ISOSpeedRatings");
				Assert.IsFalse (node == null);
				Assert.AreEqual (XmpNodeType.Seq, node.Type);
				Assert.AreEqual ("", node.Value);
				Assert.AreEqual (1, node.Children.Count);
				Assert.AreEqual ("400", node.Children [0].Value);
			}
			{
				var node = tag.FindNode (XmpTag.CRS_NS, "ToneCurve");
				Assert.IsFalse (node == null);
				Assert.AreEqual ("", node.Value);
				Assert.AreEqual (10, node.Children.Count);
				Assert.AreEqual ("0, 0", node.Children [0].Value);
				Assert.AreEqual ("35, 154", node.Children [1].Value);
				Assert.AreEqual ("64, 56", node.Children [2].Value);
				Assert.AreEqual ("97, 184", node.Children [3].Value);
				Assert.AreEqual ("134, 182", node.Children [4].Value);
				Assert.AreEqual ("151, 189", node.Children [5].Value);
				Assert.AreEqual ("181, 177", node.Children [6].Value);
				Assert.AreEqual ("192, 196", node.Children [7].Value);
				Assert.AreEqual ("202, 240", node.Children [8].Value);
				Assert.AreEqual ("255, 255", node.Children [9].Value);

			}
			{
				var node = tag.FindNode (XmpTag.TIFF_NS, "BitsPerSample");
				Assert.IsFalse (node == null);
				Assert.AreEqual (XmpNodeType.Seq, node.Type);
				Assert.AreEqual ("", node.Value);
				Assert.AreEqual (1, node.Children.Count);
				Assert.AreEqual ("8 8 8", node.Children [0].Value);
			}
			{
				var node = tag.FindNode (XmpTag.DC_NS, "description");
				Assert.IsFalse (node == null);
				Assert.AreEqual (XmpNodeType.Alt, node.Type);
				Assert.AreEqual (1, node.Children.Count);

				var child = node.Children [0];
				Assert.IsFalse (child == null);
				Assert.AreEqual ("Test", child.Value);

				var qualifier = child.GetQualifier (XmpTag.XML_NS, "lang");
				Assert.IsFalse (qualifier == null);
				Assert.AreEqual ("x-default", qualifier.Value);
			}
		}

		private void TestNode (XmpTag tag, string ns, string name, string value, int count)
		{
			var node = tag.FindNode (ns, name);
			Assert.IsFalse (node == null);
			Assert.AreEqual (XmpNodeType.Simple, node.Type);
			Assert.AreEqual (value, node.Value);
			Assert.AreEqual (count, node.Children.Count);
		}
	}
}
