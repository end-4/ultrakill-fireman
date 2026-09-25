using System.IO;
using System.Linq;
using Notiffy.API;
using NukeLib.Utils;
using ThornClient.Core;
using ThornClient.Core.ConfigurableElements;
using ThornClient.Core.DataTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fireman.Interface;

/// <summary>
/// Configuration for Fireman
/// </summary>
public class Config : Module {
    /// <summary>
    /// Keybind to toggle file manager
    /// </summary>
    public static Setting<Keybind> ToggleFileManager = null!;

    public static Setting<Keybind> NewTabBind = null!;
    public static Setting<Keybind> CloseTabBind = null!;
    public static Setting<Keybind> BackBind = null!;
    public static Setting<Keybind> BackBindAlt = null!;
    public static Setting<Keybind> ForwardBind = null!;
    public static Setting<Keybind> ForwardBindAlt = null!;
    public static Setting<Keybind> ParentDirBind = null!;
    public static Setting<Keybind> LeftTabBind = null!;
    public static Setting<Keybind> LeftTabBindAlt = null!;
    public static Setting<Keybind> RightTabBind = null!;
    public static Setting<Keybind> RightTabBindAlt = null!;
    public static Setting<Keybind> CopyBind = null!;
    public static Setting<Keybind> CopyBindAlt = null!;
    public static Setting<Keybind> CutBind = null!;
    public static Setting<Keybind> CutBindAlt = null!;
    public static Setting<Keybind> PasteBind = null!;
    public static Setting<Keybind> PasteBindAlt = null!;
    public static Setting<Keybind> PermaDeleteBind = null!;
    public static Setting<Keybind> PermaDeleteBindAlt = null!;
    public static Setting<Keybind> FocusAddressBarBind = null!;
    public static Setting<Keybind> FocusAddressBarBindAlt = null!;

    public static Setting<bool> SortFoldersBeforeFiles = null!;

    public static Setting<Keybind> DebugPicker = null!;
    public static Setting<Keybind> DebugPickerMulti = null!;
    public static Setting<Keybind> DebugPickerFolders = null!;
    public static Setting<Keybind> DebugPickerFoldersMulti = null!;

    public static Setting<string> LastVersion = null!;

    /// <inheritdoc />
    public override Sprite Icon =>
        FileAssetUtils.LoadNewSprite(Path.Combine(Plugin.workingDir, "icon_clickgui.png"));

