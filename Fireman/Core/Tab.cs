using System;
using System.IO;
using System.Linq;
using Fireman.Core.SortStrategies;

namespace Fireman.Core;

/// <summary>
/// Class storing state of a tab
/// </summary>
public class Tab {
    private NavigationHistory _history;
    private SortStrategy _sortStrategy = new ExtensionSortStrategy();
    private FileSelectionModel _selectionModel;

    public FileSelectionModel Selection {
        get => _selectionModel;
    }

    public SortStrategy SortingStrategy {
        set => _sortStrategy = value;
        get => _sortStrategy;
    }

    public FileSystemInfo[] GetSortedItems(FileSystemInfo[] items)
        => _sortStrategy.SortItems(items, new SortContext());

    public string CurrentPath {
        set {
            var old = _history.CurrentPath;
            _history.NavigateTo(value);
            UpdateSelection(_history.CurrentPath, old);
            CurrentPathChanged?.Invoke();
        }
        get => _history.CurrentPath;
    }

    public bool CanGoBack => _history.CanGoBack;
    public bool CanGoForward => _history.CanGoForward;

    public event Action? CurrentPathChanged;

    public string GoBack() {
        var newDir = _history.GoBack();
        CurrentPathChanged?.Invoke();
        return newDir;
    }

    public string GoForward() {
        var newDir = _history.GoForward();
        CurrentPathChanged?.Invoke();
        return newDir;
    }

    /// <summary>
    /// Constructor for a tab state with an initial path
    /// </summary>
    /// <param name="initialPath">The initial path to start with</param>
    public Tab(string initialPath) {
        _history = new NavigationHistory(initialPath);
        _selectionModel = new FileSelectionModel();
    }

    /// <summary>
    /// Clone an existing tab state
    /// </summary>
    /// <param name="source">The tab state to clone from</param>
    public Tab(Tab source) {
        _history = source._history;
        _selectionModel = new FileSelectionModel(source.Selection);
    }

    private void UpdateSelection(string newPath, string oldPath) {
        var dir = new DirectoryInfo(newPath);
        var oldDir = new DirectoryInfo(oldPath);

        // Navigating up: select previous dir
        if (dir.FullName == (oldDir.Parent?.FullName ?? "")) {
            var names = GetSortedItems(dir.GetFileSystemInfos()).Select(i => i.FullName).ToList();
            var index = names.IndexOf(oldDir.FullName);
            Selection.SelectOnly(oldDir.FullName, index);
        }

        // Else select first item in new dir
        else {
            var names = GetSortedItems(dir.GetFileSystemInfos()).Select(i => i.FullName).ToList();
            Selection.SelectOnly(names[0], 0);
        }
    }
}
