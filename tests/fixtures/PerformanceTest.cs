using System;
using NUnit.Framework;
using TagLib;

namespace TagLib.PerformanceTests
{           
    [TestFixture]
    public class FileTest
    {
        [Test]
        public void CreateM4a()
        {
            try {
                double total_time = 0.0;
                int iterations = 1000;
                using(new CodeTimer("Combined")) {
                    for(int i = 0; i < iterations; i++) {
                        CodeTimer timer = new CodeTimer();
                        using(timer) {
                            File.Create("samples/sample.m4a");
                        }
                        total_time += timer.ElapsedTime.TotalSeconds;
                    }
                }
                Console.WriteLine("Average time: {0}", total_time / (double)iterations);
            } catch(Exception e) {
                Console.WriteLine(e);
            }
        }

        [Test]
        public void CreateOgg()
        {
            try {
                double total_time = 0.0;
                int iterations = 1000;
                using(new CodeTimer("Combined")) {
                    for(int i = 0; i < iterations; i++) {
                        CodeTimer timer = new CodeTimer();
                        using(timer) {
                            File.Create("samples/sample.ogg");
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
