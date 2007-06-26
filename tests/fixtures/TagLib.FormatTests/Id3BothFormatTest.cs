using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.FormatTests
{   
    [TestFixture]
    public class Id3BothFormatTest : IFormatTest
    {
        private File file;
        
        private class MultiplesNotSupportedException : ApplicationException
        {
            public MultiplesNotSupportedException() : 
                base("Multiples for all tags are not supported")
            {
            }
        }
        
        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create("samples/sample_both.mp3");
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
            Assert.AreEqual("MP3 album v2", file.Tag.Album);
            Assert.AreEqual("MP3 artist", file.Tag.FirstPerformer);
            Assert.AreEqual("MP3 comment v2", file.Tag.Comment);
            Assert.AreEqual("Acid Punk", file.Tag.FirstGenre);
            Assert.AreEqual("MP3 title v2", file.Tag.Title);
            Assert.AreEqual(6, file.Tag.Track);
            Assert.AreEqual(7, file.Tag.TrackCount);
            Assert.AreEqual(1234, file.Tag.Year);
        }
        
        [Test]
        public void FirstTag()
        {
            Assert.AreEqual("MP3 title v2", file.GetTag (TagLib.TagTypes.Id3v2).Title);
            Assert.AreEqual("MP3 album v2", file.GetTag (TagLib.TagTypes.Id3v2).Album);
            Assert.AreEqual("MP3 comment v2", file.GetTag (TagLib.TagTypes.Id3v2).Comment);
            Assert.AreEqual(1234, (int)file.GetTag (TagLib.TagTypes.Id3v2).Year);
            Assert.AreEqual(6, (int)file.GetTag (TagLib.TagTypes.Id3v2).Track);
            Assert.AreEqual(7, (int)file.GetTag (TagLib.TagTypes.Id3v2).TrackCount);
        }

        [Test]
        public void SecondTag()
        {
            Assert.AreEqual("MP3 title", file.GetTag (TagLib.TagTypes.Id3v1).Title);
            Assert.AreEqual("MP3 album", file.GetTag (TagLib.TagTypes.Id3v1).Album);
            Assert.AreEqual("MP3 comment", file.GetTag (TagLib.TagTypes.Id3v1).Comment);
            Assert.AreEqual("MP3 artist", file.GetTag (TagLib.TagTypes.Id3v1).FirstPerformer);
            Assert.AreEqual(1235, (int)file.GetTag (TagLib.TagTypes.Id3v1).Year);
            Assert.AreEqual(6, (int)file.GetTag (TagLib.TagTypes.Id3v1).Track);
            Assert.AreEqual(0, (int)file.GetTag (TagLib.TagTypes.Id3v1).TrackCount);
        }
        
        [Test]
        public void TestCorruptionResistance()
        {
            try {
                File.Create("samples/corrupt/a.mp3");
            } catch(NullReferenceException e) {
                throw e;
            } catch {
            }
        }
    }
}
