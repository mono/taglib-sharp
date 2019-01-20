using NUnit.Framework;
using TagLib;

namespace TaglibSharp.Tests.Collections
{
	[TestFixture]
	public class StringCollectionTest
	{
		static StringCollection BuildList ()
		{
			var list = new StringCollection {
				"ABC",
				"DEF",
				"GHI"
			};
			return list;
		}

		[Test]
		public void Add ()
		{
			Assert.AreEqual ("ABC:DEF:GHI", BuildList ().ToString (":"));
		}

		[Test]
		public void Remove ()
		{
			var list = BuildList ();
			list.Remove ("DEF");
			Assert.AreEqual ("ABCGHI", list.ToString (string.Empty));
		}

		[Test]
		public void Insert ()
		{
			var list = BuildList ();
			list.Insert (1, "QUACK");
			Assert.AreEqual ("ABC,QUACK,DEF,GHI", list.ToString (","));
		}

		[Test]
		public void Contains ()
		{
			var list = BuildList ();
			Assert.IsTrue (list.Contains ("DEF"));
			Assert.IsFalse (list.Contains ("CDEFG"));
		}
	}
}
