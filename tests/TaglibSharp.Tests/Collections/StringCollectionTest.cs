namespace TaglibSharp.Tests.Collections
{
	[TestClass]
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

		[TestMethod]
		public void Add ()
		{
			Assert.AreEqual ("ABC:DEF:GHI", BuildList ().ToString (":"));
		}

		[TestMethod]
		public void Remove ()
		{
			var list = BuildList ();
			list.Remove ("DEF");
			Assert.AreEqual ("ABCGHI", list.ToString (string.Empty));
		}

		[TestMethod]
		public void Insert ()
		{
			var list = BuildList ();
			list.Insert (1, "QUACK");
			Assert.AreEqual ("ABC,QUACK,DEF,GHI", list.ToString (","));
		}

		[TestMethod]
		public void Contains ()
		{
			var list = BuildList ();
			Assert.IsTrue (list.Contains ("DEF"));
			Assert.IsFalse (list.Contains ("CDEFG"));
		}
	}
}
