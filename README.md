<div align="center">

![SquashPicture](src/SquashPicture/Assets/icons8-image-100.png)

# SquashPicture

[![Build](https://github.com/amuza2/SquashPicture/actions/workflows/build.yml/badge.svg)](https://github.com/amuza2/SquashPicture/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/) [![Platform](https://img.shields.io/badge/Platform-Linux-orange)](https://www.linux.org/) [![Ko-fi](https://img.shields.io/badge/Ko--fi-Support-FF5E5B?logo=ko-fi)](https://ko-fi.com/codingisamazing)

A lightweight image compression tool built with Avalonia UI. SquashPicture optimizes PNG and JPEG images using lossless compression techniques while preserving image quality.

<img width="817" height="608" alt="image" src="https://github.com/user-attachments/assets/8209c261-eda1-4778-a8e3-5a85ae172dc8" />

</div>

## Features

- **Lossless Compression** - Reduces file size without losing image quality
- **PNG Optimization** - Uses quantization and optimal compression settings
- **JPEG Optimization** - Strips metadata and optimizes Huffman tables
- **Batch Processing** - Compress multiple images simultaneously
- **System Tray** - Minimizes to system tray instead of closing
- **Dark Mode** - Modern dark theme UI
- **Cross-Platform** - Runs on Linux (Windows and macOS support planned)

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Alt+A` | Add and compress images |
| `Alt+R` | Recompress all images |
| `Alt+C` | Clear file list |
| `Ctrl+Q` | Close the Window |

## Installation

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later

### Build from Source

```bash
# Clone the repository
git clone https://github.com/amuza2/SquashPicture.git
cd SquashPicture

# Build the project
dotnet build

# Run the application
dotnet run --project src/SquashPicture
```

### Create Self-Contained Build

```bash
# Linux x64
dotnet publish src/SquashPicture -c Release -r linux-x64 --self-contained -o publish/linux-x64
```

## Usage

1. **Add Images** - Click "Add" to select images via file dialog
2. **View Results** - The table shows original size, new size, and compression ratio
3. **Recompress** - Click "Recompress" to re-process all images in the list
4. **Clear** - Click "Clear" to remove all images from the list

### Supported Formats

- PNG (`.png`)
- JPEG (`.jpg`, `.jpeg`)

## How It Works

### PNG Compression
- Quantizes colors to 256-color palette with Floyd-Steinberg dithering
- Applies maximum compression level (9)
- Uses optimal compression filter and strategy
- Performs lossless optimization pass

### JPEG Compression
- Strips all metadata (EXIF, ICC profiles, etc.)
- Optimizes Huffman tables
- Converts to progressive JPEG
- Performs lossless optimization

### Backup Strategy
- Creates temporary backup before compression
- Restores original if compressed file is larger
- Ensures no data loss on failure

## Project Structure

```
SquashPicture/
├── src/
│   └── SquashPicture/
│       ├── Assets/           # Icons and resources
│       ├── Compressors/      # PNG and JPEG compressors
│       ├── Converters/       # Value converters for UI
│       ├── Models/           # Data models
│       ├── Services/         # Business logic services
│       ├── Styles/           # XAML style resources
│       ├── ViewModels/       # MVVM view models
│       └── Views/            # XAML views
└── tests/
    └── SquashPicture.Tests/  # Unit tests
```

## Configuration

Settings are stored in:
- **Linux**: `~/.config/SquashPicture/settings.json`

Settings include:
- Window position and size
- Last used directory for file dialogs


## Dependencies

- [Avalonia UI](https://avaloniaui.net/) - Cross-platform UI framework
- [Magick.NET](https://github.com/dlemstra/Magick.NET) - ImageMagick wrapper for .NET
- [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp) - Image processing library
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) - MVVM toolkit

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome!

1. **Open an issue** first to discuss the proposed change
2. **Fork** the repository
3. **Create a branch** for your feature or fix
4. **Submit a Pull Request** referencing the issue

## Acknowledgments

- Icon by [Icons8](https://icons8.com/)

<div align="center">
Made with ❤️ for the Linux community
</div>
