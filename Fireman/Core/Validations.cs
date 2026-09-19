using System;
using System.IO;
using System.Linq;

namespace Fireman.Core;

/// <summary>
/// Helper for validating paths
/// </summary>
public static class Validations {
    /// <summary>
    /// Check whether a path is accessible
    /// </summary>
    /// <param name="path">The path string</param>
    /// <returns>Whether the path is accessible</returns>
    public static bool Accessible(string path) {
        if (string.IsNullOrWhiteSpace(path)) {
            return false;
        }

        try {
            var resolved = ResolvePath(path);

            if (File.Exists(resolved)) {
                using var fs = File.Open(resolved, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return true;
            }

            if (Directory.Exists(resolved)) {
                _ = Directory.EnumerateFileSystemEntries(resolved).FirstOrDefault();
                return true;
            }

            return false;
        } catch (UnauthorizedAccessException) {
            return false;
        } catch (IOException) {
            return false;
        } catch (Exception) {
            return false;
        }
    }

    /// <summary>
    /// Resolve a path to an absolute path
    /// </summary>
    /// <param name="path">The path</param>
    /// <returns>The absolute raw path</returns>
    public static string ResolvePath(string path) {
        if (string.IsNullOrWhiteSpace(path)) {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        // Handle file://
        if (path.StartsWith("file://", StringComparison.OrdinalIgnoreCase)) {
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri)) {
                path = uri.LocalPath;
            }
        }

        // Expand ~
        if (path.StartsWith("~")) {
            var home = Platform.Paths.Home;
            if (path.Length == 1) {
                path = home;
            } else if (path[1] == '/' || path[1] == '\\') {
                path = Path.Combine(home, path.Substring(2));
            }
        }

        return Path.GetFullPath(path);
    }
}
