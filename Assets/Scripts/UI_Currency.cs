using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class UI_Currency : MonoBehaviour
{
    private TMP_Text _currencyText;

    private void Start()
    {
        _currencyText = GetComponent<TMP_Text>();
        InventoryManager.Instance.OnBuyItem += UpdateCurrencyText;
    }

    private void OnEnable()
    {
        UpdateCurrencyText();
    }

    private void UpdateCurrencyText()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        r =>
        {
            int coins = r.VirtualCurrency["CN"];
            _currencyText.text = "$ " + coins.ToString();
        }, OnError);
    }

    private void OnError(PlayFabError e)
    {
        Debug.Log(e.GenerateErrorReport());
    }
}
