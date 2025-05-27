using TagLib.Image;

namespace TaglibSharp.Tests.Images;

/// <summary>
///    This test validates the correct mapping of different metadata formats onto ImageTag.
/// </summary>
[TestClass]
public class ImageTagTests
{
	[TestMethod]
	public void TestXMPImageTag ()
	{
		var file = TagLib.File.Create (TestPath.Samples + "sample_canon_bibble5.jpg") as TagLib.Image.File;
		Assert.IsNotNull (file);

		var tag = file.GetTag (TagTypes.XMP) as ImageTag;
		Assert.IsNotNull (tag);

		Assert.IsNull (tag.Comment, "Comment");
		CollectionAssert.AreEqual (new string[] { }, tag.Keywords, "Keywords");
		Assert.AreEqual (0u, tag.Rating, "Rating");
		Assert.IsNull (tag.DateTime, "DateTime");
		Assert.AreEqual (ImageOrientation.None, tag.Orientation, "Orientation");
		Assert.IsNull (tag.Software, "Software");
		Assert.IsNull (tag.Latitude, "Latitude");
		Assert.IsNull (tag.Longitude, "Longitude");
		Assert.IsNull (tag.Altitude, "Altitude");
		Assert.AreEqual (0.005, tag.ExposureTime, "ExposureTime");
		Assert.AreEqual (5, tag.FNumber, "FNumber");
		Assert.AreEqual (100u, tag.ISOSpeedRatings, "ISOSpeedRatings");
		Assert.AreEqual (21, tag.FocalLength, "FocalLength");
		Assert.IsNull (tag.FocalLengthIn35mmFilm, "FocalLengthIn35mmFilm");
		Assert.AreEqual ("Canon", tag.Make, "Make");
		Assert.AreEqual ("Canon EOS 400D DIGITAL", tag.Model, "Model");
		Assert.IsNull (tag.Creator, "Creator");
	}

	[TestMethod]
	public void TestXMPImageTag2 ()
	{
		var file = TagLib.File.Create (TestPath.Samples + "sample_gimp_exiftool.jpg") as TagLib.Image.File;
		Assert.IsNotNull (file);

		var tag = file.GetTag (TagTypes.XMP) as ImageTag;
		Assert.IsNotNull (tag);

		Assert.AreEqual ("This is an image Comment", tag.Comment, "Comment");
		CollectionAssert.AreEqual (new string[] { "keyword 1", "keyword 2" }, tag.Keywords, "Keywords");
		Assert.AreEqual (5u, tag.Rating, "Rating");
		Assert.IsNull (tag.DateTime, "DateTime");
		Assert.AreEqual (ImageOrientation.None, tag.Orientation, "Orientation");
		Assert.IsNull (tag.Software, "Software");
		Assert.IsNull (tag.Latitude, "Latitude");
		Assert.IsNull (tag.Longitude, "Longitude");
		Assert.IsNull (tag.Altitude, "Altitude");
		Assert.IsNull (tag.ExposureTime, "ExposureTime");
		Assert.IsNull (tag.FNumber, "FNumber");
		Assert.IsNull (tag.ISOSpeedRatings, "ISOSpeedRatings");
		Assert.IsNull (tag.FocalLength, "FocalLength");
		Assert.IsNull (tag.FocalLengthIn35mmFilm, "FocalLengthIn35mmFilm");
		Assert.IsNull (tag.Make, "Make");
		Assert.IsNull (tag.Model, "Model");
		Assert.AreEqual ("Isaac Newton", tag.Creator, "Creator");
	}
}
