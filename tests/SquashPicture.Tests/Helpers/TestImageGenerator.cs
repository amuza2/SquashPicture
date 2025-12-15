using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SquashPicture.Tests.Helpers;

public static class TestImageGenerator
{
    public static string CreateTestPng(string directory, string fileName = "test.png", int width = 100, int height = 100)
    {
        var filePath = Path.Combine(directory, fileName);
        
        using var image = new Image<Rgba32>(width, height);
        
        // Fill with a gradient pattern (compressible)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                image[x, y] = new Rgba32((byte)(x * 255 / width), (byte)(y * 255 / height), 128, 255);
            }
        }
        
        image.SaveAsPng(filePath);
        return filePath;
    }
    
    public static string CreateTestJpeg(string directory, string fileName = "test.jpg", int width = 100, int height = 100)
    {
        var filePath = Path.Combine(directory, fileName);
        
        using var image = new Image<Rgba32>(width, height);
        
        // Fill with a gradient pattern
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                image[x, y] = new Rgba32((byte)(x * 255 / width), (byte)(y * 255 / height), 128, 255);
            }
        }
        
        image.SaveAsJpeg(filePath);
        return filePath;
    }
    
    public static string CreateCorruptFile(string directory, string fileName = "corrupt.png")
    {
        var filePath = Path.Combine(directory, fileName);
        File.WriteAllBytes(filePath, [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0xFF, 0xFF, 0xFF]);
        return filePath;
    }
    
    public static string CreateTempDirectory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"SquashPictureTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        return tempDir;
    }
    
    public static void CleanupDirectory(string directory)
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
