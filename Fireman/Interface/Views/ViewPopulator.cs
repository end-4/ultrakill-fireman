using System.IO;
using Fireman.Core;
using UnityEngine;

namespace Fireman.Interface.Views;

/// <summary>
/// Controller to populate a view
/// </summary>
public abstract class ViewPopulator : MonoBehaviour {
    public TabHost? TargetHost;

    protected virtual void Start() {
        if (TargetHost == null) return;
        TargetHost.CurrentPathChanged += Repopulate;
        Repopulate();
    }

    protected virtual void OnDestroy() {
        if (TargetHost != null) TargetHost.CurrentPathChanged -= Repopulate;
    }

    /// <summary>
    /// Clears and repopulates the view
    /// </summary>
    protected void Repopulate() {
        if (TargetHost == null) return;
        foreach (Transform childTrans in transform) Destroy(childTrans.gameObject);
        foreach (FileSystemInfo fileInfo in TargetHost.GetCurrentDirItems()) {
            var obj = CreateItem(fileInfo);
            obj.transform.SetParent(transform, false);
        }
    }

    /// <summary>
    /// Method to create the UI object for a file item
    /// </summary>
    /// <returns>The GameObject of the UI object</returns>
    protected abstract GameObject CreateItem(FileSystemInfo fileInfo);
}
