using System;
using UnityEngine;

namespace Fireman.Interface.GlobalControls;

/// <summary>
/// Controller for the actions bar
/// </summary>
public class ActionsBarController : MonoBehaviour {
    private void Start() {
        gameObject.SetActive(false); // for now...
    }
}
