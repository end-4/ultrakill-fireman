using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fireman.Interface;

namespace Fireman.Core;

/// <summary>
/// Backing data/logic of a window
/// For the GUI, see <see cref="FileManager"/> and related components that it uses
/// </summary>
public class Window {
    private List<Tab> _tabs;
    private int _currTabIndex;

    public IReadOnlyList<Tab> Tabs => _tabs;
    public event Action? TabsChanged;
    public event Action? CurrentTabChanged;
    public event Action<Tab>? TabAdded;
    public event Action<Tab>? TabRemoved;
    public event Action? CurrentPathChanged;

    public event Action<string[]>? ItemsPicked;

    public int Count => Tabs.Count;

    /// <summary>
    /// Whether multiple items can be selected at once
    /// </summary>
    public bool AllowMultiSelection { get; set; }

    /// <summary>
    /// Whether the window is in picker mode
    /// </summary>
    public bool PickerMode { get; private set; }

    /// <summary>
    /// Whether the current picking picks a folder
    /// </summary>
    public bool IsSelectionFolder { get; private set; }

    /// <summary>
    /// The currently focused Tab
    /// </summary>
    public Tab CurrentTab {
        get => _tabs[_currTabIndex];
        set {
            _currTabIndex = _tabs.IndexOf(value);
            CurrentTabChanged?.Invoke();
            CurrentPathChanged?.Invoke();
        }
    }

    /// <summary>
    /// The index of the currently focused Tab
    /// </summary>
    public int CurrentTabIndex {
        get => _currTabIndex;
        set {
            _currTabIndex = value;
            CurrentTabChanged?.Invoke();
            CurrentPathChanged?.Invoke();
        }
    }

    /// <summary>
    /// The current path of the currently focused Tab
    /// </summary>
    public string CurrentPath {
        get => CurrentTab.CurrentPath;
        set {
            CurrentTab.CurrentPath = value;
            CurrentPathChanged?.Invoke();
        }
    }

    /// <summary>
    /// The current sorting strategy of the currently focused Tab
    /// </summary>
    public SortStrategy SortingStrategy {
        set => CurrentTab.SortingStrategy = value;
        get => CurrentTab.SortingStrategy;
    }

    public bool CanGoBack => CurrentTab.CanGoBack;
    public bool CanGoForward => CurrentTab.CanGoForward;

    public bool CanGoUp {
        get {
            var dirInfo = new DirectoryInfo(CurrentPath);
            return dirInfo.Parent != null;
        }
    }

    /// <summary>
    /// Get items in the current directory, sorted by the current tab's sort strategy
    /// </summary>
    /// <returns></returns>
    public FileSystemInfo[] GetCurrentDirItems() {
        var items = GetCurrentDirItemsUnsorted();
        return CurrentTab.GetSortedItems(items);
    }

    /// <summary>
    /// Gets items in the current directory, unsorted
    /// Returns an empty array if access is denied
    /// </summary>
    /// <returns>Array of file system items, or empty array if access denied</returns>
    public FileSystemInfo[] GetCurrentDirItemsUnsorted() {
        return new DirectoryInfo(CurrentPath).GetFileSystemInfos();
    }

    /// <summary>
    /// Go back in history
    /// </summary>
    /// <returns>The directory after the operation</returns>
    public string GoBack() {
        CurrentTab.GoBack();
        CurrentPathChanged?.Invoke();
        return CurrentPath;
    }

    /// <summary>
    /// Go forward in history
    /// </summary>
    /// <returns>The directory after the operation</returns>
    public string GoForward() {
        CurrentTab.GoForward();
        CurrentPathChanged?.Invoke();
        return CurrentPath;
    }

    /// <summary>
    /// Go up a directory
    /// </summary>
    /// <returns>The current directory after the operation</returns>
    public string GoUp() {
        var dirInfo = new DirectoryInfo(CurrentPath);
        var parentDir = dirInfo.Parent;
        if (parentDir != null) CurrentPath = parentDir.FullName;
        return CurrentPath;
    }

    /// <summary>
    /// Constructor for a window
    /// </summary>
    /// <param name="initialPath">The starting path</param>
    /// <param name="picker">Whether this window instance is a picker</param>
    /// <param name="allowMultiSelection">For pickers, is multiple selection allowed</param>
    /// <param name="isSelectionFolder"></param>
    public Window(string initialPath, bool picker = false, bool allowMultiSelection = false,
        bool isSelectionFolder = false) {
        PickerMode = picker;
        AllowMultiSelection = allowMultiSelection;
        IsSelectionFolder = isSelectionFolder;
        var firstTab = new Tab(initialPath);
        _tabs = [firstTab];
        CurrentTabChanged += () => CurrentPathChanged?.Invoke();
        TabsChanged?.Invoke();
        TabAdded?.Invoke(firstTab);
    }

