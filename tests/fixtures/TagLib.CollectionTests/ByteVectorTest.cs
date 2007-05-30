using System;
using System.Security.Cryptography;
using NUnit.Framework;
using TagLib;

namespace TagLib.CollectionTests
{   
    [TestFixture]
    public class ByteVectorTest
    {   
        private static readonly string TestInput = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly ByteVector TestVector = ByteVector.FromString(TestInput);
        
        [Test]
        public void Length()
        {
            Assert.AreEqual(TestInput.Length, TestVector.Count);
        }
        
        [Test]
        public void StartsWith()
        {
            Assert.IsTrue(TestVector.StartsWith("ABCDE"));
            Assert.IsFalse(TestVector.StartsWith("NOOP"));
        }
        
        [Test]
        public void EndsWith()
        {
            Assert.IsTrue(TestVector.EndsWith("UVWXYZ"));
            Assert.IsFalse(TestVector.EndsWith("NOOP"));
        }
        
        [Test]
        public void ContainsAt()
        {
            Assert.IsTrue(TestVector.ContainsAt("JKLMNO", 9));
            Assert.IsFalse(TestVector.ContainsAt("NOOP", 30));
        }
        
        [Test]
        public void Find()
        {
            Assert.AreEqual(17, TestVector.Find("RSTUV"));
            Assert.AreEqual(-1, TestVector.Find("NOOP"));
        }
    
        [Test]
        public void RFind()
        {
            Assert.AreEqual(6, TestVector.RFind("GHIJ"));
            Assert.AreEqual(-1, TestVector.RFind("NOOP"));
        }
    
        [Test]
        public void Mid()
        {
            Assert.AreEqual(ByteVector.FromString("KLMNOPQRSTUVWXYZ"), TestVector.Mid(10));
            Assert.AreEqual(ByteVector.FromString("PQRSTU"), TestVector.Mid(15, 6));
        }
    
        [Test]
        public void CopyResize()
        {
            ByteVector a = new ByteVector(TestVector);
            ByteVector b = ByteVector.FromString("ABCDEFGHIJKL");
            a.Resize(12);
            
            Assert.AreEqual(b, a);
            Assert.AreEqual( b.ToString(), a.ToString());
            Assert.IsFalse(a.Count == TestVector.Count);
        }
        
        [Test]
        public void UInt()
        {
            Assert.AreEqual(UInt32.MaxValue, ByteVector.FromUInt(UInt32.MaxValue).ToUInt());
            Assert.AreEqual(UInt32.MinValue, ByteVector.FromUInt(UInt32.MinValue).ToUInt());
            Assert.AreEqual(0, ByteVector.FromUInt(0).ToUInt());
            Assert.AreEqual(30292, ByteVector.FromUInt(30292).ToUInt());
        }
        
        [Test]
        public void Long()
        {
            Assert.AreEqual(UInt64.MaxValue, ByteVector.FromULong(UInt64.MaxValue).ToULong());
            Assert.AreEqual(UInt64.MinValue, ByteVector.FromULong(UInt64.MinValue).ToULong());
            Assert.AreEqual(0, ByteVector.FromULong(0).ToULong());
            Assert.AreEqual(30292, ByteVector.FromULong(30292).ToULong());
        }
        
        [Test]
        public void Short()
        {
            Assert.AreEqual(UInt16.MaxValue, ByteVector.FromUShort(UInt16.MaxValue).ToUShort());
            Assert.AreEqual(UInt16.MinValue, ByteVector.FromUShort(UInt16.MinValue).ToUShort());
            Assert.AreEqual(0, ByteVector.FromUShort(0).ToUShort());
            Assert.AreEqual(8009, ByteVector.FromUShort(8009).ToUShort());
        }
        
        [Test]
        public void FromUri()
        {
            ByteVector vector = ByteVector.FromPath("samples/vector.bin");
            Assert.AreEqual(3282169185, vector.Checksum);
            Assert.AreEqual("1aaa46c484d70c7c80510a5f99e7805d", MD5Hash(vector.Data));
        }
        
        [Test]
        public void OperatorAdd()
        {
            using(new CodeTimer("Operator Add")) {
                ByteVector vector = new ByteVector();
                for(int i = 0; i < 10000; i++) {
                    vector += ByteVector.FromULong((ulong)55);
                }
            }
            
            using(new CodeTimer("Function Add")) {
                ByteVector vector = new ByteVector();
                for(int i = 0; i < 10000; i++) {
                    vector.Add(ByteVector.FromULong((ulong)55));
                }
            }
        }
        
        private static string MD5Hash(byte [] bytes)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte [] hash_bytes = md5.ComputeHash(bytes);
            string hash_string = String.Empty;

            for(int i = 0; i < hash_bytes.Length; i++) {
                hash_string += Convert.ToString(hash_bytes[i], 16).PadLeft(2, '0');
            }

            return hash_string.PadLeft(32, '0');
        }
    }
}
