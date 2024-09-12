using NUnit.Framework;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Tags;
using TagLib.IFD.Entries;
using TagLib.Xmp;
using TaglibSharp.Tests.Images.Validators;

namespace TaglibSharp.Tests.Images
{
	public class BadIptcSectionTest
	{
		[Test]
		public void Test ()
		{
			ImageTest.Run ("sample_IPTC_crash.jpg",
				false,
				new BadIptcSectionTestInvariantValidator (),
				NoModificationValidator.Instance,
				new PropertyModificationValidator<string> ("Copyright", null, "Copyright 2024 by Somebody Im Sure")
			);
		}
	}
	public class BadIptcSectionTestInvariantValidator : IMetadataInvariantValidator
	{
		int calls = 0;
		public void ValidateMetadataInvariants (TagLib.Image.File file)
		{
			// If we get here, the fix works.
			++calls;

			Assert.IsNotNull (file);

			Assert.IsFalse (file.PossiblyCorrupt);	// The only problem is in the IPTC section, which we ignore.

			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			Assert.IsNotNull (tag, "IFD tag not found");

			var structure = tag.Structure;

			var exif = structure.GetEntry (0, (ushort)IFDEntryTag.ExifIFD) as SubIFDEntry;
			Assert.IsNotNull (exif, "Exif tag not found");

			var exif_structure = exif.Structure;
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ExifVersion);
				Assert.IsNotNull (entry, "Entry 0x9000 missing in IFD 0");
				Assert.IsNotNull (entry as UndefinedIFDEntry, "Entry is not an undefined IFD entry!");
				var parsed_bytes = (entry as UndefinedIFDEntry).Data.Data;
				var bytes = new byte[] { 48, 50, 51, 49 };
				Assert.AreEqual (bytes, parsed_bytes);
			}
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.ColorSpace);
				Assert.IsNotNull (entry, "Entry 0xa001 missing in IFD 0");
				Assert.IsNotNull (entry as ShortIFDEntry, "Entry is not a short IFD entry!");
				var parsed_value = (entry as ShortIFDEntry).Value;
				Assert.AreEqual (65535, parsed_value);
			}
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.PixelXDimension);
				Assert.IsNotNull (entry, "Entry 0xa001 missing in IFD 0");
				Assert.IsNotNull (entry as LongIFDEntry, "Entry is not a long IFD entry!");
				var parsed_value = (entry as LongIFDEntry).Value;
				Assert.AreEqual (4845, parsed_value);
			}
			{
				var entry = exif_structure.GetEntry (0, (ushort)ExifEntryTag.PixelYDimension);
				Assert.IsNotNull (entry, "Entry 0xa001 missing in IFD 0");
				Assert.IsNotNull (entry as LongIFDEntry, "Entry is not a long IFD entry!");
				var parsed_value = (entry as LongIFDEntry).Value;
				Assert.AreEqual (2834, parsed_value);
			}


			var xmp = file.GetTag (TagTypes.XMP) as XmpTag;
			Assert.IsNotNull (xmp, "XMP tag not found");

			Assert.AreEqual ("Adobe Photoshop 22.1 (Windows)", xmp.Software);
			// ValidateMetadataInvariants is called 3 times for each Validator: once in the
			// ParseUnmodifiedFile method, once in the ModifyFile method before changing
			// anything, and once in the ParseModifiedFile method.
			// The PropertyModificationValidator class verifies the property setting, but
			// I'm not sure I totally trust it, so I'm going to check the property value
			// here as well.
			if (calls == 1)
				Assert.IsNull (xmp.Copyright);
			if (xmp.Copyright != null)
				Assert.AreEqual ("Copyright 2024 by Somebody Im Sure", xmp.Copyright);
			Assert.IsNull (xmp.Creator);
		}
	}
}