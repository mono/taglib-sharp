using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.CollectionTests
{   
    [TestFixture]
    public class StringListTest
    {   
        private static StringList BuildList()
        {
            StringList list = new StringList();
            list.Add("ABC");
            list.Add("DEF");
            list.Add("GHI");
            return list;
        }
    
        [Test]
        public void Add()
        {
            Assert.AreEqual("ABC:DEF:GHI", BuildList().ToString(":"));
        }
        
        [Test]
        public void Remove()
        {
            StringList list = BuildList();
            list.Remove("DEF");
            Assert.AreEqual("ABCGHI", list.ToString(""));
        }
        
        [Test]
        public void Insert()
        {
            StringList list = BuildList();
            list.Insert(1, "QUACK");
            Assert.AreEqual("ABC,QUACK,DEF,GHI", list.ToString(","));
        }
        
        [Test]
        public void Contains()
        {
            StringList list = BuildList();
            Assert.IsTrue(list.Contains("DEF"));
            Assert.IsFalse(list.Contains("CDEFG"));
        }
    }
}
