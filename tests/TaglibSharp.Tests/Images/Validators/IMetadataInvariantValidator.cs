namespace TaglibSharp.Tests.Images.Validators
{
	public interface IMetadataInvariantValidator
	{
		/// <summary>
		///    Validate any metadata assumptions that should always
		///    hold (and thus never change upon modification).
		/// </summary>
		void ValidateMetadataInvariants (TagLib.Image.File file);
	}
}
