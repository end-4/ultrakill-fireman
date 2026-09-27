using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Fireman.Core.Thumbnails;

/// <summary>
/// Manages file thumbnails
/// </summary>
public static class ThumbnailManager {
    private static readonly List<IThumbnailProvider> _providers = new List<IThumbnailProvider>();
    private static readonly Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();

    static ThumbnailManager() {
        RegisterProvider(new ImageThumbnailProvider());
    }

    /// <summary>
    /// Registers a thumbnail provider
    /// </summary>
    /// <param name="provider">The provider</param>
    public static void RegisterProvider(IThumbnailProvider provider) {
        _providers.Add(provider);
        _providers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    /// <summary>
    /// Requests a thumbnail for a given path. Returns cached texture if present.
    /// </summary>
    public static async Task<Texture2D?> GetThumbnailAsync(string filePath, int width = 128, int height = 128) {
        if (string.IsNullOrEmpty(filePath)) return null;

        // Cache hit -> return it
        if (_cache.TryGetValue(filePath, out var cachedTex) && cachedTex != null) return cachedTex;

        // Cache miss -> try to generate
        var provider = _providers.FirstOrDefault(p => p.CanHandle(filePath));
        if (provider == null) return null;
        var thumbnail = await provider.GenerateThumbnailAsync(filePath, width, height);
        if (thumbnail != null) _cache[filePath] = thumbnail;

        return thumbnail;
    }

    /// <summary>
    /// Clears the thumbnail cache
    /// </summary>
    public static void ClearCache() {
        foreach (var tex in _cache.Values) {
            if (tex != null) Object.Destroy(tex);
        }

        _cache.Clear();
    }
}
