using UnityEngine;
using UnityEngine.UI;

namespace Fireman.Interface.Reusables;

/// <summary>
/// Component that forces a selected state on a button when its Active state is set to true
/// </summary>
public class ButtonActiveStateIndicator : MonoBehaviour {
    /// <summary>
    /// Active state. Set to true to force the button be visually selected
    /// </summary>
    public bool Active {
        get;
        set {
            if (field == value) return;
            field = value;
            UpdateState();
        }
    } = false;

    public Color? SelectedColor = null;
    public Color? NormalColor = null;

    private Button _btn;

    private void OnEnable() {
        _btn = GetComponent<Button>();
        // Remember original colors
        if (SelectedColor == null) SelectedColor = _btn.colors.selectedColor;
        if (NormalColor == null) NormalColor = _btn.colors.normalColor;
    }

    protected void UpdateState() {
        var clrs = _btn.colors;
        var targetColor = Active ? (SelectedColor ?? clrs.selectedColor) : (NormalColor ?? clrs.normalColor);
        clrs.normalColor = targetColor;
        _btn.colors = clrs;
    }
}
