using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.Tests.FileFormats
{   
    [TestFixture]
    public class M4aFormatTest : IFormatTest
    {
        private static string sample_file = "samples/sample.m4a";
        private static string tmp_file = "samples/tmpwrite.m4a";
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
        public void WriteStandardTags ()
        {
            StandardTests.WriteStandardTags (sample_file, tmp_file);
        }
        
        [Test]
        public void TestCorruptionResistance()
        {
            StandardTests.TestCorruptionResistance ("samples/corrupt/a.m4a");
        }
    }
}
