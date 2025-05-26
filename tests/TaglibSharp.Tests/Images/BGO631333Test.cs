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
	public class BGO631333Test
	{
		[Test]
		public void Test ()
		{
			ImageTest.Run ("sample_bgo631333.jpg",
				new BGO631333TestInvariantValidator (),
				NoModificationValidator.Instance
			);
		}
	}

	public class BGO631333TestInvariantValidator : IMetadataInvariantValidator
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
			// Image.0x0110 (Model/Ascii/21) "Canon EOS 5D Mark II"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Model);
				ClassicAssert.IsNotNull (entry, "Entry 0x0110 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("Canon EOS 5D Mark II", (entry as StringIFDEntry).Value);
			}
			// Image.0x0112 (Orientation/Short/1) "1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Orientation);
				ClassicAssert.IsNotNull (entry, "Entry 0x0112 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Image.0x011A (XResolution/Rational/1) "72/1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.XResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (72, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Image.0x011B (YResolution/Rational/1) "72/1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.YResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (72, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Image.0x0128 (ResolutionUnit/Short/1) "2"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ResolutionUnit);
				ClassicAssert.IsNotNull (entry, "Entry 0x0128 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Image.0x0131 (Software/Ascii/23) "f-spot version 0.6.1.5"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Software);
				ClassicAssert.IsNotNull (entry, "Entry 0x0131 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("f-spot version 0.6.1.5", (entry as StringIFDEntry).Value);
			}
			// Image.0x0132 (DateTime/Ascii/20) "2010:09:29 20:49:37"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.DateTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x0132 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2010:09:29 20:49:37", (entry as StringIFDEntry).Value);
			}
			// Image.0x013B (Artist/Ascii/1) ""
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Artist);
				ClassicAssert.IsNotNull (entry, "Entry 0x013B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("", (entry as StringIFDEntry).Value.Trim ());
			}
			// Image.0x0213 (YCbCrPositioning/Short/1) "2"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.YCbCrPositioning);
				ClassicAssert.IsNotNull (entry, "Entry 0x0213 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Image.0x8298 (Copyright/Ascii/1) ""
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Copyright);
				ClassicAssert.IsNotNull (entry, "Entry 0x8298 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("", (entry as StringIFDEntry).Value.Trim ());
			}
			// Image.0x8769 (ExifTag/SubIFD/1) "258"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD);
				ClassicAssert.IsNotNull (entry, "Entry 0x8769 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SubIFDEntry, "Entry is not a sub IFD!");
			}

			var exif = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD) as SubIFDEntry;
			ClassicAssert.IsNotNull (exif, "Exif tag not found");
			var exif_structure = exif.Structure;

			// Photo.0x829A (ExposureTime/Rational/1) "1/125"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x829A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (125, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x829D (FNumber/Rational/1) "4/1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FNumber);
				ClassicAssert.IsNotNull (entry, "Entry 0x829D missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (4, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x8822 (ExposureProgram/Short/1) "1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureProgram);
				ClassicAssert.IsNotNull (entry, "Entry 0x8822 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x8827 (ISOSpeedRatings/Short/1) "1600"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ISOSpeedRatings);
				ClassicAssert.IsNotNull (entry, "Entry 0x8827 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1600, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x9000 (ExifVersion/Undefined/4) "48 50 50 49 "
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExifVersion);
				ClassicAssert.IsNotNull (entry, "Entry 0x9000 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 50, 50, 49 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Photo.0x9003 (DateTimeOriginal/Ascii/20) "2009:05:16 20:37:35"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeOriginal);
				ClassicAssert.IsNotNull (entry, "Entry 0x9003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2009:05:16 20:37:35", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9004 (DateTimeDigitized/Ascii/20) "2009:05:16 18:58:35"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeDigitized);
				ClassicAssert.IsNotNull (entry, "Entry 0x9004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2009:05:16 18:58:35", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9101 (ComponentsConfiguration/Undefined/4) "1 2 3 0 "
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ComponentsConfiguration);
				ClassicAssert.IsNotNull (entry, "Entry 0x9101 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 1, 2, 3, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Photo.0x9201 (ShutterSpeedValue/SRational/1) "458752/65536"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ShutterSpeedValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9201 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SRationalIFDEntry, "Entry is not a srational!");
				ClassicAssert.AreEqual (458752, (entry as SRationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (65536, (entry as SRationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9202 (ApertureValue/Rational/1) "262144/65536"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ApertureValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9202 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (262144, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (65536, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9204 (ExposureBiasValue/SRational/1) "0/1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureBiasValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9204 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SRationalIFDEntry, "Entry is not a srational!");
				ClassicAssert.AreEqual (0, (entry as SRationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as SRationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9207 (MeteringMode/Short/1) "2"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MeteringMode);
				ClassicAssert.IsNotNull (entry, "Entry 0x9207 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x9209 (Flash/Short/1) "9"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Flash);
				ClassicAssert.IsNotNull (entry, "Entry 0x9209 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (9, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x920A (FocalLength/Rational/1) "95/1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x920A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (95, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9286 (UserComment/UserComment/8) "charset="Ascii" "
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.UserComment);
				ClassicAssert.IsNotNull (entry, "Entry 0x9286 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UserCommentIFDEntry, "Entry is not a user comment!");
				ClassicAssert.AreEqual ("", (entry as UserCommentIFDEntry).Value.Trim ());
			}
			// Photo.0x9290 (SubSecTime/Ascii/3) "09"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SubsecTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x9290 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("09", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9291 (SubSecTimeOriginal/Ascii/3) "09"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SubsecTimeOriginal);
				ClassicAssert.IsNotNull (entry, "Entry 0x9291 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("09", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9292 (SubSecTimeDigitized/Ascii/3) "09"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SubsecTimeDigitized);
				ClassicAssert.IsNotNull (entry, "Entry 0x9292 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("09", (entry as StringIFDEntry).Value);
			}
			// Photo.0xA000 (FlashpixVersion/Undefined/4) "48 49 48 48 "
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FlashpixVersion);
				ClassicAssert.IsNotNull (entry, "Entry 0xA000 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 49, 48, 48 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Photo.0xA001 (ColorSpace/Short/1) "1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ColorSpace);
				ClassicAssert.IsNotNull (entry, "Entry 0xA001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA002 (PixelXDimension/Long/1) "3253"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.PixelXDimension);
				ClassicAssert.IsNotNull (entry, "Entry 0xA002 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (3253, (entry as LongIFDEntry).Value);
			}
			// Photo.0xA003 (PixelYDimension/Long/1) "4880"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.PixelYDimension);
				ClassicAssert.IsNotNull (entry, "Entry 0xA003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (4880, (entry as LongIFDEntry).Value);
			}
			// Photo.0xA20E (FocalPlaneXResolution/Rational/1) "5616000/1459"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalPlaneXResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0xA20E missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (5616000, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1459, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0xA20F (FocalPlaneYResolution/Rational/1) "3744000/958"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalPlaneYResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0xA20F missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (3744000, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (958, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0xA210 (FocalPlaneResolutionUnit/Short/1) "2"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalPlaneResolutionUnit);
				ClassicAssert.IsNotNull (entry, "Entry 0xA210 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA401 (CustomRendered/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.CustomRendered);
				ClassicAssert.IsNotNull (entry, "Entry 0xA401 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
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
			// Image.0x8825 (GPSTag/SubIFD/1) "724"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.GPSIFD);
				ClassicAssert.IsNotNull (entry, "Entry 0x8825 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SubIFDEntry, "Entry is not a sub IFD!");
			}

			var gps = structure.GetEntry (0, (ushort)IFDEntryTag.GPSIFD) as SubIFDEntry;
			ClassicAssert.IsNotNull (gps, "GPS tag not found");
			var gps_structure = gps.Structure;

			// GPSInfo.0x0000 (GPSVersionID/Byte/4) "2 2 0 0 "
			{
				var entry = gps_structure.GetEntry (0, (ushort)GPSEntryTag.GPSVersionID);
				ClassicAssert.IsNotNull (entry, "Entry 0x0000 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ByteVectorIFDEntry, "Entry is not a byte array!");
				var parsed_bytes = (entry as ByteVectorIFDEntry).Data.Data;
				var bytes = new byte[] { 2, 2, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Thumbnail.0x0103 (Compression/Short/1) "6"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.Compression);
				ClassicAssert.IsNotNull (entry, "Entry 0x0103 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (6, (entry as ShortIFDEntry).Value);
			}
			// Thumbnail.0x011A (XResolution/Rational/1) "72/1"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.XResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011A missing in IFD 1");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (72, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Thumbnail.0x011B (YResolution/Rational/1) "72/1"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.YResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011B missing in IFD 1");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (72, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Thumbnail.0x0128 (ResolutionUnit/Short/1) "2"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.ResolutionUnit);
				ClassicAssert.IsNotNull (entry, "Entry 0x0128 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Thumbnail.0x0201 (JPEGInterchangeFormat/ThumbnailDataIFD/1) "836"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.JPEGInterchangeFormat);
				ClassicAssert.IsNotNull (entry, "Entry 0x0201 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as ThumbnailDataIFDEntry, "Entry is not a thumbnail IFD!");
			}
			// Thumbnail.0x0202 (JPEGInterchangeFormatLength/Long/1) "4463"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.JPEGInterchangeFormatLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x0202 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (4463, (entry as LongIFDEntry).Value);
			}

			//  ---------- End of IFD tests ----------


			//  ---------- Start of XMP tests ----------

			var xmp = file.GetTag (TagTypes.XMP) as XmpTag;
			// Xmp.tiff.Software (XmpText/46) "Microsoft Windows Photo Gallery 6.0.6000.16386"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "Software");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("Microsoft Windows Photo Gallery 6.0.6000.16386", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.tiff.software (XmpText/46) "Microsoft Windows Photo Gallery 6.0.6000.16386"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.TIFF_NS, "software");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("Microsoft Windows Photo Gallery 6.0.6000.16386", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.xmp.CreatorTool (XmpText/49) "Adobe Photoshop Elements for Windows, version 2.0"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.XAP_NS, "CreatorTool");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("Adobe Photoshop Elements for Windows, version 2.0", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.xmp.creatortool (XmpText/46) "Microsoft Windows Photo Gallery 6.0.6000.16386"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.XAP_NS, "creatortool");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("Microsoft Windows Photo Gallery 6.0.6000.16386", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.xmpMM.DocumentID (XmpText/58) "adobe:docid:photoshop:f48fd669-42b3-11de-a9a9-b4912218066d"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.XAP_MM_NS, "DocumentID");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual ("adobe:docid:photoshop:f48fd669-42b3-11de-a9a9-b4912218066d", node.Value);
				ClassicAssert.AreEqual (XmpNodeType.Simple, node.Type);
				ClassicAssert.AreEqual (0, node.Children.Count);
			}
			// Xmp.dc.subject (XmpBag/2) "TASI, 50v-juhlat"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.DC_NS, "subject");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual (XmpNodeType.Bag, node.Type);
				ClassicAssert.AreEqual ("", node.Value);
				ClassicAssert.AreEqual (2, node.Children.Count);
				var children_array = new System.Collections.Generic.List<string> ();
				foreach (var child in node.Children) {
					children_array.Add (child.Value);
				}
				ClassicAssert.IsTrue (children_array.Contains ("TASI"));
				ClassicAssert.IsTrue (children_array.Contains ("50v-juhlat"));
			}

			//  ---------- End of XMP tests ----------

		}
	}
}
