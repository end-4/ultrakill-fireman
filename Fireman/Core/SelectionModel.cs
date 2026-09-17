using System;
using System.Collections.Generic;

namespace Fireman.Core;

/// <summary>
/// Model for file selection
/// </summary>
public class FileSelectionModel {
    private readonly HashSet<string> _selectedPaths = new(StringComparer.Ordinal);

    public IReadOnlyCollection<string> SelectedPaths => _selectedPaths;
    public int Count => _selectedPaths.Count;
    public int FocusedIndex { get; set; } = -1;
    public int AnchorIndex { get; set; } = -1;

    public event Action? SelectionChanged;

    public bool IsSelected(string path) => _selectedPaths.Contains(path);

    public FileSelectionModel() {
    }

    public FileSelectionModel(FileSelectionModel source) {
        _selectedPaths = new HashSet<string>(source._selectedPaths, StringComparer.Ordinal);
        FocusedIndex = source.FocusedIndex;
        AnchorIndex = source.AnchorIndex;
    }

    public void SelectOnly(string path, int index) {
        _selectedPaths.Clear();
        _selectedPaths.Add(path);
        FocusedIndex = index;
        AnchorIndex = index;
        SelectionChanged?.Invoke();
    }

    public void Toggle(string path, int index) {
        if (!_selectedPaths.Remove(path)) {
            _selectedPaths.Add(path);
        }

        FocusedIndex = index;
        AnchorIndex = index;
        SelectionChanged?.Invoke();
    }

    public void SelectRange(IList<string> allPaths, int fromIndex, int toIndex) {
        int start = Math.Min(fromIndex, toIndex);
        int end = Math.Max(fromIndex, toIndex);
        _selectedPaths.Clear();
        for (int i = start; i <= end; i++) {
            _selectedPaths.Add(allPaths[i]);
        }

        FocusedIndex = toIndex;
        SelectionChanged?.Invoke();
    }

    public void SelectAll(IEnumerable<string> paths) {
        _selectedPaths.Clear();
        foreach (var path in paths) _selectedPaths.Add(path);
        SelectionChanged?.Invoke();
    }

    public void Clear() {
        if (_selectedPaths.Count == 0 && FocusedIndex == -1) return;
        _selectedPaths.Clear();
        FocusedIndex = -1;
        AnchorIndex = -1;
        SelectionChanged?.Invoke();
    }
}
