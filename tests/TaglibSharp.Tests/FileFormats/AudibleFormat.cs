using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats
{
	[TestClass]
	public class AudibleFormatTest
	{
		static readonly string BaseDirectory = TestPath.Samples + "audible";

		[TestMethod]
		public void First ()
		{
			var tag = (TagLib.Audible.Tag)File.Create (Path.Combine (BaseDirectory, "first.aa")).Tag;
			Assert.AreEqual (tag.Album, "Glyn Hughes"); // This is probably wrong. The publisher is not the album
			Assert.AreEqual (tag.Author, "Ricky Gervais, Steve Merchant, & Karl Pilkington");
			Assert.AreEqual (tag.Copyright, "&#169;2009 Ricky Gervais; (P)2009 Ricky Gervais");
			Assert.IsTrue (tag.Description.StartsWith ("This is the second in a new series of definitive discourses exploring the diversity of human"));
			Assert.AreEqual (tag.Narrator, "Ricky Gervais, Steve Merchant, & Karl Pilkington");
			Assert.AreEqual (tag.Title, "The Ricky Gervais Guide to... NATURAL HISTORY (Unabridged)");
		}

		[TestMethod]
		[Ignore ("Not supported yet")]
		public void Second ()
		{
			var tag = (TagLib.Audible.Tag)File.Create (Path.Combine (BaseDirectory, "second.aax")).Tag;
			Assert.AreEqual (tag.Album, "Glyn Hughes"); // This is probably wrong. The publisher is not the album
			Assert.AreEqual (tag.Author, "Ricky Gervais, Steve Merchant, & Karl Pilkington");
			Assert.AreEqual (tag.Copyright, "&#169;2009 Ricky Gervais; (P)2009 Ricky Gervais");
			Assert.IsTrue (tag.Description.StartsWith ("This is the second in a new series of definitive discourses exploring the diversity of human"));
			Assert.AreEqual (tag.Narrator, "Ricky Gervais, Steve Merchant, & Karl Pilkington");
			Assert.AreEqual (tag.Title, "The Ricky Gervais Guide to... NATURAL HISTORY (Unabridged)");
		}

		[TestMethod]
		public void Third ()
		{
			var tag = (TagLib.Audible.Tag)File.Create (Path.Combine (BaseDirectory, "third.aa")).Tag;
			Assert.AreEqual (tag.Album, "Glyn Hughes"); // This is probably wrong. The publisher is not the album
			Assert.AreEqual (tag.Author, "Ricky Gervais, Steve Merchant, & Karl Pilkington");
			Assert.AreEqual (tag.Copyright, "&#169;2009 Ricky Gervais; (P)2009 Ricky Gervais");
			Assert.IsTrue (tag.Description.StartsWith ("This is the second in a new series of definitive discourses exploring the diversity of human"));
			Assert.AreEqual (tag.Narrator, "Ricky Gervais, Steve Merchant, & Karl Pilkington");
			Assert.AreEqual (tag.Title, "The Ricky Gervais Guide to... NATURAL HISTORY (Unabridged)");
		}

		[TestMethod]
		public void Fourth ()
		{
			var tag = (TagLib.Audible.Tag)File.Create (Path.Combine (BaseDirectory, "fourth.aa")).Tag;
			Assert.AreEqual (tag.Album, "Glyn Hughes"); // This is probably wrong. The publisher is not the album
			Assert.AreEqual (tag.Author, "Ricky Gervais, Steve Merchant & Karl Pilkington");
			Assert.AreEqual (tag.Copyright, "&#169;2010 Ricky Gervais; (P)2010 Ricky Gervais");
			Assert.IsTrue (tag.Description.StartsWith ("The ninth episode in this new series considers the human body, its form, function, and failings"));
			Assert.AreEqual (tag.Narrator, "Ricky Gervais, Steve Merchant & Karl Pilkington");
			Assert.AreEqual (tag.Title, "The Ricky Gervais Guide to... THE HUMAN BODY");
		}
	}
}
