using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.Tests.FileFormats
{   
    public static class StandardTests
    {
        public static void ReadAudioProperties (File file)
        {
            Assert.AreEqual(44100, file.Properties.AudioSampleRate);
            Assert.AreEqual(5, file.Properties.Duration.Seconds);
        }
        
        public static void WriteStandardTags (string sample_file, string tmp_file)
        {
            if (System.IO.File.Exists (tmp_file))
                System.IO.File.Delete (tmp_file);
            
            try {
                System.IO.File.Copy(sample_file, tmp_file);
                
                File tmp = File.Create (tmp_file);
                SetTags (tmp.Tag);
                tmp.Save ();
                
                tmp = File.Create (tmp_file);
                CheckTags (tmp.Tag);
            } finally {
//                if (System.IO.File.Exists (tmp_file))
//                    System.IO.File.Delete (tmp_file);
            }
        }
        
        public static void SetTags (Tag tag)
        {
            tag.Album = "TEST album";
            tag.AlbumArtists = new string [] {"TEST artist 1", "TEST artist 2"};
            tag.BeatsPerMinute = 120;
            tag.Comment = "TEST comment";
            tag.Composers = new string [] {"TEST composer 1", "TEST composer 2"};
            tag.Conductor = "TEST conductor";
            tag.Copyright = "TEST copyright";
            tag.Disc = 100;
            tag.DiscCount = 101;
            tag.Genres = new string [] {"TEST genre 1", "TEST genre 2"};
            tag.Grouping = "TEST grouping";
            tag.Lyrics = "TEST lyrics 1\r\nTEST lyrics 2";
            tag.Performers = new string [] {"TEST performer 1", "TEST performer 2"};
            tag.Title = "TEST title";
            tag.Track = 98;
            tag.TrackCount = 99;
            tag.Year = 1999;
        }
        
        public static void CheckTags (Tag tag)
        {
            Assert.AreEqual ("TEST album", tag.Album);
            Assert.AreEqual ("TEST artist 1; TEST artist 2", tag.JoinedAlbumArtists);
            Assert.AreEqual (120, tag.BeatsPerMinute);
            Assert.AreEqual ("TEST comment", tag.Comment);
            Assert.AreEqual ("TEST composer 1; TEST composer 2", tag.JoinedComposers);
            Assert.AreEqual ("TEST conductor", tag.Conductor);
            Assert.AreEqual ("TEST copyright", tag.Copyright);
            Assert.AreEqual (100, tag.Disc);
            Assert.AreEqual (101, tag.DiscCount);
            Assert.AreEqual ("TEST genre 1; TEST genre 2", tag.JoinedGenres);
            Assert.AreEqual ("TEST grouping", tag.Grouping);
            Assert.AreEqual ("TEST lyrics 1\r\nTEST lyrics 2", tag.Lyrics);
            Assert.AreEqual ("TEST performer 1; TEST performer 2", tag.JoinedPerformers);
            Assert.AreEqual ("TEST title", tag.Title);
            Assert.AreEqual (98, tag.Track);
            Assert.AreEqual (99, tag.TrackCount);
            Assert.AreEqual (1999, tag.Year);
        }
        
        public static void TestCorruptionResistance (string path)
        {
            try {
                File.Create (path);
            } catch(CorruptFileException) {
            } catch(NullReferenceException e) {
                throw e;
            } catch {
            }
        }
    }
}
