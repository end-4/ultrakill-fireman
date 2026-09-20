using System;
using System.Linq;
using Fireman.Core;
using NukeLib.UI;
using NukeLib.Utils;
using ThornClient.Managers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for a row of tabs
/// </summary>
public class TabRowController : MonoBehaviour {
    /// <summary>
    /// The Window this controller gets tabs from
    /// </summary>
    public Window? TargetWindow;

    private GameObject? _newButton;

    private void Start() {
        // if (TargetWindow != null) TargetWindow.TabsChanged += Repopulate;
        if (TargetWindow != null) {
            TargetWindow.TabAdded += AddTab;
            TargetWindow.TabRemoved += RemoveTab;
            TargetWindow.CurrentPathChanged += gameObject.UnfuckLayoutHack;
        }

        Repopulate();
    }

    private void OnDestroy() {
        // if (TargetWindow != null) TargetWindow.TabsChanged -= Repopulate;
        if (TargetWindow != null) {
            TargetWindow.TabAdded -= AddTab;
            TargetWindow.TabRemoved -= RemoveTab;
            TargetWindow.CurrentPathChanged -= gameObject.UnfuckLayoutHack;
        }
    }

    private void AddTab(Tab tab) {
        var prefab = AssetManager.Get<GameObject>(Plugin.BundleKey, "WindowTab");
        var obj = Instantiate(prefab, transform);
        if (obj == null) return;
        var comp = obj.AddComponent<TabButtonController>();
        comp.TargetTab = tab;
        comp.TargetWindow = TargetWindow;
        _newButton?.transform.SetAsLastSibling();
    }

    private void RemoveTab(Tab tab) {
        gameObject
            .GetComponentsInChildren<TabButtonController>()
            .Where(controller => controller.TargetTab == tab)
            .ToList().ForEach(controller => Destroy(controller.gameObject));
    }

    private void Repopulate() {
        if (TargetWindow == null) return;
        foreach (Transform childTrans in transform) Destroy(childTrans.gameObject);
        foreach (Tab tab in TargetWindow.Tabs) AddTab(tab);
        EnsureNewButton();
        gameObject.UnfuckLayoutHack();
        ExecutionUtils.RunNextFrame(() => {
            if (gameObject != null) gameObject.UnfuckLayoutHack();
        });
    }

    private void EnsureNewButton() {
        var newPrefab = AssetManager.Get<GameObject>(Plugin.BundleKey, "NewTab");
        if (_newButton == null) {
            _newButton = Object.Instantiate(newPrefab, transform);
            _newButton.GetOrAddComponent<Button>().onClick.AddListener(AddTab);
        }
        _newButton?.transform.SetAsLastSibling();
    }

    private void AddTab() {
        TargetWindow?.NewTab();
    }
}
