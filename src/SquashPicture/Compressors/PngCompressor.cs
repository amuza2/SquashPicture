using System.Diagnostics;
using ImageMagick;
using SquashPicture.Models;

namespace SquashPicture.Compressors;

public class PngCompressor : IImageCompressor
{
    public string[] SupportedExtensions => [".png"];

    public async Task<CompressionResult> CompressAsync(
        string inputPath,
        string? outputPath = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var originalSize = new FileInfo(inputPath).Length;
        var replaceOriginal = string.IsNullOrEmpty(outputPath);
        var targetPath = outputPath ?? inputPath;
        string? backupPath = null;

        try
        {
            if (replaceOriginal)
            {
                backupPath = Path.Combine(
                    Path.GetTempPath(),
                    $"{Guid.NewGuid()}_{Path.GetFileName(inputPath)}");
                File.Copy(inputPath, backupPath, overwrite: true);
            }
            else
            {
                File.Copy(inputPath, targetPath, overwrite: true);
            }

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var image = new MagickImage(targetPath);

                image.Strip();

                var quantize = new QuantizeSettings
                {
                    Colors = 256,
                    DitherMethod = DitherMethod.FloydSteinberg,
                    ColorSpace = ColorSpace.sRGB
                };
                image.Quantize(quantize);

                image.Settings.SetDefine(MagickFormat.Png, "compression-filter", "5");
                image.Settings.SetDefine(MagickFormat.Png, "compression-level", "9");
                image.Settings.SetDefine(MagickFormat.Png, "compression-strategy", "1");
                
                image.Write(targetPath, MagickFormat.Png8);

                cancellationToken.ThrowIfCancellationRequested();

                var optimizer = new ImageOptimizer
                {
                    OptimalCompression = true,
                    IgnoreUnsupportedFormats = true
                };
                optimizer.LosslessCompress(targetPath);

            }, cancellationToken);

            var compressedSize = new FileInfo(targetPath).Length;

            if (replaceOriginal && compressedSize >= originalSize)
            {
                File.Copy(backupPath!, targetPath, overwrite: true);
                compressedSize = originalSize;
            }

            stopwatch.Stop();

            return new CompressionResult
            {
                Success = true,
                OriginalSize = originalSize,
                CompressedSize = compressedSize,
                Duration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            if (replaceOriginal && backupPath != null && File.Exists(backupPath))
            {
                try
                {
                    File.Copy(backupPath, inputPath, overwrite: true);
                }
                catch
                {
                }
            }

            stopwatch.Stop();

            return new CompressionResult
            {
                Success = false,
                OriginalSize = originalSize,
                CompressedSize = 0,
                ErrorMessage = ex.Message,
                Duration = stopwatch.Elapsed
            };
        }
        finally
        {
            if (backupPath != null && File.Exists(backupPath))
            {
                try
                {
                    File.Delete(backupPath);
                }
                catch
                {
                }
            }
        }
    }
}
