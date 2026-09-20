using System.Collections.Generic;
using System.IO;
using System.Linq;
using ThornClient.Managers;
using UnityEngine;

namespace Fireman.Platform;

/// <summary>
/// Determines file icons
/// </summary>
public static class FileIcons {
    public const string BundleKey = "fireman.icons";
    private static readonly string BundlePath = Path.Combine(Plugin.workingDir, "assets", "fireman_fileicons.bundle");

    static FileIcons() {
        AssetManager.LoadBundle(BundleKey, BundlePath);
    }

    private static readonly Dictionary<string, string> FolderMap = new() {
        { Paths.Game, "folder_game" },
        { Paths.BepInEx, "folder_bepinex" },
        { Paths.Cybergrind, "folder_cybergrind" },
        { Paths.Home, "folder_home" },
        { Paths.Downloads, "folder_downloads" },
        { Paths.Documents, "folder_documents" },
        { Paths.Images, "folder_images" },
        { Paths.Music, "folder_music" },
        { Paths.Videos, "folder_videos" },
        { Paths.AngryLevels, "folder_angry" },
        { Paths.ThornConfig, "folder_thorn" },
    };

    private static readonly Dictionary<string[], string> FileExtMap = new() {
        { [".txt", ".docx", ".doc", ".pdf", ".rtf", ".odt", ".tex", ".log"], "file_text" },
        { [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp", ".tiff", ".tif", ".ico",
            ".heic", ".psd", ".jxl"], "file_image" },
        { [".mp3", ".wav", ".aac", ".flac", ".ogg", ".m4a"], "file_music" },
        { [".mp4", ".mov", ".avi", ".mkv", ".wmv", ".webm", ".m4v", ".mpeg"], "file_video" },
        { [".zip", ".rar", ".7z", ".tar", ".gz", ".bz2", ".xz"], "file_zip" },
        { [".exe", ".msi", ".apk", ".app", ".dmg", ".ps1", ".sh", ".bash", ".zsh", ".fish", "makefile"],
            "file_executable" },
        { [".dll"], "file_lib" },
        { [".json", ".cfg", ".ini", ".toml"], "file_config" },
        { [".cgp"], "file_cgp" },
        { [".cgvsb"], "file_skybox" },
        { [".angry"], "file_angry" },
    };

    public static Sprite? GetFileIcon(FileSystemInfo fileInfo) {
        var targetIconName = "file";
        if (fileInfo is DirectoryInfo) {
            targetIconName = "folder";
            if (FolderMap.TryGetValue(fileInfo.FullName, out var mappedIconName)) targetIconName = mappedIconName;
        } else {
            targetIconName = "file";
            foreach (var kv in FileExtMap) {
                if (kv.Key.Any(ext => fileInfo.Name.EndsWith(ext))) {
                    targetIconName = kv.Value;
                    break;
                }
            }
        }
        return AssetManager.Get<Sprite>(BundleKey, targetIconName);
    }

    public static Sprite? GetSymbolicFolderIcon(DirectoryInfo dirInfo) {
        string iconName = "";
        if (dirInfo.Parent == null) iconName = "hard_disk_symbolic";
        else if (dirInfo.FullName == Paths.Game) iconName = "game_symbolic";
        else if (dirInfo.FullName == Paths.BepInEx) iconName = "bepinex_symbolic";
        else if (dirInfo.FullName == Paths.Cybergrind) iconName = "cybergrind_symbolic";
        else if (dirInfo.FullName == Paths.Home) iconName = "home_symbolic";
        else if (dirInfo.FullName == Paths.Downloads) iconName = "downloads_symbolic";
        else if (dirInfo.FullName == Paths.Documents) iconName = "documents_symbolic";
        else if (dirInfo.FullName == Paths.Images) iconName = "images_symbolic";
        else if (dirInfo.FullName == Paths.Music) iconName = "music_symbolic";
        else if (dirInfo.FullName == Paths.Videos) iconName = "videos_symbolic";
        else if (dirInfo.FullName == Paths.AngryLevels) iconName = "angry_symbolic";
        else if (dirInfo.FullName == Paths.ThornConfig) iconName = "thorn_symbolic";
        if (iconName != "") return AssetManager.Get<Sprite>(BundleKey, iconName);
        return null;
    }
}
