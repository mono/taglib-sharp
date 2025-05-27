using System.Security.Cryptography;
using System.Text;

using File = TagLib.File;

namespace TaglibSharp.Tests.Images;

public static class Utils
{
	static readonly MD5 md5 = MD5.Create ();

	public static File CreateTmpFile (string sample_file, string tmp_file)
	{
		if (sample_file == tmp_file)
			throw new Exception ("files cannot be equal");

		// Create tmp_file path if it doesn't exist
		Directory.CreateDirectory(Path.GetDirectoryName(tmp_file));

		if (System.IO.File.Exists(tmp_file))
			System.IO.File.Delete(tmp_file);

		System.IO.File.Copy (sample_file, tmp_file);
		var tmp = File.Create (tmp_file);

		return tmp;
	}

	public static string Md5Encode (byte[] data)
	{
		var hash = md5.ComputeHash (data);

		var shash = new StringBuilder ();
		for (int i = 0; i < hash.Length; i++) {
			shash.Append (hash[i].ToString ("x2"));
		}

		return shash.ToString ();
	}
}
