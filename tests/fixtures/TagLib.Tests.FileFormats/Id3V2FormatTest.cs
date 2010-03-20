using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.Tests.FileFormats
{   
    [TestFixture]
    public class Id3V2FormatTest : IFormatTest
    {
        private static string sample_file = "samples/sample_v2_only.mp3";
        private static string corrupt_file = "samples/corrupt/null_title_v2.mp3";
        private static string tmp_file = "samples/tmpwrite_v2_only.mp3";
        private static string ext_header_file = "samples/sample_v2_3_ext_header.mp3";
        private File file;
        
        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create(sample_file);
        }
    
        [Test]
        public void ReadAudioProperties()
        {
            Assert.AreEqual(44100, file.Properties.AudioSampleRate);
            Assert.AreEqual(1, file.Properties.Duration.Seconds);
        }
        
        [Test]
        public void ReadTags()
        {
            Assert.AreEqual("MP3 album", file.Tag.Album);
            Assert.AreEqual("MP3 artist", file.Tag.FirstPerformer);
            Assert.AreEqual("MP3 comment", file.Tag.Comment);
            Assert.AreEqual("Acid Punk", file.Tag.FirstGenre);
            Assert.AreEqual("MP3 title", file.Tag.Title);
            Assert.AreEqual(6, file.Tag.Track);
            Assert.AreEqual(7, file.Tag.TrackCount);
            Assert.AreEqual(1234, file.Tag.Year);
        }
        
        [Test]
        public void WriteStandardTags ()
        {
            StandardTests.WriteStandardTags (sample_file, tmp_file);
        }
        
        [Test] // http://bugzilla.gnome.org/show_bug.cgi?id=558123
        public void TestTruncateOnNull ()
        {
            if (System.IO.File.Exists (tmp_file)) {
                System.IO.File.Delete (tmp_file);
            }
            
            System.IO.File.Copy (corrupt_file, tmp_file);
            File tmp = File.Create (tmp_file);
            
            Assert.AreEqual ("T", tmp.Tag.Title);
        }
        
        [Test]
        public void TestCorruptionResistance()
        {
        }

        [Test]
        public void TestExtendedHeaderSize()
        {
            // bgo#604488
            var file = File.Create (ext_header_file);
            Assert.AreEqual ("Title v2", file.Tag.Title);
        }
    }
}
