using System.Diagnostics;
using System.Text.Json;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace TaglibSharp.Tests.Coverage;

[TestClass]
[TestCategory("Coverage")]
public class CoverageReport
{
    private const double MIN_LINE_COVERAGE = 80.0;
    private const double MIN_BRANCH_COVERAGE = 70.0;

    [TestMethod]
    [TestCategory("Coverage")]
    public async Task ValidateCoverageThreshold()
    {
        // This test validates that code coverage meets minimum requirements
        var coverageFile = FindCoverageFile();

        if (coverageFile == null)
        {
            Assert.Inconclusive("No coverage file found. Run tests with coverage collection enabled.");
            return;
        }

        var coverage = await ParseCoverageFile(coverageFile);
        var lineCoverage = coverage.GetLineCoveragePercentage();
        var branchCoverage = coverage.GetBranchCoveragePercentage();

        lineCoverage.Should().BeGreaterOrEqualTo(MIN_LINE_COVERAGE,
            $"Line coverage should be at least {MIN_LINE_COVERAGE}%");
        branchCoverage.Should().BeGreaterOrEqualTo(MIN_BRANCH_COVERAGE,
            $"Branch coverage should be at least {MIN_BRANCH_COVERAGE}%");

        // Log coverage details
        Console.WriteLine($"✅ Line Coverage: {lineCoverage:F2}% (threshold: {MIN_LINE_COVERAGE}%)");
        Console.WriteLine($"✅ Branch Coverage: {branchCoverage:F2}% (threshold: {MIN_BRANCH_COVERAGE}%)");

        LogCoverageDetails(coverage);
        LogUncoveredMethods(coverage);
    }

    [TestMethod]
    [TestCategory("Coverage")]
    public void AnalyzeCoverageByNamespace()
    {
        var coverageFile = FindCoverageFile();
        if (coverageFile == null)
        {
            Assert.Inconclusive("No coverage file found.");
            return;
        }

        var namespaceCoverage = AnalyzeNamespaceCoverage(coverageFile);

        Console.WriteLine("Coverage by Namespace:");
        foreach (var (ns, coverage) in namespaceCoverage.OrderByDescending(x => x.Value))
        {
            var status = coverage >= MIN_LINE_COVERAGE ? "✅" : "❌";
            Console.WriteLine($"{status} {ns}: {coverage:F1}%");
        }

        // Identify namespaces that need attention
        var lowCoverageNamespaces = namespaceCoverage
            .Where(x => x.Value < MIN_LINE_COVERAGE)
            .ToList();

        if (lowCoverageNamespaces.Any())
        {
            Console.WriteLine($"\n⚠️  Namespaces below {MIN_LINE_COVERAGE}% coverage:");
            foreach (var (ns, coverage) in lowCoverageNamespaces)
            {
                Console.WriteLine($"   {ns}: {coverage:F1}%");
            }
        }
    }

    [TestMethod]
    [TestCategory("Coverage")]
    public void IdentifyTestGaps()
    {
        // Analyze test coverage and identify areas needing more tests
        var testAssembly = typeof(CoverageReport).Assembly;
        var testClasses = testAssembly.GetTypes()
            .Where(t => t.GetCustomAttributes<TestClassAttribute>().Any())
            .ToList();

        var productionAssembly = typeof(TagLib.File).Assembly;
        var publicTypes = productionAssembly.GetExportedTypes().ToList();

        var untestedTypes = publicTypes
            .Where(pt => !testClasses.Any(tc =>
                tc.Name.Contains(pt.Name, StringComparison.OrdinalIgnoreCase) ||
                tc.Namespace?.Contains(pt.Namespace ?? "", StringComparison.OrdinalIgnoreCase) == true))
            .ToList();

        Console.WriteLine($"📊 Test Coverage Analysis:");
        Console.WriteLine($"   Public Types: {publicTypes.Count}");
        Console.WriteLine($"   Test Classes: {testClasses.Count}");
        Console.WriteLine($"   Potentially Untested Types: {untestedTypes.Count}");

        if (untestedTypes.Any())
        {
            var untestedByNamespace = untestedTypes
                .GroupBy(t => t.Namespace ?? "Global")
                .OrderByDescending(g => g.Count());

            Console.WriteLine("\n📋 Types potentially lacking dedicated tests:");
            foreach (var group in untestedByNamespace)
            {
                Console.WriteLine($"\n   {group.Key}:");
                foreach (var type in group.OrderBy(t => t.Name))
                {
                    Console.WriteLine($"     - {type.Name}");
                }
            }
        }

        // This is informational - not a hard failure unless coverage is very low
        var coverageRatio = (double)(publicTypes.Count - untestedTypes.Count) / publicTypes.Count;
        coverageRatio.Should().BeGreaterThan(0.5,
            "More than half of public types should have some form of test coverage");
    }

