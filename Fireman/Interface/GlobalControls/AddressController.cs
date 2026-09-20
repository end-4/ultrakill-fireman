using System;
using Fireman.Core;
using NukeLib.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for the address bar, with both the input and breadcrumbs
/// </summary>
public class AddressController : MonoBehaviour {
    public Window? TargetWindow;

    private GameObject? _input;
    private GameObject? _breadcrumbRow;
    private TMP_InputField? _inputComp;

    private void Start() {
        _input = gameObject?.FindRecursive("Input");
        _inputComp = _input?.GetComponent<TMP_InputField>();
        if (_inputComp != null) {
            _inputComp.onDeselect.AddListener(UnfocusInput);
            _inputComp.onEndEdit.AddListener(UnfocusInput);
        }

        var inputControllerComp = _input?.AddComponent<AddressInputController>();
        if (inputControllerComp != null) inputControllerComp.TargetWindow = TargetWindow;

        _breadcrumbRow = gameObject?.FindRecursive("BreadcrumbScrollView/Viewport/Row");
        var bComp = _breadcrumbRow?.AddComponent<AddressBreadcrumbRowController>();
        if (bComp != null) bComp.TargetWindow = TargetWindow;

        var inputActivatorBtn = gameObject?.FindRecursive("BreadcrumbScrollView/InputActivatorButton");
        var comp = inputActivatorBtn?.GetOrAddComponent<ClickHandler>();
        if (comp != null) comp.OnPress += FocusInput;
    }

    /// <summary>
    /// Focus the input field
    /// </summary>
    public void FocusInput() {
        _breadcrumbRow?.SetActive(false);
        _input?.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_input, null);
        _inputComp?.ActivateInputField();
    }

    private void UnfocusInput(string _) {
        _input?.SetActive(false);
        _breadcrumbRow?.SetActive(true);
    }
}
