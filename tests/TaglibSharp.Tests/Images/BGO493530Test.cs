using NUnit.Framework;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using TaglibSharp.Tests.Images.Validators;

namespace TaglibSharp.Tests.Images
{
	[TestFixture]
	public class BGO493530Test
	{
		[Test]
		public void Test ()
		{
			ImageTest.Run ("sample_bgo493530.jpg",
				false,
				new BGO493530TestInvariantValidator ()
			);
		}
	}

	public class BGO493530TestInvariantValidator : IMetadataInvariantValidator
	{
		public void ValidateMetadataInvariants (TagLib.Image.File file)
		{
			ClassicAssert.IsNotNull (file);
			//  ---------- Start of IFD tests ----------

			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			ClassicAssert.IsNotNull (tag, "IFD tag not found");

			var structure = tag.Structure;

			// Image.0x010F (Make/Ascii/2) "o2"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Make);
				ClassicAssert.IsNotNull (entry, "Entry 0x010F missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("o2", (entry as StringIFDEntry).Value);
			}
			// Image.0x0110 (Model/Ascii/11) "Xda_diamond"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Model);
				ClassicAssert.IsNotNull (entry, "Entry 0x0110 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("Xda_diamond", (entry as StringIFDEntry).Value);
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
			// Image.0x0131 (Software/Ascii/14) "6_00_30998_01"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Software);
				ClassicAssert.IsNotNull (entry, "Entry 0x0131 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("6_00_30998_01", (entry as StringIFDEntry).Value);
			}
			// Image.0x0213 (YCbCrPositioning/Short/1) "1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.YCbCrPositioning);
				ClassicAssert.IsNotNull (entry, "Entry 0x0213 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Image.0x8769 (ExifTag/SubIFD/1) "200"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD);
				ClassicAssert.IsNotNull (entry, "Entry 0x8769 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SubIFDEntry, "Entry is not a sub IFD!");
			}

			var exif = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD) as SubIFDEntry;
			ClassicAssert.IsNotNull (exif, "Exif tag not found");
			var exif_structure = exif.Structure;

			// Photo.0x8827 (ISOSpeedRatings/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ISOSpeedRatings);
				ClassicAssert.IsNotNull (entry, "Entry 0x8827 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x9204 (ExposureBiasValue/SRational/1) "0/1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureBiasValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9204 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SRationalIFDEntry, "Entry is not a srational!");
				ClassicAssert.AreEqual (0, (entry as SRationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as SRationalIFDEntry).Value.Denominator);
			}
			// Photo.0x0132 (0x0132/Ascii/20) "2008:08:22 15:55:31"
			{
				var entry = exif_structure.GetEntry (0, (ushort)IFDEntryTag.DateTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x0132 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2008:08:22 15:55:31", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9000 (ExifVersion/Undefined/4) "48 50 50 48"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExifVersion);
				ClassicAssert.IsNotNull (entry, "Entry 0x9000 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 50, 50, 48 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Photo.0x9003 (DateTimeOriginal/Ascii/20) "2008:08:22 15:55:31"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeOriginal);
				ClassicAssert.IsNotNull (entry, "Entry 0x9003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2008:08:22 15:55:31", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9004 (DateTimeDigitized/Ascii/20) "2008:08:22 15:55:31"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeDigitized);
				ClassicAssert.IsNotNull (entry, "Entry 0x9004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2008:08:22 15:55:31", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9101 (ComponentsConfiguration/Undefined/4) "1 2 3 0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ComponentsConfiguration);
				ClassicAssert.IsNotNull (entry, "Entry 0x9101 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 1, 2, 3, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Photo.0x9209 (Flash/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Flash);
				ClassicAssert.IsNotNull (entry, "Entry 0x9209 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA000 (FlashpixVersion/Undefined/4) "48 49 48 48"
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
			// Photo.0xA002 (PixelXDimension/Long/1) "2048"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.PixelXDimension);
				ClassicAssert.IsNotNull (entry, "Entry 0xA002 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (2048, (entry as LongIFDEntry).Value);
			}
			// Photo.0xA003 (PixelYDimension/Long/1) "1536"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.PixelYDimension);
				ClassicAssert.IsNotNull (entry, "Entry 0xA003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (1536, (entry as LongIFDEntry).Value);
			}
			// Photo.0xA005 (InteroperabilityTag/SubIFD/1) "530"
			{
				var entry = exif_structure.GetEntry (0, (ushort)IFDEntryTag.InteroperabilityIFD);
				ClassicAssert.IsNotNull (entry, "Entry 0xA005 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SubIFDEntry, "Entry is not a sub IFD!");
			}

			var iop = exif_structure.GetEntry (0, (ushort)IFDEntryTag.InteroperabilityIFD) as SubIFDEntry;
			ClassicAssert.IsNotNull (iop, "Iop tag not found");
			var iop_structure = iop.Structure;

			// Iop.0x0001 (InteroperabilityIndex/Ascii/4) "R98"
			{
				var entry = iop_structure.GetEntry (0, (ushort)IOPEntryTag.InteroperabilityIndex);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("R98", (entry as StringIFDEntry).Value);
			}
			// Iop.0x0002 (InteroperabilityVersion/Undefined/4) "48 49 48 48"
			{
				var entry = iop_structure.GetEntry (0, (ushort)IOPEntryTag.InteroperabilityVersion);
				ClassicAssert.IsNotNull (entry, "Entry 0x0002 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 49, 48, 48 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Photo.0xA300 (FileSource/Undefined/1) "3"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FileSource);
				ClassicAssert.IsNotNull (entry, "Entry 0xA300 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 3 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Photo.0xA301 (SceneType/Undefined/1) "1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SceneType);
				ClassicAssert.IsNotNull (entry, "Entry 0xA301 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 1 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Photo.0xA403 (WhiteBalance/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.WhiteBalance);
				ClassicAssert.IsNotNull (entry, "Entry 0xA403 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Thumbnail.0x0103 (Compression/Short/1) "6"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.Compression);
				ClassicAssert.IsNotNull (entry, "Entry 0x0103 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (6, (entry as ShortIFDEntry).Value);
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
			// Thumbnail.0x0201 (JPEGInterchangeFormat/ThumbnailDataIFD/1) "670"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.JPEGInterchangeFormat);
				ClassicAssert.IsNotNull (entry, "Entry 0x0201 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as ThumbnailDataIFDEntry, "Entry is not a thumbnail IFD!");
			}
			// Thumbnail.0x0202 (JPEGInterchangeFormatLength/Long/1) "16354"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.JPEGInterchangeFormatLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x0202 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (16354, (entry as LongIFDEntry).Value);
			}

			//  ---------- End of IFD tests ----------

		}
	}
}
