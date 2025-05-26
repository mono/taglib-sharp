using NUnit.Framework;
using System;
using TagLib;
using TagLib.Image;

namespace TaglibSharp.Tests.Images
{
	[TestFixture]
	public class CopyFromTest
	{
		[Test]
		public void TestJPGtoTIFF ()
		{
			var file1 = TagLib.File.Create (TestPath.Samples + "sample_canon_zoombrowser.jpg") as TagLib.Image.File;
			ClassicAssert.IsNotNull (file1);
			var file2 = TagLib.File.Create (TestPath.Samples + "sample_nikon1_bibble5_16bit.tiff") as TagLib.Image.File;
			ClassicAssert.IsNotNull (file2);

			// Verify initial values
			ClassicAssert.AreEqual (TagTypes.TiffIFD, file1.TagTypes);
			ClassicAssert.AreEqual (TagTypes.TiffIFD | TagTypes.XMP, file2.TagTypes);
			ClassicAssert.AreEqual ("%test comment%", file1.ImageTag.Comment);
			ClassicAssert.AreEqual (string.Empty, file2.ImageTag.Comment);
			ClassicAssert.AreEqual (new string[] { }, file1.ImageTag.Keywords);
			ClassicAssert.AreEqual (new string[] { }, file2.ImageTag.Keywords);
			ClassicAssert.AreEqual (null, file1.ImageTag.Rating);
			ClassicAssert.AreEqual (0, file2.ImageTag.Rating);
			ClassicAssert.AreEqual (new DateTime (2009, 8, 9, 19, 12, 44), (DateTime)file1.ImageTag.DateTime);
			ClassicAssert.AreEqual (new DateTime (2007, 2, 15, 17, 7, 48), (DateTime)file2.ImageTag.DateTime);
			ClassicAssert.AreEqual (ImageOrientation.TopLeft, file1.ImageTag.Orientation);
			ClassicAssert.AreEqual (ImageOrientation.TopLeft, file2.ImageTag.Orientation);
			ClassicAssert.AreEqual ("Digital Photo Professional", file1.ImageTag.Software);
			ClassicAssert.AreEqual (null, file2.ImageTag.Software);
			ClassicAssert.AreEqual (null, file1.ImageTag.Latitude);
			ClassicAssert.AreEqual (null, file2.ImageTag.Latitude);
			ClassicAssert.AreEqual (null, file1.ImageTag.Longitude);
			ClassicAssert.AreEqual (null, file2.ImageTag.Longitude);
			ClassicAssert.AreEqual (null, file1.ImageTag.Altitude);
			ClassicAssert.AreEqual (null, file2.ImageTag.Altitude);
			ClassicAssert.IsTrue (Math.Abs ((double)file1.ImageTag.ExposureTime - 0.005) < 0.0001);
			ClassicAssert.IsTrue (Math.Abs ((double)file2.ImageTag.ExposureTime - 0.0013) < 0.0001);
			ClassicAssert.IsTrue (Math.Abs ((double)file1.ImageTag.FNumber - 6.3) < 0.0001);
			ClassicAssert.IsTrue (Math.Abs ((double)file2.ImageTag.FNumber - 13) < 0.0001);
			ClassicAssert.AreEqual (400, file1.ImageTag.ISOSpeedRatings);
			ClassicAssert.AreEqual (1600, file2.ImageTag.ISOSpeedRatings);
			ClassicAssert.AreEqual (180, file1.ImageTag.FocalLength);
			ClassicAssert.AreEqual (50, file2.ImageTag.FocalLength);
			ClassicAssert.AreEqual (null, file1.ImageTag.FocalLengthIn35mmFilm);
			ClassicAssert.AreEqual (75, file2.ImageTag.FocalLengthIn35mmFilm);
			ClassicAssert.AreEqual ("Canon", file1.ImageTag.Make);
			ClassicAssert.AreEqual ("NIKON CORPORATION", file2.ImageTag.Make);
			ClassicAssert.AreEqual ("Canon EOS 400D DIGITAL", file1.ImageTag.Model);
			ClassicAssert.AreEqual ("NIKON D70s", file2.ImageTag.Model);
			ClassicAssert.AreEqual (null, file1.ImageTag.Creator);
			ClassicAssert.AreEqual (null, file2.ImageTag.Creator);

			// Copy Metadata
			file2.CopyFrom (file1);

			// Verify copied values
			ClassicAssert.AreEqual (TagTypes.TiffIFD, file1.TagTypes);
			ClassicAssert.AreEqual (TagTypes.TiffIFD | TagTypes.XMP, file2.TagTypes);
			ClassicAssert.AreEqual ("%test comment%", file1.ImageTag.Comment);
			ClassicAssert.AreEqual ("%test comment%", file2.ImageTag.Comment);
			ClassicAssert.AreEqual (new string[] { }, file1.ImageTag.Keywords);
			ClassicAssert.AreEqual (new string[] { }, file2.ImageTag.Keywords);
			ClassicAssert.AreEqual (null, file1.ImageTag.Rating);
			ClassicAssert.AreEqual (null, file2.ImageTag.Rating);
			ClassicAssert.AreEqual (new DateTime (2009, 8, 9, 19, 12, 44), (DateTime)file1.ImageTag.DateTime);
			ClassicAssert.AreEqual (new DateTime (2009, 8, 9, 19, 12, 44), (DateTime)file2.ImageTag.DateTime);
			ClassicAssert.AreEqual (ImageOrientation.TopLeft, file1.ImageTag.Orientation);
			ClassicAssert.AreEqual (ImageOrientation.TopLeft, file2.ImageTag.Orientation);
			ClassicAssert.AreEqual ("Digital Photo Professional", file1.ImageTag.Software);
			ClassicAssert.AreEqual ("Digital Photo Professional", file2.ImageTag.Software);
			ClassicAssert.AreEqual (null, file1.ImageTag.Latitude);
			ClassicAssert.AreEqual (null, file2.ImageTag.Latitude);
			ClassicAssert.AreEqual (null, file1.ImageTag.Longitude);
			ClassicAssert.AreEqual (null, file2.ImageTag.Longitude);
			ClassicAssert.AreEqual (null, file1.ImageTag.Altitude);
			ClassicAssert.AreEqual (null, file2.ImageTag.Altitude);
			ClassicAssert.IsTrue (Math.Abs ((double)file1.ImageTag.ExposureTime - 0.005) < 0.0001);
			ClassicAssert.IsTrue (Math.Abs ((double)file2.ImageTag.ExposureTime - 0.005) < 0.0001);
			ClassicAssert.IsTrue (Math.Abs ((double)file1.ImageTag.FNumber - 6.3) < 0.0001);
			ClassicAssert.IsTrue (Math.Abs ((double)file2.ImageTag.FNumber - 6.3) < 0.0001);
			ClassicAssert.AreEqual (400, file1.ImageTag.ISOSpeedRatings);
			ClassicAssert.AreEqual (400, file2.ImageTag.ISOSpeedRatings);
			ClassicAssert.AreEqual (180, file1.ImageTag.FocalLength);
			ClassicAssert.AreEqual (180, file2.ImageTag.FocalLength);
			ClassicAssert.AreEqual (null, file1.ImageTag.FocalLengthIn35mmFilm);
			ClassicAssert.AreEqual (null, file2.ImageTag.FocalLengthIn35mmFilm);
			ClassicAssert.AreEqual ("Canon", file1.ImageTag.Make);
			ClassicAssert.AreEqual ("Canon", file2.ImageTag.Make);
			ClassicAssert.AreEqual ("Canon EOS 400D DIGITAL", file1.ImageTag.Model);
			ClassicAssert.AreEqual ("Canon EOS 400D DIGITAL", file2.ImageTag.Model);
			ClassicAssert.AreEqual (null, file1.ImageTag.Creator);
			ClassicAssert.AreEqual (null, file2.ImageTag.Creator);
		}
	}
}
