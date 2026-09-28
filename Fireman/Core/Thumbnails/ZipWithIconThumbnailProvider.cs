using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Fireman.Core.Utils;
using UnityEngine;

namespace Fireman.Core.Thumbnails;

/// <summary>
/// Thumbnail provider for ZIP archives with an icon.png at the root like Angry levels and Thunderstore packages
/// </summary>
public class ZipWithIconThumbnailProvider : IThumbnailProvider {
    /// <inheritdoc />
    public int Priority => 0;

    private static readonly string[] SupportedExtensions = [".zip", ".angry"];

    /// <inheritdoc />
    public bool CanHandle(string filePath) {
        string ext = Path.GetExtension(filePath).ToLowerInvariant();
        return SupportedExtensions.Contains(ext);
    }

    /// <inheritdoc />
    public async Task<Texture2D> GenerateThumbnailAsync(string filePath, int width, int height) {
        byte[]? iconBytes = await Task.Run(() => ExtractIconBytes(filePath));
        if (iconBytes == null || iconBytes.Length == 0) return null!;

        var texture = await TextureUtils.GenerateFromBytesAsync(iconBytes, width, height);
        return texture!;
    }

    /// <summary>
    /// Opens the zip archive and reads the root icon.png into a byte array.
    /// </summary>
    private byte[]? ExtractIconBytes(string filePath) {
        if (!File.Exists(filePath)) return null;

        try {
            using var archive = ZipFile.OpenRead(filePath);

            var iconEntry = archive.Entries.FirstOrDefault(e =>
                string.Equals(e.FullName, "icon.png", StringComparison.OrdinalIgnoreCase)
            );

            if (iconEntry == null) return null;

            using var stream = iconEntry.Open();
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        } catch {
            return null;
        }
    }
}
