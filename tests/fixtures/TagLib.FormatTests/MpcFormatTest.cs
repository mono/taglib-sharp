using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.FormatTests
{   
    [TestFixture]
    public class MpcFormatTest : IFormatTest
    {
        private File file;
        
        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create("samples/sample.mpc");
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
            Assert.AreEqual("MPC album", file.Tag.Album);
            Assert.AreEqual("MPC artist", file.Tag.FirstPerformer);
            Assert.AreEqual("MPC comment", file.Tag.Comment);
            Assert.AreEqual("Acid Punk", file.Tag.FirstGenre);
            Assert.AreEqual("MPC title", file.Tag.Title);
            Assert.AreEqual(6, file.Tag.Track);
            Assert.AreEqual(7, file.Tag.TrackCount);
            Assert.AreEqual(1234, file.Tag.Year);
        }
                
        [Test]
        public void TestCorruptionResistance()
        {
            try {
                File.Create("samples/corrupt/a.mpc");
            } catch(CorruptFileException) {
            } catch(NullReferenceException e) {
                throw e;
            } catch {
            }
        }
    }
}
