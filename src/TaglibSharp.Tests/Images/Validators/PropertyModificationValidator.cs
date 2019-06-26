using NUnit.Framework;
using System;
using System.Reflection;

namespace TaglibSharp.Tests.Images.Validators
{
	public class PropertyModificationValidator<T> : IMetadataModificationValidator
	{
		readonly T test_value;
		readonly T orig_value;

		readonly PropertyInfo property_info;

		public PropertyModificationValidator (string property_name, T orig_value, T test_value)
		{
			this.test_value = test_value;
			this.orig_value = orig_value;

			property_info = typeof (TagLib.Image.ImageTag).GetProperty (property_name);

			if (property_info == null)
				throw new Exception ($"There is no property named {property_name} in ImageTag");
		}

		public virtual void ValidatePreModification (TagLib.Image.File file)
		{
			Assert.AreEqual (orig_value, GetValue (GetTag (file)));
		}

		public virtual void ModifyMetadata (TagLib.Image.File file)
		{
			SetValue (GetTag (file), test_value);
		}

		public void ValidatePostModification (TagLib.Image.File file)
		{
			Assert.AreEqual (test_value, GetValue (GetTag (file)));
		}

		public virtual TagLib.Image.ImageTag GetTag (TagLib.Image.File file)
		{
			return file.ImageTag;
		}

		public void SetValue (TagLib.Image.ImageTag tag, T value)
		{
			Assert.IsNotNull (tag);

			property_info.SetValue (tag, value, null);
		}

		public T GetValue (TagLib.Image.ImageTag tag)
		{
			Assert.IsNotNull (tag);

			return (T)property_info.GetValue (tag, null);
		}
	}
}
