using System.IO;
using Fireman.Core;
using UnityEngine;

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
        foreach (FileSystemInfo fileInfo in TargetWindow.GetCurrentDirItems()) {
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
