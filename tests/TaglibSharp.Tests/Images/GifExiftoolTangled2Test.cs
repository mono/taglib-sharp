using TaglibSharp.Tests.Images.Validators;

namespace TaglibSharp.Tests.Images
{
	[TestClass]
	public class GifExiftoolTangled2Test
	{
		[TestMethod]
		public void Test ()
		{
			// This file is originally created with GIMP and the metadata was modified
			// by exiftool. Furthermore, the file is modified in the following way:
			// (1) the version 89a is substituted by 87a, this leads to an invalid 87a
			//     file, but we change the version if we write metadata.
			// (2) the blocks which contain the metadata are moved to the end of the file.
			//     This is allowed and should be handled correctly by taglib.
			// (3) Gif Comment block is removed
			ImageTest.Run ("sample_exiftool_tangled2.gif",
				true,
				new GifExiftoolTangled2TestInvariantValidator (),
				NoModificationValidator.Instance,
				new TagKeywordsModificationValidator (new[] { "Keyword 1", "Keyword 2" }, TagTypes.XMP, true),
				new CommentModificationValidator (),
				new TagCommentModificationValidator (null, TagTypes.GifComment, false),
				new RemoveMetadataValidator (TagTypes.XMP)
			);
		}
	}

	public class GifExiftoolTangled2TestInvariantValidator : IMetadataInvariantValidator
	{
		public void ValidateMetadataInvariants (TagLib.Image.File file)
		{
			Assert.IsNotNull (file);
			Assert.IsNotNull (file.Properties);

			Assert.AreEqual (12, file.Properties.PhotoWidth);
			Assert.AreEqual (37, file.Properties.PhotoHeight);
		}
	}
}