    /// <summary>
    /// Clone an existing Window
    /// </summary>
    /// <param name="source">The source Window</param>
    public Window(Window source) {
        PickerMode = source.PickerMode;
        AllowMultiSelection = source.AllowMultiSelection;
        _tabs = source._tabs.Select(tab => new Tab(tab)).ToList();
        CurrentTabChanged += () => CurrentPathChanged?.Invoke();
        foreach (var tab in _tabs) TabAdded?.Invoke(tab);
        TabsChanged?.Invoke();
    }

    /// <summary>
    /// Creates a new tab
    /// </summary>
    /// <param name="initialPath">Initial path</param>
    /// <param name="switchTo">Whether to switch to the new tab. Optional, defaults to true</param>
    /// <returns>The new Tab</returns>
    public Tab NewTab(string initialPath, bool switchTo = true) {
        var tab = new Tab(initialPath);
        _tabs.Add(tab);
        TabAdded?.Invoke(tab);
        TabsChanged?.Invoke();
        if (switchTo) {
            CurrentTab = tab;
            CurrentTabChanged?.Invoke();
        }

        return tab;
    }

    /// <summary>
    /// Creates a new tab
    /// </summary>
    /// <returns>The new Tab</returns>
    public Tab NewTab() {
        return NewTab(CurrentPath);
    }

    /// <summary>
    /// Closes a tab
    /// </summary>
    /// <param name="tab">The tab</param>
    public void CloseTab(Tab tab) {
        _tabs.Remove(tab);
        if (Count > 0) {
            _currTabIndex = Math.Clamp(_currTabIndex, 0, _tabs.Count - 1);
            CurrentTabChanged?.Invoke();
        }

        TabRemoved?.Invoke(tab);
        TabsChanged?.Invoke();
    }

    /// <summary>
    /// Closes a tab
    /// </summary>
    public void CloseTab() {
        CloseTab(CurrentTab);
    }

    /// <summary>
    /// Activates current selection. Use this for a click/enter key, not file picking confirmation
    /// </summary>
    public void ActivateSelection() {
        var selection = CurrentTab.Selection;
        var count = selection.Count;
        if (count == 0 && PickerMode && IsSelectionFolder) {
            PickSelection();
        } else if (count == 1) {
            // Single selection -> folder opened in current tab
            var path = selection.SelectedPaths.First();
            if (Directory.Exists(path)) {
                CurrentPath = path;
            } else if (PickerMode && !IsSelectionFolder) {
                PickSelection();
            }
        } else if (count > 1) {
            bool hasFile = selection.SelectedPaths.Any(File.Exists);
            bool hasFolder = selection.SelectedPaths.Any(Directory.Exists);
            bool foldersOnly = hasFolder && !hasFile;
            bool filesOnly = hasFile && !hasFolder;
            if (foldersOnly) {
                // Multi + folder-only -> folders opened in their own tabs
                foreach (var path in selection.SelectedPaths) {
                    if (Directory.Exists(path)) {
                        NewTab(path);
                    }
                }
            } else {
                if (PickerMode && !AllowMultiSelection) return;
                if (PickerMode) {
                    if (filesOnly && !IsSelectionFolder) PickSelection();
                }
            }
        }
    }

    public bool CanPickSelection() {
        var items = CurrentTab.Selection.SelectedPaths;
        bool hasFile = items.Any(File.Exists);
        bool hasFolder = items.Any(Directory.Exists);
        bool foldersOnly = hasFolder && !hasFile;
        bool filesOnly = hasFile && !hasFolder;
        if (!filesOnly && !foldersOnly) return false;
        if (items.Count == 0) return false;
        if (items.Count > 1 && !AllowMultiSelection) return false;
        if (filesOnly && IsSelectionFolder) return false;
        if (foldersOnly && !IsSelectionFolder) return false;
        return true;
    }

    /// <summary>
    /// Picks the current selection
    /// </summary>
    public void PickSelection() {
        if (!CanPickSelection()) return;

        var items = CurrentTab.Selection.SelectedPaths;
        if (items.Count == 0) {
            items = [CurrentPath];
        }

        ItemsPicked?.Invoke([.. items]);
    }
}
