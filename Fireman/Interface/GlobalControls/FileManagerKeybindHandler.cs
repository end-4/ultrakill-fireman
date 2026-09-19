using System;
using System.Linq;
using Fireman.Core;
using NukeLib.Utils;
using UnityEngine;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Handles keybinds for the file manager
/// </summary>
public class FileManagerKeybindHandler : MonoBehaviour {
    /// <summary>
    /// The window to control
    /// </summary>
    public Window? TargetWindow;

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
        Config.CopyBind.OnPress += Copy;
        Config.CopyBindAlt.OnPress += Copy;
        Config.CutBind.OnPress += Cut;
        Config.CutBindAlt.OnPress += Cut;
        Config.PasteBind.OnPress += Paste;
        Config.PasteBindAlt.OnPress += Paste;
        Config.PermaDeleteBind.OnPress += PermaDelete;
        Config.PermaDeleteBindAlt.OnPress += PermaDelete;
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
        Config.CopyBind.OnPress -= Copy;
        Config.CopyBindAlt.OnPress -= Copy;
        Config.CutBind.OnPress -= Cut;
        Config.CutBindAlt.OnPress -= Cut;
        Config.PasteBind.OnPress -= Paste;
        Config.PasteBindAlt.OnPress -= Paste;
        Config.PermaDeleteBind.OnPress -= PermaDelete;
        Config.PermaDeleteBindAlt.OnPress -= PermaDelete;
    }

    private void NewTab() {
        if (!gameObject.activeInHierarchy) return;
        TargetWindow?.NewTab();
    }

    private void CloseTab() {
        if (!gameObject.activeInHierarchy) return;
        TargetWindow?.CloseTab();
    }

    private void GoForward() {
        if (!gameObject.activeInHierarchy) return;
        TargetWindow?.GoForward();
    }

    private void GoBack() {
        if (!gameObject.activeInHierarchy) return;
        TargetWindow?.GoBack();
    }

    private void GoUp() {
        if (!gameObject.activeInHierarchy) return;
        TargetWindow?.GoUp();
    }

    private void LeftTab() {
        if (!gameObject.activeInHierarchy || TargetWindow == null) return;
        var curr = TargetWindow.CurrentTabIndex;
        var count = TargetWindow.Count;
        TargetWindow.CurrentTabIndex = (curr + count - 1) % count;
    }

    private void RightTab() {
        if (!gameObject.activeInHierarchy || TargetWindow == null) return;
        var curr = TargetWindow.CurrentTabIndex;
        TargetWindow.CurrentTabIndex = (curr + 1) % TargetWindow.Count;
    }

    private void Copy() {
        // Plugin.Log.LogInfo($"Copy:  {TargetWindow?.CurrentTab.Selection.SelectedPaths.ToArray().Stringify()}");
        if (!gameObject.activeInHierarchy || TargetWindow == null) return;
        FileOperationsManager.Copy(TargetWindow.CurrentTab.Selection);
    }

    private void Cut() {
        // Plugin.Log.LogInfo($"Cut:   {TargetWindow?.CurrentTab.Selection.SelectedPaths.ToArray().Stringify()}");
        if (!gameObject.activeInHierarchy || TargetWindow == null) return;
        FileOperationsManager.Cut(TargetWindow.CurrentTab.Selection);
    }

    private void Paste() {
        // Plugin.Log.LogInfo($"Paste: {TargetWindow?.CurrentPath}");
        if (!gameObject.activeInHierarchy || TargetWindow == null) return;
        FileOperationsManager.Paste(TargetWindow.CurrentPath);
    }

    private void PermaDelete() {
        if (!gameObject.activeInHierarchy || TargetWindow == null) return;
        FileOperationsManager.PermaDelete(TargetWindow.CurrentTab.Selection);
    }
}
