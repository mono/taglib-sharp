using System;
using NUnit.Framework;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.Jpeg;
using TagLib.Xmp;

namespace TagLib.Tests.Images
{
    [TestFixture]
    public class JpegSegmentSizeTest
    {
		private static string sample_file = "samples/sample.jpg";
        private static string tmp_file = "samples/tmpwrite_exceed_segment_size.jpg";

		private static int max_segment_size = 0xFFFF;

		private TagTypes contained_types =
				TagTypes.JpegComment |
				TagTypes.TiffIFD |
				TagTypes.XMP;


		private string CreateDataString (int min_size)
		{
			string src = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

			ByteVector data = new ByteVector ();

			for (int i = 0; data.Count < min_size; i++)
			{
				int index = i % src.Length;
				data.Add (src.Substring (index, src.Length - index));
			}

			return data.ToString ();
		}

		[Test]
		public void ExifExceed ()
		{
			File tmp = Utils.CreateTmpFile (sample_file, tmp_file) as File;
			CheckTags (tmp, contained_types);

			var exif_tag = tmp.GetTag (TagTypes.TiffIFD) as IFDTag;

			Assert.IsNotNull (exif_tag, "exif tag");

			// ensure data is big enough
			exif_tag.Comment = CreateDataString (max_segment_size);

			tmp.Save ();

			// tags on disk should now differ
			Assert.AreEqual (contained_types & ~TagTypes.TiffIFD, tmp.TagTypesOnDisk, "tag types on disk");


			// reload file and check tags
			// since TiffIFD should not be written, it should now not be contained anymore
			tmp = File.Create (tmp_file);
			CheckTags (tmp, contained_types & ~TagTypes.TiffIFD);
		}

		[Test]
		public void XmpExceed ()
		{
			File tmp = Utils.CreateTmpFile (sample_file, tmp_file) as File;
			CheckTags (tmp, contained_types);

			var xmp_tag = tmp.GetTag (TagTypes.XMP) as XmpTag;

			Assert.IsNotNull (xmp_tag, "xmp tag");

			// ensure data is big enough
			xmp_tag.Comment = CreateDataString (max_segment_size);

			tmp.Save ();

			// tags on disk should now differ
			Assert.AreEqual (contained_types & ~TagTypes.XMP, tmp.TagTypesOnDisk, "tag types on disk");


			// reload file and check tags
			// since XMP should not be written, it should now not be contained anymore
			tmp = File.Create (tmp_file);
			CheckTags (tmp, contained_types & ~TagTypes.XMP);
		}

		[Test]
		public void JpegCommentExceed ()
		{
			File tmp = Utils.CreateTmpFile (sample_file, tmp_file) as File;
			CheckTags (tmp, contained_types);

			var com_tag = tmp.GetTag (TagTypes.JpegComment) as JpegCommentTag;

			Assert.IsNotNull (com_tag, "comment tag");

			// ensure data is big enough
			com_tag.Comment = CreateDataString (max_segment_size);

			tmp.Save ();

			// tags on disk should now differ
			Assert.AreEqual (contained_types & ~TagTypes.JpegComment, tmp.TagTypesOnDisk, "tag types on disk");


			// reload file and check tags
			// since XMP should not be written, it should now not be contained anymore
			tmp = File.Create (tmp_file);
			CheckTags (tmp, contained_types & ~TagTypes.JpegComment);
		}

		public void CheckTags (File file, TagTypes types) {
			Assert.IsTrue (file is Jpeg.File, "not a Jpeg file");

			Assert.AreEqual (types, file.TagTypes);
			Assert.AreEqual (types, file.TagTypesOnDisk);
		}
	}
}
