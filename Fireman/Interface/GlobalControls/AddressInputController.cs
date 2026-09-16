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
    public TabHost? TargetHost;
    private TMP_InputField? _input;
    private void Start() {
        _input = GetComponent<TMP_InputField>();
        _input.onEndEdit.AddListener(TrySetPath);
        if (TargetHost != null) TargetHost.CurrentPathChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void TrySetPath(string value) {
        if (Directory.Exists(value) && TargetHost != null) {
            TargetHost.CurrentPath = value;
        }
    }

    private void OnDestroy() {
        if (TargetHost != null) TargetHost.CurrentPathChanged -= UpdateDisplay;
    }

    private void UpdateDisplay() {
        if (TargetHost == null || _input == null) return;
        var path = TargetHost.CurrentPath;
        _input.SetTextWithoutNotify(path);
    }
}
