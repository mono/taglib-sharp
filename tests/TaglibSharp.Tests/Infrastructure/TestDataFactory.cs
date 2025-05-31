using Bogus;
using System.IO.Abstractions.TestingHelpers;

namespace TaglibSharp.Tests.Infrastructure;

/// <summary>
/// Factory for creating test data and mock file systems for testing
/// </summary>
public static class TestDataFactory
{
    private static readonly Faker _faker = new();

    /// <summary>
    /// Creates a mock file system with test media files
    /// </summary>
    public static MockFileSystem CreateMockFileSystem()
    {
        var fileSystem = new MockFileSystem();

        // Add various format test files
        fileSystem.AddFile(@"c:\test\test.mp3", CreateMp3TestData());
        fileSystem.AddFile(@"c:\test\test.flac", CreateFlacTestData());
        fileSystem.AddFile(@"c:\test\test.ogg", CreateOggTestData());
        fileSystem.AddFile(@"c:\test\corrupted.mp3", CreateCorruptedFileData());
        fileSystem.AddFile(@"c:\test\empty.mp3", new MockFileData(Array.Empty<byte>()));

        return fileSystem;
    }

    /// <summary>
    /// Creates test tag data with random but valid values
    /// </summary>
    public static TagLib.Tag CreateTestTag()
    {
        // This would need to be implemented based on actual TagLib.Tag implementation
        // For now, showing the pattern
        var tag = new MockTag();
        tag.Title = _faker.Lorem.Sentence(3);
        tag.Artist = _faker.Name.FullName();
        tag.Album = _faker.Lorem.Sentence(2);
        tag.Year = (uint)_faker.Date.Past(50).Year;
        tag.Track = (uint)_faker.Random.Number(1, 20);
        tag.Genre = _faker.Music.Genre();

        return tag;
    }

    /// <summary>
    /// Creates property-based test generators for FsCheck
    /// </summary>
    public static class Generators
    {
        public static Gen<string> ValidFileName =>
            from name in Gen.Elements("test", "audio", "music", "track")
            from ext in Gen.Elements(".mp3", ".flac", ".ogg", ".m4a")
            select $"{name}{ext}";

        public static Gen<byte[]> ValidMp3Header =>
            Gen.Constant(new byte[] { 0xFF, 0xFB, 0x90, 0x00 }); // MP3 sync word

        public static Gen<TagLib.Tag> ValidTag =>
            from title in Gen.NonEmptyString
            from artist in Gen.NonEmptyString
            from album in Gen.NonEmptyString
            from year in Gen.Choose(1900, 2024)
            select CreateTagWithValues(title, artist, album, (uint)year);
    }

    private static MockFileData CreateMp3TestData()
    {
        // Create minimal valid MP3 file structure
        var data = new List<byte>();
        data.AddRange(new byte[] { 0xFF, 0xFB, 0x90, 0x00 }); // MP3 sync word
        data.AddRange(_faker.Random.Bytes(1024)); // Audio data
        return new MockFileData(data.ToArray());
    }

    private static MockFileData CreateFlacTestData()
    {
        // Create minimal valid FLAC file structure
        var data = new List<byte>();
        data.AddRange("fLaC"u8.ToArray()); // FLAC signature
        data.AddRange(_faker.Random.Bytes(512)); // Metadata blocks and audio
        return new MockFileData(data.ToArray());
    }

    private static MockFileData CreateOggTestData()
    {
        // Create minimal valid OGG file structure
        var data = new List<byte>();
        data.AddRange("OggS"u8.ToArray()); // OGG signature
        data.AddRange(_faker.Random.Bytes(256)); // OGG pages
        return new MockFileData(data.ToArray());
    }

    private static MockFileData CreateCorruptedFileData()
    {
        // Create intentionally corrupted data for error testing
        return new MockFileData(_faker.Random.Bytes(100));
    }

    private static TagLib.Tag CreateTagWithValues(string title, string artist, string album, uint year)
    {
        // Implementation depends on actual TagLib.Tag structure
        var tag = new MockTag();
        // Set properties...
        return tag;
    }
}

/// <summary>
/// Mock implementation for testing purposes
/// </summary>
internal class MockTag : TagLib.Tag
{
    // Implementation would mirror actual Tag class for testing
}
