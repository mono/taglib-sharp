using System;
using NUnit.Framework;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.Jpeg;

namespace TagLib.Tests.FileFormats
{
    [TestFixture]
    public class JpegAddMetadataTest
    {
		private static string sample_file = "samples/sample_no_metadata.jpg";
        private static string tmp_file = "samples/tmpwrite_no_metadata.jpg";
		private File file;

		private static string comment = " test comment ";

		[Test]
		public void AddComment ()
		{
			if (System.IO.File.Exists (tmp_file))
                System.IO.File.Delete (tmp_file);

            System.IO.File.Copy (sample_file, tmp_file);

            File file = File.Create (tmp_file);

			JpegCommentTag com_tag = file.GetTag (TagTypes.JpegComment, true) as JpegCommentTag;

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
			AddImageMetadataTests.AddExif (sample_file, tmp_file);
		}
	}
}
