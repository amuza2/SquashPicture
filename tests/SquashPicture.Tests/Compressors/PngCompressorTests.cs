using FluentAssertions;
using SquashPicture.Compressors;
using SquashPicture.Tests.Helpers;

namespace SquashPicture.Tests.Compressors;

public class PngCompressorTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly PngCompressor _compressor;

    public PngCompressorTests()
    {
        _testDirectory = TestImageGenerator.CreateTempDirectory();
        _compressor = new PngCompressor();
    }

    public void Dispose()
    {
        TestImageGenerator.CleanupDirectory(_testDirectory);
    }

    [Fact]
    public void SupportedExtensions_ContainsPng()
    {
        // Assert
        _compressor.SupportedExtensions.Should().Contain(".png");
    }

    [Fact]
    public async Task CompressAsync_WithValidPng_ReturnsSuccessResult()
    {
        // Arrange
        var testFile = TestImageGenerator.CreateTestPng(_testDirectory, "test.png", 200, 200);
        var originalSize = new FileInfo(testFile).Length;

        // Act
        var result = await _compressor.CompressAsync(testFile, null, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.OriginalSize.Should().Be(originalSize);
        result.CompressedSize.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CompressAsync_WithNonExistentFile_ThrowsOrReturnsError()
    {
        // Arrange
        var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.png");

        // Act & Assert - Compressor throws FileNotFoundException when file doesn't exist
        var action = async () => await _compressor.CompressAsync(nonExistentFile, null, CancellationToken.None);
        await action.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task CompressAsync_WithCorruptFile_ReturnsErrorResult()
    {
        // Arrange
        var corruptFile = TestImageGenerator.CreateCorruptFile(_testDirectory, "corrupt.png");

        // Act
        var result = await _compressor.CompressAsync(corruptFile, null, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CompressAsync_WithCancellation_ThrowsOrReturnsError()
    {
        // Arrange
        var testFile = TestImageGenerator.CreateTestPng(_testDirectory, "test.png", 500, 500);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var action = async () => await _compressor.CompressAsync(testFile, null, cts.Token);
        
        // Should either throw OperationCanceledException or return error result
        try
        {
            var result = await action();
            result.Success.Should().BeFalse();
        }
        catch (OperationCanceledException)
        {
            // Expected behavior
        }
    }
}
