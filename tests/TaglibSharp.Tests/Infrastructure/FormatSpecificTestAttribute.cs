using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TaglibSharp.Tests.Infrastructure;

/// <summary>
/// Test attribute for format-specific tests that automatically categorizes tests by audio format
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class FormatSpecificTestAttribute : TestCategoryBaseAttribute
{
    public FormatSpecificTestAttribute(string format)
    {
        Format = format;
        TestCategories = new[] { $"Format.{format}", TestCategories.Integration };
    }

    public string Format { get; }
}

/// <summary>
/// Test attribute for tests requiring large test files or long execution times
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequiresLargeFileAttribute : TestCategoryBaseAttribute
{
    public RequiresLargeFileAttribute()
    {
        TestCategories = new[] { TestCategories.LongRunning, TestCategories.RequiresTestFiles };
    }
}

/// <summary>
/// Test attribute for memory-intensive tests that validate allocation patterns
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class MemoryTestAttribute : TestCategoryBaseAttribute
{
    public MemoryTestAttribute()
    {
        TestCategories = new[] { TestCategories.Performance, "Memory" };
    }
}

/// <summary>
/// Test attribute for async operation tests
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AsyncTestAttribute : TestCategoryBaseAttribute
{
    public AsyncTestAttribute()
    {
        TestCategories = new[] { "Async", TestCategories.Unit };
    }
}
