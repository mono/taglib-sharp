using System;
using System.IO;

namespace debug
{
    class Program
    {
        public static readonly string Samples = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(Program)).Location) + @"\..\..\..\tests\samples\";

        /// <summary>
        /// Ouput message on the console and on the Visual Studio Output
        /// </summary>
        /// <param name="str"></param>
        static void log(string str)
        {
            Console.WriteLine(str);
            System.Diagnostics.Debug.WriteLine(str);
        }


        static void Main(string[] args)
        {
            log("--------------------");
            log("* Start : Samples directory: " + Samples);
            log("");

            foreach (var fname in args) {
                var fpath = Samples + fname;

                log("+ File  : " + fpath);
                if(!File.Exists(fpath))
                {
                    log("  # File not found: " + fpath);
                    continue;
                }


                var file = TagLib.File.Create(fpath);

                log("  Title : " + file.Tag.Title);

                log("");
            }

            log("* End");
        }

    }
}
