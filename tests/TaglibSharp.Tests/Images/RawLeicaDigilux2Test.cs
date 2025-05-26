using NUnit.Framework;
using System;
using TaglibSharp.Tests.Images.Validators;

namespace TaglibSharp.Tests.Images
{
	[TestFixture]
	public class RawLeicaDigilux2Test
	{
		[Test]
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
			ClassicAssert.IsNotNull (file);
			//
			//  ---------- Start of ImageTag tests ----------

			var imagetag = file.ImageTag;
			ClassicAssert.IsNotNull (imagetag);
			ClassicAssert.AreEqual (String.Empty, imagetag.Comment, "Comment");
			ClassicAssert.AreEqual (new string[] { }, imagetag.Keywords, "Keywords");
			ClassicAssert.AreEqual (null, imagetag.Rating, "Rating");
			ClassicAssert.AreEqual (TagLib.Image.ImageOrientation.TopLeft, imagetag.Orientation, "Orientation");
			ClassicAssert.AreEqual (null, imagetag.Software, "Software");
			ClassicAssert.AreEqual (null, imagetag.Latitude, "Latitude");
			ClassicAssert.AreEqual (null, imagetag.Longitude, "Longitude");
			ClassicAssert.AreEqual (null, imagetag.Altitude, "Altitude");
			ClassicAssert.AreEqual (0.004, imagetag.ExposureTime, "ExposureTime");
			ClassicAssert.AreEqual (11, imagetag.FNumber, "FNumber");
			ClassicAssert.AreEqual (100, imagetag.ISOSpeedRatings, "ISOSpeedRatings");
			ClassicAssert.AreEqual (7, imagetag.FocalLength, "FocalLength");
			ClassicAssert.AreEqual (null, imagetag.FocalLengthIn35mmFilm, "FocalLengthIn35mmFilm");
			ClassicAssert.AreEqual ("LEICA", imagetag.Make, "Make");
			ClassicAssert.AreEqual ("DIGILUX 2", imagetag.Model, "Model");
			ClassicAssert.AreEqual (null, imagetag.Creator, "Creator");

			var properties = file.Properties;
			ClassicAssert.IsNotNull (properties);
			ClassicAssert.AreEqual (2564, properties.PhotoWidth, "PhotoWidth");
			ClassicAssert.AreEqual (1924, properties.PhotoHeight, "PhotoHeight");

			//  ---------- End of ImageTag tests ----------

			//  ---------- Start of IFD tests ----------
			//		--> Omitted, because the test generator doesn't handle them yet.
			//		--> If the above works, I'm happy.
			//  ---------- End of IFD tests ----------

		}
	}
}
