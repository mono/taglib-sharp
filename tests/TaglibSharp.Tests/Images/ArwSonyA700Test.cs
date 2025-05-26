using NUnit.Framework;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using TaglibSharp.Tests.Images.Validators;

namespace TaglibSharp.Tests.Images
{
	[TestFixture]
	public class ArwSonyA700
	{
		[Test]
		public void Test ()
		{
			ImageTest.Run (TestPath.GetRawSubDirectory ("ARW"), "RAW_SONY_A700.ARW",
						   false, new ArwSonyA700InvariantValidator ());
		}
	}

	public class ArwSonyA700InvariantValidator : IMetadataInvariantValidator
	{
		public void ValidateMetadataInvariants (TagLib.Image.File file)
		{
			ClassicAssert.IsNotNull (file);
			//  ---------- Start of IFD tests ----------

			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			ClassicAssert.IsNotNull (tag, "IFD tag not found");

			var structure = tag.Structure;

			// Image.0x00FE (NewSubfileType/Long/1) "1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.NewSubfileType);
				ClassicAssert.IsNotNull (entry, "Entry 0x00FE missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (1, (entry as LongIFDEntry).Value);
			}
			// Image.0x0103 (Compression/Short/1) "6"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Compression);
				ClassicAssert.IsNotNull (entry, "Entry 0x0103 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (6, (entry as ShortIFDEntry).Value);
			}
			// Image.0x010E (ImageDescription/Ascii/9) "SONY DSC"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ImageDescription);
				ClassicAssert.IsNotNull (entry, "Entry 0x010E missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("SONY DSC", (entry as StringIFDEntry).Value);
			}
			// Image.0x010F (Make/Ascii/6) "SONY "
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Make);
				ClassicAssert.IsNotNull (entry, "Entry 0x010F missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("SONY ", (entry as StringIFDEntry).Value);
			}
			// Image.0x0110 (Model/Ascii/10) "DSLR-A700"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Model);
				ClassicAssert.IsNotNull (entry, "Entry 0x0110 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("DSLR-A700", (entry as StringIFDEntry).Value);
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
			// Image.0x0131 (Software/Ascii/14) "DSLR-A700 v03"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Software);
				ClassicAssert.IsNotNull (entry, "Entry 0x0131 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("DSLR-A700 v03", (entry as StringIFDEntry).Value);
			}
			// Image.0x0132 (DateTime/Ascii/20) "2008:01:01 15:29:46"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.DateTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x0132 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2008:01:01 15:29:46", (entry as StringIFDEntry).Value);
			}

			var SubImage1_structure = (structure.GetEntry (0, (ushort)IFDEntryTag.SubIFDs) as SubIFDArrayEntry).Entries[0];
			ClassicAssert.IsNotNull (SubImage1_structure, "SubImage1 structure not found");

