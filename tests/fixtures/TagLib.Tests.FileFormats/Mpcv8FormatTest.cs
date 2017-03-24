using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.Tests.FileFormats
{   
    [TestFixture]
    public class MpcV8FormatTest : IFormatTest
    {
        private static string sample_file = "samples/sample_v8.mpc";
        private static string tmp_file = "samples/tmpwrite_v8.mpc";
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
            Assert.AreEqual("Mpc Album", file.Tag.Album);
            Assert.AreEqual("Mpc Artist", file.Tag.FirstPerformer);
            Assert.AreEqual("Mpc Comment", file.Tag.Comment);
            Assert.AreEqual("Pop", file.Tag.FirstGenre);
            Assert.AreEqual("Mpc Title", file.Tag.Title);
            Assert.AreEqual(1, file.Tag.Track);
            Assert.AreEqual(10, file.Tag.TrackCount);
            Assert.AreEqual(2016, file.Tag.Year);
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