    [TestMethod]
    [TestCategory("Coverage")]
    public void ValidateFormatSpecificCoverage()
    {
        // Ensure each supported format has adequate test coverage
        var supportedFormats = new[]
        {
            "Flac", "Mp3", "Mp4", "Ogg", "Aac", "Aiff", "Ape", "Asf", "Audible",
            "Dsf", "Matroska", "MusePack", "Riff", "WavPack", "Jpeg", "Tiff", "Gif"
        };

        var testAssembly = typeof(CoverageReport).Assembly;
        var formatTests = new Dictionary<string, int>();

        foreach (var format in supportedFormats)
        {
            var testCount = testAssembly.GetTypes()
                .Where(t => t.Name.Contains(format, StringComparison.OrdinalIgnoreCase))
                .SelectMany(t => t.GetMethods())
                .Count(m => m.GetCustomAttributes<TestMethodAttribute>().Any());

            formatTests[format] = testCount;
        }

        Console.WriteLine("📁 Format-Specific Test Coverage:");
        foreach (var (format, testCount) in formatTests.OrderByDescending(x => x.Value))
        {
            var status = testCount >= 5 ? "✅" : testCount >= 2 ? "⚠️" : "❌";
            Console.WriteLine($"{status} {format}: {testCount} test methods");
        }

        var insufficientlyTested = formatTests
            .Where(x => x.Value < 2)
            .Select(x => x.Key)
            .ToList();

        insufficientlyTested.Should().BeEmpty(
            $"All supported formats should have at least 2 test methods. Missing: {string.Join(", ", insufficientlyTested)}");
    }

    private static string? FindCoverageFile()
    {
        var possiblePaths = new[]
        {
            "../TestResults/**/coverage.opencover.xml",
            "../coverage/coverage.opencover.xml",
            "coverage.opencover.xml",
            "**/coverage.cobertura.xml",
            "**/TestResults/**/coverage.xml"
        };

        foreach (var pattern in possiblePaths)
        {
            var files = Directory.GetFiles(".", pattern, SearchOption.AllDirectories);
            if (files.Length > 0)
                return files.OrderByDescending(File.GetLastWriteTime).First();
        }

        return null;
    }

