using System.Collections.Generic;
using UnityEngine;

public class UI_EnableUI : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _currentUI, _UIToEnable;

    public void ChangeUI()
    {
        if (_currentUI.Count > 0)
        {
            foreach (var UI in _currentUI)
            {
                UI.SetActive(false);
            }
        }
        if (_UIToEnable.Count > 0)
        {
            foreach (var UI in _UIToEnable)
            {
                UI.SetActive(true);
            }
        }
    }
}
