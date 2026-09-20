using System;
using System.Collections.Generic;
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
    /// The target window
    /// </summary>
    public Window? TargetWindow;

    private static Dictionary<string, string> NiceBookmarkNames = new() {
        [Paths.Home] = "Home",
        [Paths.AngryLevels] = "Angry levels",
        [Paths.ThornConfig] = "Thorn config"
    };

    private ButtonActiveStateIndicator? _btnVisualState;

    private void Start() {
        if (Path == null || TargetWindow == null) return;
        var ico = gameObject.FindRecursive("Icon")?.GetComponent<Image>();
        var text = gameObject.FindRecursive("Name")?.GetComponent<TextMeshProUGUI>();
        _btnVisualState = gameObject.GetOrAddComponent<ButtonActiveStateIndicator>();
        var icoSprite = FileIcons.GetFileIcon(Path);
        if (ico != null) ico.sprite = icoSprite;
        if (text != null) text.text = GetBookmarkName(Path);

        gameObject.GetOrAddComponent<ClickHandler>().OnPress += () => TargetWindow.CurrentPath = Path.FullName;
        TargetWindow.CurrentPathChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDestroy() {
        if (TargetWindow != null) TargetWindow.CurrentPathChanged -= UpdateDisplay;
    }

    private string GetBookmarkName(DirectoryInfo path) {
        if (NiceBookmarkNames.TryGetValue(path.FullName, out var niceName)) return niceName;
        return path.Name;
    }

    private void UpdateDisplay() {
        if (_btnVisualState == null || TargetWindow == null || Path == null) return;
        _btnVisualState.Active = TargetWindow.CurrentTab.CurrentPath == Path.FullName;
    }
}
