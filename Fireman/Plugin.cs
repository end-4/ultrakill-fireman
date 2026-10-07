using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Fireman.Core.Utils;
using ThornClient.Managers;
using TMPro;

namespace Fireman;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency("com.github.end-4.thornClient")]
[BepInDependency("com.github.end-4.notiffy")]
[BepInDependency("com.github.end-4.nukeLib")]
public class Plugin : BaseUnityPlugin {
    // Logger
    internal static ManualLogSource Log;

    // Meta
    public static string workingPath = Assembly.GetExecutingAssembly().Location;
    public static string workingDir = Path.GetDirectoryName(workingPath);
    public const string PluginGUID = "com.github.end-4.fireman";
    public const string PluginName = "Fireman";
    public const string PluginVersion = "0.1.1";
    public static string PluginIconPath => Path.Combine(workingDir, "icon.png");

    // Assets
    public const string BundleKey = "fireman.main";
    private static readonly string BundlePath = Path.Combine(workingDir, "assets", "fireman.bundle");

    private void Awake() {
        Log = Logger;

        // Missing text character fix Flazhik senpai told me
        TMP_Settings.missingGlyphCharacter = '?';

        AssetManager.LoadBundle(BundleKey, BundlePath);
        UnityMainThreadDispatcher.Initialize();
    }
}
