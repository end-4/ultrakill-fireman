using System;
using Fireman.Core;
using NukeLib.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for the actions bar
/// </summary>
public class ActionsBarController : MonoBehaviour {
    public FileManager? TargetFileManager;
    public Window? TargetWindow;
    public Button? _cancel;
    public ClickHandler? _confirm;

    private void Start() {
        if (TargetWindow == null) return;
        var isPicker = TargetWindow.PickerMode;
        if (!isPicker) {
            gameObject.SetActive(false);
            return;
        }

        _cancel = gameObject.FindRecursive("Cancel")?.GetComponent<Button>();
        _confirm = gameObject.FindRecursive("Select")?.GetOrAddComponent<ClickHandler>();
        _cancel?.onClick.AddListener(() => Destroy(TargetFileManager?.gameObject) );
        _confirm?.OnPress += () => {
            TargetWindow?.PickSelection();
        };
        gameObject.SetActive(true);
    }
}
