using NUnit.Framework;
using System;
using System.Security.Cryptography;
using TagLib;

namespace TaglibSharp.Tests.Collections
{
	[TestFixture]
	public class ByteVectorTest
	{
		static readonly string TestInput = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		static readonly ByteVector TestVector = ByteVector.FromString (TestInput, StringType.UTF8);

		[Test]
		public void Length ()
		{
			ClassicAssert.AreEqual (TestInput.Length, TestVector.Count);
		}

		[Test]
		public void StartsWith ()
		{
			ClassicAssert.IsTrue (TestVector.StartsWith ("ABCDE"));
			ClassicAssert.IsFalse (TestVector.StartsWith ("NOOP"));
		}

		[Test]
		public void EndsWith ()
		{
			ClassicAssert.IsTrue (TestVector.EndsWith ("UVWXYZ"));
			ClassicAssert.IsFalse (TestVector.EndsWith ("NOOP"));
		}

		[Test]
		public void ContainsAt ()
		{
			ClassicAssert.IsTrue (TestVector.ContainsAt ("JKLMNO", 9));
			ClassicAssert.IsFalse (TestVector.ContainsAt ("NOOP", 30));
		}

		[Test]
		public void Find ()
		{
			ClassicAssert.AreEqual (17, TestVector.Find ("RSTUV"));
			ClassicAssert.AreEqual (-1, TestVector.Find ("NOOP"));
		}

		[Test]
		public void RFind ()
		{
			ClassicAssert.AreEqual (6, TestVector.RFind ("GHIJ"));
			ClassicAssert.AreEqual (-1, TestVector.RFind ("NOOP"));
		}

		[Test]
		public void Mid ()
		{
			ClassicAssert.AreEqual (ByteVector.FromString ("KLMNOPQRSTUVWXYZ", StringType.UTF8), TestVector.Mid (10));
			ClassicAssert.AreEqual (ByteVector.FromString ("PQRSTU", StringType.UTF8), TestVector.Mid (15, 6));
		}

		[Test]
		public void CopyResize ()
		{
			var a = new ByteVector (TestVector);
			var b = ByteVector.FromString ("ABCDEFGHIJKL", StringType.UTF8);
			a.Resize (12);

			ClassicAssert.AreEqual (b, a);
			ClassicAssert.AreEqual (b.ToString (), a.ToString ());
			ClassicAssert.IsFalse (a.Count == TestVector.Count);
		}

		[Test]
		public void Int ()
		{
			ClassicAssert.AreEqual (int.MaxValue, ByteVector.FromInt (int.MaxValue).ToInt ());
			ClassicAssert.AreEqual (int.MinValue, ByteVector.FromInt (int.MinValue).ToInt ());
			ClassicAssert.AreEqual (0, ByteVector.FromInt (0).ToInt ());
			ClassicAssert.AreEqual (30292, ByteVector.FromInt (30292).ToInt ());
			ClassicAssert.AreEqual (-30292, ByteVector.FromInt (-30292).ToInt ());
			ClassicAssert.AreEqual (-1, ByteVector.FromInt (-1).ToInt ());
		}

		[Test]
		public void UInt ()
		{
			ClassicAssert.AreEqual (uint.MaxValue, ByteVector.FromUInt (uint.MaxValue).ToUInt ());
			ClassicAssert.AreEqual (uint.MinValue, ByteVector.FromUInt (uint.MinValue).ToUInt ());
			ClassicAssert.AreEqual (0, ByteVector.FromUInt (0).ToUInt ());
			ClassicAssert.AreEqual (30292, ByteVector.FromUInt (30292).ToUInt ());
		}

		[Test]
		public void Long ()
		{
			ClassicAssert.AreEqual (ulong.MaxValue, ByteVector.FromULong (ulong.MaxValue).ToULong ());
			ClassicAssert.AreEqual (ulong.MinValue, ByteVector.FromULong (ulong.MinValue).ToULong ());
			ClassicAssert.AreEqual (0, ByteVector.FromULong (0).ToULong ());
			ClassicAssert.AreEqual (30292, ByteVector.FromULong (30292).ToULong ());
		}

		[Test]
		public void Short ()
		{
			ClassicAssert.AreEqual (ushort.MaxValue, ByteVector.FromUShort (ushort.MaxValue).ToUShort ());
			ClassicAssert.AreEqual (ushort.MinValue, ByteVector.FromUShort (ushort.MinValue).ToUShort ());
			ClassicAssert.AreEqual (0, ByteVector.FromUShort (0).ToUShort ());
			ClassicAssert.AreEqual (8009, ByteVector.FromUShort (8009).ToUShort ());
		}

		[Test]
		public void FromUri ()
		{
			var vector = ByteVector.FromPath (TestPath.Samples + "vector.bin");
			ClassicAssert.AreEqual (3282169185, vector.Checksum);
			ClassicAssert.AreEqual ("1aaa46c484d70c7c80510a5f99e7805d", MD5Hash (vector.Data));
		}

		[Test]
		[Ignore ("Skip performance testing")]
		public void OperatorAdd ()
		{
			using (new CodeTimer ("Operator Add")) {
				var vector = new ByteVector ();
				for (int i = 0; i < 10000; i++) {
					vector += ByteVector.FromULong (55);
				}
			}

			using (new CodeTimer ("Function Add")) {
				var vector = new ByteVector ();
				for (int i = 0; i < 10000; i++) {
					vector.Add (ByteVector.FromULong (55));
				}
			}
		}

		[Test]
		public void CommentsFrameError ()
		{
			// http://bugzilla.gnome.org/show_bug.cgi?id=582735
			// Comments data found in the wild
			var vector = new ByteVector (
				1, 255, 254, 73, 0, 68, 0, 51, 0, 71, 0, 58, 0, 32, 0, 50, 0, 55, 0, 0, 0);

			var encoding = (StringType)vector[0];
			//var language = vector.ToString (StringType.Latin1, 1, 3);
			var split = vector.ToStrings (encoding, 4, 3);
			ClassicAssert.AreEqual (2, split.Length);
		}

		static string MD5Hash (byte[] bytes)
		{
			var md5 = MD5.Create ();
			byte[] hash_bytes = md5.ComputeHash (bytes);
			string hash_string = string.Empty;

			for (int i = 0; i < hash_bytes.Length; i++) {
				hash_string += Convert.ToString (hash_bytes[i], 16).PadLeft (2, '0');
			}

			return hash_string.PadLeft (32, '0');
		}
	}
}
