using System;
using System.IO;
using Fireman.Core;
using Fireman.Platform;
using NukeLib.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fireman.Interface.Views;

/// <summary>
/// Controller for a grid item (file/folder)
/// </summary>
public class GridItemController : MonoBehaviour {
    /// <summary>
    /// The info of the item
    /// </summary>
    public FileSystemInfo? FileInfo;

    /// <summary>
    /// The window this item belongs to
    /// </summary>
    public Window? TargetWindow;

    private Image? _icon;
    private TextMeshProUGUI? _name;
    private Button? _btn;
    private ClickHandler? _clickHandler;

    private void Start() {
        _btn = GetComponent<Button>();
        _icon = gameObject.FindRecursive("Icon")?.GetComponent<Image>();
        _name = gameObject.FindRecursive("Name")?.GetComponent<TextMeshProUGUI>();
        _clickHandler = gameObject.GetOrAddComponent<ClickHandler>();
        _clickHandler.OnDoubleClick += ActivateItem;
        UpdateItemInfo();
    }

    private void ActivateItem() {
        if (FileInfo is DirectoryInfo dir) {
            if (TargetWindow == null) return;
            TargetWindow.CurrentPath = dir.FullName;
        }
    }

    private void UpdateItemInfo() {
        if (FileInfo == null) return;
        if (_icon != null) _icon.sprite = Icons.GetFileIcon(FileInfo);
        if (_name != null) _name.text = FileInfo.Name;
    }
}
