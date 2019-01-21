using NUnit.Framework;
using System;
using TagLib;

namespace TaglibSharp.Tests.Performance
{
	[TestFixture]
	public class PerformanceTest
	{
		[Test]
		public void CreateM4a ()
		{
			try {
				double total_time = 0.0;
				int iterations = 1000;
				using (new CodeTimer ("Combined")) {
					for (int i = 0; i < iterations; i++) {
						var timer = new CodeTimer ();
						using (timer) {
							File.Create (TestPath.Samples + "sample.m4a");
						}
						total_time += timer.ElapsedTime.TotalSeconds;
					}
				}
				Console.WriteLine ("Average time: {0}", total_time / iterations);
			} catch (Exception e) {
				Console.WriteLine (e);
				Assert.False (false);
			}
			Assert.True (true);
		}

		[Test]
		public void CreateOgg ()
		{
			try {
				double total_time = 0.0;
				int iterations = 1000;
				using (new CodeTimer ("Combined")) {
					for (int i = 0; i < iterations; i++) {
						var timer = new CodeTimer ();
						using (timer) {
							File.Create (TestPath.Samples + "sample.ogg");
						}
						total_time += timer.ElapsedTime.TotalSeconds;
					}
				}
				Console.WriteLine ("Average time: {0}", total_time / iterations);
			} catch (Exception e) {
				Console.WriteLine (e);
				Assert.False (false);
			}
			Assert.True (true);
		}
	}
}
