namespace SquashPicture.Models;

public class ImageFile
{
    public string FullPath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long OriginalSize { get; set; }
    public long CompressedSize { get; set; }
    public CompressionStatus Status { get; set; } = CompressionStatus.Queued;
    public bool IsRecompression { get; set; }
    public string? ErrorMessage { get; set; }

    public double CompressionRatio => OriginalSize > 0
        ? (1 - (double)CompressedSize / OriginalSize) * 100
        : 0;

    public bool IsValid => !string.IsNullOrEmpty(FullPath)
        && File.Exists(FullPath)
        && IsSupportedExtension(Extension);

    private static bool IsSupportedExtension(string ext) =>
        ext.ToLowerInvariant() is ".png" or ".jpg" or ".jpeg";
}
