using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField]
    private Color _tabIdle, _tabActive, _tabHover;

    [SerializeField]
    private List<GameObject> _objectsToSwap;

    [SerializeField]
    private int _tabsToIgnore = 0;

    private List<TabButton> _tabButtons;
    private TabButton _selectedTab;

    public void Subscribe(TabButton button)
    {
        if (_tabButtons == null)
        {
            _tabButtons = new List<TabButton>();
        }

        _tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (_selectedTab == null || button != _selectedTab)
        {
            button.background.color = _tabHover;
            if (button.childBackground) button.childBackground.color = _tabHover;
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        _selectedTab = button;
        ResetTabs();
        button.background.color = _tabActive;
        if (button.childBackground) button.childBackground.color = _tabActive;

        int index = button.transform.GetSiblingIndex() - _tabsToIgnore;

        for (int i = 0; i < _objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                _objectsToSwap[i].SetActive(true);
            }
            else
            {
                _objectsToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach (TabButton button in _tabButtons)
        {
            if (_selectedTab != null && _selectedTab ==  button) { continue; }
            button.background.color = _tabIdle;
            if (button.childBackground) button.childBackground.color = _tabIdle;
        }
    }

    private void OnEnable()
    {
        _selectedTab = _tabButtons[0];
    }
}
