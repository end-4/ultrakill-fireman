using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fireman.Core.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fireman.Core.Thumbnails;

/// <summary>
/// Thumbnail provider for Cybergrind pattern files (.cgp)
/// </summary>
public class CybergrindPatternThumbnailProvider : IThumbnailProvider {
    /// <inheritdoc />
    public int Priority => 0;

    private const int PatternWidth = 16;
    private const int PatternHeight = 16;
    private const float MinGreyIntensity = 0.15f;
    private const float MaxGreyIntensity = 0.95f;

    /// <inheritdoc />
    public bool CanHandle(string filePath) {
        string ext = Path.GetExtension(filePath).ToLowerInvariant();
        return ext == ".cgp";
    }

    /// <inheritdoc />
    public async Task<Texture2D> GenerateThumbnailAsync(string filePath, int width, int height) {
        var heights = await Task.Run(() => ReadPatternHeights(filePath));
        if (heights == null) return null!;

        Texture2D? texture = null;

        await UnityMainThreadDispatcher.RunOnMainThread(() => {
            var baseTex = CreateHeightMapTexture(heights);
            baseTex.filterMode = FilterMode.Point; // Prevent blurry cells when scaled

            var (targetWidth, targetHeight) =
                TextureUtils.CalculateAspectFitDimensions(baseTex.width, baseTex.height, width, height);

            texture = TextureUtils.Resize(baseTex, targetWidth, targetHeight);

            // Cleanup
            Object.Destroy(baseTex);
        });

        return texture;
    }

    /// <summary>
    /// Reads the file and extracts the 16x16 array of column heights from the top section.
    /// </summary>
    private static int[,]? ReadPatternHeights(string filePath) {
        if (!File.Exists(filePath)) return null;

        string[] lines = File.ReadAllLines(filePath);
        int[,] grid = new int[PatternHeight, PatternWidth];

        var numberRegex = new Regex(@"\(([-+]?\d+)\)|(\d)");

        try {
            for (int row = 0; row < PatternHeight; row++) {
                string line = lines[row].Trim();
                var matches = numberRegex.Matches(line);
                if (matches.Count < PatternWidth) return null;

                for (int col = 0; col < PatternWidth; col++) {
                    var match = matches[col];
                    string valStr = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;

                    if (int.TryParse(valStr, out int val)) {
                        grid[row, col] = val;
                    }
                }
            }
        } catch {
            return null;
        }
        return grid;
    }

    /// <summary>
    /// Maps the height values into Texture2D
    /// </summary>
    private static Texture2D CreateHeightMapTexture(int[,] heights) {
        var tex = new Texture2D(PatternWidth, PatternHeight, TextureFormat.RGBA32, false);

        int minHeight = int.MaxValue;
        int maxHeight = int.MinValue;

        for (int y = 0; y < PatternHeight; y++) {
            for (int x = 0; x < PatternWidth; x++) {
                int h = heights[y, x];
                if (h < minHeight) minHeight = h;
                if (h > maxHeight) maxHeight = h;
            }
        }

        // We will normalize heights and use greyscale values in a fixed range
        // This will look wrong compared to vanilla but it's more readable
        float range = maxHeight - minHeight;

        for (int y = 0; y < PatternHeight; y++) {
            for (int x = 0; x < PatternWidth; x++) {
                int heightVal = heights[y, x];

                // Normalization here
                float normalized = (range == 0) ? 0.5f : ((heightVal - minHeight) / range);
                float intensity = Mathf.Lerp(MinGreyIntensity, MaxGreyIntensity, normalized);

                // We invert y because texture origin is bottom-left, while grid array origin is top-left
                tex.SetPixel(x, PatternHeight - 1 - y, new Color(intensity, intensity, intensity, 1f));
            }
        }

        tex.Apply();
        return tex;
    }
}
