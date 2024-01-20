using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class DisplayUsers : MonoBehaviour
{
    [SerializeField]
    private UserElement _userElement;

    [SerializeField]
    private bool _horizontalDisplay = false;

    [SerializeField]
    private bool _ignoreFriends = false;

    private List<GameObject> _elementsToAdd = new();
    private List<FriendInfo> _friendList = new();
    private bool _addUser = false;
    private RectTransform _rectTransform;

    public void AddItem(string name, string PlayfabID)
    {
        UserElement newElement = Instantiate(_userElement);
        newElement.SetName(name, PlayfabID);
        newElement.transform.SetParent(transform);
        newElement.transform.localScale = Vector3.one;
        _elementsToAdd.Add(newElement.gameObject);
        newElement.gameObject.SetActive(false);
    }

    public void DisplayUserList()
    {
        foreach (var element in _elementsToAdd)
        {
            element.SetActive(true);
        }

        Vector2 contentSize;
        if (_horizontalDisplay) 
        {
            contentSize = new(220 * transform.childCount, _rectTransform.sizeDelta.y);
        }
        else
        {
            contentSize = new(_rectTransform.sizeDelta.x, 110 * transform.childCount);
        }
        _rectTransform.sizeDelta = contentSize;
    }

    public void ClearDisplay()
    {
        foreach (Transform child in transform)
        {
           Destroy(child.gameObject);
        }
    }

    private void OnEnable()
    {
        _elementsToAdd.Clear();
        ClearDisplay();
        GetUserList();
    }
    public void GetUserList()
    {
        var leaderboardRequest = new GetLeaderboardRequest
        {
            StatisticName = "Highscore",
            StartPosition = 0,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(leaderboardRequest, OnUserListGet, OnError);
    }

    private void OnUserListGet(GetLeaderboardResult r)
    {
        ClearDisplay();

        if (_ignoreFriends)
        {
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
            {
                //ExternalPlatformFriends = false,
                //XboxToken = null,
            },
            result => 
            {
                _friendList = result.Friends;
                foreach (var element in r.Leaderboard)
                {
                    _addUser = true;
                    foreach (var friend in _friendList)
                    {
                        if (friend.FriendPlayFabId == element.PlayFabId)
                        {
                            _addUser = false;
                        }
                    }
                    if (_addUser && element.PlayFabId != PFDataMgr.Instance.currentPlayerPlayFabID)
                    {
                        AddItem(element.DisplayName, element.PlayFabId);
                    }
                }
                
                DisplayUserList();
            }, DisplayPlayFabError);
        }
        else
        {
            foreach (var element in r.Leaderboard)
            {
                if (element.PlayFabId != PFDataMgr.Instance.currentPlayerPlayFabID)
                {
                    AddItem(element.DisplayName, element.PlayFabId);
                }
            }
            DisplayUserList();
        }
    }

    private void OnError(PlayFabError e)
    {
        Debug.LogError(e);
    }

    private void DisplayPlayFabError(PlayFabError error) 
    {
         Debug.LogError(error.GenerateErrorReport()); 
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}
