namespace TaglibSharp.Tests.Images.Validators
{
	/// <summary>
	///    This class writes a file unmodified and tests if all metadata
	///    is still present. Default behavior for the modification
	///    validator.
	/// </summary>
	public class NoModificationValidator : IMetadataModificationValidator
	{
		public static NoModificationValidator Instance { get; } = new NoModificationValidator ();

		/// <summary>
		///    No preconditions that will change (everything is checked
		///    in the invariant validator).
		/// </summary>
		public void ValidatePreModification (TagLib.Image.File file) { }

		/// <summary>
		///    No modifications.
		/// </summary>
		public void ModifyMetadata (TagLib.Image.File file) { }

		/// <summary>
		///    No changes.
		/// </summary>
		public void ValidatePostModification (TagLib.Image.File file) { }
	}
}
