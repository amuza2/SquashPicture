using FluentAssertions;
using SquashPicture.Converters;

namespace SquashPicture.Tests.Converters;

public class FileSizeConverterTests
{
    private readonly FileSizeConverter _converter = new();

    [Theory]
    [InlineData(500L, "500 B")]
    [InlineData(1023L, "1023 B")]
    [InlineData(1024L, "1 KB")]
    [InlineData(1536L, "1.5 KB")]
    [InlineData(10240L, "10 KB")]
    [InlineData(1048576L, "1 MB")]
    [InlineData(1572864L, "1.5 MB")]
    [InlineData(1073741824L, "1 GB")]
    public void Convert_WithValidBytes_ReturnsFormattedString(long bytes, string expected)
    {
        // Act
        var result = _converter.Convert(bytes, typeof(string), null!, null!);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Convert_WithNull_ReturnsDash()
    {
        // Act
        var result = _converter.Convert(null, typeof(string), null!, null!);

        // Assert
        result.Should().Be("-");
    }

    [Fact]
    public void Convert_WithNonLongValue_ReturnsDash()
    {
        // Act
        var result = _converter.Convert("not a number", typeof(string), null!, null!);

        // Assert
        result.Should().Be("-");
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Act & Assert
        var action = () => _converter.ConvertBack("1 KB", typeof(long), null!, null!);
        action.Should().Throw<NotImplementedException>();
    }
}
