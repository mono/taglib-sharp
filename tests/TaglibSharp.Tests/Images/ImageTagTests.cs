using NUnit.Framework;
using TagLib;
using TagLib.Image;

namespace TaglibSharp.Tests.Images
{
	/// <summary>
	///    This test validates the correct mapping of different metadata formats onto ImageTag.
	/// </summary>
	[TestFixture]
	public class ImageTagTests
	{
		[Test]
		public void TestXMPImageTag ()
		{
			var file = TagLib.File.Create (TestPath.Samples + "sample_canon_bibble5.jpg") as TagLib.Image.File;
			ClassicAssert.IsNotNull (file);

			var tag = file.GetTag (TagTypes.XMP) as ImageTag;
			ClassicAssert.IsNotNull (tag);

			ClassicAssert.AreEqual (null, tag.Comment, "Comment");
			ClassicAssert.AreEqual (new string[] { }, tag.Keywords, "Keywords");
			ClassicAssert.AreEqual (0, tag.Rating, "Rating");
			ClassicAssert.AreEqual (null, tag.DateTime, "DateTime");
			ClassicAssert.AreEqual (ImageOrientation.None, tag.Orientation, "Orientation");
			ClassicAssert.AreEqual (null, tag.Software, "Software");
			ClassicAssert.AreEqual (null, tag.Latitude, "Latitude");
			ClassicAssert.AreEqual (null, tag.Longitude, "Longitude");
			ClassicAssert.AreEqual (null, tag.Altitude, "Altitude");
			ClassicAssert.AreEqual (0.005, tag.ExposureTime, "ExposureTime");
			ClassicAssert.AreEqual (5, tag.FNumber, "FNumber");
			ClassicAssert.AreEqual (100, tag.ISOSpeedRatings, "ISOSpeedRatings");
			ClassicAssert.AreEqual (21, tag.FocalLength, "FocalLength");
			ClassicAssert.AreEqual (null, tag.FocalLengthIn35mmFilm, "FocalLengthIn35mmFilm");
			ClassicAssert.AreEqual ("Canon", tag.Make, "Make");
			ClassicAssert.AreEqual ("Canon EOS 400D DIGITAL", tag.Model, "Model");
			ClassicAssert.AreEqual (null, tag.Creator, "Creator");
		}

		[Test]
		public void TestXMPImageTag2 ()
		{
			var file = TagLib.File.Create (TestPath.Samples + "sample_gimp_exiftool.jpg") as TagLib.Image.File;
			ClassicAssert.IsNotNull (file);

			var tag = file.GetTag (TagTypes.XMP) as ImageTag;
			ClassicAssert.IsNotNull (tag);

			ClassicAssert.AreEqual ("This is an image Comment", tag.Comment, "Comment");
			ClassicAssert.AreEqual (new string[] { "keyword 1", "keyword 2" }, tag.Keywords, "Keywords");
			ClassicAssert.AreEqual (5, tag.Rating, "Rating");
			ClassicAssert.AreEqual (null, tag.DateTime, "DateTime");
			ClassicAssert.AreEqual (ImageOrientation.None, tag.Orientation, "Orientation");
			ClassicAssert.AreEqual (null, tag.Software, "Software");
			ClassicAssert.AreEqual (null, tag.Latitude, "Latitude");
			ClassicAssert.AreEqual (null, tag.Longitude, "Longitude");
			ClassicAssert.AreEqual (null, tag.Altitude, "Altitude");
			ClassicAssert.AreEqual (null, tag.ExposureTime, "ExposureTime");
			ClassicAssert.AreEqual (null, tag.FNumber, "FNumber");
			ClassicAssert.AreEqual (null, tag.ISOSpeedRatings, "ISOSpeedRatings");
			ClassicAssert.AreEqual (null, tag.FocalLength, "FocalLength");
			ClassicAssert.AreEqual (null, tag.FocalLengthIn35mmFilm, "FocalLengthIn35mmFilm");
			ClassicAssert.AreEqual (null, tag.Make, "Make");
			ClassicAssert.AreEqual (null, tag.Model, "Model");
			ClassicAssert.AreEqual ("Isaac Newton", tag.Creator, "Creator");
		}
	}
}
