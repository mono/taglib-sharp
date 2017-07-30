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
        public void WriteMediumTags()
        {
            StandardTests.WriteStandardTags(sample_file, tmp_file, StandardTests.TestTagLevel.Medium);
        }


        [Test]
        public void SpecificTags()
        {
            if (System.IO.File.Exists(tmp_file))
                System.IO.File.Delete(tmp_file);
            File file = null;
            try
            {
                System.IO.File.Copy(sample_file, tmp_file);
                file = File.Create(tmp_file);
            }
            finally {}

            // Write Matroska-specific tags 
            Assert.NotNull(file);

            var mtag = (TagLib.Matroska.Tag)file.GetTag(TagLib.TagTypes.Matroska);
            Assert.NotNull(mtag);

            mtag.PerformersRole = new string[] { "TEST role 1", "TEST role 2" };
            mtag.Set("CHOREGRAPHER", null, "TEST choregrapher");

            // Retrieve Matroska 'Tags' structure
            var mtags = mtag.Tags;

            // Add a Matroska 'Tag' structure in the 'Tags' structure
            var album = new Matroska.Tag(mtags, 70);

            // Add a Matroska 'SimpleTag' (TagName: 'ARRANGER') in the 'Tag' structure
            album.Set("ARRANGER", null, "TEST arranger");

            // Add a Matroska 'SimpleTag' (TagName: 'TITLE') in the 'Tag' structure
            album.Set("TITLE", null, "TEST Album title"); // This should map to the standard Album tag


            file.Save();

            // Read back the Matroska-specific tags 
            file = File.Create(tmp_file);
            Assert.NotNull(mtag);

            mtag = (TagLib.Matroska.Tag)file.GetTag(TagLib.TagTypes.Matroska);
            Assert.NotNull(mtag);

            Assert.AreEqual("TEST role 1; TEST role 2", string.Join("; ", mtag.PerformersRole));
            Assert.AreEqual("TEST choregrapher", mtag.Get("CHOREGRAPHER", null)[0]);
            Assert.AreEqual("TEST arranger", mtags.Album.Get("ARRANGER", null)[0]);
            Assert.AreEqual("TEST Album title", mtag.Album);
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
