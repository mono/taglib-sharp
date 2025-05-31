using File = TagLib.File;

namespace TaglibSharp.Tests
{
    /// <summary>
    /// Base class for test fixtures that provides proper file isolation and cleanup
    /// </summary>
    public abstract class TestFixtureBase : IDisposable
    {
        private readonly string _testTempDirectory;
        private readonly List<string> _createdFiles = new();
        private readonly List<string> _createdDirectories = new();
        private bool _disposed;

        protected TestFixtureBase()
        {
            // Create unique temp directory for this test instance
            _testTempDirectory = Path.Combine(Path.GetTempPath(), "TaglibSharp.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testTempDirectory);
            _createdDirectories.Add(_testTempDirectory);
        }

        /// <summary>
        /// Gets the unique temporary directory for this test instance
        /// </summary>
        protected string TestTempDirectory => _testTempDirectory;

        /// <summary>
        /// Creates a temporary file by copying from a source file
        /// </summary>
        /// <param name="sourceFile">Source file to copy from</param>
        /// <param name="tempFileName">Optional custom temp file name (if null, uses source filename)</param>
        /// <returns>Path to the created temporary file</returns>
        public string CreateTempFile(string sourceFile, string tempFileName = null)
        {
            if (!System.IO.File.Exists(sourceFile))
                throw new FileNotFoundException($"Source file not found: {sourceFile}");

            tempFileName ??= Path.GetFileName(sourceFile);
            var tempFilePath = Path.Combine(_testTempDirectory, tempFileName);

            // Ensure subdirectory exists if tempFileName contains path separators
            var tempFileDir = Path.GetDirectoryName(tempFilePath);
            if (!string.IsNullOrEmpty(tempFileDir) && !Directory.Exists(tempFileDir))
            {
                Directory.CreateDirectory(tempFileDir);
                _createdDirectories.Add(tempFileDir);
            }

            System.IO.File.Copy(sourceFile, tempFilePath, overwrite: true);
            _createdFiles.Add(tempFilePath);

            return tempFilePath;
        }

        /// <summary>
        /// Creates a temporary file with the specified content
        /// </summary>
        /// <param name="tempFileName">Name of the temporary file</param>
        /// <param name="content">Content to write to the file</param>
        /// <returns>Path to the created temporary file</returns>
        protected string CreateTempFileWithContent(string tempFileName, byte[] content)
        {
            var tempFilePath = Path.Combine(_testTempDirectory, tempFileName);

            // Ensure subdirectory exists if tempFileName contains path separators
            var tempFileDir = Path.GetDirectoryName(tempFilePath);
            if (!string.IsNullOrEmpty(tempFileDir) && !Directory.Exists(tempFileDir))
            {
                Directory.CreateDirectory(tempFileDir);
                _createdDirectories.Add(tempFileDir);
            }

            System.IO.File.WriteAllBytes(tempFilePath, content);
            _createdFiles.Add(tempFilePath);

            return tempFilePath;
        }

        /// <summary>
        /// Creates a TagLib File instance from a temporary copy of the source file
        /// </summary>
        /// <param name="sourceFile">Source file to copy and open</param>
        /// <param name="tempFileName">Optional custom temp file name</param>
        /// <returns>TagLib File instance</returns>
        protected File CreateTempTagLibFile(string sourceFile, string tempFileName = null)
        {
            var tempFilePath = CreateTempFile(sourceFile, tempFileName);
            return File.Create(tempFilePath);
        }

        /// <summary>
        /// Gets a path within the test temp directory
        /// </summary>
        /// <param name="relativePath">Relative path within the temp directory</param>
        /// <returns>Full path within the temp directory</returns>
        protected string GetTempPath(string relativePath)
        {
            return Path.Combine(_testTempDirectory, relativePath);
        }

        /// <summary>
        /// Ensures a file is deleted if it exists
        /// </summary>
        /// <param name="filePath">Path to the file to delete</param>
        protected void EnsureFileDeleted(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    // Log but don't fail the test for cleanup issues
                    Console.WriteLine($"Warning: Could not delete file {filePath}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Cleanup method called after each test
        /// </summary>
        [TestCleanup]
        public virtual void TestCleanup()
        {
            Cleanup();
        }

        /// <summary>
        /// Performs cleanup of temporary files and directories
        /// </summary>
        protected virtual void Cleanup()
        {
            if (_disposed) return;

            // Delete created files
            foreach (var file in _createdFiles.ToList())
            {
                EnsureFileDeleted(file);
            }
            _createdFiles.Clear();

            // Delete created directories (in reverse order)
            foreach (var directory in _createdDirectories.AsEnumerable().Reverse().ToList())
            {
                if (Directory.Exists(directory))
                {
                    try
                    {
                        Directory.Delete(directory, recursive: true);
                    }
                    catch (Exception ex)
                    {
                        // Log but don't fail the test for cleanup issues
                        Console.WriteLine($"Warning: Could not delete directory {directory}: {ex.Message}");
                    }
                }
            }
            _createdDirectories.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                Cleanup();
                _disposed = true;
            }
        }

        ~TestFixtureBase()
        {
            Dispose(false);
        }
    }
}