using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.FormatTests
{   
    [TestFixture]
    public class Id3BothFormatTest
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
            Assert.AreEqual(44100, file.AudioProperties.SampleRate);
            Assert.AreEqual(5, file.AudioProperties.Duration.Seconds);
        }
        
        [Test]
        public void ReadTags()
        {
            Assert.AreEqual("MP3 album v2", file.Tag.Album);
            Assert.AreEqual("MP3 artist", file.Tag.FirstArtist);
            Assert.AreEqual("MP3 comment v2", file.Tag.Comment);
            Assert.AreEqual("Acid Punk", file.Tag.FirstGenre);
            Assert.AreEqual("MP3 title v2", file.Tag.Title);
            Assert.AreEqual(6, file.Tag.Track);
            Assert.AreEqual(7, file.Tag.TrackCount);
            Assert.AreEqual(1234, file.Tag.Year);
        }
        
        [Test]
        [Ignore("Multiple tags not supported")]
        public void TagCounts()
        {
            // this test fails on purpose as we do not have support
            // multiples for any tag, which is possible with combined
            // with ID3v1 and ID3v2 tags for example - entagged-sharp
            // supports these cases and we should too
            
            //Assert.AreEqual(2, file.Tag.Albums.Length);
            //Assert.AreEqual(2, file.Tag.Comments.Length);
            //Assert.AreEqual(2, file.Tag.TrackNumbers.Length);
            //Assert.AreEqual(1, file.Tag.TrackCounts.Length);
            //Assert.AreEqual(2, file.Tag.Years.Length);
            
            //Assert.AreEqual(1, file.Tag.Artists.Length);
            //Assert.AreEqual(2, file.Tag.Genres.Length);
        }
        
        [Test]
        [Ignore("Multiple tags not supported")]
        public void FirstTag()
        {
            // Again, we don't support multiples
            
            //Assert.AreEqual("MP3 title v2", file.Tag.Titles[0] as string);
            //Assert.AreEqual("MP3 album v2", file.Tag.Albums[0] as string);
            //Assert.AreEqual("MP3 comment v2", file.Tag.Comments[0] as string);
            //Assert.AreEqual(1234, (int)file.Tag.Years[0]);
            //Assert.AreEqual(6, (int)file.Tag.Tracks[0]);
            //Assert.AreEqual(7, (int)file.Tag.TrackCounts[0]);
        }

        [Test]
        [Ignore("Multiple tags not supported")]
        public void SecondTag()
        {
            // Again, we don't support multiples
            
            //Assert.AreEqual("MP3 title", file.Tag.Titles[1] as string);
            //Assert.AreEqual("MP3 album", file.Tag.Albums[1] as string);
            //Assert.AreEqual("MP3 comment", file.Tag.Comments[1] as string);
            Assert.AreEqual("MP3 artist", file.Tag.Artists[0] as string);
            //Assert.AreEqual(1235, (int)file.Tag.Years[1]);
            //Assert.AreEqual(6, (int)file.Tag.Tracks[1]);
            //Assert.AreEqual(7, (int)file.Tag.TrackCounts[0]);
        }
    }
}
