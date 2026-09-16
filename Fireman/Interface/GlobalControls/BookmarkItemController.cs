using System;
using System.IO;
using Fireman.Core;
using Fireman.Interface.Reusables;
using Fireman.Platform;
using NukeLib.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for a clickable bookmark item
/// </summary>
public class BookmarkItemController : MonoBehaviour {
    /// <summary>
    /// The path of the bookmark
    /// </summary>
    public DirectoryInfo? Path;

    /// <summary>
    /// The target tab host
    /// </summary>
    public TabHost? TargetHost;

    private ButtonActiveStateIndicator? _btnVisualState;

    private void Start() {
        if (Path == null || TargetHost == null) return;
        var ico = gameObject.FindRecursive("Icon")?.GetComponent<Image>();
        var text = gameObject.FindRecursive("Name")?.GetComponent<TextMeshProUGUI>();
        _btnVisualState = gameObject.GetOrAddComponent<ButtonActiveStateIndicator>();
        var icoSprite = Icons.GetFileIcon(Path);
        if (ico != null) ico.sprite = icoSprite;
        if (text != null) text.text = GetBookmarkName(Path);

        gameObject.GetOrAddComponent<ClickHandler>().OnPress += () => TargetHost.CurrentPath = Path.FullName;
        TargetHost.CurrentPathChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDisable() {
        if (TargetHost != null) TargetHost.CurrentPathChanged -= UpdateDisplay;
    }

    private string GetBookmarkName(DirectoryInfo path) {
        return path.FullName == Paths.Home ? "Home" : path.Name;
    }

    private void UpdateDisplay() {
        if (_btnVisualState == null || TargetHost == null || Path == null) return;
        _btnVisualState.Active = TargetHost.CurrentTab.CurrentPath == Path.FullName;
    }
}
