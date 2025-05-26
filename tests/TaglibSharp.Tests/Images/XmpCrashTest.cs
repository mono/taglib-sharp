using NUnit.Framework;
using System;
using TagLib;

namespace TaglibSharp.Tests.Images
{
	/// <summary>
	///    This file presents an interesting challenge as it has an
	///    xmlns declaration in a somewhat nonstandard location.
	///    This is valid, so we need to take it into account.
	/// </summary>
	[TestFixture]
	public class XmpCrashTest
	{
		static readonly string sample_file = TestPath.Samples + "sample_xmpcrash.jpg";

		[Test]
		public void ParseXmp ()
		{
			var file = File.Create (sample_file) as TagLib.Image.File;
			Assert.IsNotNull (file, "file");

			var tag = file.ImageTag;
			Assert.IsNotNull (tag, "ImageTag");
			Assert.AreEqual ("Asahi Optical Co.,Ltd. ", tag.Make);
			Assert.AreEqual ("PENTAX Optio 230   ", tag.Model);
			Assert.AreEqual (null, tag.ISOSpeedRatings, "ISOSpeedRatings");
			Assert.AreEqual (new[] { "Türkei 2004" }, tag.Keywords);
			Assert.AreEqual (new DateTime (2004, 08, 23, 11, 20, 57), tag.DateTime);
		}
	}
}
