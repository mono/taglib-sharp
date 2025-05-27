using TagLib.IFD;
using TagLib.Jpeg;
using TagLib.Xmp;

using File = TagLib.File;

namespace TaglibSharp.Tests.Images
{
	[TestClass]
	public class JpegSegmentSizeTest
	{
		static readonly string sample_file = TestPath.Samples + "sample.jpg";
		static readonly string tmp_file = TestPath.SamplesTmp + "tmpwrite_exceed_segment_size.jpg";

		static readonly int max_segment_size = 0xFFFF;

		readonly TagTypes contained_types =
				TagTypes.JpegComment |
				TagTypes.TiffIFD |
				TagTypes.XMP;


		string CreateDataString (int min_size)
		{
			string src = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

			var data = new ByteVector ();

			for (int i = 0; data.Count < min_size; i++) {
				int index = i % src.Length;
				data.Add (src.Substring (index, src.Length - index));
			}

			return data.ToString ();
		}

		[TestMethod]
		public void ExifExceed ()
		{
			var tmp = Utils.CreateTmpFile (sample_file, tmp_file) as File;
			CheckTags (tmp);

			var exif_tag = tmp.GetTag (TagTypes.TiffIFD) as IFDTag;

			Assert.IsNotNull (exif_tag, "exif tag");

			// ensure data is big enough
			exif_tag.Comment = CreateDataString (max_segment_size);

			Assert.IsFalse (SaveFile (tmp), "file with exceed exif segment saved");
		}

		[TestMethod]
		public void XmpExceed ()
		{
			var tmp = Utils.CreateTmpFile (sample_file, tmp_file) as File;
			CheckTags (tmp);

			var xmp_tag = tmp.GetTag (TagTypes.XMP) as XmpTag;

			Assert.IsNotNull (xmp_tag, "xmp tag");

			// ensure data is big enough
			xmp_tag.Comment = CreateDataString (max_segment_size);

			Assert.IsFalse (SaveFile (tmp), "file with exceed xmp segment saved");
		}

		[TestMethod]
		public void JpegCommentExceed ()
		{
			var tmp = Utils.CreateTmpFile (sample_file, tmp_file) as File;
			CheckTags (tmp);

			var com_tag = tmp.GetTag (TagTypes.JpegComment) as JpegCommentTag;

			Assert.IsNotNull (com_tag, "comment tag");

			// ensure data is big enough
			com_tag.Comment = CreateDataString (max_segment_size);

			Assert.IsFalse (SaveFile (tmp), "file with exceed comment segment saved");
		}

		void CheckTags (File file)
		{
			Assert.IsTrue (file is TagLib.Jpeg.File, "not a Jpeg file");

			Assert.AreEqual (contained_types, file.TagTypes);
			Assert.AreEqual (contained_types, file.TagTypesOnDisk);
		}

		bool SaveFile (File file)
		{
			try {
				file.Save ();
			} catch (Exception) {
				return false;
			}

			return true;
		}
	}
}
