using System;
using System.IO;
using System.Linq;
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
public class GridItemController : FileItemController {
    private Image? _icon;
    private TextMeshProUGUI? _name;
    private Button? _btn;
    private CanvasGroup? _group;

    protected override void Start() {
        base.Start();
        _btn = GetComponent<Button>();
        _icon = gameObject.FindRecursive("Icon")?.GetComponent<Image>();
        _name = gameObject.FindRecursive("Name")?.GetComponent<TextMeshProUGUI>();
        _group = gameObject.GetComponent<CanvasGroup>();
        FileOperationsManager.CopyBufferChanged += UpdateOpacity;
        FileOperationsManager.CurrentActionChanged += UpdateOpacity;
        UpdateItemInfo();
    }

    protected override void OnDestroy() {
        FileOperationsManager.CopyBufferChanged -= UpdateOpacity;
        FileOperationsManager.CurrentActionChanged -= UpdateOpacity;
        base.OnDestroy();
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
        if (_name != null) _name.text = TextUtils.SanitizeForDisplay(FileInfo.Name);
    }

    private void UpdateOpacity() {
        if (_group == null) return;
        var thisCut = FileOperationsManager.CurrentAction == CopyAction.Cut &&
                      FileOperationsManager.CopyBufferPaths.Any(path => path == FileInfo?.FullName);
        _group.alpha = thisCut ? 0.5f : 1f;
    }
}