    private static async Task<CoverageData> ParseCoverageFile(string filePath)
    {
        try
        {
            var content = await File.ReadAllTextAsync(filePath);

            // Try parsing as OpenCover XML format
            if (filePath.Contains("opencover", StringComparison.OrdinalIgnoreCase))
            {
                return ParseOpenCoverXml(content);
            }

            // Try parsing as Cobertura XML format
            if (filePath.Contains("cobertura", StringComparison.OrdinalIgnoreCase))
            {
                return ParseCoberturaXml(content);
            }

            // Default to OpenCover format
            return ParseOpenCoverXml(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Failed to parse coverage file: {ex.Message}");
            return new CoverageData
            {
                LinesCovered = 0,
                LinesTotal = 1,
                BranchesCovered = 0,
                BranchesTotal = 1
            };
        }
    }

    private static CoverageData ParseOpenCoverXml(string xmlContent)
    {
        var doc = XDocument.Parse(xmlContent);

        var summary = doc.Descendants("Summary").FirstOrDefault();
        if (summary != null)
        {
            return new CoverageData
            {
                LinesCovered = int.Parse(summary.Attribute("visitedSequencePoints")?.Value ?? "0"),
                LinesTotal = int.Parse(summary.Attribute("numSequencePoints")?.Value ?? "1"),
                BranchesCovered = int.Parse(summary.Attribute("visitedBranchPoints")?.Value ?? "0"),
                BranchesTotal = int.Parse(summary.Attribute("numBranchPoints")?.Value ?? "1"),
                UncoveredMethods = ExtractUncoveredMethods(doc)
            };
        }

        return new CoverageData { LinesCovered = 0, LinesTotal = 1, BranchesCovered = 0, BranchesTotal = 1 };
    }

    private static CoverageData ParseCoberturaXml(string xmlContent)
    {
        var doc = XDocument.Parse(xmlContent);
        var coverage = doc.Element("coverage");

        if (coverage != null)
        {
            var lineRate = double.Parse(coverage.Attribute("line-rate")?.Value ?? "0");
            var branchRate = double.Parse(coverage.Attribute("branch-rate")?.Value ?? "0");
            var linesValid = int.Parse(coverage.Attribute("lines-valid")?.Value ?? "1");
            var branchesValid = int.Parse(coverage.Attribute("branches-valid")?.Value ?? "1");

            return new CoverageData
            {
                LinesCovered = (int)(lineRate * linesValid),
                LinesTotal = linesValid,
                BranchesCovered = (int)(branchRate * branchesValid),
                BranchesTotal = branchesValid
            };
        }

        return new CoverageData { LinesCovered = 0, LinesTotal = 1, BranchesCovered = 0, BranchesTotal = 1 };
    }

    private static List<string> ExtractUncoveredMethods(XDocument doc)
    {
        return doc.Descendants("Method")
            .Where(m => m.Attribute("visited")?.Value == "false")
            .Select(m =>
            {
                var className = m.Parent?.Parent?.Attribute("fullName")?.Value ?? "Unknown";
                var methodName = m.Attribute("name")?.Value ?? "Unknown";
                return $"{className}.{methodName}";
            })
            .Take(20) // Limit to top 20 uncovered methods
            .ToList();
    }

    private static Dictionary<string, double> AnalyzeNamespaceCoverage(string coverageFile)
    {
        var namespaceCoverage = new Dictionary<string, double>();

        try
        {
            var content = File.ReadAllText(coverageFile);
            var doc = XDocument.Parse(content);

            var modules = doc.Descendants("Module")
                .Where(m => m.Attribute("name")?.Value?.Contains("TaglibSharp") == true);

            foreach (var module in modules)
            {
                var classes = module.Descendants("Class");

                foreach (var cls in classes)
                {
                    var fullName = cls.Attribute("name")?.Value ?? "";
                    var namespaceName = ExtractNamespace(fullName);

                    var methods = cls.Descendants("Method");
                    var totalSequencePoints = methods.Sum(m =>
                        m.Descendants("SequencePoint").Count());
                    var visitedSequencePoints = methods.Sum(m =>
                        m.Descendants("SequencePoint").Count(sp => sp.Attribute("vc")?.Value != "0"));

                    if (totalSequencePoints > 0)
                    {
                        var coverage = (double)visitedSequencePoints / totalSequencePoints * 100;

                        if (namespaceCoverage.ContainsKey(namespaceName))
                        {
                            // Average the coverage (simplified)
                            namespaceCoverage[namespaceName] =
                                (namespaceCoverage[namespaceName] + coverage) / 2;
                        }
                        else
                        {
                            namespaceCoverage[namespaceName] = coverage;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Error analyzing namespace coverage: {ex.Message}");
        }

        return namespaceCoverage;
    }

    private static string ExtractNamespace(string fullClassName)
    {
        var lastDotIndex = fullClassName.LastIndexOf('.');
        return lastDotIndex > 0 ? fullClassName[..lastDotIndex] : "Global";
    }

    private static void LogCoverageDetails(CoverageData coverage)
    {
        Console.WriteLine($"\n📊 Detailed Coverage Report:");
        Console.WriteLine($"   Lines: {coverage.LinesCovered:N0} / {coverage.LinesTotal:N0} ({coverage.GetLineCoveragePercentage():F2}%)");
        Console.WriteLine($"   Branches: {coverage.BranchesCovered:N0} / {coverage.BranchesTotal:N0} ({coverage.GetBranchCoveragePercentage():F2}%)");
    }

    private static void LogUncoveredMethods(CoverageData coverage)
    {
        // Log methods with low coverage for targeted test improvement
        if (coverage.UncoveredMethods?.Any() == true)
        {
            Console.WriteLine($"\n🎯 Top uncovered methods (showing {Math.Min(coverage.UncoveredMethods.Count, 10)}):");
            foreach (var method in coverage.UncoveredMethods.Take(10))
            {
                Console.WriteLine($"   - {method}");
            }

            if (coverage.UncoveredMethods.Count > 10)
            {
                Console.WriteLine($"   ... and {coverage.UncoveredMethods.Count - 10} more");
            }
        }
    }

    private class CoverageData
    {
        public int LinesCovered { get; set; }
        public int LinesTotal { get; set; }
        public int BranchesCovered { get; set; }
        public int BranchesTotal { get; set; }
        public List<string>? UncoveredMethods { get; set; }

        public double GetLineCoveragePercentage() =>
            LinesTotal > 0 ? (LinesCovered * 100.0) / LinesTotal : 0;

        public double GetBranchCoveragePercentage() =>
            BranchesTotal > 0 ? (BranchesCovered * 100.0) / BranchesTotal : 0;
    }
}
