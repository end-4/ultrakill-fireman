using System.Collections.Generic;
using Fireman.Core;
using Fireman.Interface.GlobalControls;
using Fireman.Interface.Views;
using NukeLib.UI;
using ThornClient.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Fireman.Interface;

/// <summary>
/// A file manager instance that's a single draggable window
/// </summary>
public class FileManager : MonoBehaviour {
    private static readonly string DefaultPath = Platform.Paths.GameInfo.FullName;

    private TabHost Host;
    public IReadOnlyList<Tab> Tabs => Host.Tabs;
    public Tab CurrentTab => Host.CurrentTab;

    public string CurrentPath {
        get => Host.CurrentPath;
        set => Host.CurrentPath = value;
    }

    private void Start() {
        Host = new TabHost(DefaultPath);

        // Controller adding //

        // Titlebar stuff
        var titlebar = gameObject.FindRecursive("Titlebar");
        titlebar?.AddComponent<TitlebarDragHandler>();
        var close = gameObject.FindRecursive("Titlebar/Close");
        close?.GetComponent<Button>().onClick.AddListener(Close);
        var tabRow = gameObject.FindRecursive("Titlebar/TabScrollView/Viewport/Row");
        var tabsComp = tabRow?.AddComponent<TabRowController>();
        if (tabsComp != null) tabsComp.TargetHost = Host;

        // Top bar stuff
        var topBar = gameObject.FindRecursive("Content/TopBar");
        var back = topBar?.FindRecursive("Navi/Back");
        var backComp = back?.AddComponent<BackButtonController>();
        if (backComp != null) backComp.TargetHost = Host;
        var fwd = topBar?.FindRecursive("Navi/Forward");
        var fwdComp = fwd?.AddComponent<ForwardButtonController>();
        if (fwdComp != null) fwdComp.TargetHost = Host;
        var up = topBar?.FindRecursive("Navi/Up");
        var upComp = up?.AddComponent<UpButtonController>();
        if (upComp != null) upComp.TargetHost = Host;
        var input = topBar?.FindRecursive("Address/Input");
        var inputComp = input?.AddComponent<AddressInputController>();
        if (inputComp != null) inputComp.TargetHost = Host;
        var settings = topBar?.FindRecursive("Settings")?.GetComponent<Button>();
        settings?.gameObject.SetActive(false); // TODO allow open thorn clickgui menu and impl this btn

        // Bookmarks
        var bookmarks = gameObject?.FindRecursive("Content/Body/Bookmarks/Container/ScrollView/Viewport/Content");
        var bookmarksComp = bookmarks?.AddComponent<BookmarksController>();
        if (bookmarksComp != null) bookmarksComp.TargetHost = Host;

        // Main file pane
        var mainContainer = gameObject?.FindRecursive("Content/Body/Files/Container");
        var views = mainContainer?.FindRecursive("ScrollView/Viewport/Content");
        var grid = views?.FindRecursive("GridView");
        var gridComp = grid?.AddComponent<GridViewPopulator>();
        if (gridComp != null) gridComp.TargetHost = Host;
        // TODO list view
        var placeholder = mainContainer?.FindRecursive("Placeholders");
        var placeholderComp = placeholder?.AddComponent<MainPanelPlaceholderController>();
        if (placeholderComp != null) placeholderComp.TargetHost = Host;

        // Status & actions
        var status = mainContainer?.FindRecursive("Status");
        var statComp = status?.AddComponent<StatusBarController>();
        if (statComp != null) statComp.TargetHost = Host;
        var actions = mainContainer?.FindRecursive("Actions");
        var actionsComp = actions?.AddComponent<ActionsBarController>();

        // Hooks
        var keybindsComp = gameObject.GetOrAddComponent<FileManagerKeybindHandler>();
        keybindsComp.TargetHost = Host;
        Host.TabsChanged += CloseIfEmpty;

    }

    private void OnDestroy() {
        Host.TabsChanged -= CloseIfEmpty;
    }

    private void CloseIfEmpty() {
        if (Host.Count == 0) Close();
    }

    /// <summary>
    /// Close the file manager instance
    /// </summary>
    public void Close() {
        if (gameObject != null) Destroy(gameObject);
    }

    /// <summary>
    /// Creates a new file manager instance (both this controller and the UI)
    /// </summary>
    /// <param name="initialPath">The initial path</param>
    /// <returns>A FileManager which is a MonoBehavior attached to the file manager GameObject</returns>
    public static FileManager NewInstance(string? initialPath = null) {
        var windowPrefab = AssetManager.Get<GameObject>(Plugin.BundleKey, "Window");
        var window = Instantiate(windowPrefab);
        var comp = window?.AddComponent<FileManager>();
        if (comp == null) return null!;
        if (initialPath != null) comp.CurrentPath = initialPath;
        return comp;
    }
}
