using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.Tests.FileFormats
{   
    [TestFixture]
    public class FlacFormatTest : IFormatTest
    {
        private static string sample_file = "samples/sample.flac";
        private static string tmp_file = "samples/tmpwrite.flac";
        private File file;
        
        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create(sample_file);
        }
    
        [Test]
        public void ReadAudioProperties()
        {
            StandardTests.ReadAudioProperties (file);
        }
        
        [Test]
        public void ReadTags()
        {
            Assert.AreEqual("FLAC album", file.Tag.Album);
            Assert.AreEqual("FLAC artist", file.Tag.FirstPerformer);
            Assert.AreEqual("FLAC comment", file.Tag.Comment);
            Assert.AreEqual("Acid Punk", file.Tag.FirstGenre);
            Assert.AreEqual("FLAC title", file.Tag.Title);
            Assert.AreEqual(6, file.Tag.Track);
            Assert.AreEqual(7, file.Tag.TrackCount);
            Assert.AreEqual(1234, file.Tag.Year);
        }
        
        [Test]
        public void WriteStandardTags ()
        {
            StandardTests.WriteStandardTags (sample_file, tmp_file);
        }

        [Test]
        public void TestGetTagType ()
        {
            try {
                file.GetTag(TagTypes.Id3v2);
            } catch (System.NullReferenceException) {
                Assert.Fail ("Should not throw System.NullReferenceException calling file.GetTag method: http://bugzilla.gnome.org/show_bug.cgi?id=572380");
            }
        }
        
        [Test]
        public void TestCorruptionResistance()
        {
            StandardTests.TestCorruptionResistance ("samples/corrupt/a.flac");
        }
    }
}
