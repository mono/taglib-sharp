using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats;

[TestClass]
public class M4vFormatTest : IFormatTest
{
	readonly ReadOnlyByteVector BOXTYPE_LDES = "ldes"; // long description
	readonly ReadOnlyByteVector BOXTYPE_TVSH = "tvsh"; // TV Show or series
	readonly ReadOnlyByteVector BOXTYPE_PURD = "purd"; // purchase date

	const string LONG_DESC = "American comedy luminaries talk about the influence of Monty Python.";
	const string PURD_DATE = "2009-01-26 08:14:10";
	const string TV_SHOW = "Ask An Astronomer";

	readonly static string sample_file = TestPath.Samples + "sample.m4v";
	readonly static string tmp_file = TestPath.SamplesTmp + "tmpwrite.m4v";
	static File file;

	[ClassInitialize]
	public static void Init (TestContext testContext)
	{
		file = File.Create (sample_file);
	}

	[TestMethod]
	public void ReadAudioProperties ()
	{
		// Despite the method name, we're reading the video properties here
		Assert.AreEqual (632, file.Properties.VideoWidth);
		Assert.AreEqual (472, file.Properties.VideoHeight);
	}

	[TestMethod]
	public void ReadTags ()
	{
		bool gotLongDesc = false;
		bool gotPurdDate = false;

		Assert.AreEqual ("Will Yapp", file.Tag.FirstPerformer);
		Assert.AreEqual ("Why I Love Monty Python", file.Tag.Title);
		Assert.AreEqual (2008u, file.Tag.Year);

		// Test Apple tags
		var tag = (TagLib.Mpeg4.AppleTag)file.GetTag (TagTypes.Apple, false);
		Assert.IsNotNull (tag);

		foreach (var adbox in tag.DataBoxes (new[] { BOXTYPE_LDES })) {
			Assert.AreEqual (LONG_DESC, adbox.Text);
			gotLongDesc = true;
		}

		foreach (var adbox in tag.DataBoxes (new[] { BOXTYPE_PURD })) {
			Assert.AreEqual (PURD_DATE, adbox.Text);
			gotPurdDate = true;
		}

		Assert.IsTrue (gotLongDesc);
		Assert.IsTrue (gotPurdDate);
	}

	[TestMethod]
	public void WriteAppleTags ()
	{
		if (System.IO.File.Exists (tmp_file))
			System.IO.File.Delete (tmp_file);

		System.IO.File.Copy (sample_file, tmp_file);

		var tmp = File.Create (tmp_file);
		var tag = (TagLib.Mpeg4.AppleTag)tmp.GetTag (TagTypes.Apple, false);
		SetTags (tag);
		tmp.Save ();

		tmp = File.Create (tmp_file);
		tag = (TagLib.Mpeg4.AppleTag)tmp.GetTag (TagTypes.Apple, false);
		CheckTags (tag);
	}

	[TestMethod]
	[Ignore ("PictureLazy not supported yet")]
	public void WriteStandardPicturesLazy ()
	{
		StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy);
	}


	[TestMethod]
	public void TestCorruptionResistance ()
	{
	}

	void SetTags (TagLib.Mpeg4.AppleTag tag)
	{
		tag.Title = "TEST title";
		tag.Performers = new[] { "TEST performer 1", "TEST performer 2" };
		tag.Comment = "TEST comment";
		tag.Copyright = "TEST copyright";
		tag.Genres = new[] { "TEST genre 1", "TEST genre 2" };
		tag.Year = 1999;

		var atag = tag;
		Assert.IsNotNull (atag);

		var newbox1 = new TagLib.Mpeg4.AppleDataBox (
			ByteVector.FromString ("TEST Long Description", StringType.UTF8),
			(int)TagLib.Mpeg4.AppleDataBox.FlagType.ContainsText);
		var newbox2 = new TagLib.Mpeg4.AppleDataBox (
			ByteVector.FromString ("TEST TV Show", StringType.UTF8),
			(int)TagLib.Mpeg4.AppleDataBox.FlagType.ContainsText);
		atag.SetData (BOXTYPE_LDES, new[] { newbox1 });
		atag.SetData (BOXTYPE_TVSH, new[] { newbox2 });
	}

	void CheckTags (TagLib.Mpeg4.AppleTag tag)
	{
		Assert.AreEqual ("TEST title", tag.Title);
		Assert.AreEqual ("TEST performer 1; TEST performer 2", tag.JoinedPerformers);
		Assert.AreEqual ("TEST comment", tag.Comment);
		Assert.AreEqual ("TEST copyright", tag.Copyright);
		Assert.AreEqual ("TEST genre 1; TEST genre 2", tag.JoinedGenres);
		Assert.AreEqual (1999u, tag.Year);

		var atag = tag;
		Assert.IsNotNull (atag);

		foreach (var adbox in atag.DataBoxes (new[] { BOXTYPE_LDES })) {
			Assert.AreEqual ("TEST Long Description", adbox.Text);
		}

		foreach (var adbox in atag.DataBoxes (new[] { BOXTYPE_TVSH })) {
			Assert.AreEqual ("TEST TV Show", adbox.Text);
		}
	}
}
