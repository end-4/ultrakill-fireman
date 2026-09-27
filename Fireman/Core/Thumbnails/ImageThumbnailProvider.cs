using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fireman.Core.Utils;
using UnityEngine;

namespace Fireman.Core.Thumbnails;

/// <summary>
/// Thumbnail provider for images
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
        Texture2D texture = null;
        byte[] fileData = await Task.Run(() => File.ReadAllBytes(filePath));

        // Load tex on Unity's main thread
        await UnityMainThreadDispatcher.RunOnMainThread(() => {
            texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            // ImageConversion.LoadImage must run on the Main Thread
            if (!texture.LoadImage(fileData)) {
                Object.Destroy(texture);
                texture = null;
            }
        });

        return texture;
    }
}
