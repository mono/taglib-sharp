using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.FormatTests
{   
    [TestFixture]
    public class OggFormatTest : IFormatTest
    {
        private File file;
        
        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create("samples/sample.ogg");
        }
    
        [Test]
        public void ReadAudioProperties()
        {
            Assert.AreEqual(44100, file.Properties.AudioSampleRate);
            Assert.AreEqual(5, file.Properties.Duration.Seconds);
        }
        
        [Test]
        public void ReadTags()
        {
            Assert.AreEqual("OGG album", file.Tag.Album);
            Assert.AreEqual("OGG artist", file.Tag.FirstPerformer);
            Assert.AreEqual("OGG comment", file.Tag.Comment);
            Assert.AreEqual("Acid Punk", file.Tag.FirstGenre);
            Assert.AreEqual("OGG title", file.Tag.Title);
            Assert.AreEqual(6, file.Tag.Track);
            Assert.AreEqual(7, file.Tag.TrackCount);
            Assert.AreEqual(1234, file.Tag.Year);
        }
        
        [Test]
        public void TestCorruptionResistance()
        {
            try {
                File.Create("samples/corrupt/a.ogg");
            } catch(CorruptFileException) {
            } catch(NullReferenceException e) {
                throw e;
            } catch {
            }
        }
    }
}
