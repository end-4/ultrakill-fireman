using System;
using Fireman.Core;
using UnityEngine;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Handles keybinds for the file manager
/// </summary>
public class FileManagerKeybindHandler : MonoBehaviour {
    /// <summary>
    /// The tab host to control
    /// </summary>
    public TabHost? TargetHost;

    private void OnEnable() {
        Config.NewTabBind.OnPress += NewTab;
        Config.CloseTabBind.OnPress += CloseTab;
        Config.BackBind.OnPress += GoBack;
        Config.BackBindAlt.OnPress += GoBack;
        Config.ForwardBind.OnPress += GoForward;
        Config.ForwardBindAlt.OnPress += GoForward;
        Config.ParentDirBind.OnPress += GoUp;
        Config.LeftTabBind.OnPress += LeftTab;
        Config.LeftTabBindAlt.OnPress += LeftTab;
        Config.RightTabBind.OnPress += RightTab;
        Config.RightTabBindAlt.OnPress += RightTab;
    }

    private void OnDisable() {
        Config.NewTabBind.OnPress -= NewTab;
        Config.CloseTabBind.OnPress -= CloseTab;
        Config.BackBind.OnPress -= GoBack;
        Config.BackBindAlt.OnPress -= GoBack;
        Config.ForwardBind.OnPress -= GoForward;
        Config.ForwardBindAlt.OnPress -= GoForward;
        Config.ParentDirBind.OnPress -= GoUp;
        Config.LeftTabBind.OnPress -= LeftTab;
        Config.LeftTabBindAlt.OnPress -= LeftTab;
        Config.RightTabBind.OnPress -= RightTab;
        Config.RightTabBindAlt.OnPress -= RightTab;
    }

    private void NewTab() {
        if (!gameObject.activeInHierarchy) return;
        TargetHost?.NewTab();
    }

    private void CloseTab() {
        if (!gameObject.activeInHierarchy) return;
        TargetHost?.CloseTab();
    }

    private void GoForward() {
        TargetHost?.GoForward();
    }

    private void GoBack() {
        TargetHost?.GoBack();
    }

    private void GoUp() {
        TargetHost?.GoUp();
    }

    private void LeftTab() {
        if (TargetHost == null) return;
        var curr = TargetHost.CurrentTabIndex;
        var count = TargetHost.Count;
        TargetHost.CurrentTabIndex = (curr + count - 1) % count;
    }

    private void RightTab() {
        if (TargetHost == null) return;
        var curr = TargetHost.CurrentTabIndex;
        TargetHost.CurrentTabIndex = (curr + 1) % TargetHost.Count;
    }
}
