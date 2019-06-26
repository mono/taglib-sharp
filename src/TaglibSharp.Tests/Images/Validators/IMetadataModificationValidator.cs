namespace TaglibSharp.Tests.Images.Validators
{
	public interface IMetadataModificationValidator
	{
		/// <summary>
		///    Validate metadata assumptions that should hold
		///    before modification.
		/// </summary>
		void ValidatePreModification (TagLib.Image.File file);

		/// <summary>
		///    Modify the metadata of a file.
		/// </summary>
		void ModifyMetadata (TagLib.Image.File file);

		/// <summary>
		///    Validate metadata assumptions that should hold
		///    after modification.
		/// </summary>
		void ValidatePostModification (TagLib.Image.File file);
	}
}
