namespace TaglibSharp.Tests.Images.Validators;

/// <summary>
///    This class tests the modification of the Keywords field,
///    regardless of which metadata format is used.
/// </summary>
public class KeywordsModificationValidator : IMetadataModificationValidator
{
	readonly string[] orig_keywords;
	readonly string[] test_keywords = { "keyword 1", "§$&§%", "99 dsf", "ഈ ヰᛥกツ" };

	public KeywordsModificationValidator () : this (new string[] { }) { }

	public KeywordsModificationValidator (string[] orig_keywords)
	{
		this.orig_keywords = orig_keywords;
	}

	/// <summary>
	///    Check if the original keywords are found.
	/// </summary>
	public virtual void ValidatePreModification (TagLib.Image.File file)
	{
		CollectionAssert.AreEqual (orig_keywords, GetTag (file).Keywords);
	}

	/// <summary>
	///    Changes the keywords.
	/// </summary>
	public virtual void ModifyMetadata (TagLib.Image.File file)
	{
		GetTag (file).Keywords = test_keywords;
	}

	/// <summary>
	///    Validates if changes survived a write.
	/// </summary>
	public void ValidatePostModification (TagLib.Image.File file)
	{
		Assert.IsNotNull (file.GetTag (TagTypes.XMP, false));
		CollectionAssert.AreEqual (test_keywords, GetTag (file).Keywords);
	}

	/// <summary>
	///    Returns the tag that should be tested. Default
	///    behavior is no specific tag.
	/// </summary>
	public virtual TagLib.Image.ImageTag GetTag (TagLib.Image.File file)
	{
		return file.ImageTag;
	}
}
