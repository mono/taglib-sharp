//
//  JpegSony1Test.cs
//
//  Author:
//       Paul Lange (palango@gmx.de)
//
//  Copyright (c) 2009 Paul Lange
//
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using NUnit.Framework;
using System;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using File = TagLib.File;

namespace TaglibSharp.Tests.Images
{
	[TestFixture]
	public class JpegSony1Test
	{
		static readonly string sample_file = TestPath.Samples + "sample_sony1.jpg";
		static readonly string tmp_file = TestPath.Samples + "tmpwrite_sony1.jpg";

		readonly TagTypes contained_types = TagTypes.TiffIFD;

		File file;

		[OneTimeSetUp]
		public void Init ()
		{
			file = File.Create (sample_file);
		}

		[Test]
		public void JpegRead ()
		{
			CheckTags (file);
		}

		[Test]
		public void ExifRead ()
		{
			CheckExif (file);
		}

		[Test]
		public void MakernoteRead ()
		{
			CheckMakerNote (file);
		}

		[Test]
		public void Rewrite ()
		{
			var tmp = Utils.CreateTmpFile (sample_file, tmp_file);
			tmp.Save ();

			tmp = File.Create (tmp_file);

			CheckTags (tmp);
			CheckExif (tmp);
			CheckMakerNote (tmp);
			CheckProperties (tmp);
		}

		[Test]
		public void AddExif ()
		{
			AddImageMetadataTests.AddExifTest (sample_file, tmp_file, true);
		}

		[Test]
		public void AddGPS ()
		{
			AddImageMetadataTests.AddGPSTest (sample_file, tmp_file, true);
		}

		[Test]
		public void AddXMP1 ()
		{
			AddImageMetadataTests.AddXMPTest1 (sample_file, tmp_file, false);
		}

		[Test]
		public void AddXMP2 ()
		{
			AddImageMetadataTests.AddXMPTest2 (sample_file, tmp_file, false);
		}

		public void CheckTags (File file)
		{
			ClassicAssert.IsTrue (file is TagLib.Jpeg.File, "not a Jpeg file");

			ClassicAssert.AreEqual (contained_types, file.TagTypes);
			ClassicAssert.AreEqual (contained_types, file.TagTypesOnDisk);
		}

		public void CheckExif (File file)
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;

			ClassicAssert.IsNotNull (tag, "tag");

			var exif_ifd = tag.Structure.GetEntry (0, IFDEntryTag.ExifIFD) as SubIFDEntry;
			ClassicAssert.IsNotNull (exif_ifd, "Exif IFD");

			ClassicAssert.AreEqual ("SONY ", tag.Make);
			ClassicAssert.AreEqual ("DSLR-A200", tag.Model);
			ClassicAssert.AreEqual (400, tag.ISOSpeedRatings, "ISOSpeedRatings");
			ClassicAssert.AreEqual (1.0d / 60.0d, tag.ExposureTime);
			ClassicAssert.AreEqual (5.6d, tag.FNumber);
			ClassicAssert.AreEqual (35.0d, tag.FocalLength);
			ClassicAssert.AreEqual (52, tag.FocalLengthIn35mmFilm);
			ClassicAssert.AreEqual (new DateTime (2009, 11, 21, 12, 39, 39), tag.DateTime);
			ClassicAssert.AreEqual (new DateTime (2009, 11, 21, 12, 39, 39), tag.DateTimeDigitized);
			ClassicAssert.AreEqual (new DateTime (2009, 11, 21, 12, 39, 39), tag.DateTimeOriginal);
			ClassicAssert.AreEqual (TagLib.Image.ImageOrientation.TopLeft, tag.Orientation);
		}

		public void CheckMakerNote (File file)
		{
			var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;
			ClassicAssert.IsNotNull (tag, "tag");

			var makernote_ifd =
				tag.ExifIFD.GetEntry (0, (ushort)ExifEntryTag.MakerNote) as MakernoteIFDEntry;

			ClassicAssert.IsNotNull (makernote_ifd, "makernote ifd");
			ClassicAssert.AreEqual (MakernoteType.Sony, makernote_ifd.MakernoteType);

			var structure = makernote_ifd.Structure;
			ClassicAssert.IsNotNull (structure, "structure");
			//Tag info from http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/Sony.html
			//0x0102: image quality
			{
				var entry = structure.GetEntry (0, 0x0102) as LongIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x0102");
				ClassicAssert.AreEqual (2, entry.Value);
			}
			//0x0115: white balance
			{
				var entry = structure.GetEntry (0, 0x0115) as LongIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0x0115");
				ClassicAssert.AreEqual (0, entry.Value);
			}
			//0xb026: image stabilizer
			{
				var entry = structure.GetEntry (0, 0xb026) as LongIFDEntry;
				ClassicAssert.IsNotNull (entry, "entry 0xb026");
				ClassicAssert.AreEqual (0, entry.Value);
			}
		}

		public void CheckProperties (File file)
		{
			ClassicAssert.AreEqual (3872, file.Properties.PhotoWidth);
			ClassicAssert.AreEqual (2592, file.Properties.PhotoHeight);
			ClassicAssert.AreEqual (95, file.Properties.PhotoQuality);
		}
	}
}
