using FluentAssertions;
using SquashPicture.Compressors;
using SquashPicture.Tests.Helpers;

namespace SquashPicture.Tests.Compressors;

public class JpegCompressorTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly JpegCompressor _compressor;

    public JpegCompressorTests()
    {
        _testDirectory = TestImageGenerator.CreateTempDirectory();
        _compressor = new JpegCompressor();
    }

    public void Dispose()
    {
        TestImageGenerator.CleanupDirectory(_testDirectory);
    }

    [Fact]
    public void SupportedExtensions_ContainsJpgAndJpeg()
    {
        // Assert
        _compressor.SupportedExtensions.Should().Contain(".jpg");
        _compressor.SupportedExtensions.Should().Contain(".jpeg");
    }

    [Fact]
    public async Task CompressAsync_WithValidJpeg_ReturnsSuccessResult()
    {
        // Arrange
        var testFile = TestImageGenerator.CreateTestJpeg(_testDirectory, "test.jpg", 200, 200);
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
        var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.jpg");

        // Act & Assert - Compressor throws FileNotFoundException when file doesn't exist
        var action = async () => await _compressor.CompressAsync(nonExistentFile, null, CancellationToken.None);
        await action.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task CompressAsync_WithCorruptFile_ReturnsErrorResult()
    {
        // Arrange
        var corruptFile = TestImageGenerator.CreateCorruptFile(_testDirectory, "corrupt.jpg");

        // Act
        var result = await _compressor.CompressAsync(corruptFile, null, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }
}
