using System.IO;
using System.Linq;
using Fireman.Core;
using NukeLib.UI;
using TMPro;
using UnityEngine;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for the status bar
/// </summary>
public class StatusBarController : MonoBehaviour {
    public Window? TargetWindow;

    private TextMeshProUGUI? _text;

    private void Start() {
        _text = gameObject.FindRecursive("Text")?.GetComponent<TextMeshProUGUI>();

        if (TargetWindow != null) TargetWindow.CurrentPathChanged += UpdateDisplay;
        FileOperationsManager.CopyBufferChanged += UpdateDisplay;
        FileOperationsManager.CurrentActionChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDestroy() {
        if (TargetWindow != null) TargetWindow.CurrentPathChanged -= UpdateDisplay;
        FileOperationsManager.CopyBufferChanged -= UpdateDisplay;
        FileOperationsManager.CurrentActionChanged -= UpdateDisplay;
    }

    private void UpdateDisplay() {
        if (TargetWindow == null) return;
        FileSystemInfo[] items = FileOperationsManager.CurrentAction == CopyAction.None
            ? TargetWindow.GetCurrentDirItemsUnsorted()
            : FileOperationsManager.CopyBufferPaths
                .Select<string, FileSystemInfo>(path =>
                    Directory.Exists(path)
                        ? new DirectoryInfo(path)
                        : new FileInfo(path))
                .ToArray();

        var countString = GetCountString(items);
        if (_text != null) {
            switch (FileOperationsManager.CurrentAction) {
                case CopyAction.None:
                    _text.text = countString;
                    break;
                case CopyAction.Copy:
                    _text.text = $"Copy buffer: {countString}";
                    break;
                case CopyAction.Cut:
                    _text.text = $"Cut buffer: {countString}";
                    break;
            }
        }
        gameObject.UnfuckLayoutHack();
    }

    private static string GetCountString(FileSystemInfo[] items) {
        int folders = items.Count(i => i is DirectoryInfo);
        int files = items.Count(i => i is FileInfo);
        var statusText = "";
        if (folders > 0) statusText += $"[{folders}]";
        if (files > 0) statusText += (statusText.Length > 0 ? " + " : "") + $"{files}";
        statusText += statusText.Length == 0 ? "Nothing" : " items";
        return statusText;
    }
}
