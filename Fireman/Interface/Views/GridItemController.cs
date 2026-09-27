using System;
using System.IO;
using System.Linq;
using Fireman.Core;
using Fireman.Core.Thumbnails;
using Fireman.Core.Utils;
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

    /// <inheritdoc />
    public override async void UpdateItemInfo() {
        try {
            if (FileInfo == null) return;
            if (_name != null) _name.text = TextUtils.SanitizeForDisplay(FileInfo.Name);
            if (_icon != null) {
                var rect = _icon.rectTransform.rect;
                _icon.sprite = FileIcons.GetFileIcon(FileInfo);

                var thumb = await ThumbnailManager.GetThumbnailAsync(
                    FileInfo.FullName,
                    (int)Math.Ceiling(rect.width),
                    (int)Math.Ceiling(rect.height));

                if (thumb != null) {
                    _icon.sprite = Sprite.Create(thumb, new Rect(0, 0, thumb.width, thumb.height), Vector2.zero);
                }
            }
        } catch (Exception e) {
            Plugin.Log.LogInfo($"Error in updating item info: {e}");
        }
    }

    private void UpdateOpacity() {
        if (_group == null) return;
        var thisCut = FileOperationsManager.CurrentAction == CopyAction.Cut &&
                      FileOperationsManager.CopyBufferPaths.Any(path => path == FileInfo?.FullName);
        _group.alpha = thisCut ? 0.5f : 1f;
    }
}
