using NUnit.Framework;
using TagLib;
using TaglibSharp.Tests.Images.Validators;

namespace TaglibSharp.Tests.Images
{
	[TestFixture]
	public class JpegNoMetadataTest
	{
		[Test]
		public void Test ()
		{
			ImageTest.Run ("sample_no_metadata.jpg",
				new JpegNoMetadataTestInvariantValidator (),
				NoModificationValidator.Instance,
				new NoModificationValidator (),
				new TagCommentModificationValidator (TagTypes.TiffIFD, false),
				new TagCommentModificationValidator (TagTypes.XMP, false),
				new TagKeywordsModificationValidator (TagTypes.XMP, false)
			);
		}
	}

	public class JpegNoMetadataTestInvariantValidator : IMetadataInvariantValidator
	{
		public void ValidateMetadataInvariants (TagLib.Image.File file)
		{
			Assert.IsNotNull (file);
		}
	}
}
