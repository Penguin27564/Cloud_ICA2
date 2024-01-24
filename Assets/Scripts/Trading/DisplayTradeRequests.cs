using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;

public class DisplayTradeRequests : MonoBehaviour
{
    [SerializeField]
    private GameObject _noUsersText;

    [SerializeField]
    private TradeRequestElement _tradeElement;

    private List<GameObject> _elementsToAdd = new();
    private RectTransform _rectTransform;

    public void AddItem(string name, string tradeID, string requesterID)
    {
        TradeRequestElement newElement = Instantiate(_tradeElement);

        newElement.SetName(name);
        newElement.tradeID = tradeID;
        newElement.requesterID = requesterID;

        newElement.transform.SetParent(transform);
        newElement.transform.localScale = Vector3.one;

        _elementsToAdd.Add(newElement.gameObject);
        
        newElement.gameObject.SetActive(false);
    }

    public void DisplayTrades()
    {
        _noUsersText.SetActive(!(transform.childCount > 0));
        foreach (var element in _elementsToAdd)
        {
            element.SetActive(true);
        }

        Vector2 contentSize = new(290 * transform.childCount, _rectTransform.sizeDelta.y);
        _rectTransform.sizeDelta = contentSize;
    }

    public void ClearDisplay()
    {
        _elementsToAdd.Clear();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnEnable()
    {
        GuildManager.Instance.currentGuildMembers.Clear();
        ClearDisplay();
        GetTrades();
    }

    private void GetTrades()
    {
        Debug.Log("Getting trades");
        PlayFabClientAPI.GetUserData(new GetUserDataRequest
        {
            // Get current player's info to find all trade reqs
            PlayFabId = PFDataMgr.Instance.currentPlayerPlayFabID
        },
        result =>
        {
            if (result.Data.ContainsKey("TradeRequest"))
            {
                Dictionary<string, string> dic = 
                PlayFabSimpleJson.DeserializeObject<Dictionary<string, string>>(result.Data["TradeRequest"].Value.ToString());

                // foreach(var item in dic)
                // {
                //     PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
                //     {
                //         PlayFabId = item.Value
                //     },
                //     result =>
                //     {
                //         AddItem(result.AccountInfo.TitleInfo.DisplayName, item.Key, item.Value);

                //         if (item )
                //     },
                //     error =>
                //     {
                //         Debug.LogError(error.GenerateErrorReport());
                //     });
                // }

                foreach (var (value, i) in dic.Select((value, i) => ( value, i )))
                {
                    PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
                    {
                        PlayFabId = value.Value
                    },
                    result =>
                    {
                        AddItem(result.AccountInfo.TitleInfo.DisplayName, value.Key, value.Value);

                        if (i == dic.Count - 1)
                        {
                            Debug.Log("DIsplay DIC");
                            DisplayTrades();
                        }
                    },
                    error =>
                    {
                        Debug.LogError(error.GenerateErrorReport());
                    });
                    // Access `value` and `i` directly here.
                }
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        GuildManager.Instance.OnMemberRemoved += OnEnable;
    }
}