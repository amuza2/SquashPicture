using System.Globalization;
using Avalonia.Data.Converters;

namespace SquashPicture.Converters;

public class FileSizeConverter : IValueConverter
{
    public static readonly FileSizeConverter Instance = new();

    private static readonly string[] SizeSuffixes = ["B", "KB", "MB", "GB"];

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not long bytes || bytes <= 0)
            return "-";

        var order = 0;
        double size = bytes;

        while (size >= 1024 && order < SizeSuffixes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {SizeSuffixes[order]}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
