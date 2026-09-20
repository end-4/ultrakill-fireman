using System;
using System.IO;
using Fireman.Core;
using Fireman.Platform;
using NukeLib.UI;
using ThornClient.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for a breadcrumb piece
/// </summary>
public class BreadcrumbPieceController : MonoBehaviour {
    public Window? TargetWindow;
    public DirectoryInfo? Directory;
    public bool First = true;
    public bool Last = true;

    private Image? _bg;
    private Image? _icon;
    private TextMeshProUGUI? _text;
    private Button? _btn;

    private void Start() {
        _bg = GetComponent<Image>();
        _btn = GetComponent<Button>();
        _icon = gameObject.FindRecursive("Icon")?.GetComponent<Image>();
        _text = gameObject.FindRecursive("Name")?.GetComponent<TextMeshProUGUI>();
        _btn.onClick.AddListener(GoTo);
        UpdatePieceShape();
        UpdateInfo();
    }

    private void GoTo() {
        if (TargetWindow == null || Directory == null) return;
        TargetWindow.CurrentPath = Directory.FullName;
    }

    private void UpdatePieceShape() {
        if (_bg == null) return;
        var assetName = (First, Last) switch {
            (true, true) => "Round_FillLarge",
            (true, false) => "Round_FillLargeLeft",
            (false, true) => "Round_FillLargeRight",
            _ => "FillLarge"
        };
        var sprite = AssetManager.Get<Sprite>(Plugin.BundleKey, assetName);
        _bg.sprite = sprite;
    }

    private void UpdateInfo() {
        if (Directory == null || _icon == null || _text == null) return;
        _text.text = Directory.Name;
        string iconName = "";
        if (Directory.Parent == null) iconName = "hard_disk_symbolic";
        else if (Directory.FullName == Paths.Game) iconName = "game_symbolic";
        else if (Directory.FullName == Paths.BepInEx) iconName = "bepinex_symbolic";
        else if (Directory.FullName == Paths.Cybergrind) iconName = "cybergrind_symbolic";
        else if (Directory.FullName == Paths.Home) iconName = "home_symbolic";
        else if (Directory.FullName == Paths.Downloads) iconName = "downloads_symbolic";
        else if (Directory.FullName == Paths.Documents) iconName = "documents_symbolic";
        else if (Directory.FullName == Paths.Images) iconName = "images_symbolic";
        else if (Directory.FullName == Paths.Music) iconName = "music_symbolic";
        else if (Directory.FullName == Paths.Videos) iconName = "videos_symbolic";
        else if (Directory.FullName == Paths.Thorn) iconName = "thorn_symbolic";

        if (iconName != "") {
            _icon.sprite = AssetManager.Get<Sprite>(FileIcons.BundleKey, iconName);
            _icon.gameObject.SetActive(true);
        } else {
            _icon.gameObject.SetActive(false);
        }
    }
}
