using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.FormatTests
{   
    [TestFixture]
    public class Id3V2FormatTest : IFormatTest
    {
        private File file;
        
        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create("samples/sample_v2_only.mp3");
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
        public void TestCorruptionResistance()
        {
        }
    }
}
