namespace TaglibSharp.Tests.Images.Validators
{
	/// <summary>
	///    This class tests the removal of metadata
	/// </summary>
	public class RemoveMetadataValidator : IMetadataModificationValidator
	{
		readonly TagTypes remove_types;
		readonly TagTypes contained_types;

		public RemoveMetadataValidator (TagTypes contained_types) : this (contained_types, contained_types) { }

		public RemoveMetadataValidator (TagTypes contained_types, TagTypes remove_types)
		{
			this.contained_types = contained_types;
			this.remove_types = remove_types;
		}

		public void ValidatePreModification (TagLib.Image.File file)
		{
			Assert.AreEqual (contained_types, file.TagTypes);
		}

		public void ModifyMetadata (TagLib.Image.File file)
		{
			file.RemoveTags (remove_types);
		}

		public void ValidatePostModification (TagLib.Image.File file)
		{
			Assert.AreEqual (contained_types & (~remove_types), file.TagTypes);
		}
	}
}
