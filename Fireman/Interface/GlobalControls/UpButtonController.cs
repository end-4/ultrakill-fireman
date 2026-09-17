using Fireman.Core;
using NukeLib.UI;
using NukeLib.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for the up button
/// </summary>
// TODO: this is kind of a copy of the backward button. It's not too serious but maybe find a better way
public class UpButtonController : MonoBehaviour {
    public Window? TargetWindow;

    private Image? _icon;
    private Button? _button;

    private void Start() {
        _icon = gameObject.FindRecursive("Icon")?.GetComponent<Image>();
        _button = gameObject.GetComponent<Button>();
        gameObject.GetOrAddComponent<ClickHandler>().OnPress += () => TargetWindow?.GoUp();
        if (TargetWindow != null) TargetWindow.CurrentPathChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDestroy() {
        if (TargetWindow != null) TargetWindow.CurrentPathChanged -= UpdateDisplay;
    }

    private void UpdateDisplay() {
        if (TargetWindow == null) return;
        var canGoUp = TargetWindow.CanGoUp;
        if (_icon != null) _icon.color = canGoUp ? Color.white : Color.white.Transparentize(0.5f);
        if (_button != null) _button.interactable = canGoUp;
    }
}
