using NUnit.Framework;

namespace TagLib.Tests.Images.Validators
{
	/// <summary>
	///    This class tests the modification of the Comment field,
	///    regardless of which metadata format is used.
	/// </summary>
	public class CommentModificationValidator : IMetadataModificationValidator
	{
		string orig_comment;
		readonly string test_comment = "This is a TagLib# &Test?Comment%$@_ ";

		public CommentModificationValidator (string orig_comment)
		{
			this.orig_comment = orig_comment;
		}

		/// <summary>
		///    Check if the original comment is found.
		/// </summary>
		public virtual void ValidatePreModification (Image.File file) {
			Assert.AreEqual (orig_comment, GetTag (file).Comment);
		}

		/// <summary>
		///    Changes the comment.
		/// </summary>
		public virtual void ModifyMetadata (Image.File file) {
			GetTag (file).Comment = test_comment;
		}

		/// <summary>
		///    Validates if changes survived a write.
		/// </summary>
		public void ValidatePostModification (Image.File file) {
			Assert.AreEqual (test_comment, GetTag (file).Comment);
		}

		/// <summary>
		///    Returns the tag that should be tested. Default
		///    behavior is no specific tag.
		/// </summary>
		public virtual Image.ImageTag GetTag (Image.File file) {
			return file.ImageTag;
		}
	}
}
