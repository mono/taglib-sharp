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

		[Test]
		public void AddComment ()
		{
            File file = Utils.CreateTmpFile (sample_file, tmp_file);

			JpegCommentTag com_tag =
				file.GetTag (TagTypes.JpegComment, false) as JpegCommentTag;

			Assert.IsNull (com_tag, "JpegComment Tag contained");

			com_tag = file.GetTag (TagTypes.JpegComment, true) as JpegCommentTag;

			Assert.IsNotNull (com_tag, "JpegComment Tag not created");
			Assert.IsNull (com_tag.Value, "JpegComment Tag Value is not null");

			com_tag.Value = comment;

			file.Save ();
			file = File.Create (tmp_file);

			com_tag = file.GetTag (TagTypes.JpegComment) as JpegCommentTag;

			Assert.IsNotNull (com_tag, "JpegComment Tag not read");
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

		[Test]
		public void AddXMP1 ()
		{
			AddImageMetadataTests.AddXMPTest1 (sample_file, tmp_file, false);
		}

		[Test]
		public void AddXMP2 ()
		{
			AddImageMetadataTests.AddXMPTest2 (sample_file, tmp_file, false);
		}
	}
}
