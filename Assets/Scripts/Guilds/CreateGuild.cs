using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    private void Start()
    {
        if_guildName = GetComponentInChildren<TMP_InputField>();
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
