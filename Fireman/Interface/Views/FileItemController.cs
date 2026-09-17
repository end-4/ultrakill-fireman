using System;
using System.IO;
using System.Linq;
using Fireman.Core;
using Fireman.Interface.Reusables;
using NukeLib.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fireman.Interface.Views;

/// <summary>
/// Controller for a file item
/// </summary>
public abstract class FileItemController : MonoBehaviour, ISelectHandler {
    /// <summary>
    /// The info of the item
    /// </summary>
    public FileSystemInfo? FileInfo;

    /// <summary>
    /// The tab host this item belongs to
    /// </summary>
    public Window? TargetWindow;

    /// <summary>
    /// The index of the item in the list
    /// </summary>
    public int Index { get; set; }

    protected ButtonActiveStateIndicator? ActiveIndicator;
    protected ClickHandler? ClickHandler;

    protected virtual void Start() {
        ActiveIndicator = gameObject.GetOrAddComponent<ButtonActiveStateIndicator>();
        ClickHandler = gameObject.GetOrAddComponent<ClickHandler>();

        ClickHandler.OnPress += HandlePress;
        if (TargetWindow != null) {
            ClickHandler.OnDoubleClick += TargetWindow.ActivateSelection;
            TargetWindow.CurrentTab.Selection.SelectionChanged += UpdateSelectionVisual;
        }

        // Set initial visual state
        UpdateSelectionVisual();
    }

    protected virtual void OnDestroy() {
        if (TargetWindow != null) {
            TargetWindow.CurrentTab.Selection.SelectionChanged -= UpdateSelectionVisual;
        }
    }
    public virtual void UpdateSelectionVisual() {
        if (ActiveIndicator == null || TargetWindow == null || FileInfo == null) return;
        ActiveIndicator.Active = TargetWindow.CurrentTab.Selection.IsSelected(FileInfo.FullName);
    }

    private void HandlePress() {
        if (TargetWindow == null || FileInfo == null) return;
        var selection = TargetWindow.CurrentTab.Selection;
        bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isCtrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (isCtrl) { // Pick/Unpick
            selection.Toggle(FileInfo.FullName, Index);
        } else if (isShift) { // Range Select
            var allItems = TargetWindow.GetCurrentDirItems()
                .Select(info => info.FullName)
                .ToList();
            int anchor = selection.AnchorIndex >= 0 ? selection.AnchorIndex : Index;
            selection.SelectRange(allItems, anchor, Index);
        } else { // Single select
            selection.SelectOnly(FileInfo.FullName, Index);
        }
    }

    public virtual void SetSelected(bool isSelected) {
        if (ActiveIndicator != null) ActiveIndicator.Active = isSelected;
    }

    /// <summary>
    /// Fired by Unity's EventSystem whenever this item receives keyboard focus
    /// </summary>
    public virtual void OnSelect(BaseEventData eventData) {
        EnsureVisibleInScrollView();
        // If focus was caused by a mouse click, HandlePress will take care of it
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) return;
        if (TargetWindow == null || FileInfo == null) return;
        var selection = TargetWindow.CurrentTab.Selection;
        bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isShift) { // Shift arrow -> extend selection range
            var allItems = TargetWindow.GetCurrentDirItems()
                .Select(info => info.FullName)
                .ToList();
            int anchor = selection.AnchorIndex >= 0 ? selection.AnchorIndex : Index;
            selection.SelectRange(allItems, anchor, Index);
        } else { // Normal arrowing -> select just this one
            selection.SelectOnly(FileInfo.FullName, Index);
        }
    }

    /// <summary>
    /// Scrolls the ScrollRect viewport so this item stays visible
    /// </summary>
    protected void EnsureVisibleInScrollView() {
        var scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect == null || scrollRect.viewport == null) return;
        var target = (RectTransform)transform;
        var viewport = scrollRect.viewport;
        Vector3[] targetCorners = new Vector3[4];
        Vector3[] viewportCorners = new Vector3[4];
        target.GetWorldCorners(targetCorners);
        viewport.GetWorldCorners(viewportCorners);
        // targetCorners: 0=BottomLeft, 1=TopLeft, 2=TopRight, 3=BottomRight
        if (targetCorners[1].y > viewportCorners[1].y) {
            // Item is clipped above viewport -> scroll down
            float diff = targetCorners[1].y - viewportCorners[1].y;
            scrollRect.content.anchoredPosition -= new Vector2(0, diff);
        } else if (targetCorners[0].y < viewportCorners[0].y) {
            // Item is clipped below viewport -> scroll up
            float diff = viewportCorners[0].y - targetCorners[0].y;
            scrollRect.content.anchoredPosition += new Vector2(0, diff);
        }
    }
}

public readonly record struct PointerClickInfo(bool IsCtrl, bool IsShift) {
    public readonly bool IsCtrl = IsCtrl;
    public readonly bool IsShift = IsShift;
}
