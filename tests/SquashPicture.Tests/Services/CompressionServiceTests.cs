using FluentAssertions;
using SquashPicture.Services;
using SquashPicture.Tests.Helpers;

namespace SquashPicture.Tests.Services;

public class CompressionServiceTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly CompressionService _service;

    public CompressionServiceTests()
    {
        _testDirectory = TestImageGenerator.CreateTempDirectory();
        _service = new CompressionService();
    }

    public void Dispose()
    {
        TestImageGenerator.CleanupDirectory(_testDirectory);
    }

    [Fact]
    public async Task CompressAsync_WithPngFile_UsesCorrectCompressor()
    {
        // Arrange
        var testFile = TestImageGenerator.CreateTestPng(_testDirectory, "test.png", 100, 100);

        // Act
        var result = await _service.CompressAsync(testFile, null, null, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CompressAsync_WithJpegFile_UsesCorrectCompressor()
    {
        // Arrange
        var testFile = TestImageGenerator.CreateTestJpeg(_testDirectory, "test.jpg", 100, 100);

        // Act
        var result = await _service.CompressAsync(testFile, null, null, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CompressAsync_WithUnsupportedFormat_ReturnsError()
    {
        // Arrange
        var unsupportedFile = Path.Combine(_testDirectory, "test.bmp");
        File.WriteAllText(unsupportedFile, "fake bmp content");

        // Act
        var result = await _service.CompressAsync(unsupportedFile, null, null, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Unsupported");
    }

    [Fact]
    public async Task CompressAsync_WithNonExistentFile_ReturnsError()
    {
        // Arrange
        var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.png");

        // Act
        var result = await _service.CompressAsync(nonExistentFile, null, null, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }
}
