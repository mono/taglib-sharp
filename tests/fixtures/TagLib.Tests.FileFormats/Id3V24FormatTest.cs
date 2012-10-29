using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.Tests.FileFormats
{
    [TestFixture]
    public class Id3V24FormatTest : IFormatTest
    {
        private const string sample24unsynchronization_file = "samples/sample_v2_4_unsynch.mp3";
        private const string tmp_file = "samples/tmpwrite_v2_4_unsynch.mp3";
        private File file;

        [TestFixtureSetUp]
        public void Init ()
        {
            file = File.Create (sample24unsynchronization_file);
        }

        [Test]
        public void ReadAudioProperties ()
        {
            Assert.AreEqual (44100, file.Properties.AudioSampleRate);
            Assert.AreEqual (1, file.Properties.Duration.Seconds);
        }

        [Test]
        public void ReadTags ()
        {
            Assert.AreEqual ("MP3 album", file.Tag.Album);
            Assert.IsTrue (file.Tag.Comment.StartsWith ("MP3 comment"));
            CollectionAssert.AreEqual (file.Tag.Genres, new [] { "Acid Punk" });
            CollectionAssert.AreEqual (file.Tag.Performers, new [] {
                "MP3 artist unicode (\u1283\u12ed\u120c \u1308\u1265\u1228\u1225\u120b\u1234)" });
            CollectionAssert.AreEqual (file.Tag.Composers, new [] { "MP3 composer" });
            Assert.AreEqual ("MP3 title unicode (\u12a2\u1275\u12ee\u1335\u12eb)", file.Tag.Title);
            Assert.AreEqual (6, file.Tag.Track);
            Assert.AreEqual (7, file.Tag.TrackCount);
            Assert.AreEqual (1234, file.Tag.Year);
        }

        [Test]
        public void WriteStandardTags ()
        {
            StandardTests.WriteStandardTags (sample24unsynchronization_file, tmp_file);
        }

        public void TestCorruptionResistance ()
        {
        }

        [Test]
        public void ReplayGainTest()
        {
            string inFile = "samples/sample_replaygain.mp3";
            string tempFile = "samples/tmpwrite_sample_replaygain.mp3";

            File rgFile = File.Create(inFile);
            Assert.AreEqual(2.22d, rgFile.Tag.ReplayGainTrackGain);
            Assert.AreEqual(0.418785d, rgFile.Tag.ReplayGainTrackPeak);
            Assert.AreEqual(2.32d, rgFile.Tag.ReplayGainAlbumGain);
            Assert.AreEqual(0.518785d, rgFile.Tag.ReplayGainAlbumPeak);
            rgFile.Dispose();

            System.IO.File.Copy(inFile, tempFile, true);

            rgFile = File.Create(tempFile);
            rgFile.Tag.ReplayGainTrackGain = -1;
            rgFile.Tag.ReplayGainTrackPeak = 1;
            rgFile.Tag.ReplayGainAlbumGain = 2;
            rgFile.Tag.ReplayGainAlbumPeak = 0;
            rgFile.Save();
            rgFile.Dispose();

            rgFile = File.Create(tempFile);
            Assert.AreEqual(-1d, rgFile.Tag.ReplayGainTrackGain);
            Assert.AreEqual(1d, rgFile.Tag.ReplayGainTrackPeak);
            Assert.AreEqual(2d, rgFile.Tag.ReplayGainAlbumGain);
            Assert.AreEqual(0d, rgFile.Tag.ReplayGainAlbumPeak);
            rgFile.Tag.ReplayGainTrackGain = double.NaN;
            rgFile.Tag.ReplayGainTrackPeak = double.NaN;
            rgFile.Tag.ReplayGainAlbumGain = double.NaN;
            rgFile.Tag.ReplayGainAlbumPeak = double.NaN;
            rgFile.Save();
            rgFile.Dispose();
            
            rgFile = File.Create(tempFile);
            Assert.AreEqual(double.NaN, rgFile.Tag.ReplayGainTrackGain);
            Assert.AreEqual(double.NaN, rgFile.Tag.ReplayGainTrackPeak);
            Assert.AreEqual(double.NaN, rgFile.Tag.ReplayGainAlbumGain);
            Assert.AreEqual(double.NaN, rgFile.Tag.ReplayGainAlbumPeak);
            rgFile.Dispose();

            System.IO.File.Delete(tempFile);
        }
    }
}
