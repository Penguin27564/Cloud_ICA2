using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEditor.VersionControl;
using Unity.VisualScripting;

public class TradeManager : MonoBehaviour
{
    private static TradeManager _instance;

    public static TradeManager Instance
    {
        get
        {
            if (_instance == null) _instance = new TradeManager();
            return _instance;
        }
    }

    public TradeManager()
    {
        _instance = this;
    }

    [SerializeField]
    private List<GameObject> _tradingUI;

    public Action OnStartTrading;
    public string receivingID;

    public Action OnTradeRemoved;

    public void SendTradeRequest(List<string> itemOffersID, List<string> itemRequestsID)
    {
        Debug.Log("Offer item before trade: " + itemOffersID.ToCommaSeparatedString());
        Debug.Log("Requested item before trade: " + itemRequestsID.ToCommaSeparatedString());

        PlayFabClientAPI.OpenTrade(new OpenTradeRequest
        {
            AllowedPlayerIds = new List<string>{ receivingID },
            OfferedInventoryInstanceIds = itemOffersID,
            RequestedCatalogItemIds = itemRequestsID
        },
        result =>
        {
            Debug.Log("Trade offers: " + itemOffersID.ToCommaSeparatedString());
            Debug.Log("Trade requests: " + itemRequestsID.ToCommaSeparatedString());
            // Need to store offering player (current player) id and this trade id in the receiving player's data
            // This is to allow the accepting and cancelling of the trade requesrt
            SaveTradeToData(receivingID, result.Trade.TradeId, result.Trade.OfferingPlayerId);
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void SaveTradeToData(string playerID, string tradeID, string requesterID)
    {
        Dictionary<string, string> tradeInfo = new()
        {
            { tradeID, requesterID }
        };

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
        {
            FunctionName = "UpdatePlayerData",
            FunctionParameter = new
            {
                TradeInfo = tradeInfo,
                PlayFabId = playerID,
                ReplaceData = "false"
            }
        },
        result =>
        {
            MessageBoxManager.Instance.DisplayMessage("Sent trade!");
            DisableTradeUI();
            Debug.Log("Successfully saved trade into user data");
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    private void Awake()
    {
        OnStartTrading += EnableTradeUI;
    }

    private void EnableTradeUI()
    {
        foreach (var ui in _tradingUI)
        {
            ui.SetActive(true);
        }
    }

    private void DisableTradeUI()
    {
        foreach (var ui in _tradingUI)
        {
            ui.SetActive(false);
        }
    }
}
