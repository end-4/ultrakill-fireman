using System.Collections.Generic;

namespace Fireman.Core;

using System;
using System.Collections.Generic;

/// <summary>
/// Class storing the navigation history in a tab
/// </summary>
public class NavigationHistory {
    // We use a list with an index for this
    private readonly List<string> _dirList = [];
    private int _currentIndex;

    public string CurrentPath =>
        (_currentIndex >= 0 && _currentIndex < _dirList.Count) ? _dirList[_currentIndex] : null!;

    public bool CanGoBack => _currentIndex > 0;
    public bool CanGoForward => _currentIndex < _dirList.Count - 1;

    /// <summary>
    /// Initialize a fresh history with an initial path
    /// </summary>
    public NavigationHistory(string initialPath) {
        if (string.IsNullOrEmpty(initialPath))
            throw new ArgumentNullException(nameof(initialPath));

        _dirList.Add(initialPath);
        _currentIndex = 0;
    }

    /// <summary>
    /// Clone an existing TabHistory
    /// </summary>
    public NavigationHistory(NavigationHistory other) {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        _dirList = new List<string>(other._dirList);
        _currentIndex = other._currentIndex;
    }

    /// <summary>
    /// Go to a new directory
    /// </summary>
    /// <returns>Whether the navigation was successful</returns>
    public bool NavigateTo(string newPath) {
        if (string.IsNullOrEmpty(newPath)) return false;
        var resolvedPath = Validations.ResolvePath(newPath);

        // Don't push to history if navigating to the exact path we are already on
        if (CurrentPath.Equals(resolvedPath, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!Validations.Accessible(resolvedPath)) return false;

        // Truncate any forward history
        if (_currentIndex < _dirList.Count - 1) {
            _dirList.RemoveRange(_currentIndex + 1, _dirList.Count - (_currentIndex + 1));
        }

        _dirList.Add(resolvedPath);
        _currentIndex++;
        return true;
    }

    /// <summary>
    /// Navigate to the previous directory in history
    /// </summary>
    /// <returns>Path of the previous directory (or current if it's not possible to go back further)</returns>
    public string GoBack() {
        if (CanGoBack) {
            _currentIndex--;
        }

        return CurrentPath;
    }

    /// <summary>
    /// Navigate to the next directory in history
    /// </summary>
    /// <returns>Path of the next directory (or current if it's not possible to go forward further)</returns>
    public string GoForward() {
        if (CanGoForward) {
            _currentIndex++;
        }

        return CurrentPath;
    }
}
