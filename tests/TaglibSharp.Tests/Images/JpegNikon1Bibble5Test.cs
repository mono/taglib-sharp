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
	public class JpegNikon1Bibble5Test
	{
		[Test]
		public void Test ()
		{
			ImageTest.Run ("sample_nikon1_bibble5.jpg",
				new JpegNikon1Bibble5TestInvariantValidator (),
				NoModificationValidator.Instance,
				new CommentModificationValidator (string.Empty),
				new TagCommentModificationValidator (null, TagTypes.TiffIFD, true),
				new TagCommentModificationValidator (null, TagTypes.XMP, true),
				new TagKeywordsModificationValidator (new string[] { }, TagTypes.XMP, true)
			);
		}
	}

	public class JpegNikon1Bibble5TestInvariantValidator : IMetadataInvariantValidator
	{
		public void ValidateMetadataInvariants (TagLib.Image.File file)
		{
			ClassicAssert.IsNotNull (file);
			//  ---------- Start of IFD tests ----------

			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			ClassicAssert.IsNotNull (tag, "IFD tag not found");

			var structure = tag.Structure;

			// Image.0x010F (Make/Ascii/18) "NIKON CORPORATION"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Make);
				ClassicAssert.IsNotNull (entry, "Entry 0x010F missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("NIKON CORPORATION", (entry as StringIFDEntry).Value);
			}
			// Image.0x0110 (Model/Ascii/11) "NIKON D70s"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Model);
				ClassicAssert.IsNotNull (entry, "Entry 0x0110 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("NIKON D70s", (entry as StringIFDEntry).Value);
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

			// Photo.0x010F (0x010f/Ascii/18) "NIKON CORPORATION"
			{
				var entry = exif_structure.GetEntry (0, (ushort)IFDEntryTag.Make);
				ClassicAssert.IsNotNull (entry, "Entry 0x010F missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("NIKON CORPORATION", (entry as StringIFDEntry).Value);
			}
			// Photo.0x0110 (0x0110/Ascii/11) "NIKON D70s"
			{
				var entry = exif_structure.GetEntry (0, (ushort)IFDEntryTag.Model);
				ClassicAssert.IsNotNull (entry, "Entry 0x0110 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("NIKON D70s", (entry as StringIFDEntry).Value);
			}
			// Photo.0x0132 (0x0132/Ascii/20) "2010:02:03 11:02:18"
			{
				var entry = exif_structure.GetEntry (0, (ushort)IFDEntryTag.DateTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x0132 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2010:02:03 11:02:18", (entry as StringIFDEntry).Value);
			}
			// Photo.0x829A (ExposureTime/Rational/1) "10/7500"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x829A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (10, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (7500, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x829D (FNumber/Rational/1) "13/1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FNumber);
				ClassicAssert.IsNotNull (entry, "Entry 0x829D missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (13, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x8822 (ExposureProgram/Short/1) "2"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureProgram);
				ClassicAssert.IsNotNull (entry, "Entry 0x8822 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x8827 (ISOSpeedRatings/Short/1) "1600"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ISOSpeedRatings);
				ClassicAssert.IsNotNull (entry, "Entry 0x8827 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1600, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x9003 (DateTimeOriginal/Ascii/20) "2007:02:15 17:07:48"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeOriginal);
				ClassicAssert.IsNotNull (entry, "Entry 0x9003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2007:02:15 17:07:48", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9004 (DateTimeDigitized/Ascii/20) "2007:02:15 17:07:48"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeDigitized);
				ClassicAssert.IsNotNull (entry, "Entry 0x9004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2007:02:15 17:07:48", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9204 (ExposureBiasValue/SRational/1) "0/6"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureBiasValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9204 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SRationalIFDEntry, "Entry is not a srational!");
				ClassicAssert.AreEqual (0, (entry as SRationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (6, (entry as SRationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9205 (MaxApertureValue/Rational/1) "50/10"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MaxApertureValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9205 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (50, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (10, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9207 (MeteringMode/Short/1) "5"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MeteringMode);
				ClassicAssert.IsNotNull (entry, "Entry 0x9207 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (5, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x9208 (LightSource/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.LightSource);
				ClassicAssert.IsNotNull (entry, "Entry 0x9208 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x920A (FocalLength/Rational/1) "50/1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x920A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (50, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9290 (SubSecTime/Ascii/4) "889"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SubsecTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x9290 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("889", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9291 (SubSecTimeOriginal/Ascii/4) "800"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SubsecTimeOriginal);
				ClassicAssert.IsNotNull (entry, "Entry 0x9291 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("800", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9292 (SubSecTimeDigitized/Ascii/4) "800"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SubsecTimeDigitized);
				ClassicAssert.IsNotNull (entry, "Entry 0x9292 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("800", (entry as StringIFDEntry).Value);
			}
			// Photo.0xA402 (ExposureMode/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureMode);
				ClassicAssert.IsNotNull (entry, "Entry 0xA402 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA403 (WhiteBalance/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.WhiteBalance);
				ClassicAssert.IsNotNull (entry, "Entry 0xA403 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA405 (FocalLengthIn35mmFilm/Short/1) "75"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalLengthIn35mmFilm);
				ClassicAssert.IsNotNull (entry, "Entry 0xA405 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (75, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA406 (SceneCaptureType/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SceneCaptureType);
				ClassicAssert.IsNotNull (entry, "Entry 0xA406 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA408 (Contrast/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Contrast);
				ClassicAssert.IsNotNull (entry, "Entry 0xA408 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA409 (Saturation/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Saturation);
				ClassicAssert.IsNotNull (entry, "Entry 0xA409 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA40A (Sharpness/Short/1) "1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Sharpness);
				ClassicAssert.IsNotNull (entry, "Entry 0xA40A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA40C (SubjectDistanceRange/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SubjectDistanceRange);
				ClassicAssert.IsNotNull (entry, "Entry 0xA40C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}

			//  ---------- End of IFD tests ----------


			//  ---------- Start of XMP tests ----------

			var xmp = file.GetTag (TagTypes.XMP) as XmpTag;
			// Xmp.tiff.Model (XmpText/10) "NIKON D70s"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "Model");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("NIKON D70s", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.tiff.Make (XmpText/17) "NIKON CORPORATION"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "Make");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("NIKON CORPORATION", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.tiff.ImageWidth (XmpText/4) "3024"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "ImageWidth");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("3024", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.tiff.ImageLength (XmpText/4) "1998"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "ImageLength");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("1998", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.tiff.FNumber (XmpText/6) "130/10"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "FNumber");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("130/10", node.Value);
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
			// Xmp.exif.ExposureProgram (XmpText/1) "2"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "ExposureProgram");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("2", node.Value);
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
			// Xmp.exif.LightSource (XmpText/1) "0"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "LightSource");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("0", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.ExposureMode (XmpText/1) "0"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "ExposureMode");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("0", node.Value);
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
			// Xmp.exif.FocalLengthIn35mmFilm (XmpText/2) "75"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "FocalLengthIn35mmFilm");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("75", node.Value);
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
			// Xmp.exif.Contrast (XmpText/1) "0"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "Contrast");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("0", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.Saturation (XmpText/1) "0"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "Saturation");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("0", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.Sharpness (XmpText/1) "1"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "Sharpness");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("1", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.SubjectDistanceRange (XmpText/1) "0"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "SubjectDistanceRange");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("0", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.ISOSpeedRating (XmpText/4) "1600"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "ISOSpeedRating");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("1600", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.MaxApertureValue (XmpText/5) "50/10"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "MaxApertureValue");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("50/10", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.ExposureTime (XmpText/7) "10/7500"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "ExposureTime");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("10/7500", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.ExposureBiasValue (XmpText/3) "0/6"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "ExposureBiasValue");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("0/6", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.exif.FocalLength (XmpText/6) "500/10"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.EXIF_NS, "FocalLength");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("500/10", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.photoshop.DateCreated (XmpText/24) "2007-02-15T17:07:48.800Z"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.PHOTOSHOP_NS, "DateCreated");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("2007-02-15T17:07:48.800Z", node.Value);
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
