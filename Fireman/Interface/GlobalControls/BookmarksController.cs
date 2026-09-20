using System;
using System.IO;
using BepInEx.Bootstrap;
using Fireman.Core;
using Fireman.Platform;
using ThornClient.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for the bookmarks pane column
/// </summary>
public class BookmarksController : MonoBehaviour {
    public Window? TargetWindow;

    private const string Separator = "__SEPARATOR";
    private static string[] ItemPaths => [
        Paths.Game,
        Paths.BepInEx,
        Paths.Cybergrind,
        .. (Chainloader.PluginInfos.ContainsKey("com.eternalUnion.angryLevelLoader") ? new[]{Paths.AngryLevels} : new string[]{}),
        .. (Chainloader.PluginInfos.ContainsKey("com.github.end-4.thornClient") ? new[]{Paths.ThornConfig} : new string[]{}),
        Separator,
        Paths.Home,
        Paths.Downloads,
        Paths.Documents,
        Paths.Images,
        Paths.Music,
        Paths.Videos,
    ];

    private void Start() {
        foreach (Transform child in transform) Destroy(child.gameObject);
        foreach (var path in ItemPaths) {
            if (path == Separator) CreateSeparator();
            else CreateButton(path);
        }
    }

    private void CreateSeparator() {
        var prefab = AssetManager.Get<GameObject>(Plugin.BundleKey, "BookmarkSeparator");
        Instantiate(prefab, transform);
    }

    private void CreateButton(string path) {
        var prefab = AssetManager.Get<GameObject>(Plugin.BundleKey, "BookmarkItem");
        var item = Instantiate(prefab, transform);
        if (item == null) return;
        var comp = item.AddComponent<BookmarkItemController>();
        comp.Path = new DirectoryInfo(path);
        comp.TargetWindow = TargetWindow;
    }
}
