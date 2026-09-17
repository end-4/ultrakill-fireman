using System;
using Fireman.Core;
using NukeLib.UI;
using UnityEngine;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for the main panel's placeholder
/// </summary>
public class MainPanelPlaceholderController : MonoBehaviour {
    public Window? TargetWindow;

    private GameObject? _empty;

    private void Start() {
        if (TargetWindow == null) return;
        _empty = gameObject.FindRecursive("Empty");
        TargetWindow.CurrentPathChanged += UpdatePlaceholder;
        UpdatePlaceholder();
    }

    private void UpdatePlaceholder() {
        if (TargetWindow == null) return;
        var itemCount = TargetWindow.GetCurrentDirItemsUnsorted().Length;
        _empty?.SetActiveAnimated(itemCount == 0, new Vector2(0, -10f), 30f);
    }
}
