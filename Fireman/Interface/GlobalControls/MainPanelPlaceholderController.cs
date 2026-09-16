using System;
using Fireman.Core;
using NukeLib.UI;
using UnityEngine;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for the main panel's placeholder
/// </summary>
public class MainPanelPlaceholderController : MonoBehaviour {
    public TabHost? TargetHost;

    private GameObject? _empty;

    private void Start() {
        if (TargetHost == null) return;
        _empty = gameObject.FindRecursive("Empty");
        TargetHost.CurrentPathChanged += UpdatePlaceholder;
        UpdatePlaceholder();
    }

    private void UpdatePlaceholder() {
        if (TargetHost == null) return;
        var itemCount = TargetHost.GetCurrentDirItemsUnsorted().Length;
        _empty?.SetActiveAnimated(itemCount == 0, new Vector2(0, -10f), 30f);
    }
}
