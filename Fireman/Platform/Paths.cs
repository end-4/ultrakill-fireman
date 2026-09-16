using System;
using System.IO;

namespace Fireman.Platform;

/// <summary>
/// Common system and ULTRAKILL-related paths
/// </summary>
public static class Paths {
    public static readonly DirectoryInfo GameInfo = new (global::BepInEx.Paths.GameRootPath);
    public static readonly DirectoryInfo BepInExInfo = new (global::BepInEx.Paths.BepInExRootPath);
    public static readonly DirectoryInfo CybergrindInfo = new (Path.Combine(GameInfo.FullName, "Cybergrind"));
    public static readonly DirectoryInfo HomeInfo = new (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    public static readonly DirectoryInfo DownloadsInfo = new (Path.Combine(HomeInfo.FullName, "Downloads"));
    public static readonly DirectoryInfo DocumentsInfo = new (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
    public static readonly DirectoryInfo ImagesInfo = new (Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
    public static readonly DirectoryInfo MusicInfo = new (Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
    public static readonly DirectoryInfo VideosInfo = new (Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
    public static readonly DirectoryInfo ThornInfo = new (Path.Combine(BepInExInfo.FullName, "config", "ThornClient"));
    public static string Game => GameInfo.FullName;
    public static string BepInEx => BepInExInfo.FullName;
    public static string Cybergrind => CybergrindInfo.FullName;
    public static string Home => HomeInfo.FullName;
    public static string Downloads => DownloadsInfo.FullName;
    public static string Documents => DocumentsInfo .FullName;
    public static string Images => ImagesInfo.FullName;
    public static string Music => MusicInfo.FullName;
    public static string Videos => VideosInfo.FullName;
    public static string Thorn => ThornInfo.FullName;
}
