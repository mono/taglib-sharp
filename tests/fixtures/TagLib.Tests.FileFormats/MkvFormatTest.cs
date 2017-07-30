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
            Assert.AreEqual("Lime", file.Tag.FirstPerformer);
            Assert.AreEqual("no comments", file.Tag.Comment);
            Assert.AreEqual("Test", file.Tag.FirstGenre);
            Assert.AreEqual("Turning Lime", file.Tag.Title);
            Assert.AreEqual(2017, file.Tag.Year);
            Assert.AreEqual("Starwer", file.Tag.FirstComposer);
            Assert.AreEqual("Starwer", file.Tag.Conductor);
            Assert.AreEqual("Starwer 2017", file.Tag.Copyright);

            // Specific Matroska Tag test
            var mkvTag = (TagLib.Matroska.Tag)file.GetTag(TagTypes.Matroska);
            Assert.AreEqual("This is a test Video showing a lime moving on a table", mkvTag.SimpleTags["SUMMARY"][0]);
        }

        [Test]
        public void ReadPictures()
        {
            var pics = file.Tag.Pictures;
            Assert.AreEqual("cover.png", pics[0].Description);
            Assert.AreEqual(PictureType.FrontCover, pics[0].Type);
            Assert.AreEqual("image/png", pics[0].MimeType);
        }


        [Test]
        public void WriteStandardTags ()
        {
            StandardTests.WriteStandardTags (sample_file, tmp_file);
        }

        [Test]
        public void RemoveStandardTags()
        {
            StandardTests.RemoveStandardTags(sample_file, tmp_file);
        }

        [Test]
        public void TestCorruptionResistance()
        {
            StandardTests.TestCorruptionResistance (Debugger.Samples + "corrupt/a.mkv");
        }
    }
}
