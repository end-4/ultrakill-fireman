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
    /// The TabHost this controller gets tabs from
    /// </summary>
    public TabHost? TargetHost;

    private GameObject? _newButton;

    private void Start() {
        // if (TargetHost != null) TargetHost.TabsChanged += Repopulate;
        if (TargetHost != null) {
            TargetHost.TabAdded += AddTab;
            TargetHost.TabRemoved += RemoveTab;
            TargetHost.CurrentPathChanged += gameObject.UnfuckLayoutHack;
        }

        Repopulate();
    }

    private void OnDestroy() {
        // if (TargetHost != null) TargetHost.TabsChanged -= Repopulate;
        if (TargetHost != null) {
            TargetHost.TabAdded -= AddTab;
            TargetHost.TabRemoved -= RemoveTab;
            TargetHost.CurrentPathChanged -= gameObject.UnfuckLayoutHack;
        }
    }

    private void AddTab(Tab tab) {
        var prefab = AssetManager.Get<GameObject>(Plugin.BundleKey, "WindowTab");
        var obj = Instantiate(prefab, transform);
        if (obj == null) return;
        var comp = obj.AddComponent<TabButtonController>();
        comp.TargetTab = tab;
        comp.TargetHost = TargetHost;
        _newButton?.transform.SetAsLastSibling();
    }

    private void RemoveTab(Tab tab) {
        gameObject
            .GetComponentsInChildren<TabButtonController>()
            .Where(controller => controller.TargetTab == tab)
            .ToList().ForEach(controller => Destroy(controller.gameObject));
    }

    private void Repopulate() {
        if (TargetHost == null) return;
        foreach (Transform childTrans in transform) Destroy(childTrans.gameObject);
        foreach (Tab tab in TargetHost.Tabs) AddTab(tab);
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
        TargetHost?.NewTab();
    }
}
