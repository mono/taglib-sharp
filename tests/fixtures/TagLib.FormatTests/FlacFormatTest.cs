using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.FormatTests
{   
    [TestFixture]
    public class FlacFormatTest : IFormatTest
    {
        private File file;
        
        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create("samples/sample.flac");
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
        public void TestCorruptionResistance()
        {
            try {
                File.Create("samples/corrupt/a.flac");
            } catch(CorruptFileException) {
            } catch(NullReferenceException e) {
                throw e;
            } catch {
            }
        }
    }
}
