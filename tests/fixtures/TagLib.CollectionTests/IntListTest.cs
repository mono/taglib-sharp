using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.CollectionTests
{   
    [TestFixture]
    public class IntListTest
    {   
        private static IntList BuildList()
        {
            IntList list = new IntList();
            list.Add(55);
            list.Add(67);
            list.Add(33);
            return list;
        }
    /*
        [Test]
        public void Add()
        {
            Assert.AreEqual("55:67:33", BuildList().ToString(":"));
        }
        
        [Test]
        public void Remove()
        {
            IntList list = BuildList();
            list.Remove(67);
            Assert.AreEqual("5533", list.ToString(String.Empty));
        }
        
        [Test]
        public void Insert()
        {
            IntList list = BuildList();
            list.Insert(1, 99);
            Assert.AreEqual("55,99,67,33", list.ToString(","));
        }*/
        
        [Test]
        public void Contains()
        {
            IntList list = BuildList();
            Assert.IsTrue(list.Contains(67));
            Assert.IsFalse(list.Contains(391));
        }
    }
}
