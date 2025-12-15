using SquashPicture.Models;

namespace SquashPicture.Services.Interfaces;

public interface ISettingsService
{
    AppSettings Settings { get; }
    void Load();
    void Save();
}
