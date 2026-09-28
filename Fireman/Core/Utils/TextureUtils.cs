using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fireman.Core.Utils;

/// <summary>
/// Utility methods for textures
/// </summary>
public static class TextureUtils {
    /// <summary>
    /// Calculates the dimensions of a texture to fit within a given width and height while preserving aspect ratio.
    /// </summary>
    /// <param name="width">Original width</param>
    /// <param name="height">Original height</param>
    /// <param name="maxWidth">Max allowed width</param>
    /// <param name="maxHeight">Max allowed height</param>
    /// <returns>The target width and height of the image</returns>
    public static (int targetWidth, int targetHeight) CalculateAspectFitDimensions(
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

    /// <summary>
    /// Resizes a Texture2D to given dimensions
    /// </summary>
    /// <param name="source">The original Texture2D</param>
    /// <param name="targetWidth">Target width</param>
    /// <param name="targetHeight">Target height</param>
    /// <param name="filterMode">Filter mode for the scaling</param>
    /// <returns>The Texture2D scaled to fit the given size</returns>
    public static Texture2D Resize(
        Texture2D source,
        int targetWidth,
        int targetHeight,
        FilterMode? filterMode = null
    ) {
        FilterMode mode = filterMode ?? source.filterMode;
        var rt = RenderTexture.GetTemporary(targetWidth, targetHeight, 0);
        rt.filterMode = mode;
        RenderTexture.active = rt;

        Graphics.Blit(source, rt);

        var texture = new Texture2D(targetWidth, targetHeight) {
            filterMode = mode
        };
        texture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return texture;
    }

    /// <summary>
    /// Creates 2D texture from a byte array
    /// </summary>
    /// <param name="fileData">The byte array</param>
    /// <param name="maxWidth">Max image width</param>
    /// <param name="maxHeight">Max image height</param>
    /// <returns></returns>
    public static async Task<Texture2D?> GenerateFromBytesAsync(byte[] fileData, int maxWidth, int maxHeight) {
        Texture2D? texture = null;

        await UnityMainThreadDispatcher.RunOnMainThread(() => {
            var fullSizeTex = new Texture2D(2, 2);
            if (!fullSizeTex.LoadImage(fileData)) {
                Object.Destroy(fullSizeTex);
                return;
            }

            var (targetWidth, targetHeight) =
                CalculateAspectFitDimensions(fullSizeTex.width, fullSizeTex.height, maxWidth, maxHeight);

            if (fullSizeTex.width <= targetWidth && fullSizeTex.height <= targetHeight) {
                texture = fullSizeTex;
                return;
            }

            texture = Resize(fullSizeTex, targetWidth, targetHeight);
            Object.Destroy(fullSizeTex);
        });

        return texture;
    }
}
