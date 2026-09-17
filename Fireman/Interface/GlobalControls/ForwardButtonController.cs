using System;
using Fireman.Core;
using NukeLib.UI;
using NukeLib.Utils;
using ThornClient.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for the forward button
/// </summary>
// Note: this is kind of a copy of the backward button
public class ForwardButtonController : MonoBehaviour {
    public Window? TargetWindow;

    private Image? _icon;
    private Button? _button;

    private void Start() {
        _icon = gameObject.FindRecursive("Icon")?.GetComponent<Image>();
        _button = gameObject.GetComponent<Button>();
        gameObject.GetOrAddComponent<ClickHandler>().OnPress += () => TargetWindow?.GoForward();
        if (TargetWindow != null) TargetWindow.CurrentPathChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDestroy() {
        if (TargetWindow != null) TargetWindow.CurrentPathChanged -= UpdateDisplay;
    }

    private void UpdateDisplay() {
        if (TargetWindow == null) return;
        var iconName = TargetWindow.CanGoForward ? "direction_right" : "direction_right_inactive";
        var icon = AssetManager.Get<Sprite>(Plugin.BundleKey, iconName);
        if (_icon != null) {
            _icon.sprite = icon;
            _icon.color = TargetWindow.CanGoForward ? Color.white : Color.white.Transparentize(0.5f);
        }
        if (_button != null) _button.interactable = TargetWindow.CanGoForward;
    }
}
