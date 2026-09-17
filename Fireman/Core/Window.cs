using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fireman.Core;
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

    public int Count => Tabs.Count;

    public Tab CurrentTab {
        get => _tabs[_currTabIndex];
        set {
            _currTabIndex = _tabs.IndexOf(value);
            CurrentTabChanged?.Invoke();
            CurrentPathChanged?.Invoke();
        }
    }

    public int CurrentTabIndex {
        get => _currTabIndex;
        set {
            _currTabIndex = value;
            CurrentTabChanged?.Invoke();
            CurrentPathChanged?.Invoke();
        }
    }

    public string CurrentPath {
        get => CurrentTab.CurrentPath;
        set {
            CurrentTab.CurrentPath = value;
            CurrentPathChanged?.Invoke();
        }
    }

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
    /// </summary>
    /// <returns></returns>
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
    public Window(string initialPath) {
        var firstTab = new Tab(initialPath);
        _tabs = [firstTab];
        CurrentTabChanged += () => CurrentPathChanged?.Invoke();
        TabsChanged?.Invoke();
        TabAdded?.Invoke(firstTab);
    }

    /// <summary>
    /// Clone an existing TabHost
    /// </summary>
    /// <param name="source">The source TabHost</param>
    public Window(Window source) {
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
}
