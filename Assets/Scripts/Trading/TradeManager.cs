using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEditor.Experimental.GraphView;

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

    public void SendTradeRequest(List<string> itemOffersID, List<string> itemRequestsID)
    {
        PlayFabClientAPI.OpenTrade(new OpenTradeRequest
        {
            AllowedPlayerIds = new List<string>{ receivingID },
            OfferedInventoryInstanceIds = itemOffersID,
            RequestedCatalogItemIds = itemRequestsID 
        },
        result =>
        {
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
            }
        },
        result =>
        {
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
}
