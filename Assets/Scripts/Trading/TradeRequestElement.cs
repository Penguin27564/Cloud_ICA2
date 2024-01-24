using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;

public class TradeRequestElement : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _userNameText;

    [SerializeField]
    private GameObject _redOffer, _blueOffer, _yellowOffer, _redRequest, _blueRequest, _yellowRequest;

    public string tradeID, requesterID;

    public void SetName(string _userName)
    {
        _userNameText.text = _userName;
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
            Debug.Log("Offered items: " + result.Trade.OfferedCatalogItemIds.ToCommaSeparatedString());
            Debug.Log("Requested items: " + result.Trade.RequestedCatalogItemIds.ToCommaSeparatedString());
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
