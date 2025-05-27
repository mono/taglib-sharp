//
//  IptcTest.cs
//
//  Author:
//       Eberhard Beilharz <eb1@sil.org>
//
//  Copyright (c) 2012 Eberhard Beilharz
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

using TagLib.Xmp;

using File = TagLib.File;

namespace TaglibSharp.Tests.Images
{
	[TestClass]
	public class IptcIimTest
	{
		[TestMethod]
		public void Iim_Keywords ()
		{
			var file = File.Create (TestPath.Samples + "sample_iptc1.jpg");
			var tag = file.GetTag (TagTypes.XMP) as XmpTag;

			Assert.IsNotNull (tag, "tag");

			CollectionAssert.AreEqual (new[] { "kw1", "kw2", "kw3 " }, tag.Keywords);
		}

		[TestMethod]
		public void Iim_AllInfo ()
		{
			var file = File.Create (TestPath.Samples + "sample_iptc2.jpg");
			var tag = file.GetTag (TagTypes.XMP) as XmpTag;

			Assert.IsNotNull (tag, "tag");

			CollectionAssert.AreEqual (new[] { "kw" }, tag.Keywords);
			Assert.AreEqual ("Title", tag.Title);
			Assert.AreEqual ("Creator", tag.Creator);
			Assert.AreEqual ("Copyright", tag.Copyright);
		}

		[TestMethod]
		public void IimAndXmp ()
		{
			var file = File.Create (TestPath.Samples + "sample_iptc3.jpg");
			var tag = file.GetTag (TagTypes.XMP) as XmpTag;

			Assert.IsNotNull (tag, "tag");

			CollectionAssert.AreEqual (new[] { "XmpKw" }, tag.Keywords);
			Assert.AreEqual ("XmpTitle", tag.Title);
			Assert.AreEqual ("XmpCreator", tag.Creator);
			Assert.AreEqual ("XmpCopyright", tag.Copyright);
		}
	}
}
