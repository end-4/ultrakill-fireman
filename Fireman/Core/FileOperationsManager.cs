using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Fireman.Core;

public static class FileOperationsManager {
    public static CopyAction _currentAction = CopyAction.None;

    public static CopyAction CurrentAction {
        get => _currentAction;
        private set {
            _currentAction = value;
            CurrentActionChanged?.Invoke();
        }
    }
    private static readonly HashSet<string> _copyBufferPaths = new(StringComparer.Ordinal);

    public static IReadOnlyCollection<string> CopyBufferPaths {
        get => _copyBufferPaths;
        private set {
            _copyBufferPaths.Clear();
            _copyBufferPaths.UnionWith(value);
            CopyBufferChanged?.Invoke();
        }
    }

    public static event Action? CopyBufferChanged;
    public static event Action? CurrentActionChanged;

    public static void Copy(FileSelectionModel selection) {
        Grab(selection, CopyAction.Copy);
    }

    public static void Cut(FileSelectionModel selection) {
        Grab(selection, CopyAction.Cut);
    }

    public static void Paste(string destination) {
        if (CopyBufferPaths.Count == 0) return;
        var destInfo = new DirectoryInfo(destination);
        if (CurrentAction == CopyAction.Copy) {
            foreach (var path in _copyBufferPaths) {
                var destPath = Path.Combine(destInfo.FullName, Path.GetFileName(path));
                if (path == destPath) {
                    destPath = Path.Combine(destInfo.FullName, Path.GetFileNameWithoutExtension(path) + " copy" + Path.GetExtension(path));
                }
                File.Copy(path, destPath, true);
            }
        } else if (CurrentAction == CopyAction.Cut) {
            foreach (var path in _copyBufferPaths) {
                var destPath = Path.Combine(destInfo.FullName, Path.GetFileName(path));
                if (path == destPath) continue;
                File.Move(path, destPath);
            }
            _copyBufferPaths.Clear();
            CurrentAction = CopyAction.None;
        }
    }

    public static void PermaDelete(FileSelectionModel selection) {
        if (selection?.SelectedPaths == null || selection.SelectedPaths.Count == 0) return;

        foreach (var path in selection.SelectedPaths) {
            try {
                if (File.Exists(path)) { // just a file
                    File.Delete(path);
                } else if (Directory.Exists(path)) { // rm -rf
                    Directory.Delete(path, true);
                }
            } catch (Exception ex) {
                Plugin.Log.LogError("Failed to delete " + path + ": " + ex);
            }
        }
    }

    private static void Grab(FileSelectionModel selection, CopyAction action) {
        CopyBufferPaths = selection.SelectedPaths;
        GUIUtility.systemCopyBuffer = string.Join("\n", selection.SelectedPaths);
        CurrentAction = action;
    }
}

public enum CopyAction {
    Copy,
    Cut,
    None
}
