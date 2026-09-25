using System;
using System.Collections.Generic;
using Fireman.Core;
using Fireman.Interface.GlobalControls;
using Fireman.Interface.Reusables;
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

    /// <summary>
    /// The starting path for the file manager
    /// </summary>
    public string StartingPath = DefaultPath;

    /// <summary>
    /// The file manager window data model
    /// </summary>
    private Window? WindowInstance;

    /// <summary>
    /// The tabs of the file manager
    /// </summary>
    public IReadOnlyList<Tab> Tabs => WindowInstance?.Tabs ?? [];

    /// <summary>
    /// The current tab of the file manager
    /// </summary>
    public Tab? CurrentTab => WindowInstance.CurrentTab;

    /// <summary>
    /// Emitted when item(s) are picked
    /// </summary>
    public event Action<string[]>? ItemsPicked;

    /// <summary>
    /// Emitted when the file manager is closed
    /// </summary>
    public event Action? Closed;

    private bool _pickerMode = false;
    private bool _allowMultiSelection = true;
    private bool _isSelectionFolder = false;

    /// <summary>
    /// The current path of the file manager instance
    /// </summary>
    public string? CurrentPath {
        get => WindowInstance?.CurrentPath;
        set {
            if (WindowInstance != null) WindowInstance.CurrentPath = value;
        }
    }

    private void Start() {
        WindowInstance = new Window(StartingPath, picker: _pickerMode, allowMultiSelection: _allowMultiSelection,
            isSelectionFolder: _isSelectionFolder);

        // Controller adding //

        // Titlebar & window
        var titlebar = gameObject.FindRecursive("Titlebar");
        titlebar?.AddComponent<TitlebarDragHandler>();
        var close = gameObject.FindRecursive("Titlebar/Close");
        close?.GetComponent<Button>().onClick.AddListener(Close);
        var tabRow = gameObject.FindRecursive("Titlebar/TabScrollView/Viewport/Row");
        var tabsComp = tabRow?.AddComponent<TabRowController>();
        if (tabsComp != null) tabsComp.TargetWindow = WindowInstance;
        var resizeComp = gameObject.AddComponent<ResizeController>();
        resizeComp.minWidth = 300;
        resizeComp.minHeight = 200;

        // Top bar stuff
        var topBar = gameObject.FindRecursive("Content/TopBar");
        var back = topBar?.FindRecursive("Navi/Back");
        var backComp = back?.AddComponent<BackButtonController>();
        if (backComp != null) backComp.TargetWindow = WindowInstance;
        var fwd = topBar?.FindRecursive("Navi/Forward");
        var fwdComp = fwd?.AddComponent<ForwardButtonController>();
        if (fwdComp != null) fwdComp.TargetWindow = WindowInstance;
        var up = topBar?.FindRecursive("Navi/Up");
        var upComp = up?.AddComponent<UpButtonController>();
        if (upComp != null) upComp.TargetWindow = WindowInstance;
        var address = topBar?.FindRecursive("Address");
        var addressComp = address?.AddComponent<AddressController>();
        if (addressComp != null) addressComp.TargetWindow = WindowInstance;
        var settings = topBar?.FindRecursive("Settings")?.GetComponent<Button>();
        settings?.gameObject.SetActive(false); // TODO allow open thorn clickgui menu and impl this btn

        // Bookmarks
        var bookmarks = gameObject.FindRecursive("Content/Body/Bookmarks/Container/ScrollView/Viewport/Content");
        var bookmarksComp = bookmarks?.AddComponent<BookmarksController>();
        if (bookmarksComp != null) bookmarksComp.TargetWindow = WindowInstance;

        // Main file pane
        var mainContainer = gameObject?.FindRecursive("Content/Body/Files/Container");
        var views = mainContainer?.FindRecursive("ScrollView/Viewport/Content");
        var grid = views?.FindRecursive("GridView");
        var gridComp = grid?.AddComponent<GridViewPopulator>();
        if (gridComp != null) gridComp.TargetWindow = WindowInstance;
        // TODO list view
        var placeholder = mainContainer?.FindRecursive("Placeholders");
        var placeholderComp = placeholder?.AddComponent<MainPanelPlaceholderController>();
        if (placeholderComp != null) placeholderComp.TargetWindow = WindowInstance;

        // Status & actions
        var status = mainContainer?.FindRecursive("Status");
        var statComp = status?.AddComponent<StatusBarController>();
        if (statComp != null) statComp.TargetWindow = WindowInstance;
        var actions = mainContainer?.FindRecursive("Actions");
        var actionsComp = actions?.AddComponent<ActionsBarController>();
        if (actionsComp != null) {
            actionsComp.TargetWindow = WindowInstance;
            actionsComp.TargetFileManager = this;
        }

        // Hooks
        var keybindsComp = gameObject.GetOrAddComponent<FileManagerKeybindHandler>();
        keybindsComp.TargetWindow = WindowInstance;
        WindowInstance.TabsChanged += CloseIfEmpty;
        WindowInstance.ItemsPicked += PickAndDestroy;
    }

    private void OnDestroy() {
        if (WindowInstance != null) {
            WindowInstance.ItemsPicked -= PickAndDestroy;
            WindowInstance.TabsChanged -= CloseIfEmpty;
        }

        Closed?.Invoke();
    }

    private void PickAndDestroy(string[] paths) {
        if (!_pickerMode) return;
        ItemsPicked?.Invoke(paths);
        Destroy(gameObject);
    }

    private void CloseIfEmpty() {
        if (WindowInstance.Count == 0) Close();
    }

    /// <summary>
    /// Close the file manager instance
    /// </summary>
    public void Close() {
        if (gameObject != null) Destroy(gameObject);
    }

    private static FileManager NewInstance(string? initialPath, bool pickerMode, bool allowMultiSelection,
        bool isSelectionFolder = false) {
        var windowPrefab = AssetManager.Get<GameObject>(Plugin.BundleKey, "Window");
        var window = Instantiate(windowPrefab);
        var comp = window?.AddComponent<FileManager>();
        if (comp == null) return null!;
        comp._pickerMode = pickerMode;
        comp._allowMultiSelection = allowMultiSelection;
        comp._isSelectionFolder = isSelectionFolder;
        if (initialPath != null) comp.StartingPath = initialPath;
        return comp;
    }

    /// <summary>
    /// Creates a new file manager instance (both this controller and the UI)
    /// </summary>
    /// <param name="initialPath">The initial path</param>
    /// <returns>A FileManager which is a MonoBehavior attached to the file manager GameObject</returns>
    public static FileManager CreateManager(string? initialPath = null) {
        return NewInstance(initialPath, pickerMode: false, allowMultiSelection: true);
    }

    /// <summary>
    /// Creates a new file manager instance (both this controller and the UI)
    /// </summary>
    /// <param name="initialPath">The initial path</param>
    /// <param name="isSelectionFolder">Whether this picker is selecting folders</param>
    /// <param name="allowMultiSelection">Whether to allow multiple selection</param>
    /// <returns>A FileManager which is a MonoBehavior attached to the file manager GameObject</returns>
    public static FileManager CreatePicker(string? initialPath = null, bool isSelectionFolder = false,
        bool allowMultiSelection = false) {
        return NewInstance(initialPath, pickerMode: true, allowMultiSelection: allowMultiSelection,
            isSelectionFolder: isSelectionFolder);
    }
}
