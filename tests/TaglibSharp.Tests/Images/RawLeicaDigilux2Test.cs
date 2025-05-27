using TaglibSharp.Tests.Images.Validators;

namespace TaglibSharp.Tests.Images;

[TestClass]
public class RawLeicaDigilux2Test
{
	[TestMethod]
	public void Test ()
	{
		ImageTest.Run (TestPath.GetRawSubDirectory ("RAW"), "RAW_LEICA_DIGILUX2_SRGB.RAW",
					   false, new RawLeicaDigilux2TestInvariantValidator ());
	}
}

public class RawLeicaDigilux2TestInvariantValidator : IMetadataInvariantValidator
{
	public void ValidateMetadataInvariants (TagLib.Image.File file)
	{
		Assert.IsNotNull (file);
		//
		//  ---------- Start of ImageTag tests ----------

		var imagetag = file.ImageTag;
		Assert.IsNotNull (imagetag);
		Assert.AreEqual (String.Empty, imagetag.Comment, "Comment");
		CollectionAssert.AreEqual (new string[] { }, imagetag.Keywords, "Keywords");
		Assert.IsNull (imagetag.Rating, "Rating");
		Assert.AreEqual (TagLib.Image.ImageOrientation.TopLeft, imagetag.Orientation, "Orientation");
		Assert.IsNull (imagetag.Software, "Software");
		Assert.IsNull (imagetag.Latitude, "Latitude");
		Assert.IsNull (imagetag.Longitude, "Longitude");
		Assert.IsNull (imagetag.Altitude, "Altitude");
		Assert.AreEqual (0.004, imagetag.ExposureTime, "ExposureTime");
		Assert.AreEqual (11, imagetag.FNumber, "FNumber");
		Assert.AreEqual (100u, imagetag.ISOSpeedRatings, "ISOSpeedRatings");
		Assert.AreEqual (7, imagetag.FocalLength, "FocalLength");
		Assert.IsNull (imagetag.FocalLengthIn35mmFilm, "FocalLengthIn35mmFilm");
		Assert.AreEqual ("LEICA", imagetag.Make, "Make");
		Assert.AreEqual ("DIGILUX 2", imagetag.Model, "Model");
		Assert.IsNull (imagetag.Creator, "Creator");

		var properties = file.Properties;
		Assert.IsNotNull (properties);
		Assert.AreEqual (2564, properties.PhotoWidth, "PhotoWidth");
		Assert.AreEqual (1924, properties.PhotoHeight, "PhotoHeight");

		//  ---------- End of ImageTag tests ----------

		//  ---------- Start of IFD tests ----------
		//		--> Omitted, because the test generator doesn't handle them yet.
		//		--> If the above works, I'm happy.
		//  ---------- End of IFD tests ----------

	}
}