    /// <inheritdoc />
    public Config() : base(
        "fireman.config", "Fireman", "File manager for ULTRAKILL",
        ModuleCategory.Misc, hasToggling: false) {
        ToggleFileManager = CreateSetting("toggleManager", "Toggle file manager",
            "Keybind to toggle the file manager overlay", new Keybind(KeyCode.E, KeyCode.LeftAlt));
        ToggleFileManager.OnPress += ToggleOverlay;
        var bindsGroup = CreateGroup("moreBinds", "More keybinds", "Other keybinds");
        NewTabBind = CreateSetting("newTab", "New tab", "Keybind to open a new tab",
            new Keybind(KeyCode.T, KeyCode.LeftControl), bindsGroup);
        CloseTabBind = CreateSetting("closeTab", "Close tab", "Keybind to close current tab",
            new Keybind(KeyCode.W, KeyCode.LeftControl), bindsGroup);
        BackBind = CreateSetting("back", "Back", "Keybind to navigate back in history",
            new Keybind(KeyCode.LeftArrow, KeyCode.LeftAlt), bindsGroup);
        BackBindAlt = CreateSetting("backAlt", "Back (alternative)", "Keybind to navigate back in history",
            new Keybind(KeyCode.Mouse3), bindsGroup);
        ForwardBind = CreateSetting("forward", "Forward", "Keybind to navigate forward in history",
            new Keybind(KeyCode.RightArrow, KeyCode.LeftAlt), bindsGroup);
        ForwardBindAlt = CreateSetting("forwardAlt", "Forward (alternative)", "Keybind to navigate forward in history",
            new Keybind(KeyCode.Mouse4), bindsGroup);
        ParentDirBind = CreateSetting("parentDir", "To parent directory", "Keybind to navigate to parent directory",
            new Keybind(KeyCode.UpArrow, KeyCode.LeftAlt), bindsGroup);
        LeftTabBind = CreateSetting("leftTab", "Left tab", "Keybind to switch to the tab on the left",
            new Keybind(KeyCode.PageUp, KeyCode.LeftControl), bindsGroup);
        LeftTabBindAlt = CreateSetting("leftTabAlt", "Left tab (alternative)",
            "Keybind to switch to the tab on the left", new Keybind(KeyCode.None), bindsGroup);
        RightTabBind = CreateSetting("rightTab", "Right tab", "Keybind to switch to the tab on the right",
            new Keybind(KeyCode.PageDown, KeyCode.LeftControl), bindsGroup);
        RightTabBindAlt = CreateSetting("rightTabAlt", "Right tab (alternative)",
            "Keybind to switch to the tab on the right", new Keybind(KeyCode.Tab, KeyCode.LeftControl), bindsGroup);
        CopyBind = CreateSetting("copy", "Copy", "Keybind to copy items", new Keybind(KeyCode.C, KeyCode.LeftControl),
            bindsGroup);
        CopyBindAlt = CreateSetting("copyAlt", "Copy (alternative)", "Keybind to copy items",
            new Keybind(KeyCode.C, KeyCode.RightControl), bindsGroup);
        CutBind = CreateSetting("cut", "Cut", "Keybind to cut items", new Keybind(KeyCode.X, KeyCode.LeftControl),
            bindsGroup);
        CutBindAlt = CreateSetting("cutAlt", "Cut (alternative)", "Keybind to cut items",
            new Keybind(KeyCode.X, KeyCode.RightControl), bindsGroup);
        PasteBind = CreateSetting("paste", "Paste", "Keybind to paste items",
            new Keybind(KeyCode.V, KeyCode.LeftControl), bindsGroup);
        PasteBindAlt = CreateSetting("pasteAlt", "Paste (alternative)", "Keybind to paste items",
            new Keybind(KeyCode.V, KeyCode.RightControl), bindsGroup);
        PermaDeleteBind = CreateSetting("permaDelete", "Delete permanently", "Keybind to permanently delete items",
            new Keybind(KeyCode.Delete, KeyCode.LeftShift), bindsGroup);
        PermaDeleteBindAlt = CreateSetting("permaDeleteAlt", "Delete permanently (alternative)",
            "Keybind to permanently delete items", new Keybind(KeyCode.Delete, KeyCode.RightShift), bindsGroup);
        FocusAddressBarBind = CreateSetting("focusAddressBar", "Focus address bar", "Keybind to focus the address bar",
            new Keybind(KeyCode.L, KeyCode.LeftControl), bindsGroup);
        FocusAddressBarBindAlt = CreateSetting("focusAddressBarAlt", "Focus address bar (alternative)",
            "Keybind to focus the address bar", new Keybind(KeyCode.L, KeyCode.RightControl), bindsGroup);

        SortFoldersBeforeFiles = CreateSetting("sortFoldersBeforeFiles", "Sort folders before files",
            "Folders on top, then goes files", true);

        var debugGroup = CreateGroup("debug", "Debug", "Debug options");
        DebugPicker = CreateSetting("debugPicker", "File picker", "Activates a file picker", new Keybind(KeyCode.None),
            debugGroup);
        DebugPickerMulti = CreateSetting("debugPickerMulti", "File picker (multi)", "Activates a multi-select picker",
            new Keybind(KeyCode.None), debugGroup);
        DebugPickerFolders = CreateSetting("debugPickerFolders", "Folder picker", "Activates a folder picker",
            new Keybind(KeyCode.None), debugGroup);
        DebugPickerFoldersMulti = CreateSetting("debugPickerFoldersMulti", "Folder picker (multi)",
            "Activates a multi-select folder picker", new Keybind(KeyCode.None), debugGroup);
        LastVersion = CreateSetting("lastVersion", "Last version", "Last loaded version", "0.0.0", parent: debugGroup);
        DebugPicker.OnPress += () => DebugPick(false, false);
        DebugPickerMulti.OnPress += () => DebugPick(true, false);
        DebugPickerFolders.OnPress += () => DebugPick(false, true);
        DebugPickerFoldersMulti.OnPress += () => DebugPick(true, true);

        SceneUtils.SafeSceneLoadedNoParam += SendStartupNotifications;
    }

    private void DebugPick(bool multi, bool folders) {
        var comp = FileManager.CreatePicker(allowMultiSelection: multi, isSelectionFolder: folders);
        var picker = comp.gameObject;
        var kanvas = SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(obj => obj.name == "Canvas");
        if (kanvas == null || picker == null) return;
        picker.transform.SetParent(kanvas.transform, false);
        comp.ItemsPicked += pathArr => {
            NotificationSystem.NotifySend("Fireman::<color=#a5f2e2>Debug</color>",
                $"PATHS PICKED\n{pathArr.Stringify()}");
        };
    }

    // These are for the standalone window
    private static FileManager? _fm;

    private void ToggleOverlay() {
        if (!SceneUtils.IsSafe()) return;
        InitializeIfNeeded();
        var fmg = _fm?.gameObject;
        if (fmg != null) fmg.SetActive(!fmg.activeSelf);
        fmg?.transform.SetAsLastSibling();
    }

    private static void InitializeIfNeeded() {
        if (_fm != null) return;
        var canvas = SceneManager.GetActiveScene()
            .GetRootGameObjects().FirstOrDefault(obj => obj.name == "Canvas");
        if (canvas == null) return;
        _fm = FileManager.CreateManager();
        _fm.transform.SetParent(canvas.transform, false);
        _fm.hideFlags = HideFlags.HideAndDontSave;
        _fm.gameObject.SetActive(false); // False cuz we'll toggle after this
    }

    private void SendStartupNotifications() {
        try {
            if (LastVersion.Value == "0.0.0" && Plugin.PluginVersion == "0.1.0") {
                NotificationSystem.NotifySend("Fireman::<color=#e09330>Hello</color>",
                    $"Press {ToggleFileManager.Value.ToString(pretty: true)} to toggle the file manager",
                    iconFilePath: Plugin.PluginIconPath);
            }

            LastVersion.Value = Plugin.PluginVersion;
        } catch {
            // Ignore
        }
    }
}
