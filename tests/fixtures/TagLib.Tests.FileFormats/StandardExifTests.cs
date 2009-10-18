using System;
using NUnit.Framework;

using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;

namespace TagLib.Tests.FileFormats
{
    public static class StandardExifTests
    {
        public static void WriteTags (string sample_file, string tmp_file)
        {
            if (System.IO.File.Exists (tmp_file))
                System.IO.File.Delete (tmp_file);

            try {
                System.IO.File.Copy(sample_file, tmp_file);

                File tmp = File.Create (tmp_file);
                SetExifTags (tmp);
                tmp.Save ();

                tmp = File.Create (tmp_file);
                CheckTags (tmp);
            } finally {
//                if (System.IO.File.Exists (tmp_file))
//                    System.IO.File.Delete (tmp_file);
            }
        }

        public static void SetExifTags (File file)
        {
			IFDTag tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			Assert.IsFalse (tag == null);

			tag.Comment = "Test Comment äüö";
        }

        public static void CheckTags (File file)
        {
			IFDTag tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			Assert.IsFalse (tag == null);


			Assert.AreEqual ("Test Comment äüö", tag.Comment);
        }
	}
}
