using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Fireman.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fireman.Interface.Views;

/// <summary>
/// Controller to populate a view
/// </summary>
public abstract class ViewPopulator : MonoBehaviour {
    public Window? TargetWindow;

    private FileSystemWatcher? _watcher;
    private readonly ConcurrentQueue<FileSystemEventArgs> _fileEventQueue = new();

    protected virtual void Start() {
        if (TargetWindow == null) return;
        TargetWindow.CurrentPathChanged += OnPathChanged;

        SetupWatcher();
        Repopulate();
    }

    protected virtual void OnDestroy() {
        if (TargetWindow != null) TargetWindow.CurrentPathChanged -= OnPathChanged;
        DisposeWatcher();
    }

    private void OnPathChanged() {
        SetupWatcher();
        Repopulate();
    }

    private void SetupWatcher() {
        DisposeWatcher();

        if (TargetWindow?.CurrentPath == null || !Directory.Exists(TargetWindow.CurrentPath)) return;

        try {
            _watcher = new FileSystemWatcher(TargetWindow.CurrentPath) {
                NotifyFilter = NotifyFilters.FileName
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.LastWrite
                               | NotifyFilters.Size,
                Filter = "*.*",
                EnableRaisingEvents = true
            };

            _watcher.Created += OnFileSystemChanged;
            _watcher.Deleted += OnFileSystemChanged;
            _watcher.Renamed += OnFileSystemChanged;
            _watcher.Changed += OnFileSystemChanged;
        } catch (System.Exception ex) {
            Debug.LogWarning($"Failed to initialize FileSystemWatcher for {TargetWindow.CurrentPath}: {ex.Message}");
        }
    }

    private void DisposeWatcher() {
        if (_watcher == null) return;
        _watcher.EnableRaisingEvents = false;
        _watcher.Created -= OnFileSystemChanged;
        _watcher.Deleted -= OnFileSystemChanged;
        _watcher.Renamed -= OnFileSystemChanged;
        _watcher.Changed -= OnFileSystemChanged;
        _watcher.Dispose();
        _watcher = null;
        _fileEventQueue.Clear();
    }

    private void OnFileSystemChanged(object sender, FileSystemEventArgs e) {
        _fileEventQueue.Enqueue(e);
    }

    /// <summary>
    /// Clears and repopulates the view
    /// </summary>
    protected void Repopulate() {
        if (TargetWindow == null) return;
        foreach (Transform childTrans in transform) Destroy(childTrans.gameObject);
        var items = TargetWindow.GetCurrentDirItems();
        for (var index = 0; index < items.Length; index++) {
            var fileInfo = items[index];
            var obj = CreateItem(fileInfo, index);
            obj.transform.SetParent(transform, false);
        }
    }

    /// <summary>
    /// Method to create the UI object for a file item
    /// </summary>
    /// <returns>The GameObject of the UI object</returns>
    protected abstract GameObject CreateItem(FileSystemInfo fileInfo, int index);

    protected virtual void Update() {
        while (_fileEventQueue.TryDequeue(out var fileEvent)) {
            switch (fileEvent.ChangeType) {
                // case WatcherChangeTypes.Deleted:
                //     var match = transform.GetComponentsInChildren<FileItemController>()
                //         .FirstOrDefault(comp => comp.FileInfo?.FullName == fileEvent.FullPath);
                //     if (match != null) Destroy(match.gameObject);
                //     RefreshIndices();
                //     break;
                case WatcherChangeTypes.Created:
                    var path = fileEvent.FullPath;
                    FileSystemInfo fileInfo = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path);
                    var obj = CreateItem(fileInfo, transform.childCount);
                    obj.transform.SetParent(transform, false);
                    break;
                case WatcherChangeTypes.Renamed:
                    if (fileEvent is RenamedEventArgs renamed) {
                        var oldPath = renamed.OldFullPath;
                        var newPath = renamed.FullPath;
                        var renamedMatch = transform.GetComponentsInChildren<FileItemController>()
                            .FirstOrDefault(comp => comp.FileInfo?.FullName == oldPath);
                        if (renamedMatch != null) {
                            FileSystemInfo renamedFileInfo = File.Exists(newPath) ? new FileInfo(newPath) : new DirectoryInfo(newPath);
                            renamedMatch.FileInfo = renamedFileInfo;
                            renamedMatch.UpdateItemInfo();
                        }
                    } else {
                        Repopulate();
                    }

                    break;
                default:
                    Repopulate();
                    break;
            }
        }

        if (TargetWindow == null) return;

        // Don't steal arrow presses if typing
        var currentSelected = EventSystem.current?.currentSelectedGameObject;
        if (currentSelected != null && currentSelected.GetComponent<TMP_InputField>() != null) return;
        bool arrowPressed = Input.GetKeyDown(KeyCode.UpArrow) ||
                            Input.GetKeyDown(KeyCode.DownArrow) ||
                            Input.GetKeyDown(KeyCode.LeftArrow) ||
                            Input.GetKeyDown(KeyCode.RightArrow);

        // Arrow was pressed but nothing focused -> focus first item
        if (arrowPressed && (currentSelected == null || currentSelected.GetComponent<FileItemController>() == null)) {
            var firstItem = GetComponentInChildren<FileItemController>();
            if (firstItem != null) {
                firstItem.GetComponent<Selectable>()?.Select();
            }

            return;
        }

        // Enter -> activate
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            TargetWindow.ActivateSelection();
        }

        // Escape -> clear selection
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TargetWindow.CurrentTab.Selection.Clear();
        }
    }

    private void RefreshIndices() {
        var childrenComps = GetComponentsInChildren<FileItemController>();
        for (int i = 0; i < childrenComps.Length; i++) {
            var c = childrenComps[i];
            c.Index = i;
        }
    }
}
