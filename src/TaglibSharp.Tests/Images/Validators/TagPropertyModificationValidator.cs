using NUnit.Framework;
using TagLib;

namespace TaglibSharp.Tests.Images.Validators
{
	public class TagPropertyModificationValidator<T> : PropertyModificationValidator<T>
	{
		readonly TagTypes type;
		readonly bool tag_present;

		public TagPropertyModificationValidator (string property_name, T orig_value, T test_value, TagTypes type, bool tag_present)
			: base (property_name, orig_value, test_value)
		{
			this.type = type;
			this.tag_present = tag_present;
		}

		public override void ValidatePreModification (TagLib.Image.File file)
		{
			if (!tag_present) {
				Assert.IsNull (GetTag (file));
			} else {
				Assert.IsNotNull (GetTag (file));
				base.ValidatePreModification (file);
			}
		}

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
