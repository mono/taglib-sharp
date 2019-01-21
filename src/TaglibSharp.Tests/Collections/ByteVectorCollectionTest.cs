using NUnit.Framework;
using TagLib;

namespace TaglibSharp.Tests.Collections
{
	[TestFixture]
	public class ByteVectorCollectionTest
	{
		static ByteVectorCollection BuildList ()
		{
			var list = new ByteVectorCollection {
				"ABC",
				"DEF",
				"GHI"
			};
			return list;
		}

		[Test]
		public void Add ()
		{
			Assert.AreEqual ("ABC:DEF:GHI", BuildList ().ToByteVector (":").ToString ());
		}

		[Test]
		public void Remove ()
		{
			var list = BuildList ();
			list.Remove ("DEF");
			Assert.AreEqual ("ABCGHI", list.ToByteVector ("").ToString ());
		}

		[Test]
		public void Insert ()
		{
			var list = BuildList ();
			list.Insert (1, "QUACK");
			Assert.AreEqual ("ABC,QUACK,DEF,GHI", list.ToByteVector (",").ToString ());
		}

		[Test]
		public void Contains ()
		{
			var list = BuildList ();
			Assert.IsTrue (list.Contains ("DEF"));
			Assert.IsFalse (list.Contains ("CDEFG"));
			Assert.AreEqual (2, list.ToByteVector ("").Find ("CDEFG"));
		}

		/*[Test]
		public void SortedInsert()
		{
			ByteVectorCollection list = BuildList();
			list.SortedInsert("000");
			Console.WriteLine(list.ToByteVector(",").ToString());
		}*/
	}
}
