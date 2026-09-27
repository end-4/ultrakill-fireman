using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fireman.Core.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

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

        // Load and resize texture on Unity's main thread
        await UnityMainThreadDispatcher.RunOnMainThread(() => {
            // Load original image
            var fullSizeTex = new Texture2D(2, 2);
            if (!fullSizeTex.LoadImage(fileData)) {
                Object.Destroy(fullSizeTex);
                return;
            }

            var (targetWidth, targetHeight) =
                CalculateAspectFitDimensions(fullSizeTex.width, fullSizeTex.height, width, height);

            // If smaller than max size -> just return
            if (fullSizeTex.width <= targetWidth && fullSizeTex.height <= targetHeight) {
                texture = fullSizeTex;
                return;
            }

            // Else downscale to target size
            var rt = RenderTexture.GetTemporary(targetWidth, targetHeight, 0);
            RenderTexture.active = rt;

            Graphics.Blit(fullSizeTex, rt);

            texture = new Texture2D(targetWidth, targetHeight);
            texture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
            texture.Apply();

            // Cleanup
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            Object.Destroy(fullSizeTex);
        });

        return texture;
    }

    /// <summary>
    /// Calculates the dimensions of a texture to fit within a given width and height while preserving aspect ratio.
    /// </summary>
    /// <param name="width">Original width</param>
    /// <param name="height">Original height</param>
    /// <param name="maxWidth">Max allowed width</param>
    /// <param name="maxHeight">Max allowed height</param>
    /// <returns>The target width and height of the image</returns>
    private static (int targetWidth, int targetHeight) CalculateAspectFitDimensions(
        int width, int height, int maxWidth, int maxHeight
    ) {
        float aspect = (float)width / height;

        int targetWidth = maxWidth;
        int targetHeight = Mathf.RoundToInt(maxWidth / aspect);

        if (targetHeight > maxHeight) {
            targetHeight = maxHeight;
            targetWidth = Mathf.RoundToInt(maxHeight * aspect);
        }

        return (Math.Max(1, targetWidth), Math.Max(1, targetHeight));
    }
}
