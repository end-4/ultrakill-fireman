using System.IO;
using Fireman.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fireman.Interface.Views;

/// <summary>
/// Controller to populate a view
/// </summary>
public abstract class ViewPopulator : MonoBehaviour {
    public Window? TargetWindow;

    protected virtual void Start() {
        if (TargetWindow == null) return;
        TargetWindow.CurrentPathChanged += Repopulate;
        Repopulate();
    }

    protected virtual void OnDestroy() {
        if (TargetWindow != null) TargetWindow.CurrentPathChanged -= Repopulate;
    }

    /// <summary>
    /// Clears and repopulates the view
    /// </summary>
    protected void Repopulate() {
        if (TargetWindow == null) return;
        foreach (Transform childTrans in transform) Destroy(childTrans.gameObject);
        var items = TargetWindow.GetCurrentDirItems();
        for (var index = 0; index < items.Length; index++) {
            var fileInfo = items[index];
            var obj = CreateItem(fileInfo, index);
            obj.transform.SetParent(transform, false);
        }
    }

    /// <summary>
    /// Method to create the UI object for a file item
    /// </summary>
    /// <returns>The GameObject of the UI object</returns>
    protected abstract GameObject CreateItem(FileSystemInfo fileInfo, int index);

    protected virtual void Update() {
        if (TargetWindow == null) return;

        // Don't steal arrow presses if typing
        var currentSelected = EventSystem.current?.currentSelectedGameObject;
        if (currentSelected != null && currentSelected.GetComponent<TMP_InputField>() != null) return;
        bool arrowPressed = Input.GetKeyDown(KeyCode.UpArrow) ||
                            Input.GetKeyDown(KeyCode.DownArrow) ||
                            Input.GetKeyDown(KeyCode.LeftArrow) ||
                            Input.GetKeyDown(KeyCode.RightArrow);

        // Arrow was pressed but nothing focused -> focus first item
        if (arrowPressed && (currentSelected == null || currentSelected.GetComponent<FileItemController>() == null)) {
            var firstItem = GetComponentInChildren<FileItemController>();
            if (firstItem != null) {
                firstItem.GetComponent<Selectable>()?.Select();
            }
            return;
        }

        // Enter -> activate
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            TargetWindow.ActivateSelection();
        }

        // Escape -> clear selection
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TargetWindow.CurrentTab.Selection.Clear();
        }
    }
}
