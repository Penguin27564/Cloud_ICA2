using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem _instance;

    public static TooltipSystem Instance
    {
        get
        {
            if (_instance == null) _instance = new TooltipSystem();
            return _instance;
        }
    }

    public TooltipSystem()
    {
        _instance = this;
    }

    public Tooltip tooltip;

    public static void Show(string content, string header = "")
    {
        Instance.tooltip.SetText(content, header);
        Instance.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        Instance.tooltip.gameObject.SetActive(false);
    }
}
