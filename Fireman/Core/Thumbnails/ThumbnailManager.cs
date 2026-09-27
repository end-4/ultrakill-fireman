using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NukeLib.Utils;
using UnityEngine;

namespace Fireman.Core.Thumbnails;

/// <summary>
/// Manages file thumbnails
/// </summary>
public static class ThumbnailManager {
    private const int DefaultCacheCapacity = 300;
    private static readonly List<IThumbnailProvider> _providers = new();
    private static readonly LRUCache<string, Texture2D> _cache = new(DefaultCacheCapacity, OnTextureUncached);

    /// <summary>
    /// Makes the cache size at least as big as the given size
    /// </summary>
    /// <param name="size">The cache size</param>
    public static void BumpCacheSize(int size) {
        if (size > _cache.MaxSize) {
            _cache.MaxSize = size;
        }
    }

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
    /// Requests a thumbnail texture for a given path. Returns cached texture if present.
    /// </summary>
    public static async Task<Texture2D?> GetThumbnailAsync(string filePath, int width = 128, int height = 128) {
        if (string.IsNullOrEmpty(filePath)) return null;

    int targetSize = GetNormalizedThumbnailSize(Math.Max(width, height));
        string cacheKey = $"{filePath}@{targetSize}";

        // Cache hit -> return it
        if (_cache.TryGetValue(cacheKey, out var cachedTex) && cachedTex != null) return cachedTex;

        // Cache miss -> try to generate
        var provider = _providers.FirstOrDefault(p => p.CanHandle(filePath));
        if (provider == null) return null;

        var thumbnail = await provider.GenerateThumbnailAsync(filePath, targetSize, targetSize);
        if (thumbnail != null) _cache.Add(cacheKey, thumbnail);

        return thumbnail;
    }

    /// <summary>
    /// Requests a sprite thumbnail for a given path.
    /// </summary>
    public static async Task<Sprite?> GetSpriteThumbnailAsync(string filePath, int width = 128, int height = 128) {
        var texture = await GetThumbnailAsync(filePath, width, height);
        if (texture == null) return null;

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }

    /// <summary>
    /// Clears the thumbnail cache
    /// </summary>
    public static void ClearCache() {
        _cache.Clear();
    }

    /// <summary>
    /// Normalizes requested dimension according to Freedesktop thumbnail specs
    /// Which is the next-largest in 128, 256, 512, 1024
    /// </summary>
    private static int GetNormalizedThumbnailSize(int requestedSize) {
        if (requestedSize <= 128) return 128;
        if (requestedSize <= 256) return 256;
        if (requestedSize <= 512) return 512;
        return 1024;
    }

    private static void OnTextureUncached(Texture2D texture) {
        if (texture != null) {
            UnityEngine.Object.Destroy(texture);
        }
    }
}
