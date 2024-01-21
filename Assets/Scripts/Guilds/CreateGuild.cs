using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab.GroupsModels;
using PlayFab;

public class CreateGuild : MonoBehaviour
{
    private TMP_InputField if_guildName;

    private List<GuildInviteMemberElement> _userList = new();

    public void AddToUserList(GuildInviteMemberElement userElement)
    {
        _userList.Add(userElement);
    }

    public void Create()
    {
        if(!VerifyGuildName()) return;

        List<PlayFab.GroupsModels.EntityKey> initialMemberList = new();

        var jsonConverter = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);

        foreach (var user in _userList)
        {
            if (user.inviteOnCreate)
            {
                PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
                {
                    PlayFabId = user.GetComponent<UserElement>().playFabID
                },
                result =>
                {
                    initialMemberList.Add(jsonConverter.DeserializeObject<PlayFab.GroupsModels.EntityKey>
                                        (jsonConverter.SerializeObject(result.AccountInfo.TitleInfo.TitlePlayerAccount)));
                },
                error =>
                {
                    Debug.LogError(error.GenerateErrorReport());
                });
            } 
        }
        
        GuildManager.Instance.CreateGroup(if_guildName.text, initialMemberList);
    }

    public void InviteMembers()
    {
        var jsonConverter = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);

        foreach (var user in _userList)
        {
            if (user.inviteOnCreate)
            {
                PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
                {
                    PlayFabId = user.GetComponent<UserElement>().playFabID
                },
                result =>
                {
                    GuildManager.Instance.InviteMember(jsonConverter.DeserializeObject<PlayFab.GroupsModels.EntityKey>
                                        (jsonConverter.SerializeObject(result.AccountInfo.TitleInfo.TitlePlayerAccount)));
                },
                error =>
                {
                    Debug.LogError(error.GenerateErrorReport());
                });
                
            } 
        }
    }

    private void Start()
    {
        if_guildName = GetComponentInChildren<TMP_InputField>();
    }

    private void OnEnable()
    {
        _userList.Clear();
    }

    public bool VerifyGuildName()
    {
        if (if_guildName.text.Length == 0)
        {
            MessageBoxManager.Instance.DisplayMessage("Please input a guild name");
            return false;
        }

        if(if_guildName.text[0] == ' ')
        {
            MessageBoxManager.Instance.DisplayMessage("First character cannot be blank");
            return false;
        }

        return true;
    }
}
