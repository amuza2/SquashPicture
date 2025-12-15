namespace SquashPicture.Models;

public class CompressionResult
{
    public bool Success { get; set; }
    public long OriginalSize { get; set; }
    public long CompressedSize { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan Duration { get; set; }
}
