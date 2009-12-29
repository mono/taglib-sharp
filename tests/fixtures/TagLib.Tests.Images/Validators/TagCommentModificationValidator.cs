using System;
using NUnit.Framework;

namespace TagLib.Tests.Images.Validators
{
	/// <summary>
	///    This class tests the modification of the Comment field,
	///    in a specific tag.
	/// </summary>
	public class TagCommentModificationValidator : CommentModificationValidator
	{
		bool tag_present;
		TagTypes type;

		public TagCommentModificationValidator (TagTypes type, bool tag_present) : this (String.Empty, type, tag_present) { }

		public TagCommentModificationValidator (string orig_comment, TagTypes type, bool tag_present) : base (orig_comment)
		{
			this.type = type;
			this.tag_present = tag_present;
		}

		/// <summary>
		///    Check if the original comment is found.
		/// </summary>
		public override void ValidatePreModification (Image.File file) {
			if (!tag_present) {
				Assert.IsNull (file.GetTag (type, false));
			} else {
				Assert.IsNotNull (file.GetTag (type, false));
				base.ValidatePreModification (file);
			}
		}

		/// <summary>
		///    Creates the tag if needed.
		/// </summary>
		public override void ModifyMetadata (Image.File file) {
			if (!tag_present)
				file.GetTag (type, true);
			base.ModifyMetadata (file);
		}

		public override Image.ImageTag GetTag (Image.File file) {
			return file.GetTag (type, false) as Image.ImageTag;
		}
	}
}
