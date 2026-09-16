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
    public TabHost? TargetHost;

    private TextMeshProUGUI? _text;

    private void Start() {
        _text = gameObject.FindRecursive("Text")?.GetComponent<TextMeshProUGUI>();

        if (TargetHost != null) TargetHost.CurrentPathChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDestroy() {
        if (TargetHost != null) TargetHost.CurrentPathChanged -= UpdateDisplay;
    }

    private void UpdateDisplay() {
        if (TargetHost == null) return;
        var items = TargetHost.GetCurrentDirItemsUnsorted();
        int folders = items.Count(i => i is DirectoryInfo);
        int files = items.Count(i => i is FileInfo);
        var statusText = "";
        if (folders > 0) statusText += $"{folders} folders";
        if (files > 0) statusText += (statusText.Length > 0 ? " + " : "") + $"{files} files";
        if (statusText.Length == 0) statusText = "Empty";
        if (_text != null) _text.text = statusText;

        gameObject.UnfuckLayoutHack();
    }
}
