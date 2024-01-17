using System.Collections.Generic;
using UnityEngine;

public class UI_EnableUI : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _currentUI, _UIToEnable;

    [SerializeField]
    private bool _toggle = false;

    public void ChangeUI()
    {
        if (_currentUI.Count > 0)
        {
            foreach (var UI in _currentUI)
            {
                if (!_toggle) UI.SetActive(false);
                else UI.SetActive(!UI.activeInHierarchy);
            }
        }
        if (_UIToEnable.Count > 0)
        {
            foreach (var UI in _UIToEnable)
            {
                if (!_toggle) UI.SetActive(true);
                else UI.SetActive(!UI.activeInHierarchy);
            }
        }
    }
}
