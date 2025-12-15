#!/bin/bash

# SquashPicture Release Build Script
# Usage: ./scripts/build-release.sh

set -e

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUTPUT_DIR="$PROJECT_DIR/publish"
PROJECT_FILE="$PROJECT_DIR/src/SquashPicture/SquashPicture.csproj"

echo "=== SquashPicture Release Build ==="
echo "Project: $PROJECT_FILE"
echo "Output: $OUTPUT_DIR"
echo ""

# Clean previous builds
echo "Cleaning previous builds..."
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

# Build for Linux x64
echo ""
echo "Building for Linux x64..."
dotnet publish "$PROJECT_FILE" \
    -c Release \
    -r linux-x64 \
    --self-contained \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -o "$OUTPUT_DIR/linux-x64"

echo ""
echo "=== Build Complete ==="
echo ""
echo "Output files:"
ls -lh "$OUTPUT_DIR/linux-x64/SquashPicture"

echo ""
echo "To run: $OUTPUT_DIR/linux-x64/SquashPicture"
