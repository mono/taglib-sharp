using System;
using NUnit.Framework;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.Jpeg;
using TagLib.Xmp;
using TagLib.Tests.Images.Validators;

namespace TagLib.Tests.Images
{
	[TestFixture]
	public class JpegCanonZoombrowserTest
	{
		[Test]
		public void TestModifications () {
			ImageTest.Run ("sample_canon_zoombrowser.jpg",
				new JpegCanonZoombrowserInvariantValidator (),
				new CommentModificationValidator ("%test comment%"),
				new TagCommentModificationValidator ("%test comment%", TagTypes.TiffIFD, true),
				new TagCommentModificationValidator ("%test comment%", TagTypes.XMP, false),
				new TagKeywordsModificationValidator (TagTypes.XMP, false)
			);
		}
	}


	public class JpegCanonZoombrowserInvariantValidator : IMetadataInvariantValidator
	{
		private TagTypes contained_types = TagTypes.TiffIFD;

		public void ValidateMetadataInvariants (Image.File file)
		{
			CheckExif (file);
			CheckMakerNote (file);
			CheckProperties (file);
		}

		void CheckExif (File file) {
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			Assert.IsNotNull (tag, "Tiff Tag not contained");

			var exif_ifd = tag.Structure.GetEntry(0, IFDEntryTag.ExifIFD) as SubIFDEntry;
			Assert.IsNotNull (exif_ifd, "Exif SubIFD not contained");

			Assert.AreEqual ("Canon", tag.Make);
			Assert.AreEqual ("Canon EOS 400D DIGITAL", tag.Model);
			Assert.AreEqual (400, tag.ISOSpeedRatings);
			Assert.AreEqual (1.0d/200.0d, tag.ExposureTime);
			Assert.AreEqual (6.3d, tag.FNumber);
			Assert.AreEqual (180.0d, tag.FocalLength);
			Assert.AreEqual (new DateTime (2009, 08, 09, 19, 12, 44), tag.DateTime);
			Assert.AreEqual (new DateTime (2009, 08, 09, 19, 12, 44), tag.DateTimeDigitized);
			Assert.AreEqual (new DateTime (2009, 08, 09, 19, 12, 44), tag.DateTimeOriginal);
		}


		void CheckMakerNote (File file) {
			IFDTag tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			Assert.IsNotNull (tag, "Tiff Tag not contained");

			var makernote_ifd =
				tag.ExifIFD.GetEntry (0, (ushort) ExifEntryTag.MakerNote) as MakernoteIFDEntry;

			Assert.IsNotNull (makernote_ifd, "Makernote SubIFD not contained");
			Assert.AreEqual (MakernoteType.Canon, makernote_ifd.MakernoteType);

			var structure = makernote_ifd.Structure;
			Assert.IsNotNull (structure, "Makernote IFD Structure not contained");
			/* TODO Check some Markenote entries */
		}

		void CheckProperties (File file)
		{
			Assert.AreEqual (19, file.Properties.PhotoWidth);
			Assert.AreEqual (41, file.Properties.PhotoHeight);
			Assert.AreEqual (90, file.Properties.PhotoQuality);
		}
	}
}
