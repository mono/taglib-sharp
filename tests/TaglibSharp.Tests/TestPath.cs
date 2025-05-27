
namespace TaglibSharp.Tests;

/// <summary>
/// OS-Independent Path composition
/// </summary>
public static class TestPath
{
	public static string TestsDir { get; } = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(Debugger)).Location);
	public static string Samples { get; } = Path.Combine(TestsDir, "..", "..", "..", "samples", " ").TrimEnd();
	public static string SamplesTmp { get; } = Path.Combine(TestsDir, "..", "..", "..", "samples", Environment.Version.ToString(), " ").TrimEnd();
	public static string RawSamples { get; } = Path.Combine(TestsDir, "..", "..", "..", "raw-samples", " ").TrimEnd();
	public static string GetRawSubDirectory(string subdir) => Path.Combine(RawSamples, subdir);
	public static string Covers { get; } = Path.Combine(TestsDir, "..", "..", "..", "..", "..", "examples", "covers", " ").TrimEnd();
}