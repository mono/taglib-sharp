using System.Security.Cryptography;
using System.Text;

using File = TagLib.File;

namespace TaglibSharp.Tests.Images;

public static class Utils
{
	static readonly MD5 md5 = MD5.Create ();

	/// <summary>
	/// Creates a temporary file by copying from a source file using TestFixtureBase
	/// </summary>
	/// <param name="testFixture">The test fixture instance to use for file management</param>
	/// <param name="sample_file">Source file to copy from</param>
	/// <param name="tmp_file">Target temporary file path (can be just a filename)</param>
	/// <returns>TagLib File instance for the temporary file</returns>
	public static File CreateTmpFile (TestFixtureBase testFixture, string sample_file, string tmp_file)
	{
		if (sample_file == tmp_file)
			throw new Exception ("files cannot be equal");

		// Use TestFixtureBase to create the temporary file
		// Extract just the filename if a full path is provided
		var tempFileName = Path.GetFileName(tmp_file);
		var tempFilePath = testFixture.CreateTempFile(sample_file, tempFileName);
		
		return File.Create (tempFilePath);
	}

	/// <summary>
	/// Legacy method for backwards compatibility - creates temporary file without TestFixtureBase
	/// This method should be avoided in new tests in favor of the TestFixtureBase version
	/// </summary>
	[Obsolete("Use CreateTmpFile(TestFixtureBase, string, string) instead for proper file isolation")]
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
