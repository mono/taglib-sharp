using NUnit.Framework;
using TagLib;
using File = TagLib.File;

namespace TaglibSharp.Tests.Images
{
	/// <summary>
	///    This file contains XMP data ended with null (0x00) value.
	/// </summary>
	[TestFixture]
	public class XmpNullEndedTest
	{
		static readonly string sample_file = TestPath.Samples + "sample_xmpnullended.jpg";

		[Test]
		public void ParseXmp ()
		{
			var file = File.Create (sample_file, "taglib/jpeg", ReadStyle.Average) as TagLib.Image.File;
			ClassicAssert.IsNotNull (file, "file");

			var tag = file.ImageTag;
			ClassicAssert.IsNotNull (tag, "ImageTag");
			ClassicAssert.AreEqual ("SONY ", tag.Make);
			ClassicAssert.AreEqual ("DSLR-A330", tag.Model);
		}
	}
}
