namespace SquashPicture.Models;

public class AppSettings
{
    public double WindowWidth { get; set; } = 600;
    public double WindowHeight { get; set; } = 400;
    public double WindowX { get; set; } = 100;
    public double WindowY { get; set; } = 100;
    public string LastDirectory { get; set; } = string.Empty;
    public bool MinimizeToTray { get; set; } = true;
}
