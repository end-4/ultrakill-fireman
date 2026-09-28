using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fireman.Core.Utils;
using UnityEngine;

namespace Fireman.Core.Thumbnails;

/// <summary>
/// Thumbnail provider for images (PNG, JPEG)
/// </summary>
public class ImageThumbnailProvider : IThumbnailProvider {
    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public bool CanHandle(string filePath) {
        string ext = Path.GetExtension(filePath).ToLowerInvariant();
        string[] supported = [".png", ".jpg", ".jpeg"];
        return supported.Any(se => ext == se);
    }

    /// <inheritdoc />
    public async Task<Texture2D> GenerateThumbnailAsync(string filePath, int width, int height) {
        if (!File.Exists(filePath)) return null!;

        byte[] fileData = await Task.Run(() => File.ReadAllBytes(filePath));
        var texture = await TextureUtils.GenerateFromBytesAsync(fileData, width, height);

        return texture!;
    }
}
