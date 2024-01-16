using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class DisplayUsers : MonoBehaviour
{
    [SerializeField]
    private UserElement _userElement;

    private List<GameObject> _elementsToAdd = new();
    private List<FriendInfo> _friendList = new();
    private bool _addUser = false;

    public void AddItem(string name)
    {
        UserElement newElement = Instantiate(_userElement);
        newElement.SetName(name);
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
    //---------------------------------------------
    public void GetUserList()
    {
        var leaderboardRequest = new GetLeaderboardRequest
        {
            StatisticName = "Highscore",
            StartPosition = 0
        };

        PlayFabClientAPI.GetLeaderboard(leaderboardRequest, OnUserListGet, OnError);
    }

    private void OnUserListGet(GetLeaderboardResult r)
    {
        ClearDisplay();

        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            //ExternalPlatformFriends = false,
            //XboxToken = null,
        },
        result => 
        {
            _addUser = true;
            _friendList = result.Friends;
            foreach (var element in r.Leaderboard)
            {
                foreach (var friend in _friendList)
                {
                    if (friend.FriendPlayFabId == element.PlayFabId)
                    {
                        Debug.Log("REMOVED USER");
                        _addUser = false;
                    }
                }
                if (_addUser) AddItem(element.DisplayName);
            }
            
            DisplayUserList();
        }, DisplayPlayFabError);
    }

    private void OnError(PlayFabError e)
    {
        Debug.LogError(e);
    }

    private void DisplayPlayFabError(PlayFabError error) 
    {
         Debug.LogError(error.GenerateErrorReport()); 
    }
}
