using NUnit.Framework;
using TagLib;
using TaglibSharp.Tests.Images.Validators;

namespace TaglibSharp.Tests.Images
{
	[TestFixture]
	public class GifExiftoolTangled3Test
	{
		[Test]
		public void Test ()
		{
			// This file is originally created with GIMP and the metadata was modified
			// by exiftool. Furthermore, the file is modified in the following way:
			// (1) the blocks which contain the metadata are moved to the end of the file.
			//     This is allowed and should be handled correctly by taglib.
			// (2) XMP Block is removed.
			ImageTest.Run ("sample_exiftool_tangled3.gif",
				true,
				new GifExiftoolTangled3TestInvariantValidator (),
				NoModificationValidator.Instance,
				new TagKeywordsModificationValidator (new string[] { }, TagTypes.XMP, false),
				new CommentModificationValidator ("Created with GIMP"),
				new TagCommentModificationValidator ("Created with GIMP", TagTypes.GifComment, true),
				new RemoveMetadataValidator (TagTypes.GifComment)
			);
		}
	}

	public class GifExiftoolTangled3TestInvariantValidator : IMetadataInvariantValidator
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
