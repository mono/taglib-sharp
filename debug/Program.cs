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
                var tpath = Samples + "tmpwrite" + Path.GetExtension(fname);

                log("+ File  : " + fpath);
                if(!File.Exists(fpath))
                {
                    log("  # File not found: " + fpath);
                    continue;
                }

                File.Copy(fpath, tpath, true);

                var file = TagLib.File.Create(tpath);

                log("  Title : " + file.Tag.Title);
                file.Tag.Title = "Test Video";
                file.Save();
                log("  Title : " + file.Tag.Title);

                // Now read it again
                var rfile = TagLib.File.Create(tpath);
                log("  Title : " + rfile.Tag.Title);

                log("");
            }

            log("* End");
        }

    }
}
