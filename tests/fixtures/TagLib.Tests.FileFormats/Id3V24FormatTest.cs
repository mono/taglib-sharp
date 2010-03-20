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
    }
}
