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
    public class JpegAddEmptyTest
    {
		private static string sample_file = "samples/sample_no_metadata.jpg";
        private static string tmp_file = "samples/tmpwrite_no_metadata.jpg";

		private static string comment = " test comment ";

		private File file;


		[Test]
		public void AddComment ()
		{
            File file = Utils.CreateTmpFile (sample_file, tmp_file);

			JpegCommentTag com_tag =
				file.GetTag (TagTypes.JpegComment, false) as JpegCommentTag;

			Assert.IsTrue (com_tag == null);

			com_tag = file.GetTag (TagTypes.JpegComment, true) as JpegCommentTag;

			Assert.IsFalse (com_tag == null);
			Assert.IsTrue (com_tag.Value == null);

			com_tag.Value = comment;

			file.Save ();
			file = File.Create (tmp_file);

			com_tag = file.GetTag (TagTypes.JpegComment) as JpegCommentTag;

			Assert.IsFalse (com_tag == null);
			Assert.AreEqual (comment, com_tag.Value);
		}

		[Test]
		public void AddExif ()
		{
			AddImageMetadataTests.AddExifTest (sample_file, tmp_file, false);
		}

		[Test]
		public void AddGPS ()
		{
			AddImageMetadataTests.AddGPSTest (sample_file, tmp_file, false);
		}
	}
}
