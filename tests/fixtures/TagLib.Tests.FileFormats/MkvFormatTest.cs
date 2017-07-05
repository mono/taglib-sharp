using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.Tests.FileFormats
{   
    [TestFixture]
    public class MkvFormatTest : IFormatTest
    {
        private static string sample_file = Debugger.Samples + "Turning Lime.mkv";
        private static string tmp_file = Debugger.Samples + "tmpwrite.mkv";
        private File file;
        
        [OneTimeSetUp]
        public void Init()
        {
            file = File.Create(sample_file);
        }
    
        [Test]
        public void ReadAudioProperties()
        {
            Assert.AreEqual(48000, file.Properties.AudioSampleRate);
            Assert.AreEqual(1, file.Properties.Duration.Seconds);
        }

        [Test]
        public void ReadTags()
        {
            Assert.AreEqual("Avi album", file.Tag.Album);
            Assert.AreEqual("Dan Drake", file.Tag.FirstAlbumArtist);
            Assert.AreEqual("AVI artist", file.Tag.FirstPerformer);
            Assert.AreEqual("AVI comment", file.Tag.Comment);
            Assert.AreEqual("Brit Pop", file.Tag.FirstGenre);
            Assert.AreEqual("AVI title", file.Tag.Title);
            Assert.AreEqual(5, file.Tag.Track);
            Assert.AreEqual(2005, file.Tag.Year);
        }

        [Test]
        public void ReadPictures()
        {
            var pics = file.Tag.Pictures;
            Assert.AreEqual("cover", pics[0].Description);
        }


        [Test]
        public void WriteStandardTags ()
        {
            StandardTests.WriteStandardTags (sample_file, tmp_file);
        }
        
        [Test]
        public void TestCorruptionResistance()
        {
            StandardTests.TestCorruptionResistance (Debugger.Samples + "corrupt/a.mkv");
        }
    }
}
