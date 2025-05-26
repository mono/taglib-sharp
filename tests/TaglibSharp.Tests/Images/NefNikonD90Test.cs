using NUnit.Framework;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using TaglibSharp.Tests.Images.Validators;

namespace TaglibSharp.Tests.Images
{
	[TestFixture]
	public class Nikon
	{
		[Test]
		public void Test ()
		{
			ImageTest.Run (TestPath.GetRawSubDirectory ("NEF"), "RAW_NIKON_D90.NEF",
						   false, new NikonInvariantValidator ());
		}
	}

	public class NikonInvariantValidator : IMetadataInvariantValidator
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
			// Image.0x0100 (ImageWidth/Long/1) "160"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ImageWidth);
				ClassicAssert.IsNotNull (entry, "Entry 0x0100 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (160, (entry as LongIFDEntry).Value);
			}
			// Image.0x0101 (ImageLength/Long/1) "120"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ImageLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x0101 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (120, (entry as LongIFDEntry).Value);
			}
			// Image.0x0102 (BitsPerSample/Short/3) "8 8 8"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.BitsPerSample);
				ClassicAssert.IsNotNull (entry, "Entry 0x0102 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.AreEqual (new ushort[] { 8, 8, 8 }, (entry as ShortArrayIFDEntry).Values);
			}
			// Image.0x0103 (Compression/Short/1) "1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Compression);
				ClassicAssert.IsNotNull (entry, "Entry 0x0103 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Image.0x0106 (PhotometricInterpretation/Short/1) "2"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.PhotometricInterpretation);
				ClassicAssert.IsNotNull (entry, "Entry 0x0106 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Image.0x010F (Make/Ascii/18) "NIKON CORPORATION"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Make);
				ClassicAssert.IsNotNull (entry, "Entry 0x010F missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("NIKON CORPORATION", (entry as StringIFDEntry).Value);
			}
			// Image.0x0110 (Model/Ascii/10) "NIKON D90"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Model);
				ClassicAssert.IsNotNull (entry, "Entry 0x0110 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("NIKON D90", (entry as StringIFDEntry).Value);
			}
			// Image.0x0111 (StripOffsets/StripOffsets/1) "126088"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.StripOffsets);
				ClassicAssert.IsNotNull (entry, "Entry 0x0111 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StripOffsetsIFDEntry, "Entry is not a strip offsets entry!");
				ClassicAssert.AreEqual (1, (entry as StripOffsetsIFDEntry).Values.Length);
			}
			// Image.0x0112 (Orientation/Short/1) "1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Orientation);
				ClassicAssert.IsNotNull (entry, "Entry 0x0112 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Image.0x0115 (SamplesPerPixel/Short/1) "3"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.SamplesPerPixel);
				ClassicAssert.IsNotNull (entry, "Entry 0x0115 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (3, (entry as ShortIFDEntry).Value);
			}
			// Image.0x0116 (RowsPerStrip/Long/1) "120"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.RowsPerStrip);
				ClassicAssert.IsNotNull (entry, "Entry 0x0116 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (120, (entry as LongIFDEntry).Value);
			}
			// Image.0x0117 (StripByteCounts/Long/1) "57600"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.StripByteCounts);
				ClassicAssert.IsNotNull (entry, "Entry 0x0117 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (57600, (entry as LongIFDEntry).Value);
			}
			// Image.0x011A (XResolution/Rational/1) "300/1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.XResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (300, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Image.0x011B (YResolution/Rational/1) "300/1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.YResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (300, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Image.0x011C (PlanarConfiguration/Short/1) "1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.PlanarConfiguration);
				ClassicAssert.IsNotNull (entry, "Entry 0x011C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Image.0x0128 (ResolutionUnit/Short/1) "2"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ResolutionUnit);
				ClassicAssert.IsNotNull (entry, "Entry 0x0128 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Image.0x0131 (Software/Ascii/10) "Ver.1.00 "
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Software);
				ClassicAssert.IsNotNull (entry, "Entry 0x0131 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("Ver.1.00 ", (entry as StringIFDEntry).Value);
			}
			// Image.0x0132 (DateTime/Ascii/20) "2009:02:10 19:47:07"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.DateTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x0132 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2009:02:10 19:47:07", (entry as StringIFDEntry).Value);
			}

			var SubImage1_structure = (structure.GetEntry (0, (ushort)IFDEntryTag.SubIFDs) as SubIFDArrayEntry).Entries[0];
			ClassicAssert.IsNotNull (SubImage1_structure, "SubImage1 structure not found");

			// SubImage1.0x00FE (NewSubfileType/Long/1) "1"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.NewSubfileType);
				ClassicAssert.IsNotNull (entry, "Entry 0x00FE missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (1, (entry as LongIFDEntry).Value);
			}
			// SubImage1.0x0103 (Compression/Short/1) "6"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.Compression);
				ClassicAssert.IsNotNull (entry, "Entry 0x0103 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (6, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x011A (XResolution/Rational/1) "300/1"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.XResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (300, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// SubImage1.0x011B (YResolution/Rational/1) "300/1"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.YResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (300, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// SubImage1.0x0128 (ResolutionUnit/Short/1) "2"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.ResolutionUnit);
				ClassicAssert.IsNotNull (entry, "Entry 0x0128 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// SubImage1.0x0201 (JPEGInterchangeFormat/ThumbnailDataIFD/1) "184064"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.JPEGInterchangeFormat);
				ClassicAssert.IsNotNull (entry, "Entry 0x0201 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ThumbnailDataIFDEntry, "Entry is not a thumbnail IFD!");
			}
			// SubImage1.0x0202 (JPEGInterchangeFormatLength/Long/1) "1382859"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.JPEGInterchangeFormatLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x0202 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (1382859, (entry as LongIFDEntry).Value);
			}
			// SubImage1.0x0213 (YCbCrPositioning/Short/1) "2"
			{
				var entry = SubImage1_structure.GetEntry (0, (ushort)IFDEntryTag.YCbCrPositioning);
				ClassicAssert.IsNotNull (entry, "Entry 0x0213 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}

			var SubImage2_structure = (structure.GetEntry (0, (ushort)IFDEntryTag.SubIFDs) as SubIFDArrayEntry).Entries[1];
			ClassicAssert.IsNotNull (SubImage2_structure, "SubImage2 structure not found");

			// SubImage2.0x00FE (NewSubfileType/Long/1) "0"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.NewSubfileType);
				ClassicAssert.IsNotNull (entry, "Entry 0x00FE missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (0, (entry as LongIFDEntry).Value);
			}
			// SubImage2.0x0100 (ImageWidth/Long/1) "4352"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.ImageWidth);
				ClassicAssert.IsNotNull (entry, "Entry 0x0100 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (4352, (entry as LongIFDEntry).Value);
			}
			// SubImage2.0x0101 (ImageLength/Long/1) "2868"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.ImageLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x0101 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (2868, (entry as LongIFDEntry).Value);
			}
			// SubImage2.0x0102 (BitsPerSample/Short/1) "12"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.BitsPerSample);
				ClassicAssert.IsNotNull (entry, "Entry 0x0102 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (12, (entry as ShortIFDEntry).Value);
			}
			// SubImage2.0x0103 (Compression/Short/1) "34713"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.Compression);
				ClassicAssert.IsNotNull (entry, "Entry 0x0103 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (34713, (entry as ShortIFDEntry).Value);
			}
			// SubImage2.0x0106 (PhotometricInterpretation/Short/1) "32803"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.PhotometricInterpretation);
				ClassicAssert.IsNotNull (entry, "Entry 0x0106 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (32803, (entry as ShortIFDEntry).Value);
			}
			// SubImage2.0x0111 (StripOffsets/StripOffsets/1) "1566944"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.StripOffsets);
				ClassicAssert.IsNotNull (entry, "Entry 0x0111 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StripOffsetsIFDEntry, "Entry is not a strip offsets entry!");
				ClassicAssert.AreEqual (1, (entry as StripOffsetsIFDEntry).Values.Length);
			}
			// SubImage2.0x0115 (SamplesPerPixel/Short/1) "1"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.SamplesPerPixel);
				ClassicAssert.IsNotNull (entry, "Entry 0x0115 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// SubImage2.0x0116 (RowsPerStrip/Long/1) "2868"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.RowsPerStrip);
				ClassicAssert.IsNotNull (entry, "Entry 0x0116 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (2868, (entry as LongIFDEntry).Value);
			}
			// SubImage2.0x0117 (StripByteCounts/Long/1) "9441711"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.StripByteCounts);
				ClassicAssert.IsNotNull (entry, "Entry 0x0117 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (9441711, (entry as LongIFDEntry).Value);
			}
			// SubImage2.0x011A (XResolution/Rational/1) "300/1"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.XResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (300, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// SubImage2.0x011B (YResolution/Rational/1) "300/1"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.YResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (300, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// SubImage2.0x011C (PlanarConfiguration/Short/1) "1"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.PlanarConfiguration);
				ClassicAssert.IsNotNull (entry, "Entry 0x011C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// SubImage2.0x0128 (ResolutionUnit/Short/1) "2"
			{
				var entry = SubImage2_structure.GetEntry (0, (ushort)IFDEntryTag.ResolutionUnit);
				ClassicAssert.IsNotNull (entry, "Entry 0x0128 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// SubImage2.0x828D (CFARepeatPatternDim/Short/2) "2 2"
			{
				// TODO: Unknown IFD tag: SubImage2 / 0x828D
				var entry = SubImage2_structure.GetEntry (0, 0x828D);
				ClassicAssert.IsNotNull (entry, "Entry 0x828D missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.AreEqual (new ushort[] { 2, 2 }, (entry as ShortArrayIFDEntry).Values);
			}
			// SubImage2.0x828E (CFAPattern/Byte/4) "1 2 0 1"
			{
				// TODO: Unknown IFD tag: SubImage2 / 0x828E
				var entry = SubImage2_structure.GetEntry (0, 0x828E);
				ClassicAssert.IsNotNull (entry, "Entry 0x828E missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ByteVectorIFDEntry, "Entry is not a byte array!");
				var parsed_bytes = (entry as ByteVectorIFDEntry).Data.Data;
				var bytes = new byte[] { 1, 2, 0, 1 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// SubImage2.0x9217 (SensingMethod/Short/1) "2"
			{
				// TODO: Unknown IFD tag: SubImage2 / 0x9217
				var entry = SubImage2_structure.GetEntry (0, 0x9217);
				ClassicAssert.IsNotNull (entry, "Entry 0x9217 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Image.0x0214 (ReferenceBlackWhite/Rational/6) "0/1 255/1 0/1 255/1 0/1 255/1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ReferenceBlackWhite);
				ClassicAssert.IsNotNull (entry, "Entry 0x0214 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalArrayIFDEntry, "Entry is not a rational array!");
				var parts = (entry as RationalArrayIFDEntry).Values;
				ClassicAssert.AreEqual (6, parts.Length);
				ClassicAssert.AreEqual (0, parts[0].Numerator);
				ClassicAssert.AreEqual (1, parts[0].Denominator);
				ClassicAssert.AreEqual (255, parts[1].Numerator);
				ClassicAssert.AreEqual (1, parts[1].Denominator);
				ClassicAssert.AreEqual (0, parts[2].Numerator);
				ClassicAssert.AreEqual (1, parts[2].Denominator);
				ClassicAssert.AreEqual (255, parts[3].Numerator);
				ClassicAssert.AreEqual (1, parts[3].Denominator);
				ClassicAssert.AreEqual (0, parts[4].Numerator);
				ClassicAssert.AreEqual (1, parts[4].Denominator);
				ClassicAssert.AreEqual (255, parts[5].Numerator);
				ClassicAssert.AreEqual (1, parts[5].Denominator);
			}
			// Image.0x8769 (ExifTag/SubIFD/1) "480"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD);
				ClassicAssert.IsNotNull (entry, "Entry 0x8769 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SubIFDEntry, "Entry is not a sub IFD!");
			}

			var exif = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD) as SubIFDEntry;
			ClassicAssert.IsNotNull (exif, "Exif tag not found");
			var exif_structure = exif.Structure;

			// Photo.0x829A (ExposureTime/Rational/1) "10/600"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x829A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (10, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (600, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x829D (FNumber/Rational/1) "35/10"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FNumber);
				ClassicAssert.IsNotNull (entry, "Entry 0x829D missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (35, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (10, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x8822 (ExposureProgram/Short/1) "2"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureProgram);
				ClassicAssert.IsNotNull (entry, "Entry 0x8822 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x9003 (DateTimeOriginal/Ascii/20) "2009:02:10 19:47:07"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeOriginal);
				ClassicAssert.IsNotNull (entry, "Entry 0x9003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2009:02:10 19:47:07", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9004 (DateTimeDigitized/Ascii/20) "2009:02:10 19:47:07"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeDigitized);
				ClassicAssert.IsNotNull (entry, "Entry 0x9004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2009:02:10 19:47:07", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9204 (ExposureBiasValue/SRational/1) "0/6"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureBiasValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9204 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SRationalIFDEntry, "Entry is not a srational!");
				ClassicAssert.AreEqual (0, (entry as SRationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (6, (entry as SRationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9205 (MaxApertureValue/Rational/1) "10/10"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MaxApertureValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9205 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (10, (entry as RationalIFDEntry).Value.Numerator);
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
			// Photo.0x9209 (Flash/Short/1) "15"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Flash);
				ClassicAssert.IsNotNull (entry, "Entry 0x9209 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (15, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x920A (FocalLength/Rational/1) "500/10"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x920A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (500, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (10, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x927C (MakerNote/MakerNote/125070) "(Value ommitted)"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MakerNote);
				ClassicAssert.IsNotNull (entry, "Entry 0x927C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as MakernoteIFDEntry, "Entry is not a makernote IFD!");
			}

			var makernote = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MakerNote) as MakernoteIFDEntry;
			ClassicAssert.IsNotNull (makernote, "MakerNote tag not found");
			var makernote_structure = makernote.Structure;


			ClassicAssert.AreEqual (MakernoteType.Nikon3, makernote.MakernoteType);

			// Nikon3.0x0001 (Version/Undefined/4) "48 50 49 48"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.Version);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 50, 49, 48 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x0002 (ISOSpeed/Undefined/4) "0 0 0 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.ISOSpeed);
				ClassicAssert.IsNotNull (entry, "Entry 0x0002 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 0, 0, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x0004 (Quality/Ascii/8) "RAW    "
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.Quality);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("RAW    ", (entry as StringIFDEntry).Value);
			}
			// Nikon3.0x0005 (WhiteBalance/Ascii/13) "AUTO        "
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.WhiteBalance);
				ClassicAssert.IsNotNull (entry, "Entry 0x0005 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("AUTO        ", (entry as StringIFDEntry).Value);
			}
			// Nikon3.0x0007 (Focus/Ascii/7) "AF-S  "
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.Focus);
				ClassicAssert.IsNotNull (entry, "Entry 0x0007 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("AF-S  ", (entry as StringIFDEntry).Value);
			}
			// Nikon3.0x0008 (FlashSetting/Ascii/13) "NORMAL      "
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.FlashSetting);
				ClassicAssert.IsNotNull (entry, "Entry 0x0008 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("NORMAL      ", (entry as StringIFDEntry).Value);
			}
			// Nikon3.0x0009 (FlashDevice/Ascii/20) "Optional,TTL       "
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.FlashDevice);
				ClassicAssert.IsNotNull (entry, "Entry 0x0009 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("Optional,TTL       ", (entry as StringIFDEntry).Value);
			}
			// Nikon3.0x000B (WhiteBalanceBias/SShort/2) "0 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.WhiteBalanceBias);
				ClassicAssert.IsNotNull (entry, "Entry 0x000B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SShortArrayIFDEntry, "Entry is not a signed short array!");
				ClassicAssert.AreEqual (new short[] { 0, 0 }, (entry as SShortArrayIFDEntry).Values);
			}
			// Nikon3.0x000C (WB_RBLevels/Rational/4) "475/256 319/256 256/256 256/256"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.WB_RBLevels);
				ClassicAssert.IsNotNull (entry, "Entry 0x000C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalArrayIFDEntry, "Entry is not a rational array!");
				var parts = (entry as RationalArrayIFDEntry).Values;
				ClassicAssert.AreEqual (4, parts.Length);
				ClassicAssert.AreEqual (475, parts[0].Numerator);
				ClassicAssert.AreEqual (256, parts[0].Denominator);
				ClassicAssert.AreEqual (319, parts[1].Numerator);
				ClassicAssert.AreEqual (256, parts[1].Denominator);
				ClassicAssert.AreEqual (256, parts[2].Numerator);
				ClassicAssert.AreEqual (256, parts[2].Denominator);
				ClassicAssert.AreEqual (256, parts[3].Numerator);
				ClassicAssert.AreEqual (256, parts[3].Denominator);
			}
			// Nikon3.0x000D (ProgramShift/Undefined/4) "226 1 6 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.ProgramShift);
				ClassicAssert.IsNotNull (entry, "Entry 0x000D missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 226, 1, 6, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x000E (ExposureDiff/Undefined/4) "160 1 12 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.ExposureDiff);
				ClassicAssert.IsNotNull (entry, "Entry 0x000E missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 160, 1, 12, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x0011 (Preview/SubIFD/1) "13954"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.Preview);
				ClassicAssert.IsNotNull (entry, "Entry 0x0011 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SubIFDEntry, "Entry is not a sub IFD!");
			}

			var nikonpreview = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.Preview) as SubIFDEntry;
			ClassicAssert.IsNotNull (nikonpreview, "Nikon preview tag not found");
			var nikonpreview_structure = nikonpreview.Structure;

			// NikonPreview.0x0103 (Compression/Short/1) "6"
			{
				var entry = nikonpreview_structure.GetEntry (0, (ushort)NikonPreviewMakerNoteEntryTag.Compression);
				ClassicAssert.IsNotNull (entry, "Entry 0x0103 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (6, (entry as ShortIFDEntry).Value);
			}
			// NikonPreview.0x011A (XResolution/Rational/1) "300/1"
			{
				var entry = nikonpreview_structure.GetEntry (0, (ushort)NikonPreviewMakerNoteEntryTag.XResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (300, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// NikonPreview.0x011B (YResolution/Rational/1) "300/1"
			{
				var entry = nikonpreview_structure.GetEntry (0, (ushort)NikonPreviewMakerNoteEntryTag.YResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (300, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// NikonPreview.0x0128 (ResolutionUnit/Short/1) "2"
			{
				var entry = nikonpreview_structure.GetEntry (0, (ushort)NikonPreviewMakerNoteEntryTag.ResolutionUnit);
				ClassicAssert.IsNotNull (entry, "Entry 0x0128 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// NikonPreview.0x0201 (JPEGInterchangeFormat/ThumbnailDataIFD/1) "14062"
			{
				var entry = nikonpreview_structure.GetEntry (0, (ushort)NikonPreviewMakerNoteEntryTag.PreviewImageStart);
				ClassicAssert.IsNotNull (entry, "Entry 0x0201 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ThumbnailDataIFDEntry, "Entry is not a thumbnail IFD!");
			}
			// NikonPreview.0x0202 (JPEGInterchangeFormatLength/Long/1) "110997"
			{
				var entry = nikonpreview_structure.GetEntry (0, (ushort)NikonPreviewMakerNoteEntryTag.PreviewImageLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x0202 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (110997, (entry as LongIFDEntry).Value);
			}
			// NikonPreview.0x0213 (YCbCrPositioning/Short/1) "2"
			{
				var entry = nikonpreview_structure.GetEntry (0, (ushort)NikonPreviewMakerNoteEntryTag.YCbCrPositioning);
				ClassicAssert.IsNotNull (entry, "Entry 0x0213 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Nikon3.0x0012 (FlashComp/Undefined/4) "0 1 6 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.FlashComp);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 0, 1, 6, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x0013 (ISOSettings/Undefined/4) "0 0 0 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.ISOSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0013 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 0, 0, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x0017 (0x0017/Undefined/4) "0 1 6 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.Unknown23);
				ClassicAssert.IsNotNull (entry, "Entry 0x0017 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 0, 1, 6, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x0018 (FlashBracketComp/Undefined/4) "0 1 6 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.FlashBracketComp);
				ClassicAssert.IsNotNull (entry, "Entry 0x0018 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 0, 1, 6, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x0019 (ExposureBracketComp/SRational/1) "0/6"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.ExposureBracketComp);
				ClassicAssert.IsNotNull (entry, "Entry 0x0019 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SRationalIFDEntry, "Entry is not a srational!");
				ClassicAssert.AreEqual (0, (entry as SRationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (6, (entry as SRationalIFDEntry).Value.Denominator);
			}
			// Nikon3.0x001B (CropHiSpeed/Short/7) "0 4352 2868 4352 2868 0 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.CropHiSpeed);
				ClassicAssert.IsNotNull (entry, "Entry 0x001B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.AreEqual (new ushort[] { 0, 4352, 2868, 4352, 2868, 0, 0 }, (entry as ShortArrayIFDEntry).Values);
			}
			// Nikon3.0x001C (0x001c/Undefined/3) "0 1 6"
			{
				// TODO: Unknown IFD tag: Nikon3 / 0x001C
				var entry = makernote_structure.GetEntry (0, 0x001C);
				ClassicAssert.IsNotNull (entry, "Entry 0x001C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 0, 1, 6 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x001D (SerialNumber/Ascii/8) "3002025"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.SerialNumber);
				ClassicAssert.IsNotNull (entry, "Entry 0x001D missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("3002025", (entry as StringIFDEntry).Value);
			}
			// Nikon3.0x001E (ColorSpace/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.ColorSpace);
				ClassicAssert.IsNotNull (entry, "Entry 0x001E missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Nikon3.0x0022 (ActiveDLighting/Short/1) "3"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.ActiveDLighting);
				ClassicAssert.IsNotNull (entry, "Entry 0x0022 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (3, (entry as ShortIFDEntry).Value);
			}
			// Nikon3.0x002C (0x002c/Undefined/94) "48 49 48 48 5 0 1 100 0 236 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0"
			{
				// TODO: Unknown IFD tag: Nikon3 / 0x002C
				var entry = makernote_structure.GetEntry (0, 0x002C);
				ClassicAssert.IsNotNull (entry, "Entry 0x002C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 49, 48, 48, 5, 0, 1, 100, 0, 236, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x0083 (LensType/Byte/1) "2"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.LensType);
				ClassicAssert.IsNotNull (entry, "Entry 0x0083 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ByteIFDEntry, "Entry is not a byte!");
				ClassicAssert.AreEqual (2, (entry as ByteIFDEntry).Value);
			}
			// Nikon3.0x0084 (Lens/Rational/4) "500/10 500/10 14/10 14/10"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.Lens);
				ClassicAssert.IsNotNull (entry, "Entry 0x0084 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalArrayIFDEntry, "Entry is not a rational array!");
				var parts = (entry as RationalArrayIFDEntry).Values;
				ClassicAssert.AreEqual (4, parts.Length);
				ClassicAssert.AreEqual (500, parts[0].Numerator);
				ClassicAssert.AreEqual (10, parts[0].Denominator);
				ClassicAssert.AreEqual (500, parts[1].Numerator);
				ClassicAssert.AreEqual (10, parts[1].Denominator);
				ClassicAssert.AreEqual (14, parts[2].Numerator);
				ClassicAssert.AreEqual (10, parts[2].Denominator);
				ClassicAssert.AreEqual (14, parts[3].Numerator);
				ClassicAssert.AreEqual (10, parts[3].Denominator);
			}
			// Nikon3.0x0087 (FlashMode/Byte/1) "7"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.FlashMode);
				ClassicAssert.IsNotNull (entry, "Entry 0x0087 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ByteIFDEntry, "Entry is not a byte!");
				ClassicAssert.AreEqual (7, (entry as ByteIFDEntry).Value);
			}
			// Nikon3.0x0089 (ShootingMode/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.ShootingMode);
				ClassicAssert.IsNotNull (entry, "Entry 0x0089 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Nikon3.0x008A (AutoBracketRelease/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.AutoBracketRelease);
				ClassicAssert.IsNotNull (entry, "Entry 0x008A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Nikon3.0x008B (LensFStops/Undefined/4) "84 1 12 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.LensFStops);
				ClassicAssert.IsNotNull (entry, "Entry 0x008B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 84, 1, 12, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x008C (ContrastCurve/Undefined/578) "(Value ommitted)"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.ContrastCurve);
				ClassicAssert.IsNotNull (entry, "Entry 0x008C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var parsed_hash = Utils.Md5Encode (parsed_bytes);
				ClassicAssert.AreEqual ("759eb15ba2e1894d0755d0db67212ec9", parsed_hash);
				ClassicAssert.AreEqual (578, parsed_bytes.Length);
			}
			// Nikon3.0x0093 (NEFCompression/Short/1) "4"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.NEFCompression);
				ClassicAssert.IsNotNull (entry, "Entry 0x0093 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (4, (entry as ShortIFDEntry).Value);
			}
			// Nikon3.0x0095 (NoiseReduction/Ascii/5) "OFF "
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.NoiseReduction);
				ClassicAssert.IsNotNull (entry, "Entry 0x0095 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("OFF ", (entry as StringIFDEntry).Value);
			}
			// Nikon3.0x0096 (LinearizationTable/Undefined/624) "(Value ommitted)"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.LinearizationTable);
				ClassicAssert.IsNotNull (entry, "Entry 0x0096 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var parsed_hash = Utils.Md5Encode (parsed_bytes);
				ClassicAssert.AreEqual ("489ae56582af0b796cce2b7ce798a593", parsed_hash);
				ClassicAssert.AreEqual (624, parsed_bytes.Length);
			}
			// Nikon3.0x0097 (ColorBalance/Undefined/1302) "(Value ommitted)"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.ColorBalance);
				ClassicAssert.IsNotNull (entry, "Entry 0x0097 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var parsed_hash = Utils.Md5Encode (parsed_bytes);
				ClassicAssert.AreEqual ("39a042200a869c791a2c31d925f5d95a", parsed_hash);
				ClassicAssert.AreEqual (1302, parsed_bytes.Length);
			}
			// Nikon3.0x0099 (RawImageCenter/Short/2) "2176 1434"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.RawImageCenter);
				ClassicAssert.IsNotNull (entry, "Entry 0x0099 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.AreEqual (new ushort[] { 2176, 1434 }, (entry as ShortArrayIFDEntry).Values);
			}
			// Nikon3.0x009E (RetouchHistory/Short/10) "0 0 0 0 0 0 0 0 0 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.RetouchHistory);
				ClassicAssert.IsNotNull (entry, "Entry 0x009E missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.AreEqual (new ushort[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, (entry as ShortArrayIFDEntry).Values);
			}
			// Nikon3.0x00A3 (0x00a3/Byte/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.Unknown163);
				ClassicAssert.IsNotNull (entry, "Entry 0x00A3 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ByteIFDEntry, "Entry is not a byte!");
				ClassicAssert.AreEqual (0, (entry as ByteIFDEntry).Value);
			}
			// Nikon3.0x00A4 (0x00a4/Undefined/4) "48 50 48 48"
			{
				// TODO: Unknown IFD tag: Nikon3 / 0x00A4
				var entry = makernote_structure.GetEntry (0, 0x00A4);
				ClassicAssert.IsNotNull (entry, "Entry 0x00A4 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 50, 48, 48 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x00A7 (ShutterCount/Long/1) "2659"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.ShutterCount);
				ClassicAssert.IsNotNull (entry, "Entry 0x00A7 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (2659, (entry as LongIFDEntry).Value);
			}
			// Nikon3.0x00A8 (FlashInfo/Undefined/22) "48 49 48 51 1 46 4 4 133 1 0 42 27 0 0 0 0 0 0 0 0 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.FlashInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x00A8 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 49, 48, 51, 1, 46, 4, 4, 133, 1, 0, 42, 27, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x00AB (VariProgram/Ascii/16) "               "
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.VariProgram);
				ClassicAssert.IsNotNull (entry, "Entry 0x00AB missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("               ", (entry as StringIFDEntry).Value);
			}
			// Nikon3.0x00B0 (MultiExposure/Undefined/16) "48 49 48 48 0 0 0 0 0 0 0 0 0 0 0 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.MultiExposure);
				ClassicAssert.IsNotNull (entry, "Entry 0x00B0 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 49, 48, 48, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x00B1 (HighISONoiseReduction/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.HighISONoiseReduction);
				ClassicAssert.IsNotNull (entry, "Entry 0x00B1 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Nikon3.0x00B6 (0x00b6/Undefined/8) "7 217 2 10 19 45 53 0"
			{
				// TODO: Unknown IFD tag: Nikon3 / 0x00B6
				var entry = makernote_structure.GetEntry (0, 0x00B6);
				ClassicAssert.IsNotNull (entry, "Entry 0x00B6 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 7, 217, 2, 10, 19, 45, 53, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x00B7 (AFInfo2/Undefined/30) "48 49 48 48 0 8 2 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.AFInfo2);
				ClassicAssert.IsNotNull (entry, "Entry 0x00B7 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 49, 48, 48, 0, 8, 2, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x00B8 (FileInfo/Undefined/172) "48 49 48 48 0 0 0 100 0 3 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)Nikon3MakerNoteEntryTag.FileInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x00B8 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 49, 48, 48, 0, 0, 0, 100, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x00BB (0x00bb/Undefined/6) "48 49 48 48 255 0"
			{
				// TODO: Unknown IFD tag: Nikon3 / 0x00BB
				var entry = makernote_structure.GetEntry (0, 0x00BB);
				ClassicAssert.IsNotNull (entry, "Entry 0x00BB missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 49, 48, 48, 255, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Nikon3.0x00BC (0x00bc/Undefined/3500) "(Value ommitted)"
			{
				// TODO: Unknown IFD tag: Nikon3 / 0x00BC
				var entry = makernote_structure.GetEntry (0, 0x00BC);
				ClassicAssert.IsNotNull (entry, "Entry 0x00BC missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var parsed_hash = Utils.Md5Encode (parsed_bytes);
				ClassicAssert.AreEqual ("47098865aca2b97ee0380d198fb88e6b", parsed_hash);
				ClassicAssert.AreEqual (3500, parsed_bytes.Length);
			}
			// Photo.0x9286 (UserComment/UserComment/44) "charset="Ascii"                                     "
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.UserComment);
				ClassicAssert.IsNotNull (entry, "Entry 0x9286 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UserCommentIFDEntry, "Entry is not a user comment!");
				ClassicAssert.AreEqual ("", (entry as UserCommentIFDEntry).Value.Trim ());
			}
			// Photo.0x9290 (SubSecTime/Ascii/3) "00"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SubsecTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x9290 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("00", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9291 (SubSecTimeOriginal/Ascii/3) "00"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SubsecTimeOriginal);
				ClassicAssert.IsNotNull (entry, "Entry 0x9291 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("00", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9292 (SubSecTimeDigitized/Ascii/3) "00"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SubsecTimeDigitized);
				ClassicAssert.IsNotNull (entry, "Entry 0x9292 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("00", (entry as StringIFDEntry).Value);
			}
			// Photo.0xA217 (SensingMethod/Short/1) "2"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SensingMethod);
				ClassicAssert.IsNotNull (entry, "Entry 0xA217 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
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
			// Photo.0xA302 (CFAPattern/Undefined/8) "0 2 0 2 1 2 0 1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.CFAPattern2);
				ClassicAssert.IsNotNull (entry, "Entry 0xA302 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 0, 2, 0, 2, 1, 2, 0, 1 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Photo.0xA401 (CustomRendered/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.CustomRendered);
				ClassicAssert.IsNotNull (entry, "Entry 0xA401 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
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
			// Photo.0xA404 (DigitalZoomRatio/Rational/1) "1/1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DigitalZoomRatio);
				ClassicAssert.IsNotNull (entry, "Entry 0xA404 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
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
			// Photo.0xA407 (GainControl/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.GainControl);
				ClassicAssert.IsNotNull (entry, "Entry 0xA407 missing in IFD 0");
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
			// Photo.0xA40A (Sharpness/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Sharpness);
				ClassicAssert.IsNotNull (entry, "Entry 0xA40A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA40C (SubjectDistanceRange/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SubjectDistanceRange);
				ClassicAssert.IsNotNull (entry, "Entry 0xA40C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Image.0x8825 (GPSTag/SubIFD/1) "126070"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.GPSIFD);
				ClassicAssert.IsNotNull (entry, "Entry 0x8825 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SubIFDEntry, "Entry is not a sub IFD!");
			}

			var gps = structure.GetEntry (0, (ushort)IFDEntryTag.GPSIFD) as SubIFDEntry;
			ClassicAssert.IsNotNull (gps, "GPS tag not found");
			var gps_structure = gps.Structure;

			// GPSInfo.0x0000 (GPSVersionID/Byte/4) "2 2 0 0"
			{
				var entry = gps_structure.GetEntry (0, (ushort)GPSEntryTag.GPSVersionID);
				ClassicAssert.IsNotNull (entry, "Entry 0x0000 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ByteVectorIFDEntry, "Entry is not a byte array!");
				var parsed_bytes = (entry as ByteVectorIFDEntry).Data.Data;
				var bytes = new byte[] { 2, 2, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Image.0x9003 (DateTimeOriginal/Ascii/20) "2009:02:10 19:47:07"
			{
				var entry = structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeOriginal);
				ClassicAssert.IsNotNull (entry, "Entry 0x9003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2009:02:10 19:47:07", (entry as StringIFDEntry).Value);
			}
			// Image.0x9216 (TIFFEPStandardID/Byte/4) "1 0 0 0"
			{
				// TODO: Unknown IFD tag: Image / 0x9216
				var entry = structure.GetEntry (0, 0x9216);
				ClassicAssert.IsNotNull (entry, "Entry 0x9216 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ByteVectorIFDEntry, "Entry is not a byte array!");
				var parsed_bytes = (entry as ByteVectorIFDEntry).Data.Data;
				var bytes = new byte[] { 1, 0, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}

			//  ---------- End of IFD tests ----------

		}
	}
}
