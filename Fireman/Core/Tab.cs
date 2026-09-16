using System;
using System.IO;
using Fireman.Core.User;
using Fireman.Core.User.SortStrategies;

namespace Fireman.Core;

/// <summary>
/// Class storing state of a tab
/// </summary>
public class Tab {
    private NavigationHistory _history;
    private SortStrategy _sortStrategy = new ExtensionSortStrategy();

    public SortStrategy SortingStrategy {
        set => _sortStrategy = value;
        get => _sortStrategy;
    }

    public FileSystemInfo[] GetSortedItems(FileSystemInfo[] items)
        => _sortStrategy.SortItems(items, new SortContext());

    public string CurrentPath {
        set {
            _history.NavigateTo(value);
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
    }

    /// <summary>
    /// Clone an existing tab state
    /// </summary>
    /// <param name="source">The tab state to clone from</param>
    public Tab(Tab source) {
        _history = source._history;
    }
}
