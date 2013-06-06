using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

using TagLib;
using TagLib.Mpeg4;

namespace TagLib.Tests.FileFormats
{   
    [TestFixture]
    public class M4aFormatTest : IFormatTest
    {
        class Mpeg4TestFile : TagLib.Mpeg4.File
        {
            public Mpeg4TestFile (string path)
                :base (path)
            {

            }

            public new List<IsoUserDataBox>  UdtaBoxes {
                get { return base.UdtaBoxes; }
            }
        }

        private static string sample_file = "samples/sample.m4a";
        private static string tmp_file = "samples/tmpwrite.m4a";
        private static string aac_broken_tags = "samples/bgo_658920.m4a";
        private File file;
        
        [TestFixtureSetUp]
        public void Init()
        {
            file = File.Create(sample_file);
        }

		[Test]
		public void AppleTags_MoreTests ()
		{
			// This tests that a 'text' atom inside an 'stsd' atom is parsed correctly
			// We just ensure that this does not throw an exception. I don't know how to
			// verify the content is correct.
			File.Create ("samples/apple_tags.m4a");
		}

		
		[Test]
		public void bgo_676934 ()
		{
			// This file contains an atom which says its 800MB in size
			var file = File.Create ("samples/bgo_676934.m4a");
			Assert.IsTrue (file.CorruptionReasons.Any (), "#1");
		}


		[Test]
		public void bgo_701689 ()
		{
			// This file contains a musicbrainz track id "883821fc-9bbc-4e04-be79-b4b12c4c4a4e"
			// This case also handles bgo #701690 as a proper value for the tag must be returned
			var file = File.Create ("samples/bgo_701689.m4a");
			Assert.AreEqual ("883821fc-9bbc-4e04-be79-b4b12c4c4a4e", file.Tag.MusicBrainzTrackId, "#1");
		}

        [Test]
        public void ReadAppleAacTags ()
        {
            var file = new Mpeg4TestFile (aac_broken_tags);
            Assert.AreEqual (2, file.UdtaBoxes.Count, "#1");

            var first = file.UdtaBoxes [0];
            Assert.AreEqual (1, first.Children.Count (), "#2");

            Assert.IsInstanceOfType (typeof (AppleAdditionalInfoBox), first.Children.First ());
            var child = (AppleAdditionalInfoBox) first.Children.First ();
            Assert.AreEqual ((ReadOnlyByteVector)"name", child.BoxType, "#3");
            Assert.AreEqual (0 , child.Data.Count, "#4");
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
