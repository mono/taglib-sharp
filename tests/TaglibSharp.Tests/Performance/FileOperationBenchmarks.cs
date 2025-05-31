using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using System.IO.Abstractions.TestingHelpers;
using TaglibSharp.Tests.Infrastructure;

namespace TaglibSharp.Tests.Performance;

[TestClass]
[TestCategory(TestCategories.Performance)]
public class FileOperationBenchmarks
{
    private MockFileSystem? _fileSystem;
    private byte[]? _testMp3Data;

    [GlobalSetup]
    public void Setup()
    {
        _fileSystem = TestDataFactory.CreateMockFileSystem();
        _testMp3Data = new byte[1024 * 1024]; // 1MB test file
        new Random(42).NextBytes(_testMp3Data);
    }

    [Benchmark]
    [Arguments(@"c:\test\test.mp3")]
    public void ReadTagFromFile(string filePath)
    {
        // This would use actual TagLib methods
        // var file = TagLib.File.Create(filePath);
        // var tag = file.Tag;

        // Simulate file reading operation
        var data = _fileSystem!.File.ReadAllBytes(filePath);
        ProcessFileData(data);
    }

    [Benchmark]
    [Arguments(1024)]
    [Arguments(1024 * 1024)]
    [Arguments(10 * 1024 * 1024)]
    public void ProcessFileOfSize(int sizeBytes)
    {
        var data = new byte[sizeBytes];
        new Random(42).NextBytes(data);
        ProcessFileData(data);
    }

    [Benchmark]
    public void CreateMultipleTags()
    {
        for (int i = 0; i < 100; i++)
        {
            var tag = TestDataFactory.CreateTestTag();
            tag.Title = $"Test Title {i}";
            tag.Artist = $"Test Artist {i}";
        }
    }

    private static void ProcessFileData(byte[] data)
    {
        // Simulate processing logic
        var checksum = data.Aggregate(0, (acc, b) => acc + b);
        _ = checksum; // Prevent optimization
    }

    [TestMethod]
    public void RunBenchmarks()
    {
        // This test method allows running benchmarks in test context
        var summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<FileOperationBenchmarks>();

        // Assert that performance hasn't regressed significantly
        foreach (var report in summary.Reports)
        {
            // Add assertions based on expected performance thresholds
            report.ResultStatistics.Mean.Should().BeLessThan(1000); // Example: less than 1000ns
        }
    }
}
