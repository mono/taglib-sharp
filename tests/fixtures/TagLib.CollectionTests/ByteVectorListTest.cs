using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.CollectionTests
{   
    [TestFixture]
    public class ByteVectorListTest
    {   
        private static ByteVectorList BuildList()
        {
            ByteVectorList list = new ByteVectorList();
            list.Add("ABC");
            list.Add("DEF");
            list.Add("GHI");
            return list;
        }
    
        [Test]
        public void Add()
        {
            Assert.AreEqual("ABC:DEF:GHI", BuildList().ToByteVector(":").ToString());
        }
        
        [Test]
        public void Remove()
        {
            ByteVectorList list = BuildList();
            list.Remove("DEF");
            Assert.AreEqual("ABCGHI", list.ToByteVector("").ToString());
        }
        
        [Test]
        public void Insert()
        {
            ByteVectorList list = BuildList();
            list.Insert(1, "QUACK");
            Assert.AreEqual("ABC,QUACK,DEF,GHI", list.ToByteVector(",").ToString());
        }
        
        [Test]
        public void Contains()
        {
            ByteVectorList list = BuildList();
            Assert.IsTrue(list.Contains("DEF"));
            Assert.IsFalse(list.Contains("CDEFG"));
            Assert.AreEqual(2, list.ToByteVector("").Find("CDEFG"));
        }
        
        /*[Test]
        public void SortedInsert()
        {
            ByteVectorList list = BuildList();
            list.SortedInsert("000");
            Console.WriteLine(list.ToByteVector(",").ToString());
        }*/
    }
}
