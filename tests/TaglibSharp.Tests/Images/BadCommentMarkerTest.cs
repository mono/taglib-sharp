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
	public class BadCommentMarkerTest
	{
		[Test]
		public void Test ()
		{
			ImageTest.Run ("sample_unicode5.jpg",
				false,
				new BadCommentMarkerTestInvariantValidator ()
			);
		}
	}

	public class BadCommentMarkerTestInvariantValidator : IMetadataInvariantValidator
	{
		public void ValidateMetadataInvariants (TagLib.Image.File file)
		{
			ClassicAssert.IsNotNull (file);

			ClassicAssert.IsTrue (file.PossiblyCorrupt);

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
			// Image.0x0110 (Model/Ascii/22) "Canon PowerShot S2 IS"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Model);
				ClassicAssert.IsNotNull (entry, "Entry 0x0110 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("Canon PowerShot S2 IS", (entry as StringIFDEntry).Value);
			}
			// Image.0x0112 (Orientation/Short/1) "1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Orientation);
				ClassicAssert.IsNotNull (entry, "Entry 0x0112 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Image.0x011A (XResolution/Rational/1) "180/1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.XResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (180, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Image.0x011B (YResolution/Rational/1) "180/1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.YResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011B missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (180, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Image.0x0128 (ResolutionUnit/Short/1) "2"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ResolutionUnit);
				ClassicAssert.IsNotNull (entry, "Entry 0x0128 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Image.0x0131 (Software/Ascii/21) "f-spot version 0.4.1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.Software);
				ClassicAssert.IsNotNull (entry, "Entry 0x0131 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("f-spot version 0.4.1", (entry as StringIFDEntry).Value);
			}
			// Image.0x0132 (DateTime/Ascii/20) "2007:11:02 17:13:24"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.DateTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x0132 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2007:11:02 17:13:24", (entry as StringIFDEntry).Value);
			}
			// Image.0x0213 (YCbCrPositioning/Short/1) "1"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.YCbCrPositioning);
				ClassicAssert.IsNotNull (entry, "Entry 0x0213 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Image.0x8769 (ExifTag/SubIFD/1) "220"
			{
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD);
				ClassicAssert.IsNotNull (entry, "Entry 0x8769 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SubIFDEntry, "Entry is not a sub IFD!");
			}

			var exif = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD) as SubIFDEntry;
			ClassicAssert.IsNotNull (exif, "Exif tag not found");
			var exif_structure = exif.Structure;

			// Photo.0x829A (ExposureTime/Rational/1) "3/10"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureTime);
				ClassicAssert.IsNotNull (entry, "Entry 0x829A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (3, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (10, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x829D (FNumber/Rational/1) "27/10"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FNumber);
				ClassicAssert.IsNotNull (entry, "Entry 0x829D missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (27, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (10, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9000 (ExifVersion/Undefined/4) "48 50 50 48 "
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExifVersion);
				ClassicAssert.IsNotNull (entry, "Entry 0x9000 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 50, 50, 48 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Photo.0x9003 (DateTimeOriginal/Ascii/20) "2005:12:22 22:39:11"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeOriginal);
				ClassicAssert.IsNotNull (entry, "Entry 0x9003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2005:12:22 22:39:11", (entry as StringIFDEntry).Value);
			}
			// Photo.0x9004 (DateTimeDigitized/Ascii/20) "2005:12:22 23:39:11"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DateTimeDigitized);
				ClassicAssert.IsNotNull (entry, "Entry 0x9004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("2005:12:22 23:39:11", (entry as StringIFDEntry).Value);
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
			// Photo.0x9102 (CompressedBitsPerPixel/Rational/1) "5/1"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.CompressedBitsPerPixel);
				ClassicAssert.IsNotNull (entry, "Entry 0x9102 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (5, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9201 (ShutterSpeedValue/SRational/1) "56/32"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ShutterSpeedValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9201 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SRationalIFDEntry, "Entry is not a srational!");
				ClassicAssert.AreEqual (56, (entry as SRationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (32, (entry as SRationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9202 (ApertureValue/Rational/1) "92/32"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ApertureValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9202 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (92, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (32, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9204 (ExposureBiasValue/SRational/1) "0/3"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExposureBiasValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9204 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as SRationalIFDEntry, "Entry is not a srational!");
				ClassicAssert.AreEqual (0, (entry as SRationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (3, (entry as SRationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9205 (MaxApertureValue/Rational/1) "92/32"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MaxApertureValue);
				ClassicAssert.IsNotNull (entry, "Entry 0x9205 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (92, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (32, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x9207 (MeteringMode/Short/1) "5"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MeteringMode);
				ClassicAssert.IsNotNull (entry, "Entry 0x9207 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (5, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x9209 (Flash/Short/1) "16"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.Flash);
				ClassicAssert.IsNotNull (entry, "Entry 0x9209 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (16, (entry as ShortIFDEntry).Value);
			}
			// Photo.0x920A (FocalLength/Rational/1) "6000/1000"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x920A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (6000, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1000, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0x927C (MakerNote/MakerNote/1192) "(Value ommitted)"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MakerNote);
				ClassicAssert.IsNotNull (entry, "Entry 0x927C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as MakernoteIFDEntry, "Entry is not a makernote IFD!");
			}

			var makernote = exif_structure.GetEntry (0, (ushort)ExifEntryTag.MakerNote) as MakernoteIFDEntry;
			ClassicAssert.IsNotNull (makernote, "MakerNote tag not found");
			var makernote_structure = makernote.Structure;


			ClassicAssert.AreEqual (MakernoteType.Canon, makernote.MakernoteType);

			// CanonCs.0x0000 (0x0000/Short/1) "92"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (1 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (92, (entry as ShortArrayIFDEntry).Values[0]);
			}
			// CanonCs.0x0001 (Macro/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (2 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (1, (entry as ShortArrayIFDEntry).Values[1]);
			}
			// CanonCs.0x0002 (Selftimer/Short/1) "20"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (3 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (20, (entry as ShortArrayIFDEntry).Values[2]);
			}
			// CanonCs.0x0003 (Quality/Short/1) "5"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (4 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (5, (entry as ShortArrayIFDEntry).Values[3]);
			}
			// CanonCs.0x0004 (FlashMode/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (5 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[4]);
			}
			// CanonCs.0x0005 (DriveMode/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (6 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[5]);
			}
			// CanonCs.0x0006 (0x0006/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (7 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[6]);
			}
			// CanonCs.0x0007 (FocusMode/Short/1) "4"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (8 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (4, (entry as ShortArrayIFDEntry).Values[7]);
			}
			// CanonCs.0x0008 (0x0008/Short/1) "65535"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (9 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (65535, (entry as ShortArrayIFDEntry).Values[8]);
			}
			// CanonCs.0x0009 (0x0009/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (10 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (1, (entry as ShortArrayIFDEntry).Values[9]);
			}
			// CanonCs.0x000A (ImageSize/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (11 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[10]);
			}
			// CanonCs.0x000B (EasyMode/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (12 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (1, (entry as ShortArrayIFDEntry).Values[11]);
			}
			// CanonCs.0x000C (DigitalZoom/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (13 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[12]);
			}
			// CanonCs.0x000D (Contrast/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (14 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[13]);
			}
			// CanonCs.0x000E (Saturation/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (15 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[14]);
			}
			// CanonCs.0x000F (Sharpness/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (16 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[15]);
			}
			// CanonCs.0x0010 (ISOSpeed/Short/1) "17"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (17 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (17, (entry as ShortArrayIFDEntry).Values[16]);
			}
			// CanonCs.0x0011 (MeteringMode/Short/1) "3"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (18 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (3, (entry as ShortArrayIFDEntry).Values[17]);
			}
			// CanonCs.0x0012 (FocusType/Short/1) "11"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (19 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (11, (entry as ShortArrayIFDEntry).Values[18]);
			}
			// CanonCs.0x0013 (AFPoint/Short/1) "8197"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (20 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (8197, (entry as ShortArrayIFDEntry).Values[19]);
			}
			// CanonCs.0x0014 (ExposureProgram/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (21 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (1, (entry as ShortArrayIFDEntry).Values[20]);
			}
			// CanonCs.0x0015 (0x0015/Short/1) "32767"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (22 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (32767, (entry as ShortArrayIFDEntry).Values[21]);
			}
			// CanonCs.0x0016 (LensType/Short/1) "65535"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (23 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (65535, (entry as ShortArrayIFDEntry).Values[22]);
			}
			// CanonCs.0x0017 (Lens/Short/3) "7200 600 100"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (26 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (7200, (entry as ShortArrayIFDEntry).Values[23]);
				ClassicAssert.AreEqual (600, (entry as ShortArrayIFDEntry).Values[24]);
				ClassicAssert.AreEqual (100, (entry as ShortArrayIFDEntry).Values[25]);
			}
			// CanonCs.0x001A (MaxAperture/Short/1) "92"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (27 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (92, (entry as ShortArrayIFDEntry).Values[26]);
			}
			// CanonCs.0x001B (MinAperture/Short/1) "192"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (28 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (192, (entry as ShortArrayIFDEntry).Values[27]);
			}
			// CanonCs.0x001C (FlashActivity/Short/1) "65535"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (29 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (65535, (entry as ShortArrayIFDEntry).Values[28]);
			}
			// CanonCs.0x001D (FlashDetails/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (30 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[29]);
			}
			// CanonCs.0x001E (0x001e/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (31 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[30]);
			}
			// CanonCs.0x001F (0x001f/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (32 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[31]);
			}
			// CanonCs.0x0020 (FocusContinuous/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (33 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (1, (entry as ShortArrayIFDEntry).Values[32]);
			}
			// CanonCs.0x0021 (AESetting/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (34 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[33]);
			}
			// CanonCs.0x0022 (ImageStabilization/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (35 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (1, (entry as ShortArrayIFDEntry).Values[34]);
			}
			// CanonCs.0x0023 (DisplayAperture/Short/1) "27"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (36 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (27, (entry as ShortArrayIFDEntry).Values[35]);
			}
			// CanonCs.0x0024 (ZoomSourceWidth/Short/1) "2592"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (37 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (2592, (entry as ShortArrayIFDEntry).Values[36]);
			}
			// CanonCs.0x0025 (ZoomTargetWidth/Short/1) "2592"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (38 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (2592, (entry as ShortArrayIFDEntry).Values[37]);
			}
			// CanonCs.0x0026 (0x0026/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (39 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[38]);
			}
			// CanonCs.0x0027 (0x0027/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (40 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[39]);
			}
			// CanonCs.0x0028 (PhotoEffect/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (41 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[40]);
			}
			// CanonCs.0x0029 (0x0029/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (42 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[41]);
			}
			// CanonCs.0x002A (ColorTone/Short/1) "32767"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (43 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (32767, (entry as ShortArrayIFDEntry).Values[42]);
			}
			// CanonCs.0x002B (0x002b/Short/1) "32767"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (44 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (32767, (entry as ShortArrayIFDEntry).Values[43]);
			}
			// CanonCs.0x002C (0x002c/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (45 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[44]);
			}
			// CanonCs.0x002D (0x002d/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.CameraSettings);
				ClassicAssert.IsNotNull (entry, "Entry 0x0001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (46 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[45]);
			}
			// Canon.0x0002 (FocalLength/Short/4) "2 600 230 172"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.FocalLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x0002 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.AreEqual (new ushort[] { 2, 600, 230, 172 }, (entry as ShortArrayIFDEntry).Values);
			}
			// Canon.0x0003 (0x0003/Short/4) "0 0 0 0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.Unknown3);
				ClassicAssert.IsNotNull (entry, "Entry 0x0003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.AreEqual (new ushort[] { 0, 0, 0, 0 }, (entry as ShortArrayIFDEntry).Values);
			}
			// CanonSi.0x0000 (0x0000/Short/1) "68"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (1 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (68, (entry as ShortArrayIFDEntry).Values[0]);
			}
			// CanonSi.0x0001 (0x0001/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (2 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[1]);
			}
			// CanonSi.0x0002 (ISOSpeed/Short/1) "160"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (3 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (160, (entry as ShortArrayIFDEntry).Values[2]);
			}
			// CanonSi.0x0003 (0x0003/Short/1) "65495"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (4 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (65495, (entry as ShortArrayIFDEntry).Values[3]);
			}
			// CanonSi.0x0004 (TargetAperture/Short/1) "92"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (5 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (92, (entry as ShortArrayIFDEntry).Values[4]);
			}
			// CanonSi.0x0005 (TargetShutterSpeed/Short/1) "56"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (6 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (56, (entry as ShortArrayIFDEntry).Values[5]);
			}
			// CanonSi.0x0006 (0x0006/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (7 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[6]);
			}
			// CanonSi.0x0007 (WhiteBalance/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (8 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[7]);
			}
			// CanonSi.0x0008 (0x0008/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (9 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[8]);
			}
			// CanonSi.0x0009 (Sequence/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (10 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[9]);
			}
			// CanonSi.0x000A (0x000a/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (11 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[10]);
			}
			// CanonSi.0x000B (0x000b/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (12 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[11]);
			}
			// CanonSi.0x000C (0x000c/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (13 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[12]);
			}
			// CanonSi.0x000D (0x000d/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (14 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[13]);
			}
			// CanonSi.0x000E (AFPointUsed/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (15 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[14]);
			}
			// CanonSi.0x000F (FlashBias/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (16 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[15]);
			}
			// CanonSi.0x0010 (0x0010/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (17 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[16]);
			}
			// CanonSi.0x0011 (0x0011/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (18 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[17]);
			}
			// CanonSi.0x0012 (0x0012/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (19 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (1, (entry as ShortArrayIFDEntry).Values[18]);
			}
			// CanonSi.0x0013 (SubjectDistance/Short/1) "8"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (20 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (8, (entry as ShortArrayIFDEntry).Values[19]);
			}
			// CanonSi.0x0014 (0x0014/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (21 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[20]);
			}
			// CanonSi.0x0015 (ApertureValue/Short/1) "94"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (22 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (94, (entry as ShortArrayIFDEntry).Values[21]);
			}
			// CanonSi.0x0016 (ShutterSpeedValue/Short/1) "50"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (23 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (50, (entry as ShortArrayIFDEntry).Values[22]);
			}
			// CanonSi.0x0017 (0x0017/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (24 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[23]);
			}
			// CanonSi.0x0018 (0x0018/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (25 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[24]);
			}
			// CanonSi.0x0019 (0x0019/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (26 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[25]);
			}
			// CanonSi.0x001A (0x001a/Short/1) "250"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (27 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (250, (entry as ShortArrayIFDEntry).Values[26]);
			}
			// CanonSi.0x001B (0x001b/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (28 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[27]);
			}
			// CanonSi.0x001C (0x001c/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (29 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[28]);
			}
			// CanonSi.0x001D (0x001d/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (30 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[29]);
			}
			// CanonSi.0x001E (0x001e/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (31 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[30]);
			}
			// CanonSi.0x001F (0x001f/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (32 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[31]);
			}
			// CanonSi.0x0020 (0x0020/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (33 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[32]);
			}
			// CanonSi.0x0021 (0x0021/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ShotInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0004 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (34 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[33]);
			}
			// CanonPi.0x0000 (0x0000/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.PictureInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (1 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (1, (entry as ShortArrayIFDEntry).Values[0]);
			}
			// CanonPi.0x0001 (0x0001/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.PictureInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (2 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (1, (entry as ShortArrayIFDEntry).Values[1]);
			}
			// CanonPi.0x0002 (ImageWidth/Short/1) "2592"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.PictureInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (3 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (2592, (entry as ShortArrayIFDEntry).Values[2]);
			}
			// CanonPi.0x0003 (ImageHeight/Short/1) "1944"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.PictureInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (4 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (1944, (entry as ShortArrayIFDEntry).Values[3]);
			}
			// CanonPi.0x0004 (ImageWidthAsShot/Short/1) "1296"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.PictureInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (5 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (1296, (entry as ShortArrayIFDEntry).Values[4]);
			}
			// CanonPi.0x0005 (ImageHeightAsShot/Short/1) "242"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.PictureInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (6 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (242, (entry as ShortArrayIFDEntry).Values[5]);
			}
			// CanonPi.0x0006 (0x0006/Short/1) "233"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.PictureInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (7 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (233, (entry as ShortArrayIFDEntry).Values[6]);
			}
			// CanonPi.0x0007 (0x0007/Short/1) "44"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.PictureInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (8 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (44, (entry as ShortArrayIFDEntry).Values[7]);
			}
			// CanonPi.0x0008 (0x0008/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.PictureInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (9 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[8]);
			}
			// CanonPi.0x0009 (0x0009/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.PictureInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (10 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[9]);
			}
			// CanonPi.0x000A (0x000a/Short/1) "1"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.PictureInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (11 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (1, (entry as ShortArrayIFDEntry).Values[10]);
			}
			// CanonPi.0x000B (0x000b/Short/1) "0"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.PictureInfo);
				ClassicAssert.IsNotNull (entry, "Entry 0x0012 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.IsTrue (12 <= (entry as ShortArrayIFDEntry).Values.Length);
				ClassicAssert.AreEqual (0, (entry as ShortArrayIFDEntry).Values[11]);
			}
			// Canon.0x0013 (0x0013/Short/4) "0 0 0 0"
			{
				// TODO: Unknown IFD tag: Canon / 0x0013
				var entry = makernote_structure.GetEntry (0, 0x0013);
				ClassicAssert.IsNotNull (entry, "Entry 0x0013 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.AreEqual (new ushort[] { 0, 0, 0, 0 }, (entry as ShortArrayIFDEntry).Values);
			}
			// Canon.0x0006 (ImageType/Ascii/25) "IMG:PowerShot S2 IS JPEG"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ImageType);
				ClassicAssert.IsNotNull (entry, "Entry 0x0006 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("IMG:PowerShot S2 IS JPEG", (entry as StringIFDEntry).Value);
			}
			// Canon.0x0007 (FirmwareVersion/Ascii/22) "Firmware Version 1.00"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.FirmwareVersion);
				ClassicAssert.IsNotNull (entry, "Entry 0x0007 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("Firmware Version 1.00", (entry as StringIFDEntry).Value);
			}
			// Canon.0x0008 (ImageNumber/Long/1) "1111185"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ImageNumber);
				ClassicAssert.IsNotNull (entry, "Entry 0x0008 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (1111185, (entry as LongIFDEntry).Value);
			}
			// Canon.0x0009 (OwnerName/Ascii/32) ""
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.OwnerName);
				ClassicAssert.IsNotNull (entry, "Entry 0x0009 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as StringIFDEntry, "Entry is not a string!");
				ClassicAssert.AreEqual ("", (entry as StringIFDEntry).Value.Trim ());
			}
			// Canon.0x0010 (ModelID/Long/1) "23330816"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.ModelID);
				ClassicAssert.IsNotNull (entry, "Entry 0x0010 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (23330816, (entry as LongIFDEntry).Value);
			}
			// Canon.0x000D (0x000d/Long/85) "0 1 0 4294967221 4294967200 4294967200 4294967291 0 0 0 9 10 4294967279 4294967231 283 10 4294967183 4294967269 460 0 0 0 0 0 0 0 0 0 79 641 199 0 0 0 0 0 0 376 0 583 183 0 0 1228 1535 583 182 114 1032 1481 2590 1032 1 480 283 4294967173 556 288 0 0 1 0 0 0 0 4756 0 0 0 0 0 0 0 5012 0 0 0 0 0 1 0 1 4736 23 13"
			{
				var entry = makernote_structure.GetEntry (0, (ushort)CanonMakerNoteEntryTag.Unknown13);
				ClassicAssert.IsNotNull (entry, "Entry 0x000D missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongArrayIFDEntry, "Entry is not a long array!");
				ClassicAssert.AreEqual (new long[] { 0, 1, 0, 4294967221, 4294967200, 4294967200, 4294967291, 0, 0, 0, 9, 10, 4294967279, 4294967231, 283, 10, 4294967183, 4294967269, 460, 0, 0, 0, 0, 0, 0, 0, 0, 0, 79, 641, 199, 0, 0, 0, 0, 0, 0, 376, 0, 583, 183, 0, 0, 1228, 1535, 583, 182, 114, 1032, 1481, 2590, 1032, 1, 480, 283, 4294967173, 556, 288, 0, 0, 1, 0, 0, 0, 0, 4756, 0, 0, 0, 0, 0, 0, 0, 5012, 0, 0, 0, 0, 0, 1, 0, 1, 4736, 23, 13 }, (entry as LongArrayIFDEntry).Values);
			}
			// Canon.0x0018 (0x0018/Byte/256) "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 "
			{
				// TODO: Unknown IFD tag: Canon / 0x0018
				var entry = makernote_structure.GetEntry (0, 0x0018);
				ClassicAssert.IsNotNull (entry, "Entry 0x0018 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ByteVectorIFDEntry, "Entry is not a byte array!");
				var parsed_bytes = (entry as ByteVectorIFDEntry).Data.Data;
				var bytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Canon.0x0019 (0x0019/Short/1) "1"
			{
				// TODO: Unknown IFD tag: Canon / 0x0019
				var entry = makernote_structure.GetEntry (0, 0x0019);
				ClassicAssert.IsNotNull (entry, "Entry 0x0019 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1, (entry as ShortIFDEntry).Value);
			}
			// Canon.0x001A (0x001a/Short/1) "2"
			{
				// TODO: Unknown IFD tag: Canon / 0x001A
				var entry = makernote_structure.GetEntry (0, 0x001A);
				ClassicAssert.IsNotNull (entry, "Entry 0x001A missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Canon.0x001C (0x001c/Short/1) "0"
			{
				// TODO: Unknown IFD tag: Canon / 0x001C
				var entry = makernote_structure.GetEntry (0, 0x001C);
				ClassicAssert.IsNotNull (entry, "Entry 0x001C missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (0, (entry as ShortIFDEntry).Value);
			}
			// Canon.0x001D (0x001d/Short/16) "32 1 0 2 2 2 2 0 0 0 0 0 0 0 0 0"
			{
				// TODO: Unknown IFD tag: Canon / 0x001D
				var entry = makernote_structure.GetEntry (0, 0x001D);
				ClassicAssert.IsNotNull (entry, "Entry 0x001D missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortArrayIFDEntry, "Entry is not a short array!");
				ClassicAssert.AreEqual (new ushort[] { 32, 1, 0, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, (entry as ShortArrayIFDEntry).Values);
			}
			// Canon.0x001E (0x001e/Long/1) "16778496"
			{
				// TODO: Unknown IFD tag: Canon / 0x001E
				var entry = makernote_structure.GetEntry (0, 0x001E);
				ClassicAssert.IsNotNull (entry, "Entry 0x001E missing in IFD 0");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (16778496, (entry as LongIFDEntry).Value);
			}
			// Photo.0x9286 (UserComment/UserComment/24) "charset="InvalidCharsetId" 5"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.UserComment);
				ClassicAssert.IsNotNull (entry, "Entry 0x9286 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UserCommentIFDEntry, "Entry is not a user comment!");
				// Commented, it's corrupt anyway.
				//ClassicAssert.AreEqual ("charset="InvalidCharsetId" 5", (entry as UserCommentIFDEntry).Value.Trim ());
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
			// Photo.0xA002 (PixelXDimension/Short/1) "2592"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.PixelXDimension);
				ClassicAssert.IsNotNull (entry, "Entry 0xA002 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2592, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA003 (PixelYDimension/Short/1) "1944"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.PixelYDimension);
				ClassicAssert.IsNotNull (entry, "Entry 0xA003 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1944, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA005 (InteroperabilityTag/SubIFD/1) "1942"
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
			// Iop.0x0002 (InteroperabilityVersion/Undefined/4) "48 49 48 48 "
			{
				var entry = iop_structure.GetEntry (0, (ushort)IOPEntryTag.InteroperabilityVersion);
				ClassicAssert.IsNotNull (entry, "Entry 0x0002 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 49, 48, 48 };
				ClassicAssert.AreEqual (bytes, parsed_bytes);
			}
			// Iop.0x1001 (RelatedImageWidth/Short/1) "2592"
			{
				var entry = iop_structure.GetEntry (0, (ushort)IOPEntryTag.RelatedImageWidth);
				ClassicAssert.IsNotNull (entry, "Entry 0x1001 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2592, (entry as ShortIFDEntry).Value);
			}
			// Iop.0x1002 (RelatedImageLength/Short/1) "1944"
			{
				var entry = iop_structure.GetEntry (0, (ushort)IOPEntryTag.RelatedImageLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x1002 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (1944, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA20E (FocalPlaneXResolution/Rational/1) "2592000/225"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalPlaneXResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0xA20E missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (2592000, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (225, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0xA20F (FocalPlaneYResolution/Rational/1) "1944000/168"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalPlaneYResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0xA20F missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (1944000, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (168, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0xA210 (FocalPlaneResolutionUnit/Short/1) "2"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FocalPlaneResolutionUnit);
				ClassicAssert.IsNotNull (entry, "Entry 0xA210 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA217 (SensingMethod/Short/1) "2"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SensingMethod);
				ClassicAssert.IsNotNull (entry, "Entry 0xA217 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Photo.0xA300 (FileSource/Undefined/1) "3 "
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.FileSource);
				ClassicAssert.IsNotNull (entry, "Entry 0xA300 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 3 };
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
			// Photo.0xA404 (DigitalZoomRatio/Rational/1) "2592/2592"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.DigitalZoomRatio);
				ClassicAssert.IsNotNull (entry, "Entry 0xA404 missing in IFD 0");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (2592, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (2592, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Photo.0xA406 (SceneCaptureType/Short/1) "0"
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.SceneCaptureType);
				ClassicAssert.IsNotNull (entry, "Entry 0xA406 missing in IFD 0");
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
			// Thumbnail.0x011A (XResolution/Rational/1) "180/1"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.XResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011A missing in IFD 1");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (180, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Thumbnail.0x011B (YResolution/Rational/1) "180/1"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.YResolution);
				ClassicAssert.IsNotNull (entry, "Entry 0x011B missing in IFD 1");
				ClassicAssert.IsNotNull (entry as RationalIFDEntry, "Entry is not a rational!");
				ClassicAssert.AreEqual (180, (entry as RationalIFDEntry).Value.Numerator);
				ClassicAssert.AreEqual (1, (entry as RationalIFDEntry).Value.Denominator);
			}
			// Thumbnail.0x0128 (ResolutionUnit/Short/1) "2"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.ResolutionUnit);
				ClassicAssert.IsNotNull (entry, "Entry 0x0128 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short!");
				ClassicAssert.AreEqual (2, (entry as ShortIFDEntry).Value);
			}
			// Thumbnail.0x0201 (JPEGInterchangeFormat/ThumbnailDataIFD/1) "2090"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.JPEGInterchangeFormat);
				ClassicAssert.IsNotNull (entry, "Entry 0x0201 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as ThumbnailDataIFDEntry, "Entry is not a thumbnail IFD!");
			}
			// Thumbnail.0x0202 (JPEGInterchangeFormatLength/Long/1) "7866"
			{
				var entry = structure.GetEntry (1, (ushort)IFDEntryTag.JPEGInterchangeFormatLength);
				ClassicAssert.IsNotNull (entry, "Entry 0x0202 missing in IFD 1");
				ClassicAssert.IsNotNull (entry as LongIFDEntry, "Entry is not a long!");
				ClassicAssert.AreEqual (7866, (entry as LongIFDEntry).Value);
			}

			//  ---------- End of IFD tests ----------


			//  ---------- Start of XMP tests ----------

			var xmp = file.GetTag (TagTypes.XMP) as XmpTag;
			// Xmp.dc.subject (XmpBag/1) "Macro"
			{
				var node = xmp.NodeTree;
				node = node.GetChild (XmpTag.DC_NS, "subject");
				ClassicAssert.IsNotNull (node);
				ClassicAssert.AreEqual (XmpNodeType.Bag, node.Type);
				ClassicAssert.AreEqual ("", node.Value);
				ClassicAssert.AreEqual (1, node.Children.Count);
				ClassicAssert.AreEqual ("Macro", node.Children[0].Value);
			}

			//  ---------- End of XMP tests ----------

		}
	}
}
