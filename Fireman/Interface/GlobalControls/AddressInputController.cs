using System;
using System.IO;
using Fireman.Core;
using TMPro;
using UnityEngine;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for the address bar of the file explorer
/// </summary>
public class AddressInputController : MonoBehaviour {
    public Window? TargetWindow;
    private TMP_InputField? _input;
    private void Start() {
        _input = GetComponent<TMP_InputField>();
        _input.onEndEdit.AddListener(TrySetPath);
        if (TargetWindow != null) TargetWindow.CurrentPathChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void TrySetPath(string value) {
        if (Validations.Accessible(value) && TargetWindow != null) {
            TargetWindow.CurrentPath = value;
        } else {
            UpdateDisplay();
        }
    }

    private void OnDestroy() {
        if (TargetWindow != null) TargetWindow.CurrentPathChanged -= UpdateDisplay;
    }

    private void UpdateDisplay() {
        if (TargetWindow == null || _input == null) return;
        var path = TargetWindow.CurrentPath;
        _input.SetTextWithoutNotify(path);
    }
}
