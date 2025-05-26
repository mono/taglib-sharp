using NUnit.Framework;
using TagLib;
using TaglibSharp.Tests.Images.Validators;

namespace TaglibSharp.Tests.Images
{
	[TestFixture]
	public class GifGimpTest
	{
		[Test]
		public void Test ()
		{
			// This file is originally created with GIMP.
			ImageTest.Run ("sample_gimp.gif",
				true,
				new GifGimpTestInvariantValidator (),
				NoModificationValidator.Instance,
				new CommentModificationValidator ("Created with GIMP"),
				new TagCommentModificationValidator ("Created with GIMP", TagTypes.GifComment, true),
				new TagKeywordsModificationValidator (TagTypes.XMP, false),
				new RemoveMetadataValidator (TagTypes.GifComment)
			);
		}
	}

	public class GifGimpTestInvariantValidator : IMetadataInvariantValidator
	{
		public void ValidateMetadataInvariants (TagLib.Image.File file)
		{
			ClassicAssert.IsNotNull (file);
			ClassicAssert.IsNotNull (file.Properties);

			ClassicAssert.AreEqual (12, file.Properties.PhotoWidth);
			ClassicAssert.AreEqual (37, file.Properties.PhotoHeight);
		}
	}
}
