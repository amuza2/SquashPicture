using FluentAssertions;
using SquashPicture.Models;

namespace SquashPicture.Tests.Services;

public class SettingsServiceTests
{
    [Fact]
    public void AppSettings_HasCorrectDefaults()
    {
        // Arrange & Act
        var settings = new AppSettings();

        // Assert
        settings.WindowWidth.Should().Be(600);
        settings.WindowHeight.Should().Be(400);
        settings.WindowX.Should().Be(100);
        settings.WindowY.Should().Be(100);
        settings.LastDirectory.Should().BeEmpty();
        settings.MinimizeToTray.Should().BeTrue();
    }

    [Fact]
    public void AppSettings_CanBeModified()
    {
        // Arrange
        var settings = new AppSettings();

        // Act
        settings.WindowWidth = 1024;
        settings.WindowHeight = 768;
        settings.WindowX = 50;
        settings.WindowY = 75;
        settings.LastDirectory = "/home/user/Documents";
        settings.MinimizeToTray = false;

        // Assert
        settings.WindowWidth.Should().Be(1024);
        settings.WindowHeight.Should().Be(768);
        settings.WindowX.Should().Be(50);
        settings.WindowY.Should().Be(75);
        settings.LastDirectory.Should().Be("/home/user/Documents");
        settings.MinimizeToTray.Should().BeFalse();
    }
}
