using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.FormatTests
{   
    [TestFixture]
    public class AsfFormatTest : IFormatTest
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
            Assert.AreEqual(44100, file.Properties.AudioSampleRate);
            Assert.AreEqual(5, file.Properties.Duration.Seconds);
        }
        
        [Test]
        public void ReadTags()
        {
            Assert.AreEqual("WMA album", file.Tag.Album);
            Assert.AreEqual("Dan Drake", file.Tag.FirstAlbumArtist);
            Assert.AreEqual("WMA artist", file.Tag.FirstPerformer);
            Assert.AreEqual("WMA comment", file.Tag.Comment);
            Assert.AreEqual("Acid Punk", file.Tag.FirstGenre);
            Assert.AreEqual("WMA title", file.Tag.Title);
            Assert.AreEqual(5, file.Tag.Track);
            Assert.AreEqual(2005, file.Tag.Year);
        }
         
        [Test]
        public void TestCorruptionResistance()
        {
            try {
                File.Create("samples/corrupt/a.wma");
            } catch(CorruptFileException) {
            } catch(NullReferenceException e) {
                throw e;
            } catch {
            }
        }
    }
}
