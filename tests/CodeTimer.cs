using System;

namespace TagLib
{
    public class CodeTimer : IDisposable
    {
        private DateTime start;
        private string label;
        
        public CodeTimer(string label) 
        {
            this.label = label;
            start = DateTime.Now;
        }

        public TimeSpan ElapsedTime {
            get { return DateTime.Now - start; }
        }

        public void WriteElapsed(string message)
        {
            Console.WriteLine("{0} {1} {2}", label, message, ElapsedTime);
        }

        public void Dispose()
        {
            WriteElapsed("timer stopped:");
        }
    }
}
