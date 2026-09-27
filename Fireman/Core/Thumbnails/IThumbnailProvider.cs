using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Interface for a thumbnail provider
/// </summary>
public interface IThumbnailProvider {
    /// <summary>
    /// Higher number = given priority.
    /// For example there's a generic image thumbnail provider with priority 0 and another .bmp one with priority 1,
    ///   in which case the .bmp one will be used for .bmp files and the generic one for other image formats
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Checks whether this provider can generate a thumbnail for the file.
    /// </summary>
    bool CanHandle(string filePath);

    /// <summary>
    /// Asynchronously generate a Texture2D thumbnail
    /// </summary>
    Task<Texture2D> GenerateThumbnailAsync(string filePath, int width, int height);
}
