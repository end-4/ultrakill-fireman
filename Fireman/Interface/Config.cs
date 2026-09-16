using System.IO;
using System.Linq;
using Notiffy.API;
using NukeLib.Utils;
using ThornClient.Core;
using ThornClient.Core.ConfigurableElements;
using ThornClient.Core.DataTypes;
using ThornClient.Managers;
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
    public static Setting<bool> SortFoldersBeforeFiles = null!;

    /// <inheritdoc />
    public override Sprite Icon =>
        FileAssetUtils.LoadNewSprite(Path.Combine(Plugin.workingDir, "icon_clickgui.png"));

    /// <inheritdoc />
    public Config() : base(
        "fireman.config", "Fireman file manager", "File manager for ULTRAKILL",
        ModuleCategory.Misc, hasToggling: false) {
        ToggleFileManager = CreateSetting("toggleManager", "Toggle file manager",
            "Keybind to toggle the file manager overlay", new Keybind(KeyCode.E, KeyCode.LeftAlt));
        ToggleFileManager.OnPress += ToggleOverlay;
        var bindsGroup = CreateGroup("moreBinds", "More keybinds", "Other keybinds");
        NewTabBind = CreateSetting("newTab", "New tab", "Keybind to open a new tab", new Keybind(KeyCode.T, KeyCode.LeftControl), bindsGroup);
        CloseTabBind = CreateSetting("closeTab", "Close tab", "Keybind to close current tab", new Keybind(KeyCode.W, KeyCode.LeftControl), bindsGroup);
        BackBind = CreateSetting("back", "Back", "Keybind to navigate back in history", new Keybind(KeyCode.LeftArrow, KeyCode.LeftAlt), bindsGroup);
        BackBindAlt = CreateSetting("backAlt", "Back (alternative)", "Keybind to navigate back in history", new Keybind(KeyCode.Mouse3), bindsGroup);
        ForwardBind = CreateSetting("forward", "Forward", "Keybind to navigate forward in history", new Keybind(KeyCode.RightArrow, KeyCode.LeftAlt), bindsGroup);
        ForwardBindAlt = CreateSetting("forwardAlt", "Forward (alternative)", "Keybind to navigate forward in history", new Keybind(KeyCode.Mouse4), bindsGroup);
        ParentDirBind = CreateSetting("parentDir", "To parent directory", "Keybind to navigate to parent directory", new Keybind(KeyCode.UpArrow, KeyCode.LeftAlt), bindsGroup);
        LeftTabBind = CreateSetting("leftTab", "Left tab", "Keybind to switch to the tab on the left", new Keybind(KeyCode.PageUp, KeyCode.LeftControl), bindsGroup);
        LeftTabBindAlt = CreateSetting("leftTab", "Left tab (alternative)", "Keybind to switch to the tab on the left", new Keybind(KeyCode.None), bindsGroup);
        RightTabBind = CreateSetting("rightTab", "Right tab", "Keybind to switch to the tab on the right", new Keybind(KeyCode.PageDown, KeyCode.LeftControl), bindsGroup);
        RightTabBindAlt = CreateSetting("rightTab", "Right tab (alternative)", "Keybind to switch to the tab on the right", new Keybind(KeyCode.Tab, KeyCode.LeftControl), bindsGroup);

        SortFoldersBeforeFiles = CreateSetting("sortFoldersBeforeFiles", "Sort folders before files", "Folders on top, then goes files", true);
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
        _fm = FileManager.NewInstance();
        _fm.transform.SetParent(canvas.transform, false);
        _fm.hideFlags = HideFlags.HideAndDontSave;
        _fm.gameObject.SetActive(false); // False cuz we'll toggle after this
    }
}
