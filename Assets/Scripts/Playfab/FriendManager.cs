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
    private FriendListDisplay _friendDisplayScript;

    public FriendIdType selectedFriendIdType = FriendIdType.Displayname;

    private List<FriendInfo> _friendList = null;

    public enum FriendIdType
    {
        PlayFabID,
        Username,
        Email,
        Displayname
    }

    public void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            //ExternalPlatformFriends = false,
            //XboxToken = null,
        },
        result => 
        {
            _friendList = result.Friends;
            DisplayFriends(_friendList); // Trigger UI
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

    private void AddFriend(FriendIdType idType, string friendId)
    {
        var request = new AddFriendRequest();
        switch (idType)
        {
            case FriendIdType.PlayFabID:
                request.FriendPlayFabId = friendId;
                break;
            case FriendIdType.Username:
                request.FriendUsername = friendId;
                break;
            case FriendIdType.Email:
                request.FriendEmail = friendId;
                break;
            case FriendIdType.Displayname:
                request.FriendTitleDisplayName = friendId;
                break;
        }

        // Execute request and update friends when done
        PlayFabClientAPI.AddFriend(request,
        result =>
        {
            Debug.Log("Friend added successfully!");
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
        }, DisplayPlayFabError);
    }

    private void DisplayFriends(List<FriendInfo> friendsCache)
    {
        friendsCache.ForEach(f =>
        {
            Debug.Log("PlayfabID: " + f.FriendPlayFabId + " , display name: " + f.TitleDisplayName);
            _friendDisplayScript.AddItem(f.TitleDisplayName);
            if(f.Profile != null) Debug.Log(f.FriendPlayFabId + " / " + f.Profile.DisplayName);
        });
        _friendDisplayScript.DisplayFriendsList();
    }

    private void DisplayPlayFabError(PlayFabError error) 
    {
         Debug.LogError(error.GenerateErrorReport()); 
    }

    private void DisplayError(string error) 
    {
         Debug.LogError(error);
    }
}
