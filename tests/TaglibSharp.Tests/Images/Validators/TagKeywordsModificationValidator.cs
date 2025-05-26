using NUnit.Framework;
using TagLib;

namespace TaglibSharp.Tests.Images.Validators
{
	/// <summary>
	///    This class tests the modification of the Keywords field,
	///    in a specific tag.
	/// </summary>
	public class TagKeywordsModificationValidator : KeywordsModificationValidator
	{
		readonly bool tag_present;
		readonly TagTypes type;

		public TagKeywordsModificationValidator (TagTypes type, bool tag_present) : this (new string[] { }, type, tag_present) { }

		public TagKeywordsModificationValidator (string[] orig_keywords, TagTypes type, bool tag_present) : base (orig_keywords)
		{
			this.type = type;
			this.tag_present = tag_present;
		}

		/// <summary>
		///    Check if the original keywords are found.
		/// </summary>
		public override void ValidatePreModification (TagLib.Image.File file)
		{
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
		public override void ModifyMetadata (TagLib.Image.File file)
		{
			if (!tag_present)
				file.GetTag (type, true);
			base.ModifyMetadata (file);
		}

		public override TagLib.Image.ImageTag GetTag (TagLib.Image.File file)
		{
			return file.GetTag (type, false) as TagLib.Image.ImageTag;
		}
	}
}
