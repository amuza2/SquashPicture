using System.Globalization;
using FluentAssertions;
using SquashPicture.Converters;
using SquashPicture.Models;

namespace SquashPicture.Tests.Converters;

public class CompressionRatioConverterTests
{
    private readonly CompressionRatioConverter _converter = new();

    [Fact]
    public void Convert_WithCompletedStatusAndValidRatio_ReturnsPercentage()
    {
        // Arrange - Status first, then ratio (as per actual converter)
        object?[] values = [CompressionStatus.Completed, 20.0];

        // Act
        var result = _converter.Convert(values, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be("20.0%");
    }

    [Fact]
    public void Convert_WithQueuedStatus_ReturnsQueued()
    {
        // Arrange
        object?[] values = [CompressionStatus.Queued, 0.0];

        // Act
        var result = _converter.Convert(values, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be("Queued");
    }

    [Fact]
    public void Convert_WithCompressingStatus_ReturnsCompressing()
    {
        // Arrange
        object?[] values = [CompressionStatus.Compressing, 0.0];

        // Act
        var result = _converter.Convert(values, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be("Compressing...");
    }

    [Fact]
    public void Convert_WithErrorStatus_ReturnsError()
    {
        // Arrange
        object?[] values = [CompressionStatus.Error, 0.0];

        // Act
        var result = _converter.Convert(values, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be("Error");
    }

    [Fact]
    public void Convert_WithNegativeRatio_ReturnsNegativePercentage()
    {
        // Arrange - file got larger
        object?[] values = [CompressionStatus.Completed, -20.0];

        // Act
        var result = _converter.Convert(values, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be("-20.0%");
    }

    [Fact]
    public void Convert_WithInsufficientValues_ReturnsDash()
    {
        // Arrange
        object?[] values = [CompressionStatus.Completed];

        // Act
        var result = _converter.Convert(values, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be("-");
    }
}
