using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.Tests.FileFormats
{   
    [TestFixture]
    public class MpcFormatTest : IFormatTest
    {
        private static string sample_file = "samples/sample.mpc";
        private static string tmp_file = "samples/tmpwrite.mpc";
        private File file;
        
        [OneTimeSetUp]
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
        public void WriteStandardTags ()
        {
            StandardTests.WriteStandardTags (sample_file, tmp_file);
        }
        
        [Test]
        public void TestCorruptionResistance()
        {
            StandardTests.TestCorruptionResistance ("samples/corrupt/a.mpc");
        }
    }
}
