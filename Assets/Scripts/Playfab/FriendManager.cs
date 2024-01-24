using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System;

public class FriendManager : MonoBehaviour
{
    private static FriendManager _instance;

    public static FriendManager Instance
    {
        get
        {
            if (_instance == null) _instance = new FriendManager();
            return _instance;
        }
    }

    public FriendManager()
    {
        _instance = this;
    }

    public Action OnUpdateSelectedFriendID;

    [SerializeField]
    private TMP_InputField if_getFriend, if_unfriend;

    [SerializeField]
    private FriendListDisplay _friendDisplayGrid, _requestDisplayGrid;

    public FriendIdType selectedFriendIdType = FriendIdType.Displayname;

    private List<FriendInfo> _friendList = null;

    public enum FriendIdType
    {
        PlayFabID,
        Username,
        Email,
        Displayname
    }

    public void GetFriends(bool isPending)
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            //ExternalPlatformFriends = false,
            //XboxToken = null,
        },
        result => 
        {
            _friendList = result.Friends;
            if (!isPending) DisplayFriends(_friendList); // Trigger UI
            else DisplayRequests(_friendList);
        }, DisplayPlayFabError);
    }
    
    public void OnAddFriend()
    {
        // IdType should be linked to the type selector in UI
        AddFriend(selectedFriendIdType, if_getFriend.text);
    }

    public void OnRemoveFriend()
    {
        RemoveFriendByID(if_unfriend.text);
    }
    
    public void SetTypeToUsername()
    {
        selectedFriendIdType = FriendIdType.Username;
        OnUpdateSelectedFriendID.Invoke();
    }

    public void SetTypeToDisplayName()
    {
        selectedFriendIdType = FriendIdType.Displayname;
        OnUpdateSelectedFriendID.Invoke();
    }

    public void SetTypeToPlayFabID()
    {
        selectedFriendIdType = FriendIdType.PlayFabID;
        OnUpdateSelectedFriendID.Invoke();
    }

    public void SetTypeToEmail()
    {
        selectedFriendIdType = FriendIdType.Email;
        OnUpdateSelectedFriendID.Invoke();
    }

    public void RemoveFriendByDisplayName(string name)
    {
        foreach (var f in _friendList)
        {
            if (f.TitleDisplayName == name)
            {
                RemoveFriendByID(f.FriendPlayFabId);
                return;
            }
        }
    }

    public void AddFriend(FriendIdType idType, string friendId)
    {
        if (idType != FriendIdType.PlayFabID)
        {
            var request = new GetAccountInfoRequest();
            switch (idType)
            {
                case FriendIdType.Username:
                    request.Username = friendId;
                    break;
                case FriendIdType.Email:
                    request.Email = friendId;
                    break;
                case FriendIdType.Displayname:
                    request.TitleDisplayName = friendId;
                    break;
            }
            PlayFabClientAPI.GetAccountInfo(request,
            result =>
            {
                var csrequest = new ExecuteCloudScriptRequest
                {
                    FunctionName = "SendFriendRequest",
                    FunctionParameter = new
                    {
                        FriendPlayFabId = result.AccountInfo.PlayFabId
                    }
                };

                PlayFabClientAPI.ExecuteCloudScript(csrequest,
                result =>
                {
                    MessageBoxManager.Instance.DisplayMessage("Sent request to " + friendId);
                }, DisplayPlayFabError);
            }, DisplayPlayFabError);
        }
        else
        {
            var csrequest = new ExecuteCloudScriptRequest
            {
                FunctionName = "SendFriendRequest",
                FunctionParameter = new
                {
                    FriendPlayFabId = friendId
                }
            };

            PlayFabClientAPI.ExecuteCloudScript(csrequest,
            result =>
            {
                MessageBoxManager.Instance.DisplayMessage("Sent request to " + friendId);
            }, DisplayPlayFabError);
        }
    }

    public void AcceptFriendRequest(string name)
    {
        var request = new GetAccountInfoRequest
        {
            TitleDisplayName = name
        };
        PlayFabClientAPI.GetAccountInfo(request,
        result =>
        {
            var csrequest = new ExecuteCloudScriptRequest
            {
                FunctionName = "AcceptFriendRequest",
                FunctionParameter = new
                {
                    FriendPlayFabId = result.AccountInfo.PlayFabId
                }
            };

            PlayFabClientAPI.ExecuteCloudScript(csrequest,
            result =>
            {
                MessageBoxManager.Instance.DisplayMessage("Accepted " + name + " as friend!");
            }, DisplayPlayFabError);
        }, DisplayPlayFabError);
    }

    public void RejectFriendRequest(string name)
    {
        var request = new GetAccountInfoRequest
        {
            TitleDisplayName = name
        };
        PlayFabClientAPI.GetAccountInfo(request,
        result =>
        {
            var csrequest = new ExecuteCloudScriptRequest
            {
                FunctionName = "DenyFriendRequest",
                FunctionParameter = new
                {
                    FriendPlayFabId = result.AccountInfo.PlayFabId
                }
            };

            PlayFabClientAPI.ExecuteCloudScript(csrequest,
            result =>
            {
                MessageBoxManager.Instance.DisplayMessage("Rejected " + name + "'s request.");
            }, DisplayPlayFabError);
        }, DisplayPlayFabError);
    }

    private void RemoveFriendByInfo(FriendInfo friendInfo)
    {
        PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest
        {
            FriendPlayFabId = friendInfo.FriendPlayFabId
        },
        result =>
        {
            _friendList.Remove(friendInfo);
        }, DisplayPlayFabError);
    }

    private void RemoveFriendByID(string pfID)
    {
        var request = new RemoveFriendRequest
        {
            FriendPlayFabId = pfID
        };

        PlayFabClientAPI.RemoveFriend(request,
        result =>
        {
            Debug.Log("Unfriended!");
            MessageBoxManager.Instance.DisplayMessage("Unfriended!");
        }, DisplayPlayFabError);
    }

    private void DisplayFriends(List<FriendInfo> friendsCache)
    {
        _friendDisplayGrid.ClearDisplay();
        friendsCache.ForEach(f =>
        {
            if (f.Tags[0] != "requestee" && f.Tags[0] != "requester")
            {
                //Debug.Log("PlayfabID: " + f.FriendPlayFabId + " , display name: " + f.TitleDisplayName);
                Debug.Log(f.Tags);
                _friendDisplayGrid.AddItem(f.TitleDisplayName, f.FriendPlayFabId);
            }
        });
        _friendDisplayGrid.DisplayFriendsList();
    }
    
    private void DisplayRequests(List<FriendInfo> friendsCache)
    {
        _requestDisplayGrid.ClearDisplay();
        friendsCache.ForEach(f =>
        {
            if (f.Tags[0] == "requester")
            {
                _requestDisplayGrid.AddItem(f.TitleDisplayName, f.FriendPlayFabId);
            }
        });
        _requestDisplayGrid.DisplayFriendsList();
    }

    private void DisplayPlayFabError(PlayFabError error) 
    {
        Debug.LogError(error.GenerateErrorReport()); 
        MessageBoxManager.Instance.DisplayMessage(error.GenerateErrorReport());
    }
}