			// SubImage1.0x00FE (NewSubfileType/Long/1) "0"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.NewSubfileType);
				ClassicAssert.IsNotNull (entry, "Entry 0x00FE missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (0, (entry as LongIFDEntry).Value);
			}
			// SubImage1.0x0100 (ImageWidth/Short/1) "4288"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.ImageWidth);
				ClassicAssert.IsNotNull (entry, "Entry 0x0100 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (4288, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x0101 (ImageLength/Short/1) "2856"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.ImageLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x0101 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2856, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x0102 (BitsPerSample/Short/1) "8"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.BitsPerSample);
				ClassicAssert.IsNotNull (entry, "Entry 0x0102 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (8, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x0103 (Compression/Short/1) "32767"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.Compression);
				ClassicAssert.IsNotNull (entry, "Entry 0x0103 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (32767, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x0106 (PhotometricInterpretation/Short/1) "32803"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.PhotometricInterpretation);
				ClassicAssert.IsNotNull (entry, "Entry 0x0106 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (32803, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x0112 (Orientation/Short/1) "1"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.Orientation);
				ClassicAssert.IsNotNull (entry, "Entry 0x0112 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x0115 (SamplesPerPixel/Short/1) "1"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.SamplesPerPixel);
				ClassicAssert.IsNotNull (entry, "Entry 0x0115 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x011C (PlanarConfiguration/Short/1) "1"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.PlanarConfiguration);
				ClassicAssert.IsNotNull (entry, "Entry 0x011C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x011A (XResolution/Rational/1) "1/72"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.XResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (72, (entry as RationalIFDEntry).Value.Denominator);
			}
			// SubImage1.0x011B (YResolution/Rational/1) "1/72"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.YResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (72, (entry as RationalIFDEntry).Value.Denominator);
			}
			// SubImage1.0x0128 (ResolutionUnit/Short/1) "3"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.ResolutionUnit);
				ClassicAssert.IsNotNull (entry, "Entry 0x0128 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (3, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x828D (CFARepeatPatternDim/Short/2) "2 2"
			{
				// TODO: Unknown IFD tag: SubImage1 / 0x828D
				var entry = SubImage1_structure.GetEntry (0, 0x828D);
				ClassicAssert.IsNotNull (entry, "Entry 0x828D missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.AreEqual (new ushort[] { 2, 2 }, (entry as ShortArrayIFDEntry).Values);
			}
			// SubImage1.0x828E (CFAPattern/Byte/4) "0 1 1 2"
			{
				// TODO: Unknown IFD tag: SubImage1 / 0x828E
				var entry = SubImage1_structure.GetEntry (0, 0x828E);
				ClassicAssert.IsNotNull (entry, "Entry 0x828E missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ByteVectorIFDEntry, "Entry is not a byte array!");
				var parsed_bytes = (entry as ByteVectorIFDEntry).Data.Data;
				var bytes = new byte[] { 0, 1, 1, 2 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// SubImage1.0x7000 (0x7000/Short/1) "2"
			{
				// TODO: Unknown IFD tag: SubImage1 / 0x7000
				var entry = SubImage1_structure.GetEntry (0, 0x7000);
				ClassicAssert.IsNotNull (entry, "Entry 0x7000 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x7001 (0x7001/Short/1) "1"
			{
				// TODO: Unknown IFD tag: SubImage1 / 0x7001
				var entry = SubImage1_structure.GetEntry (0, 0x7001);
				ClassicAssert.IsNotNull (entry, "Entry 0x7001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x7010 (0x7010/Short/4) "8000 10400 12900 14100"
			{
				// TODO: Unknown IFD tag: SubImage1 / 0x7010
				var entry = SubImage1_structure.GetEntry (0, 0x7010);
				ClassicAssert.IsNotNull (entry, "Entry 0x7010 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.AreEqual (new ushort[] { 8000, 10400, 12900, 14100 }, (entry as ShortArrayIFDEntry).Values);
			}
			// SubImage1.0x7011 (0x7011/Short/4) "4000 7200 10050 12075"
			{
				// TODO: Unknown IFD tag: SubImage1 / 0x7011
				var entry = SubImage1_structure.GetEntry (0, 0x7011);
				ClassicAssert.IsNotNull (entry, "Entry 0x7011 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.AreEqual (new ushort[] { 4000, 7200, 10050, 12075 }, (entry as ShortArrayIFDEntry).Values);
			}
			// SubImage1.0x0111 (StripOffsets/StripOffsets/1) "688128"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.StripOffsets);
				ClassicAssert.IsNotNull (entry, "Entry 0x0111 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StripOffsetsIFDEntry, "Entry is not a strip offsets entry!");
				ClassicAssert.AreEqual (1, (entry as StripOffsetsIFDEntry).Values.Length);
			}
			// SubImage1.0x0116 (RowsPerStrip/Short/1) "2856"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.RowsPerStrip);
				ClassicAssert.IsNotNull (entry, "Entry 0x0116 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2856, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x0117 (StripByteCounts/Long/1) "12246528"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.StripByteCounts);
				ClassicAssert.IsNotNull (entry, "Entry 0x0117 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (12246528, (entry as LongIFDEntry).Value);
			}
			// Image.0x0201 (JPEGInterchangeFormat/ThumbnailDataIFD/1) "155683"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.JPEGInterchangeFormat);
				ClassicAssert.IsNotNull (entry, "Entry 0x0201 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ThumbnailDataIFDEntry, "Entry is not a thumbnail IFD!");
			}
			// Image.0x0202 (JPEGInterchangeFormatLength/Long/1) "502499"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.JPEGInterchangeFormatLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x0202 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (502499, (entry as LongIFDEntry).Value);
			}
			// Image.0x8769 (ExifTag/SubIFD/1) "322"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD);
				ClassicAssert.IsNotNull (entry, "Entry 0x8769 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SubIFDEntry, "Entry is not a sub IFD!");
			}

			var exif = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD) as SubIFDEntry;
			ClassicAssert.IsNotNull (exif, "Exif tag not found");
			var exif_structure = exif.Structure;

			// Photo.0x829A (ExposureTime/Rational/1) "1/1000"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x829A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1000, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x829D (FNumber/Rational/1) "80/10"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FNumber);
				ClassicAssert.IsNotNull (entry, "Entry 0x829D missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (80, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (10, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x8822 (ExposureProgram/Short/1) "3"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureProgram);
				ClassicAssert.IsNotNull (entry, "Entry 0x8822 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (3, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x8827 (ISOSpeedRatings/Short/1) "250"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ISOSpeedRatings);
				ClassicAssert.IsNotNull (entry, "Entry 0x8827 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (250, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x9000 (ExifVersion/Undefined/4) "48 50 50 49"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExifVersion);
				ClassicAssert.IsNotNull (entry, "Entry 0x9000 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 50, 50, 49 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Photo.0x9003 (DateTimeOriginal/Ascii/20) "2008:01:01 15:29:46"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeOriginal);
				ClassicAssert.IsNotNull (entry, "Entry 0x9003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2008:01:01 15:29:46", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9004 (DateTimeDigitized/Ascii/20) "2008:01:01 15:29:46"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeDigitized);
				ClassicAssert.IsNotNull (entry, "Entry 0x9004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2008:01:01 15:29:46", (entry as StringIFDEntry).Value);
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
			// Photo.0x9102 (CompressedBitsPerPixel/Rational/1) "8/1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.CompressedBitsPerPixel);
				ClassicAssert.IsNotNull (entry, "Entry 0x9102 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (8, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9203 (BrightnessValue/SRational/1) "900/100"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.BrightnessValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9203 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SRationalIFDEntry, "Entry is not a srational!");
				ClassicAssert.AreEqual (900, (entry as SRationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (100, (entry as SRationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9204 (ExposureBiasValue/SRational/1) "-8/10"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureBiasValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9204 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SRationalIFDEntry, "Entry is not a srational!");
				ClassicAssert.AreEqual (-8, (entry as SRationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (10, (entry as SRationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9205 (MaxApertureValue/Rational/1) "531/100"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MaxApertureValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9205 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (531, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (100, (entry as RationalIFDEntry).Value.Denominator);
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
			// Photo.0x9209 (Flash/Short/1) "16"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Flash);
				ClassicAssert.IsNotNull (entry, "Entry 0x9209 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (16, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x920A (FocalLength/Rational/1) "2500/10"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x920A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (2500, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (10, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x927C (MakerNote/MakerNote/24448) "(Value ommitted)"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MakerNote);
				ClassicAssert.IsNotNull (entry, "Entry 0x927C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as MakernoteIFDEntry, "Entry is not a makernote IFD!");
			}

			var makernote = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MakerNote) as MakernoteIFDEntry;
			ClassicAssert.IsNotNull (makernote, "MakerNote tag not found");
			var makernote_structure = makernote.Structure;

			// Sony.0x0010 (0x0010/Undefined/368) "0 1 2 24 53 80 0 99 2 1 4 3 6 5 8 7 64 114 128 0 1 0 48 2 8 8 8 6 17 11 255 44 255 160 255 110 255 96 255 6 128 0 255 222 0 135 255 37 255 44 254 227 128 0 255 99 254 186 255 87 255 56 128 0 128 0 0 205 255 93 255 177 255 130 255 131 128 0 1 148 4 47 4 24 1 189 0 255 1 80 2 33 2 134 1 236 1 243 4 185 2 86 1 128 3 45 1 53 1 37 1 100 2 4 2 52 1 51 1 87 2 163 0 0 0 0 1 227 2 14 2 224 3 121 15 14 1 187 2 62 2 45 0 192 2 77 4 134 255 255 4 56 0 0 20 28 43 195 29 45 27 162 1 77 2 162 16 35 15 238 13 106 14 178 20 147 6 115 6 245 26 83 21 232 0 245 1 77 23 253 18 145 4 167 17 67 37 113 0 0 0 0 2 74 9 133 3 63 1 162 4 213 1 93 2 66 2 192 1 250 3 204 2 109 1 168 3 173 0 0 132 16 0 65 128 16 255 193 255 255 255 255 0 8 0 128 0 128 0 128 0 128 0 128 0 128 68 128 71 255 97 255 0 255 0 128 0 128 0 128 0 128 0 128 0 128 0 128 0 128 0 128 0 128 0 128 0 128 147 128 20 2 0 0 0 0 0 0 0 0 0 0 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255 255"
			{
				// TODO: Unknown IFD tag: Sony / 0x0010
				var entry = makernote_structure.GetEntry (0, 0x0010);
				ClassicAssert.IsNotNull (entry, "Entry 0x0010 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 0, 1, 2, 24, 53, 80, 0, 99, 2, 1, 4, 3, 6, 5, 8, 7, 64, 114, 128, 0, 1, 0, 48, 2, 8, 8, 8, 6, 17, 11, 255, 44, 255, 160, 255, 110, 255, 96, 255, 6, 128, 0, 255, 222, 0, 135, 255, 37, 255, 44, 254, 227, 128, 0, 255, 99, 254, 186, 255, 87, 255, 56, 128, 0, 128, 0, 0, 205, 255, 93, 255, 177, 255, 130, 255, 131, 128, 0, 1, 148, 4, 47, 4, 24, 1, 189, 0, 255, 1, 80, 2, 33, 2, 134, 1, 236, 1, 243, 4, 185, 2, 86, 1, 128, 3, 45, 1, 53, 1, 37, 1, 100, 2, 4, 2, 52, 1, 51, 1, 87, 2, 163, 0, 0, 0, 0, 1, 227, 2, 14, 2, 224, 3, 121, 15, 14, 1, 187, 2, 62, 2, 45, 0, 192, 2, 77, 4, 134, 255, 255, 4, 56, 0, 0, 20, 28, 43, 195, 29, 45, 27, 162, 1, 77, 2, 162, 16, 35, 15, 238, 13, 106, 14, 178, 20, 147, 6, 115, 6, 245, 26, 83, 21, 232, 0, 245, 1, 77, 23, 253, 18, 145, 4, 167, 17, 67, 37, 113, 0, 0, 0, 0, 2, 74, 9, 133, 3, 63, 1, 162, 4, 213, 1, 93, 2, 66, 2, 192, 1, 250, 3, 204, 2, 109, 1, 168, 3, 173, 0, 0, 132, 16, 0, 65, 128, 16, 255, 193, 255, 255, 255, 255, 0, 8, 0, 128, 0, 128, 0, 128, 0, 128, 0, 128, 0, 128, 68, 128, 71, 255, 97, 255, 0, 255, 0, 128, 0, 128, 0, 128, 0, 128, 0, 128, 0, 128, 0, 128, 0, 128, 0, 128, 0, 128, 0, 128, 0, 128, 147, 128, 20, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Sony.0x0018 (0x0018/Undefined/4096) "(Value ommitted)"
			{
				// TODO: Unknown IFD tag: Sony / 0x0018
				var entry = makernote_structure.GetEntry (0, 0x0018);
				ClassicAssert.IsNotNull (entry, "Entry 0x0018 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var parsed_hash = Utils.Md5Encode (parsed_bytes);
				ClassicAssert.AreEqual ("7777789ce0a4d6c76f66adbca9c58ebe", parsed_hash);
				ClassicAssert.AreEqual (4096, parsed_bytes.Length);
			}
			// Sony.0x0020 (0x0020/Undefined/19148) "(Value ommitted)"
			{
				// TODO: Unknown IFD tag: Sony / 0x0020
				var entry = makernote_structure.GetEntry (0, 0x0020);
				ClassicAssert.IsNotNull (entry, "Entry 0x0020 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var parsed_hash = Utils.Md5Encode (parsed_bytes);
				ClassicAssert.AreEqual ("c6dfa82a9b660630e64b64e39220fb98", parsed_hash);
				ClassicAssert.AreEqual (19148, parsed_bytes.Length);
			}
			// Sony.0x0102 (0x0102/Long/1) "8"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.Quality);
				ClassicAssert.IsNotNull (entry, "Entry 0x0102 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (8, (entry as LongIFDEntry).Value);
			}
			// Sony.0x0104 (0x0104/SRational/1) "0/10"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.FlashExposureComp);
				ClassicAssert.IsNotNull (entry, "Entry 0x0104 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SRationalIFDEntry, "Entry is not a srational!");
				ClassicAssert.AreEqual (0, (entry as SRationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (10, (entry as SRationalIFDEntry).Value.Denominator);
			}
			// Sony.0x0105 (0x0105/Long/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.Teleconverter);
				ClassicAssert.IsNotNull (entry, "Entry 0x0105 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (0, (entry as LongIFDEntry).Value);
			}
			// Sony.0x0112 (0x0112/Long/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.WhiteBalanceFineTune);
				ClassicAssert.IsNotNull (entry, "Entry 0x0112 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (0, (entry as LongIFDEntry).Value);
			}
			// Sony.0x0114 (0x0114/Undefined/280) "0 129 0 56 0 0 0 96 0 7 0 2 0 0 0 55 0 0 0 0 0 0 0 0 0 55 0 0 0 0 0 2 0 1 0 0 0 1 0 4 0 128 0 1 0 58 0 0 0 0 0 3 0 1 0 0 0 10 0 9 0 9 0 10 0 10 0 10 0 10 0 0 0 80 0 56 0 0 0 0 0 0 0 1 0 0 0 0 0 1 0 1 0 0 0 80 0 56 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 3 0 1 0 0 0 0 0 1 0 100 0 112 0 124 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 5 0 1 0 0 0 0 0 6 0 94 0 0 0 1 0 1 0 1 0 35 0 6 0 33 0 0 0 0 0 1 0 1 0 1 0 1 0 1 0 0 0 0 0 1 0 0 0 1 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 1 0 2 0 0 0 1 0 1 0 0 0 1 0 0 0 53 0 23 0 8 0 0 0 18 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0114 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 0, 129, 0, 56, 0, 0, 0, 96, 0, 7, 0, 2, 0, 0, 0, 55, 0, 0, 0, 0, 0, 0, 0, 0, 0, 55, 0, 0, 0, 0, 0, 2, 0, 1, 0, 0, 0, 1, 0, 4, 0, 128, 0, 1, 0, 58, 0, 0, 0, 0, 0, 3, 0, 1, 0, 0, 0, 10, 0, 9, 0, 9, 0, 10, 0, 10, 0, 10, 0, 10, 0, 0, 0, 80, 0, 56, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 80, 0, 56, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 1, 0, 0, 0, 0, 0, 1, 0, 100, 0, 112, 0, 124, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 1, 0, 0, 0, 0, 0, 6, 0, 94, 0, 0, 0, 1, 0, 1, 0, 1, 0, 35, 0, 6, 0, 33, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 2, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 53, 0, 23, 0, 8, 0, 0, 0, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Sony.0x0115 (0x0115/Long/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.WhiteBalance);
				ClassicAssert.IsNotNull (entry, "Entry 0x0115 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (0, (entry as LongIFDEntry).Value);
			}
			// Sony.0x2000 (0x2000/Undefined/1) "0"
			{
				// TODO: Unknown IFD tag: Sony / 0x2000
				var entry = makernote_structure.GetEntry (0, 0x2000);
				ClassicAssert.IsNotNull (entry, "Entry 0x2000 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Sony.0x2002 (0x2002/Long/1) "0"
			{
				// TODO: Unknown IFD tag: Sony / 0x2002
				var entry = makernote_structure.GetEntry (0, 0x2002);
				ClassicAssert.IsNotNull (entry, "Entry 0x2002 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (0, (entry as LongIFDEntry).Value);
			}
			// Sony.0x2003 (0x2003/Ascii/256) ""
			{
				// TODO: Unknown IFD tag: Sony / 0x2003
				var entry = makernote_structure.GetEntry (0, 0x2003);
				ClassicAssert.IsNotNull (entry, "Entry 0x2003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("", (entry as StringIFDEntry).Value.Trim ());
			}
			// Sony.0xB000 (0xb000/Byte/4) "3 0 0 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.FileFormat);
				ClassicAssert.IsNotNull (entry, "Entry 0xB000 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ByteVectorIFDEntry, "Entry is not a byte array!");
				var parsed_bytes = (entry as ByteVectorIFDEntry).Data.Data;
				var bytes = new byte[] { 3, 0, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Sony.0xB001 (0xb001/Short/1) "258"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.SonyModelID);
				ClassicAssert.IsNotNull (entry, "Entry 0xB001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (258, (entry as ShortIFDEntry).Value);
			}
			// Sony.0xB020 (0xb020/Ascii/16) "Standard"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.ColorReproduction);
				ClassicAssert.IsNotNull (entry, "Entry 0xB020 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("Standard", (entry as StringIFDEntry).Value);
			}
			// Sony.0xB021 (0xb021/Long/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.ColorTemperature);
				ClassicAssert.IsNotNull (entry, "Entry 0xB021 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (0, (entry as LongIFDEntry).Value);
			}
			// Sony.0xB022 (0xb022/Long/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.ColorCompensationFilter);
				ClassicAssert.IsNotNull (entry, "Entry 0xB022 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (0, (entry as LongIFDEntry).Value);
			}
			// Sony.0xB023 (0xb023/Long/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.SceneMode);
				ClassicAssert.IsNotNull (entry, "Entry 0xB023 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (0, (entry as LongIFDEntry).Value);
			}
			// Sony.0xB024 (0xb024/Long/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.ZoneMatching);
				ClassicAssert.IsNotNull (entry, "Entry 0xB024 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (0, (entry as LongIFDEntry).Value);
			}
			// Sony.0xB025 (0xb025/Long/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.DynamicRangeOptimizer);
				ClassicAssert.IsNotNull (entry, "Entry 0xB025 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (0, (entry as LongIFDEntry).Value);
			}
			// Sony.0xB026 (0xb026/Long/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.ImageStabilization);
				ClassicAssert.IsNotNull (entry, "Entry 0xB026 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (1, (entry as LongIFDEntry).Value);
			}
			// Sony.0xB027 (0xb027/Long/1) "50"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.LensID);
				ClassicAssert.IsNotNull (entry, "Entry 0xB027 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (50, (entry as LongIFDEntry).Value);
			}
			// Sony.0xB029 (0xb029/Long/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.ColorMode);
				ClassicAssert.IsNotNull (entry, "Entry 0xB029 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (0, (entry as LongIFDEntry).Value);
			}
			// Sony.0xB02A (0xb02a/Byte/8) "1 0 24 2 80 53 99 0"
			{
				// TODO: Unknown IFD tag: Sony / 0xB02A
				var entry = makernote_structure.GetEntry (0, 0xB02A);
				ClassicAssert.IsNotNull (entry, "Entry 0xB02A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ByteVectorIFDEntry, "Entry is not a byte array!");
				var parsed_bytes = (entry as ByteVectorIFDEntry).Data.Data;
				var bytes = new byte[] { 1, 0, 24, 2, 80, 53, 99, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Sony.0xB02B (0xb02b/Long/2) "2848 4272"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.FullImageSize);
				ClassicAssert.IsNotNull (entry, "Entry 0xB02B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongArrayIFDEntry, "Entry is not a long array!");
				ClassicAssert.AreEqual (new long[] { 2848, 4272 }, (entry as LongArrayIFDEntry).Values);
			}
			// Sony.0xB02C (0xb02c/Long/2) "1080 1616"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)SonyMakerNoteEntryTag.PreviewImageSize);
				ClassicAssert.IsNotNull (entry, "Entry 0xB02C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongArrayIFDEntry, "Entry is not a long array!");
				ClassicAssert.AreEqual (new long[] { 1080, 1616 }, (entry as LongArrayIFDEntry).Values);
			}
			// Photo.0x9286 (UserComment/UserComment/64) ""
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.UserComment);
				ClassicAssert.IsNotNull (entry, "Entry 0x9286 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UserCommentIFDEntry, "Entry is not a user comment!");
				ClassicAssert.AreEqual ("", (entry as UserCommentIFDEntry).Value.Trim ());
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
			// Photo.0xA002 (PixelXDimension/Long/1) "4288"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.PixelXDimension);
				ClassicAssert.IsNotNull (entry, "Entry 0xA002 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (4288, (entry as LongIFDEntry).Value);
			}
			// Photo.0xA003 (PixelYDimension/Long/1) "2856"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.PixelYDimension);
				ClassicAssert.IsNotNull (entry, "Entry 0xA003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (2856, (entry as LongIFDEntry).Value);
			}
			// Photo.0xA005 (InteroperabilityTag/SubIFD/1) "25440"
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
			// Photo.0xA401 (CustomRendered/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.CustomRendered);
				ClassicAssert.IsNotNull (entry, "Entry 0xA401 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA402 (ExposureMode/Short/1) "2"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureMode);
				ClassicAssert.IsNotNull (entry, "Entry 0xA402 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA403 (WhiteBalance/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.WhiteBalance);
				ClassicAssert.IsNotNull (entry, "Entry 0xA403 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA404 (DigitalZoomRatio/Rational/1) "0/1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DigitalZoomRatio);
				ClassicAssert.IsNotNull (entry, "Entry 0xA404 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (0, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0xA405 (FocalLengthIn35mmFilm/Short/1) "375"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalLengthIn35mmFilm);
				ClassicAssert.IsNotNull (entry, "Entry 0xA405 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (375, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA406 (SceneCaptureType/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SceneCaptureType);
				ClassicAssert.IsNotNull (entry, "Entry 0xA406 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA408 (Contrast/Short/1) "1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Contrast);
				ClassicAssert.IsNotNull (entry, "Entry 0xA408 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA409 (Saturation/Short/1) "1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Saturation);
				ClassicAssert.IsNotNull (entry, "Entry 0xA409 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA40A (Sharpness/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Sharpness);
				ClassicAssert.IsNotNull (entry, "Entry 0xA40A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Image.0xC634 (DNGPrivateData/Byte/4) "112 137 0 0"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.DNGPrivateData);
				ClassicAssert.IsNotNull (entry, "Entry 0xC634 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ByteVectorIFDEntry, "Entry is not a byte array!");
				var parsed_bytes = (entry as ByteVectorIFDEntry).Data.Data;
				var bytes = new byte[] { 112, 137, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Thumbnail.0x00FE (NewSubfileType/Long/1) "1"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.NewSubfileType);
				ClassicAssert.IsNotNull (entry, "Entry 0x00FE missing in IFD 1");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (1, (entry as LongIFDEntry).Value);
			}
			// Thumbnail.0x0103 (Compression/Short/1) "6"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.Compression);
				ClassicAssert.IsNotNull (entry, "Entry 0x0103 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (6, (entry as ShortIFDEntry).Value);
			}
			// Thumbnail.0x010F (Make/Ascii/6) "SONY "
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.Make);
				ClassicAssert.IsNotNull (entry, "Entry 0x010F missing in IFD 1");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("SONY ", (entry as StringIFDEntry).Value);
			}
			// Thumbnail.0x0110 (Model/Ascii/10) "DSLR-A700"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.Model);
				ClassicAssert.IsNotNull (entry, "Entry 0x0110 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("DSLR-A700", (entry as StringIFDEntry).Value);
			}
			// Thumbnail.0x0112 (Orientation/Short/1) "1"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.Orientation);
				ClassicAssert.IsNotNull (entry, "Entry 0x0112 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
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
			// Thumbnail.0x0132 (DateTime/Ascii/20) "2008:01:01 15:29:46"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.DateTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x0132 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2008:01:01 15:29:46", (entry as StringIFDEntry).Value);
			}
			// Thumbnail.0x0201 (JPEGInterchangeFormat/ThumbnailDataIFD/1) "32770"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.JPEGInterchangeFormat);
				ClassicAssert.IsNotNull (entry, "Entry 0x0201 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as ThumbnailDataIFDEntry, "Entry is not a thumbnail IFD!");
			}
			// Thumbnail.0x0202 (JPEGInterchangeFormatLength/Long/1) "2414"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.JPEGInterchangeFormatLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x0202 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (2414, (entry as LongIFDEntry).Value);
			}

			//  ---------- End of IFD tests ----------

		}
	}
}
