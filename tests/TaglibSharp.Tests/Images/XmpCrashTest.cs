using NUnit.Framework;
using System;
using TagLib;

using File = TagLib.File;

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
			ClassicAssert.IsNotNull (file, "file");

			var tag = file.ImageTag;
			ClassicAssert.IsNotNull (tag, "ImageTag");
			ClassicAssert.AreEqual ("Asahi Optical Co.,Ltd. ", tag.Make);
			ClassicAssert.AreEqual ("PENTAX Optio 230   ", tag.Model);
			ClassicAssert.AreEqual (null, tag.ISOSpeedRatings, "ISOSpeedRatings");
			ClassicAssert.AreEqual (new[] { "Türkei 2004" }, tag.Keywords);
			ClassicAssert.AreEqual (new DateTime (2004, 08, 23, 11, 20, 57), tag.DateTime);
		}
	}
}
