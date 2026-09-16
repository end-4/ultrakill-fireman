using System;
using Fireman.Core;
using NukeLib.UI;
using NukeLib.Utils;
using ThornClient.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for the back button
/// </summary>
public class BackButtonController : MonoBehaviour {
    public TabHost? TargetHost;

    private Image? _icon;
    private Button? _button;

    private void Start() {
        _icon = gameObject.FindRecursive("Icon")?.GetComponent<Image>();
        _button = gameObject.GetComponent<Button>();
        gameObject.GetOrAddComponent<ClickHandler>().OnPress += () => TargetHost?.GoBack();
        if (TargetHost != null) TargetHost.CurrentPathChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDestroy() {
        if (TargetHost != null) TargetHost.CurrentPathChanged -= UpdateDisplay;
    }

    private void UpdateDisplay() {
        if (TargetHost == null) return;
        var iconName = TargetHost.CanGoBack ? "direction_left" : "direction_left_inactive";
        var icon = AssetManager.Get<Sprite>(Plugin.BundleKey, iconName);
        if (_icon != null) {
            _icon.sprite = icon;
            _icon.color = TargetHost.CanGoBack ? Color.white : Color.white.Transparentize(0.5f);
        }
        if (_button != null) _button.interactable = TargetHost.CanGoBack;
    }
}
