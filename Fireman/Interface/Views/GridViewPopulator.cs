using System.IO;
using ThornClient.Managers;
using UnityEngine;

namespace Fireman.Interface.Views;

/// <summary>
/// Populator for grid view
/// </summary>
public class GridViewPopulator : ViewPopulator {
    protected override GameObject CreateItem(FileSystemInfo fileInfo) {
        var prefab = AssetManager.Get<GameObject>(Plugin.BundleKey, "GridFileItem");
        var obj = Instantiate(prefab);
        if (obj == null) return null!;
        var comp = obj.AddComponent<GridItemController>();
        comp.FileInfo = fileInfo;
        comp.TargetWindow = TargetWindow;
        return obj;
    }
}
