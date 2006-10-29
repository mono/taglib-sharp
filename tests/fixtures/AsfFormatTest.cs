using System;
using NUnit.Framework;

namespace TagLib
{   
    [TestFixture]
    public class AsfFormatTest
    {
        private File file;
        
        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create("samples/sample.wma");
        }
    
        [Test]
        public void ReadAudioProperties()
        {
            Assert.AreEqual(44100, file.AudioProperties.SampleRate);
            Assert.AreEqual(5, file.AudioProperties.Duration.Seconds);
        }
        
        [Test]
        public void ReadTags()
        {
            Assert.AreEqual("WMA album", file.Tag.Album);
            Assert.AreEqual("WMA artist", file.Tag.FirstArtist);
            Assert.AreEqual("WMA comment", file.Tag.Comment);
            Assert.AreEqual("Acid Punk", file.Tag.FirstGenre);
            Assert.AreEqual("WMA title", file.Tag.Title);
            Assert.AreEqual(5, file.Tag.Track);
            Assert.AreEqual(2005, file.Tag.Year);
        }
    }
}
