using NUnit.Framework;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using TagLib.Xmp;
using TaglibSharp.Tests.Images.Validators;

namespace TaglibSharp.Tests.Images
{
	[TestFixture]
	public class JpegCanonBibble5Test
	{
		[Test]
		public void Test ()
		{
			ImageTest.Run ("sample_canon_bibble5.jpg",
				new JpegCanonBibble5TestInvariantValidator (),
				NoModificationValidator.Instance
			);
		}
	}

	public class JpegCanonBibble5TestInvariantValidator : IMetadataInvariantValidator
	{
		public void ValidateMetadataInvariants (TagLib.Image.File file)
		{
			ClassicAssert.IsNotNull (file);
			//  ---------- Start of IFD tests ----------

			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			ClassicAssert.IsNotNull (tag, "IFD tag not found");

			var structure = tag.Structure;

			// Image.0x010F (Make/Ascii/6) "Canon"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Make);
				ClassicAssert.IsNotNull (entry, "Entry 0x010F missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("Canon", (entry as StringIFDEntry).Value);
			}
			// Image.0x0110 (Model/Ascii/23) "Canon EOS 400D DIGITAL"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Model);
				ClassicAssert.IsNotNull (entry, "Entry 0x0110 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("Canon EOS 400D DIGITAL", (entry as StringIFDEntry).Value);
			}
			// Image.0x011A (XResolution/Rational/1) "150/1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.XResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (150, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Image.0x011B (YResolution/Rational/1) "150/1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.YResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (150, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Image.0x0128 (ResolutionUnit/Short/1) "2"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ResolutionUnit);
				ClassicAssert.IsNotNull (entry, "Entry 0x0128 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Image.0x0131 (Software/Ascii/17) "Bibble 5 Pro 5.0"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Software);
				ClassicAssert.IsNotNull (entry, "Entry 0x0131 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("Bibble 5 Pro 5.0", (entry as StringIFDEntry).Value);
			}
			// Image.0x8769 (ExifTag/SubIFD/1) "8"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD);
				ClassicAssert.IsNotNull (entry, "Entry 0x8769 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SubIFDEntry, "Entry is not a sub IFD!");
			}

			var exif = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD) as SubIFDEntry;
			ClassicAssert.IsNotNull (exif, "Exif tag not found");
			var exif_structure = exif.Structure;

			// Photo.0x010F (0x010f/Ascii/6) "Canon"
			{
				var entry = exif_structure.GetEntry (0, (ushort)IFDEntryTag.Make);
				ClassicAssert.IsNotNull (entry, "Entry 0x010F missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("Canon", (entry as StringIFDEntry).Value);
			}
			// Photo.0x0110 (0x0110/Ascii/23) "Canon EOS 400D DIGITAL"
			{
				var entry = exif_structure.GetEntry (0, (ushort)IFDEntryTag.Model);
				ClassicAssert.IsNotNull (entry, "Entry 0x0110 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("Canon EOS 400D DIGITAL", (entry as StringIFDEntry).Value);
			}
			// Photo.0x0132 (0x0132/Ascii/20) "2010:02:03 10:51:35"
			{
				var entry = exif_structure.GetEntry (0, (ushort)IFDEntryTag.DateTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x0132 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2010:02:03 10:51:35", (entry as StringIFDEntry).Value);
			}
			// Photo.0x829A (ExposureTime/Rational/1) "1/200"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x829A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (200, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x829D (FNumber/Rational/1) "5/1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FNumber);
				ClassicAssert.IsNotNull (entry, "Entry 0x829D missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (5, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x8822 (ExposureProgram/Short/1) "1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureProgram);
				ClassicAssert.IsNotNull (entry, "Entry 0x8822 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x8827 (ISOSpeedRatings/Short/1) "100"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ISOSpeedRatings);
				ClassicAssert.IsNotNull (entry, "Entry 0x8827 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (100, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x9003 (DateTimeOriginal/Ascii/20) "2010:01:06 19:50:44"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeOriginal);
				ClassicAssert.IsNotNull (entry, "Entry 0x9003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2010:01:06 19:50:44", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9004 (DateTimeDigitized/Ascii/20) "2010:01:06 19:50:44"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeDigitized);
				ClassicAssert.IsNotNull (entry, "Entry 0x9004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2010:01:06 19:50:44", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9201 (ShutterSpeedValue/Rational/1) "500948/65536"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ShutterSpeedValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9201 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (500948, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (65536, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9202 (ApertureValue/Rational/1) "304340/65536"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ApertureValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9202 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (304340, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (65536, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9204 (ExposureBiasValue/SRational/1) "0/3"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureBiasValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9204 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SRationalIFDEntry, "Entry is not a srational!");
				ClassicAssert.AreEqual (0, (entry as SRationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (3, (entry as SRationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9205 (MaxApertureValue/Rational/1) "85/32"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MaxApertureValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9205 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (85, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (32, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9207 (MeteringMode/Short/1) "5"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MeteringMode);
				ClassicAssert.IsNotNull (entry, "Entry 0x9207 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (5, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x9209 (Flash/Short/1) "9"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Flash);
				ClassicAssert.IsNotNull (entry, "Entry 0x9209 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (9, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x920A (FocalLength/Rational/1) "21/1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x920A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (21, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9290 (SubSecTime/Ascii/4) "976"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SubsecTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x9290 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("976", (entry as StringIFDEntry).Value);
			}
			// Photo.0xA402 (ExposureMode/Short/1) "1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureMode);
				ClassicAssert.IsNotNull (entry, "Entry 0xA402 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA403 (WhiteBalance/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.WhiteBalance);
				ClassicAssert.IsNotNull (entry, "Entry 0xA403 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA406 (SceneCaptureType/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SceneCaptureType);
				ClassicAssert.IsNotNull (entry, "Entry 0xA406 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}

			//  ---------- End of IFD tests ----------


			//  ---------- Start of XMP tests ----------

			var xmp = file.GetTag (TagTypes.XMP) as XmpTag;
			// Xmp.tiff.Model (XmpText/22) "Canon EOS 400D DIGITAL"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "Model");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("Canon EOS 400D DIGITAL", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.tiff.Make (XmpText/5) "Canon"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "Make");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("Canon", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.tiff.ImageWidth (XmpText/4) "3888"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "ImageWidth");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("3888", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.tiff.ImageLength (XmpText/4) "2592"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "ImageLength");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("2592", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.tiff.FNumber (XmpText/5) "50/10"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "FNumber");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("50/10", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.tiff.XResolution (XmpText/5) "150/1"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "XResolution");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("150/1", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.tiff.YResolution (XmpText/5) "150/1"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "YResolution");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("150/1", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.tiff.ResolutionUnit (XmpText/1) "2"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "ResolutionUnit");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("2", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.ExposureProgram (XmpText/1) "1"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "ExposureProgram");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("1", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.MeteringMode (XmpText/1) "5"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "MeteringMode");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("5", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.ExposureMode (XmpText/1) "1"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "ExposureMode");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("1", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.WhiteBalance (XmpText/1) "0"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "WhiteBalance");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("0", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.SceneCaptureType (XmpText/1) "0"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "SceneCaptureType");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("0", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.ISOSpeedRating (XmpText/3) "100"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "ISOSpeedRating");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("100", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.Flash (XmpText/1) "9"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "Flash");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("9", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.ApertureValue (XmpText/12) "304340/65536"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "ApertureValue");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("304340/65536", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.ShutterSpeedValue (XmpText/12) "500948/65536"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "ShutterSpeedValue");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("500948/65536", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.ExposureTime (XmpText/5) "1/200"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "ExposureTime");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("1/200", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.ExposureBiasValue (XmpText/3) "0/3"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "ExposureBiasValue");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("0/3", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.FocalLength (XmpText/4) "21/1"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "FocalLength");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("21/1", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.photoshop.DateCreated (XmpText/24) "2010-01-06T19:50:44.000Z"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.PHOTOSHOP_NS, "DateCreated");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("2010-01-06T19:50:44.000Z", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.xmp.Rating (XmpText/1) "0"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.XAP_NS, "Rating");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("0", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}

			//  ---------- End of XMP tests ----------

		}
	}
}
