using System;
using NUnit.Framework;
using TagLib.Image;

namespace TagLib.Tests.Images
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
            var file = TagLib.File.Create ("samples/sample_canon_bibble5.jpg") as TagLib.Image.File;
            Assert.IsNotNull (file);

            var tag = file.GetTag (TagTypes.XMP) as TagLib.Image.ImageTag;
            Assert.IsNotNull (tag);

            Assert.AreEqual (null, tag.Comment, "Comment");
            Assert.AreEqual (new string [] {}, tag.Keywords, "Keywords");
            Assert.AreEqual (0, tag.Rating, "Rating");
            Assert.AreEqual (null, tag.DateTime, "DateTime");
            Assert.AreEqual (ImageOrientation.TopLeft, tag.Orientation, "Orientation");
            Assert.AreEqual (null, tag.Software, "Software");
            Assert.AreEqual (null, tag.Latitude, "Latitude");
            Assert.AreEqual (null, tag.Longitude, "Longitude");
            Assert.AreEqual (null, tag.Altitude, "Altitude");
            Assert.AreEqual (0.005, tag.ExposureTime, "ExposureTime");
            Assert.AreEqual (5, tag.FNumber, "FNumber");
            Assert.AreEqual (100, tag.ISOSpeedRatings, "ISOSpeedRatings");
            Assert.AreEqual (21, tag.FocalLength, "FocalLength");
            Assert.AreEqual (null, tag.FocalLengthIn35mmFilm, "FocalLengthIn35mmFilm");
            Assert.AreEqual ("Canon", tag.Make, "Make");
            Assert.AreEqual ("Canon EOS 400D DIGITAL", tag.Model, "Model");
            Assert.AreEqual (null, tag.Creator, "Creator");
        }
    }
}
