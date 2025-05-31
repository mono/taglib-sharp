namespace TaglibSharp.Tests;

/// <summary>
/// Test category constants for organizing and filtering tests
/// </summary>
public static class TestCategories
{
    /// <summary>
    /// Fast unit tests that don't require file system access
    /// </summary>
    public const string Unit = "Unit";

    /// <summary>
    /// Integration tests that require file system or external resources
    /// </summary>
    public const string Integration = "Integration";

    /// <summary>
    /// Performance and benchmarking tests
    /// </summary>
    public const string Performance = "Performance";

    /// <summary>
    /// Property-based tests using FsCheck
    /// </summary>
    public const string PropertyBased = "PropertyBased";

    /// <summary>
    /// Tests for error handling and edge cases
    /// </summary>
    public const string ErrorHandling = "ErrorHandling";

    /// <summary>
    /// Tests for specific audio formats
    /// </summary>
    public static class Formats
    {
        public const string Mp3 = "Format.Mp3";
        public const string Flac = "Format.Flac";
        public const string Ogg = "Format.Ogg";
        public const string Mp4 = "Format.Mp4";
        public const string Ape = "Format.Ape";
    }

    /// <summary>
    /// Tests requiring external test files
    /// </summary>
    public const string RequiresTestFiles = "RequiresTestFiles";

    /// <summary>
    /// Long-running tests
    /// </summary>
    public const string LongRunning = "LongRunning";
}
