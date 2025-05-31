using File = TagLib.File;

namespace TaglibSharp.Tests.Images
{
    /// <summary>
    /// Helper class for tests that use TestFixtureBase to provide proper temp file handling
    /// </summary>
    public static class TestFileHelper
    {
        /// <summary>
        /// Creates a temporary file using TestFixtureBase and returns the TagLib.File instance
        /// </summary>
        /// <param name="testFixture">The test fixture for temporary file management</param>
        /// <param name="sampleFile">Path to the sample file</param>
        /// <param name="tempFileName">Name to use for the temporary file</param>
        /// <returns>TagLib.File instance for the temporary file</returns>
        public static File CreateTempFile(TestFixtureBase testFixture, string sampleFile, string tempFileName)
        {
            return Utils.CreateTmpFile(testFixture, sampleFile, tempFileName);
        }

        /// <summary>
        /// Creates a temporary file path using TestFixtureBase, then loads and returns it as a TagLib.File
        /// </summary>
        /// <param name="testFixture">The test fixture for temporary file management</param>
        /// <param name="sampleFile">Path to the sample file</param>
        /// <param name="tempFileName">Name to use for the temporary file</param>
        /// <returns>TagLib.File instance for the temporary file</returns>
        public static File LoadTempFile(TestFixtureBase testFixture, string sampleFile, string tempFileName)
        {
            var path = testFixture.CreateTempFile(sampleFile, tempFileName);
            return File.Create(path);
        }
    }
}