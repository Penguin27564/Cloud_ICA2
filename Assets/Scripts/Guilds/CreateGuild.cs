using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab.ClientModels;
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

        List<string> initialMemberList = new();

        foreach (var user in _userList)
        {
            if (user.inviteOnCreate)
            {
                initialMemberList.Add(user.GetComponent<UserElement>().playFabID);
            } 
        }
        
        GuildManager.Instance.CreateGroup(if_guildName.text, initialMemberList);
    }

    public void InviteMembers()
    {
        List<string> memberInviteList = new();

        foreach (var user in _userList)
        {
            if (user.inviteOnCreate)
            {
                memberInviteList.Add(user.GetComponent<UserElement>().playFabID);
            } 
        }

        var csrequest = new ExecuteCloudScriptRequest
                {
                    FunctionName = "AddMembersToGroup",
                    FunctionParameter = new
                    {
                        GroupId = GuildManager.Instance.currentGroupKey,
                        MemberIDs = memberInviteList
                    }
                };

                PlayFabClientAPI.ExecuteCloudScript(csrequest,
                result =>
                {   
                    MessageBoxManager.Instance.DisplayMessage("Added members!");
                    Debug.Log("Added members!");
                }, 
                error =>
                {
                    Debug.Log(error.GenerateErrorReport());
                    MessageBoxManager.Instance.DisplayMessage("Error adding members");
                });
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
