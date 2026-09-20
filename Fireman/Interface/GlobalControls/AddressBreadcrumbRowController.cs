using System.IO;
using System.Linq;
using Fireman.Core;
using NukeLib.Utils;
using ThornClient.Managers;
using UnityEngine;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for breadcrumb row in the address bar
/// </summary>
public class AddressBreadcrumbRowController : MonoBehaviour {
    public Window? TargetWindow;
    private string? lastPath;

    private void Start() {
        if (TargetWindow != null) TargetWindow.CurrentPathChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDestroy() {
        if (TargetWindow != null) TargetWindow.CurrentPathChanged -= UpdateDisplay;
    }

    private void UpdateDisplay() {
        if (TargetWindow?.CurrentPath == lastPath) return;
        lastPath = TargetWindow?.CurrentPath;
        Repopulate();
    }

    private void Repopulate() {
        if (TargetWindow == null) return;
        foreach (Transform childTrans in transform) Destroy(childTrans.gameObject);
        var dirInfo = new DirectoryInfo(TargetWindow.CurrentPath);
        DirectoryInfo[] items = dirInfo.GetPathItems().ToArray();
        for (var i = 0; i < items.Length; i++) {
            var item = items[i];
            AddCrumb(item, i == 0, i == items.Length - 1);
        }
    }

    private void AddCrumb(DirectoryInfo dirInfo, bool first, bool last) {
        var prefab = AssetManager.Get<GameObject>(Plugin.BundleKey, "BreadcrumbPiece");
        var obj = Instantiate(prefab, transform);
        if (obj == null) return;
        var comp = obj.AddComponent<BreadcrumbPieceController>();
        comp.Directory = dirInfo;
        comp.First = first;
        comp.Last = last;
        comp.TargetWindow = this.TargetWindow;
    }
}
