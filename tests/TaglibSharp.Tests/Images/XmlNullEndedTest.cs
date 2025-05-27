using File = TagLib.File;

namespace TaglibSharp.Tests.Images
{
	/// <summary>
	///    This file contains XMP data ended with null (0x00) value.
	/// </summary>
	[TestClass]
	public class XmpNullEndedTest
	{
		static readonly string sample_file = TestPath.Samples + "sample_xmpnullended.jpg";

		[TestMethod]
		public void ParseXmp ()
		{
			var file = File.Create (sample_file, "taglib/jpeg", ReadStyle.Average) as TagLib.Image.File;
			Assert.IsNotNull (file, "file");

			var tag = file.ImageTag;
			Assert.IsNotNull (tag, "ImageTag");
			Assert.AreEqual ("SONY ", tag.Make);
			Assert.AreEqual ("DSLR-A330", tag.Model);
		}
	}
}
