using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using Unity.VisualScripting;

public class TradeRequestElement : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _userNameText;

    [SerializeField]
    private GameObject _redOffer, _blueOffer, _yellowOffer, _redRequest, _blueRequest, _yellowRequest;

    public string tradeID, requesterID;

    public List<string> accpetedInventoryInstanceIds = new();

    public void SetName(string _userName)
    {
        _userNameText.text = _userName;
    }

    public void AcceptTrade()
    {
        PlayFabClientAPI.AcceptTrade(new AcceptTradeRequest
        {
            OfferingPlayerId = requesterID,
            TradeId = tradeID,
            AcceptedInventoryInstanceIds = accpetedInventoryInstanceIds
        },
        result =>
        {
            MessageBoxManager.Instance.DisplayMessage("Trade accepted!");
            PFDataMgr.Instance.GetUserInventory();
            RemoveTradeFromData();
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
            MessageBoxManager.Instance.DisplayMessage("Error accepting trade");
        });
    }

    public void DeclineTrade()
    {
        RemoveTradeFromData();
    }

    private void RemoveTradeFromData()
    {
        Debug.Log("Getting trade to remove data");
        PlayFabClientAPI.GetUserData(new GetUserDataRequest
        {
            // Get current player's info to find all trade reqs
            PlayFabId = PFDataMgr.Instance.currentPlayerPlayFabID
        },
        result =>
        {
            if (result.Data.ContainsKey("TradeRequest"))
            {
                List<Dictionary<string, string>> dic = 
                PlayFabSimpleJson.DeserializeObject<List<Dictionary<string, string>>>(result.Data["TradeRequest"].Value.ToString());

                foreach (var tradeReq in dic)
                {
                    if (tradeReq.ContainsKey(tradeID))
                    {
                        Debug.Log("BEFORE DELETION: " + dic.ToCommaSeparatedString());
                        dic.Remove(tradeReq);

                        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
                        {
                            FunctionName = "UpdatePlayerData",
                            FunctionParameter = new
                            {
                                TradeInfo = dic,
                                PlayFabId = PFDataMgr.Instance.currentPlayerPlayFabID,
                                ReplaceData = "true"
                            }
                        },
                        result =>
                        {
                            MessageBoxManager.Instance.DisplayMessage("Updated user info:");
                            Debug.Log("Successfully saved trade into user data");

                            TradeManager.Instance.OnTradeRemoved.Invoke();
                        },
                        error =>
                        {
                            Debug.LogError(error.GenerateErrorReport());
                        });

                        return;
                    }
                }
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void GetTradeInfo()
    {
        PlayFabClientAPI.GetTradeStatus(new GetTradeStatusRequest
        {
            OfferingPlayerId = requesterID,
            TradeId = tradeID
        },
        result =>
        {
            foreach (var item in result.Trade.OfferedCatalogItemIds)
            {
                if (item == "Red")
                {
                    _redOffer.SetActive(true);
                }
                else if (item == "Blue")
                {
                    _blueOffer.SetActive(true);
                }
                else if (item == "Yellow")
                {
                    _yellowOffer.SetActive(true);
                }
            }

            foreach (var item in result.Trade.RequestedCatalogItemIds)
            {
                if (item == "Red")
                {
                    _redRequest.SetActive(true);
                }
                else if (item == "Blue")
                {
                    _blueRequest.SetActive(true);
                }
                else if (item == "Yellow")
                {
                    _yellowRequest.SetActive(true);
                }

                foreach (var inventoryItem in PFDataMgr.Instance.currentPlayerInventoryItems)
                {
                    // If the requested item id shows up in the player's inventory,
                    if (inventoryItem.ItemId == item)
                    {
                        Debug.Log("Added " + inventoryItem.ItemId);
                        accpetedInventoryInstanceIds.Add(inventoryItem.ItemInstanceId);
                    }
                }
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void OnEnable()
    {
        _redOffer.SetActive(false);
        _blueOffer.SetActive(false);
        _yellowOffer.SetActive(false);
        _redRequest.SetActive(false);
        _blueRequest.SetActive(false);
        _yellowRequest.SetActive(false);

        GetTradeInfo();
    }

    private void Start()
    {
        GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
    }
}
