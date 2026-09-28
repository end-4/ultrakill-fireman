using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Fireman.Core.Utils;
using UnityEngine;

namespace Fireman.Core.Thumbnails;

/// <summary>
/// Thumbnail provider for osu! beatmap packages (.osz)
/// </summary>
public class OsuBeatmapThumbnailProvider : IThumbnailProvider {
    /// <inheritdoc />
    public int Priority => 1;

    private static readonly string[] ImageExtensions = [".png", ".jpg", ".jpeg"];

    /// <inheritdoc />
    public bool CanHandle(string filePath) {
        string ext = Path.GetExtension(filePath).ToLowerInvariant();
        return ext == ".osz";
    }

    /// <inheritdoc />
    public async Task<Texture2D> GenerateThumbnailAsync(string filePath, int width, int height) {
        var imageBytes = await Task.Run(() => ExtractThumbnailBytes(filePath));
        if (imageBytes == null || imageBytes.Length == 0) return null!;

        var texture = await TextureUtils.GenerateFromBytesAsync(imageBytes, width, height);
        return texture!;
    }

    private static byte[]? ExtractThumbnailBytes(string filePath) {
        if (!File.Exists(filePath)) return null;

        try {
            // .osz is just a zip
            using var archive = ZipFile.OpenRead(filePath);

            // The thumbnail of a beatmap is an arbitrary image file in the root of the zip
            var imageEntry = archive.Entries.FirstOrDefault(e => {
                if (e.FullName.Contains('/') || e.FullName.Contains('\\')) return false;

                var ext = Path.GetExtension(e.Name).ToLowerInvariant();
                return ImageExtensions.Contains(ext);
            });

            if (imageEntry == null) return null;

            using var stream = imageEntry.Open();
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        } catch {
            return null;
        }
    }
}
