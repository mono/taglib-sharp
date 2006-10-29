using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.PerformanceTests
{           
    [TestFixture]
    public class FileTest
    {
        [Test]
        public void Create()
        {
            try {
                double total_time = 0.0;
                int iterations = 10000;
                using(new CodeTimer("Combined")) {
                    for(int i = 0; i < iterations; i++) {
                        CodeTimer timer = new CodeTimer();
                        using(timer) {
                            try {
                                File.Create("samples/performance.m4a");
                            } catch (Exception e) {
                                Console.WriteLine(e);
                            }
                        }
                        total_time += timer.ElapsedTime.TotalSeconds;
                    }
                }
                Console.WriteLine("Average time: {0}", total_time / (double)iterations);
            } catch(Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
