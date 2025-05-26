using NUnit.Framework;
using TagLib;

using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats
{
	[TestFixture]
	public class Id3V24FormatTest : IFormatTest
	{
		readonly string sample_file = TestPath.Samples + "sample_v2_4_unsynch.mp3";
		readonly string tmp_file = TestPath.Samples + "tmpwrite_v2_4_unsynch.mp3";
		File file;

		[OneTimeSetUp]
		public void Init ()
		{
			file = File.Create (sample_file);
		}

		[Test]
		public void ReadAudioProperties ()
		{
			ClassicAssert.AreEqual (44100, file.Properties.AudioSampleRate);
			ClassicAssert.AreEqual (1, file.Properties.Duration.Seconds);
		}

		[Test]
		public void ReadTags ()
		{
			ClassicAssert.AreEqual ("MP3 album", file.Tag.Album);
			ClassicAssert.IsTrue (file.Tag.Comment.StartsWith ("MP3 comment"));
			CollectionAssert.AreEqual (file.Tag.Genres, new[] { "Acid Punk" });
			CollectionAssert.AreEqual (file.Tag.Performers, new[] {
				"MP3 artist unicode (\u1283\u12ed\u120c \u1308\u1265\u1228\u1225\u120b\u1234)" });
			CollectionAssert.AreEqual (file.Tag.Composers, new[] { "MP3 composer" });
			ClassicAssert.AreEqual ("MP3 title unicode (\u12a2\u1275\u12ee\u1335\u12eb)", file.Tag.Title);
			ClassicAssert.AreEqual (6, file.Tag.Track);
			ClassicAssert.AreEqual (7, file.Tag.TrackCount);
			ClassicAssert.AreEqual (1234, file.Tag.Year);
		}

		[Test]
		public void WriteStandardTags ()
		{
			StandardTests.WriteStandardTags (sample_file, tmp_file);
		}

		[Test]
		public void WriteStandardPictures ()
		{
			StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.None);
		}

		[Test]
		public void WriteStandardPicturesLazy ()
		{
			StandardTests.WriteStandardPictures (sample_file, tmp_file, ReadStyle.PictureLazy);
		}

		public void TestCorruptionResistance ()
		{
		}

		[Test]
		public void ReplayGainTest ()
		{
			string inFile = TestPath.Samples + "sample_replaygain.mp3";
			string tempFile = TestPath.Samples + "tmpwrite_sample_replaygain.mp3";

			var rgFile = File.Create (inFile);
			ClassicAssert.AreEqual (2.22d, rgFile.Tag.ReplayGainTrackGain);
			ClassicAssert.AreEqual (0.418785d, rgFile.Tag.ReplayGainTrackPeak);
			ClassicAssert.AreEqual (2.32d, rgFile.Tag.ReplayGainAlbumGain);
			ClassicAssert.AreEqual (0.518785d, rgFile.Tag.ReplayGainAlbumPeak);
			rgFile.Dispose ();

			System.IO.File.Copy (inFile, tempFile, true);

			rgFile = File.Create (tempFile);
			rgFile.Tag.ReplayGainTrackGain = -1;
			rgFile.Tag.ReplayGainTrackPeak = 1;
			rgFile.Tag.ReplayGainAlbumGain = 2;
			rgFile.Tag.ReplayGainAlbumPeak = 0;
			rgFile.Save ();
			rgFile.Dispose ();

			rgFile = File.Create (tempFile);
			ClassicAssert.AreEqual (-1d, rgFile.Tag.ReplayGainTrackGain);
			ClassicAssert.AreEqual (1d, rgFile.Tag.ReplayGainTrackPeak);
			ClassicAssert.AreEqual (2d, rgFile.Tag.ReplayGainAlbumGain);
			ClassicAssert.AreEqual (0d, rgFile.Tag.ReplayGainAlbumPeak);
			rgFile.Tag.ReplayGainTrackGain = double.NaN;
			rgFile.Tag.ReplayGainTrackPeak = double.NaN;
			rgFile.Tag.ReplayGainAlbumGain = double.NaN;
			rgFile.Tag.ReplayGainAlbumPeak = double.NaN;
			rgFile.Save ();
			rgFile.Dispose ();

			rgFile = File.Create (tempFile);
			ClassicAssert.AreEqual (double.NaN, rgFile.Tag.ReplayGainTrackGain);
			ClassicAssert.AreEqual (double.NaN, rgFile.Tag.ReplayGainTrackPeak);
			ClassicAssert.AreEqual (double.NaN, rgFile.Tag.ReplayGainAlbumGain);
			ClassicAssert.AreEqual (double.NaN, rgFile.Tag.ReplayGainAlbumPeak);
			rgFile.Dispose ();

			System.IO.File.Delete (tempFile);
		}

		[Test]
		public void URLLinkFrameTest ()
		{
			string tempFile = TestPath.Samples + "tmpwrite_urllink_v2_4_unsynch.mp3";

			System.IO.File.Copy (sample_file, tempFile, true);

			var urlLinkFile = File.Create (tempFile);
			var id3v2tag = urlLinkFile.GetTag (TagTypes.Id3v2) as TagLib.Id3v2.Tag;
			id3v2tag.SetTextFrame ("WCOM", "www.commercial.com");
			id3v2tag.SetTextFrame ("WCOP", "www.copyright.com");
			id3v2tag.SetTextFrame ("WOAF", "www.official-audio.com");
			id3v2tag.SetTextFrame ("WOAR", "www.official-artist.com");
			id3v2tag.SetTextFrame ("WOAS", "www.official-audio-source.com");
			id3v2tag.SetTextFrame ("WORS", "www.official-internet-radio.com");
			id3v2tag.SetTextFrame ("WPAY", "www.payment.com");
			id3v2tag.SetTextFrame ("WPUB", "www.official-publisher.com");
			urlLinkFile.Save ();
			urlLinkFile.Dispose ();

			urlLinkFile = File.Create (tempFile);
			id3v2tag = urlLinkFile.GetTag (TagTypes.Id3v2) as TagLib.Id3v2.Tag;
			ClassicAssert.AreEqual ("www.commercial.com", id3v2tag.GetTextAsString ("WCOM"));
			ClassicAssert.AreEqual ("www.copyright.com", id3v2tag.GetTextAsString ("WCOP"));
			ClassicAssert.AreEqual ("www.official-audio.com", id3v2tag.GetTextAsString ("WOAF"));
			ClassicAssert.AreEqual ("www.official-artist.com", id3v2tag.GetTextAsString ("WOAR"));
			ClassicAssert.AreEqual ("www.official-audio-source.com", id3v2tag.GetTextAsString ("WOAS"));
			ClassicAssert.AreEqual ("www.official-internet-radio.com", id3v2tag.GetTextAsString ("WORS"));
			ClassicAssert.AreEqual ("www.payment.com", id3v2tag.GetTextAsString ("WPAY"));
			ClassicAssert.AreEqual ("www.official-publisher.com", id3v2tag.GetTextAsString ("WPUB"));
			urlLinkFile.Dispose ();

			System.IO.File.Delete (tempFile);
		}
	}
}
