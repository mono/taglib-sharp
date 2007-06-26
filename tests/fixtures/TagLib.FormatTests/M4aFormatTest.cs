using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.FormatTests
{   
    [TestFixture]
    public class M4aFormatTest : IFormatTest
    {
        private File file;
        
        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create("samples/sample.m4a");
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
            Assert.AreEqual("M4A album", file.Tag.Album);
            Assert.AreEqual("M4A artist", file.Tag.FirstPerformer);
            Assert.AreEqual("M4A comment", file.Tag.Comment);
            Assert.AreEqual("Acid Punk", file.Tag.FirstGenre);
            Assert.AreEqual("M4A title", file.Tag.Title);
            Assert.AreEqual(6, file.Tag.Track);
            //Assert.AreEqual(7, file.Tag.TrackCount);
            Assert.AreEqual(1234, file.Tag.Year);
        }       
           
        [Test]
        public void TestCorruptionResistance()
        {
            try {
                File.Create("samples/corrupt/a.m4a");
            } catch(CorruptFileException) {
            } catch(NullReferenceException e) {
                throw e;
            } catch {
            }
        }
    }
}
