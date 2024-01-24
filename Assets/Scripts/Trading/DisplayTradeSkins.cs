using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class DisplayTradeSkins : MonoBehaviour
{
    [SerializeField]
    private Button _redSkin, _blueSkin, _yellowSkin;

    [SerializeField]
    private bool _isOffering = true;

    private void OnEnable()
    {
        _redSkin.interactable = false;
        _blueSkin.interactable = false;
        _yellowSkin.interactable = false;

        GetPlayerSkins();
    }

    private void GetPlayerSkins()
    {
        // Get inventory skins
        foreach (ItemInstance item in PFDataMgr.Instance.currentPlayerInventoryItems)
        {
            if (item.ItemId == "Blue")
            {
                _blueSkin.interactable = true;
            }
            else if (item.ItemId == "Red")
            {
                _redSkin.interactable = true;
            }
            else if (item.ItemId == "Yellow")
            {
                _yellowSkin.interactable = true;
            }
        }
    }
}
