using System.IO;

namespace TaglibSharp.Tests
{
	/// <summary>
	/// OS-Independent Path composition
	/// </summary>
	public static class TestPath
	{
		public static readonly string TestsDir = Path.GetDirectoryName (System.Reflection.Assembly.GetAssembly (typeof (Debugger)).Location);
		public static readonly string Samples = Path.Combine (TestsDir, "..", "..", "..", "samples", " ").TrimEnd ();
		public static readonly string RawSamples = Path.Combine (TestsDir, "..", "..", "..", "raw-samples", " ").TrimEnd ();
		public static string GetRawSubDirectory (string subdir) => Path.Combine (RawSamples, subdir);
		public static readonly string Covers = Path.Combine (TestsDir, "..", "..", "..", "..", "..", "examples", "covers", " ").TrimEnd ();
	}
}