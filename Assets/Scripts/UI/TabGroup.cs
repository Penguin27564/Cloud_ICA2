using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField]
    private Color _tabIdle, _tabActive;
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
        button.childBackground.color = _tabActive;
    }

    public void ResetTabs()
    {
        foreach (TabButton button in _tabButtons)
        {
            if (_selectedTab != null && _selectedTab ==  button) { continue; }
            button.background.color = _tabIdle;
            button.childBackground.color = _tabIdle;
        }
    }
}
