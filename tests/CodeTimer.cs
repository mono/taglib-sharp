using System;

namespace TagLib
{
    public class CodeTimer : IDisposable
    {
        private DateTime start;
        private TimeSpan elapsed_time = TimeSpan.Zero;
        private string label;
        
        public CodeTimer()
        {
            start = DateTime.Now;
        }
        
        public CodeTimer(string label) : this()
        {
            this.label = label;
        }

        public TimeSpan ElapsedTime {
            get { 
                DateTime now = DateTime.Now;
                return elapsed_time == TimeSpan.Zero ? now - start : elapsed_time;
            }
        }

        public void WriteElapsed(string message)
        {
            Console.WriteLine("{0} {1} {2}", label, message, ElapsedTime);
        }

        public void Dispose()
        {
            elapsed_time = DateTime.Now - start;
            if(label != null) {
                WriteElapsed("timer stopped:");
            }
        }
    }
}
