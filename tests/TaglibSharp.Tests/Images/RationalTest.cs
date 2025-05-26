using NUnit.Framework;
using TagLib.IFD.Entries;

namespace TaglibSharp.Tests.Images
{

	[TestFixture]
	public class RationalTest
	{
		[Test]
		public void Rational1 ()
		{
			var r1 = new Rational (5, 3);

			ClassicAssert.AreEqual (5, r1.Numerator);
			ClassicAssert.AreEqual (3, r1.Denominator);
			ClassicAssert.AreEqual (5.0d / 3.0d, (double)r1);
			ClassicAssert.AreEqual ("5/3", r1.ToString ());

			ClassicAssert.AreEqual (5, r1.Reduce ().Numerator);
			ClassicAssert.AreEqual (3, r1.Reduce ().Denominator);
		}

		[Test]
		public void Rational2 ()
		{
			var r2 = new Rational (48, 18);

			ClassicAssert.AreEqual (48, r2.Numerator);
			ClassicAssert.AreEqual (18, r2.Denominator);
			ClassicAssert.AreEqual (48.0d / 18.0d, (double)r2);
			ClassicAssert.AreEqual ("8/3", r2.ToString ());

			ClassicAssert.AreEqual (8, r2.Reduce ().Numerator);
			ClassicAssert.AreEqual (3, r2.Reduce ().Denominator);
		}

		[Test]
		public void Rational3 ()
		{
			var r3 = new Rational (0, 17);

			ClassicAssert.AreEqual (0, r3.Numerator);
			ClassicAssert.AreEqual (17, r3.Denominator);
			ClassicAssert.AreEqual (0.0d / 17.0d, (double)r3);
			ClassicAssert.AreEqual ("0/1", r3.ToString ());

			ClassicAssert.AreEqual (0, r3.Reduce ().Numerator);
			ClassicAssert.AreEqual (1, r3.Reduce ().Denominator);
		}

		[Test]
		public void SRational1 ()
		{
			var r1 = new SRational (5, 3);

			ClassicAssert.AreEqual (5, r1.Numerator);
			ClassicAssert.AreEqual (3, r1.Denominator);
			ClassicAssert.AreEqual (5.0d / 3.0d, (double)r1);
			ClassicAssert.AreEqual ("5/3", r1.ToString ());

			ClassicAssert.AreEqual (5, r1.Reduce ().Numerator);
			ClassicAssert.AreEqual (3, r1.Reduce ().Denominator);
		}

		[Test]
		public void SRational2 ()
		{
			var r2 = new SRational (48, 18);

			ClassicAssert.AreEqual (48, r2.Numerator);
			ClassicAssert.AreEqual (18, r2.Denominator);
			ClassicAssert.AreEqual (48.0d / 18.0d, (double)r2);
			ClassicAssert.AreEqual ("8/3", r2.ToString ());

			ClassicAssert.AreEqual (8, r2.Reduce ().Numerator);
			ClassicAssert.AreEqual (3, r2.Reduce ().Denominator);
		}

		[Test]
		public void SRational3 ()
		{
			var r3 = new SRational (0, -17);

			ClassicAssert.AreEqual (0, r3.Numerator);
			ClassicAssert.AreEqual (-17, r3.Denominator);
			ClassicAssert.AreEqual (0.0d / -17.0d, (double)r3);
			ClassicAssert.AreEqual ("0/1", r3.ToString ());

			ClassicAssert.AreEqual (0, r3.Reduce ().Numerator);
			ClassicAssert.AreEqual (1, r3.Reduce ().Denominator);
		}

		[Test]
		public void SRational4 ()
		{
			var r4 = new SRational (-108, -46);

			ClassicAssert.AreEqual (-108, r4.Numerator);
			ClassicAssert.AreEqual (-46, r4.Denominator);
			ClassicAssert.AreEqual (-108.0d / -46.0d, (double)r4);
			ClassicAssert.AreEqual ("54/23", r4.ToString ());

			ClassicAssert.AreEqual (54, r4.Reduce ().Numerator);
			ClassicAssert.AreEqual (23, r4.Reduce ().Denominator);
		}

		[Test]
		public void SRational5 ()
		{
			var r5 = new SRational (-256, 96);

			ClassicAssert.AreEqual (-256, r5.Numerator);
			ClassicAssert.AreEqual (96, r5.Denominator);
			ClassicAssert.AreEqual (-256.0d / 96.0d, (double)r5);
			ClassicAssert.AreEqual ("-8/3", r5.ToString ());

			ClassicAssert.AreEqual (-8, r5.Reduce ().Numerator);
			ClassicAssert.AreEqual (3, r5.Reduce ().Denominator);
		}
	}
}
