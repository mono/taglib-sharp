using System;
using NUnit.Framework;
using TagLib;
using TagLib.Xmp;

namespace TagLib.Tests.Images
{
	/// <summary>
	///    Older versions of XMP were called XAP. This test takes care of
	///    those files.
	/// </summary>
    [TestFixture]
    public class XapTest
    {
		private static string sample_file = "samples/sample_xap.jpg";

		[Test]
		public void ParseXmp ()
		{
			var file = File.Create (sample_file) as Image.File;
			Assert.IsNotNull (file, "file");

			XmpTag tag = file.GetTag (TagTypes.XMP) as XmpTag;
			Assert.IsNotNull (tag, "ImageTag");

			var PS_NS = "http://ns.adobe.com/photoshop/1.0/";
			var XAPMM_NS = XmpTag.XAP_NS + "mm/";
			var XAPRIGHTS_NS = XmpTag.XAP_NS + "rights/";

			TestNode (tag, PS_NS, "CaptionWriter", "Ian Britton", 0);
			TestNode (tag, PS_NS, "Headline", "Communications", 0);
			TestNode (tag, PS_NS, "AuthorsPosition", "Photographer", 0);
			TestNode (tag, PS_NS, "Credit", "Ian Britton", 0);
			TestNode (tag, PS_NS, "Source", "FreeFoto.com", 0);
			TestNode (tag, PS_NS, "City", "", 0);
			TestNode (tag, PS_NS, "State", "", 0);
			TestNode (tag, PS_NS, "Country", "Ubited Kingdom", 0);
			TestNode (tag, PS_NS, "Category", "BUS", 0);
			TestNode (tag, PS_NS, "DateCreated", "2002-06-20", 0);
			TestNode (tag, PS_NS, "Urgency", "5", 0);
			TestNode (tag, XAPMM_NS, "DocumentID", "adobe:docid:photoshop:84d4dba8-9b11-11d6-895d-c4d063a70fb0", 0);
			TestNode (tag, XAPRIGHTS_NS, "WebStatement", "www.freefoto.com", 0);
			TestNode (tag, XAPRIGHTS_NS, "Marked", "True", 0);

			{
				var node = tag.FindNode (PS_NS, "SupplementalCategories");
				Assert.IsFalse (node == null);
				Assert.AreEqual (XmpNodeType.Bag, node.Type);
				Assert.AreEqual ("", node.Value);
				Assert.AreEqual (1, node.Children.Count);
				Assert.AreEqual ("Communications", node.Children [0].Value);
			}

			{
				var node = tag.FindNode (XmpTag.DC_NS, "description");
				Assert.IsFalse (node == null);
				Assert.AreEqual (XmpNodeType.Alt, node.Type);
				Assert.AreEqual ("", node.Value);
				Assert.AreEqual (1, node.Children.Count);
				Assert.AreEqual ("Communications", node.Children [0].Value);
				Assert.AreEqual ("x-default", node.Children [0].GetQualifier (XmpTag.XML_NS, "lang").Value);
			}

			{
				var node = tag.FindNode (XmpTag.DC_NS, "creator");
				Assert.IsFalse (node == null);
				Assert.AreEqual (XmpNodeType.Seq, node.Type);
				Assert.AreEqual ("", node.Value);
				Assert.AreEqual (1, node.Children.Count);
				Assert.AreEqual ("Ian Britton", node.Children [0].Value);
			}

			{
				var node = tag.FindNode (XmpTag.DC_NS, "title");
				Assert.IsFalse (node == null);
				Assert.AreEqual (XmpNodeType.Alt, node.Type);
				Assert.AreEqual ("", node.Value);
				Assert.AreEqual (1, node.Children.Count);
				Assert.AreEqual ("Communications", node.Children [0].Value);
				Assert.AreEqual ("x-default", node.Children [0].GetQualifier (XmpTag.XML_NS, "lang").Value);
			}

			{
				var node = tag.FindNode (XmpTag.DC_NS, "rights");
				Assert.IsFalse (node == null);
				Assert.AreEqual (XmpNodeType.Alt, node.Type);
				Assert.AreEqual ("", node.Value);
				Assert.AreEqual (1, node.Children.Count);
				Assert.AreEqual ("ian Britton - FreeFoto.com", node.Children [0].Value);
				Assert.AreEqual ("x-default", node.Children [0].GetQualifier (XmpTag.XML_NS, "lang").Value);
			}

			{
				var node = tag.FindNode (XmpTag.DC_NS, "subject");
				Assert.IsFalse (node == null);
				Assert.AreEqual (XmpNodeType.Bag, node.Type);
				Assert.AreEqual ("", node.Value);
				Assert.AreEqual (1, node.Children.Count);
				Assert.AreEqual ("Communications", node.Children [0].Value);
			}
		}

		private void TestNode (XmpTag tag, string ns, string name, string value, int count)
		{
			var node = tag.FindNode (ns, name);
			Assert.IsFalse (node == null, String.Format ("Failed to find node: {0}{1}", ns, name));
			Assert.AreEqual (XmpNodeType.Simple, node.Type);
			Assert.AreEqual (value, node.Value);
			Assert.AreEqual (count, node.Children.Count);
		}
	}
}
