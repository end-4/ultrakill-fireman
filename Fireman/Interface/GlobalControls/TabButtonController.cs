using System;
using System.IO;
using Fireman.Core;
using Fireman.Interface.Reusables;
using Fireman.Platform;
using NukeLib.UI;
using NukeLib.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for a tab button
/// </summary>
public class TabButtonController : MonoBehaviour {
    public Tab? TargetTab;
    public Window? TargetWindow;

    private Button? _btn;
    private Image? _icon;
    private TextMeshProUGUI? _name;
    private Button? _close;
    private ButtonActiveStateIndicator? _btnVisualState;

    private void Start() {
        // Find stuff, add components

        _btn = gameObject.GetComponent<Button>();
        _icon = gameObject.FindRecursive("Icon")?.GetComponent<Image>();
        _name = gameObject.FindRecursive("Name")?.GetComponent<TextMeshProUGUI>();
        _close = gameObject.FindRecursive("Close")?.GetComponent<Button>();
        _btnVisualState = gameObject.GetOrAddComponent<ButtonActiveStateIndicator>();

        // Hooks
        _btn?.onClick.AddListener(() => {
            if (TargetWindow != null && TargetTab != null) TargetWindow.CurrentTab = TargetTab;
        });
        _close?.onClick.AddListener(() => {
            if (TargetWindow != null && TargetTab != null) TargetWindow.CloseTab(TargetTab);
        });
        gameObject.GetOrAddComponent<ClickHandler>().OnMiddlePress += () => {
            if (TargetWindow != null && TargetTab != null) TargetWindow.CloseTab(TargetTab);
        };
        if (TargetTab != null) TargetTab.CurrentPathChanged += UpdateCurrentDir;
        if (TargetWindow != null) TargetWindow.CurrentTabChanged += UpdateActive;
        UpdateCurrentDir();
        UpdateActive();
    }

    private void OnDestroy() {
        if (TargetTab != null) TargetTab.CurrentPathChanged -= UpdateCurrentDir;
        if (TargetWindow != null) TargetWindow.CurrentTabChanged -= UpdateActive;
    }

    private void UpdateCurrentDir() {
        if (_name == null || _icon == null || TargetTab == null) return;
        var path = TargetTab.CurrentPath;
        var dirInfo = new DirectoryInfo(path);
        var baseName = dirInfo.Name;
        var targetIcon = Icons.GetFileIcon(dirInfo);
        if (_name.text != baseName) _name.text = baseName;
        if (_icon.sprite != targetIcon) _icon.sprite = targetIcon;

        gameObject.UnfuckLayoutHack();
    }

    private void UpdateActive() {
        if (_btnVisualState == null) return;
        _btnVisualState.Active = TargetWindow?.CurrentTab == TargetTab;
        gameObject.UnfuckLayoutHack();
    }
}
