
using System;

using TagLib;


namespace TagLib.Tests.Images
{
    public static class Utils
    {

		public static File CreateTmpFile (string sample_file, string tmp_file) {

			if (sample_file == tmp_file)
				throw new Exception ("files cannot be equal");

			if (System.IO.File.Exists (tmp_file))
                System.IO.File.Delete (tmp_file);

			System.IO.File.Copy (sample_file, tmp_file);
			File tmp = File.Create (tmp_file);

			return tmp;
		}

	}
}
