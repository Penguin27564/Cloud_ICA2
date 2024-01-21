using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.GroupsModels;
using TMPro;
using UnityEngine;


public class JoinGuild : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField if_guildName;

    public void ApplyToGuild()
    {
        PlayFabGroupsAPI.GetGroup(new GetGroupRequest()
        {
            GroupName = if_guildName.text
        },
        result =>
        {
            GuildManager.Instance.ApplyToGroup(result.Group.Id);
            MessageBoxManager.Instance.DisplayMessage("Applied to " + if_guildName.text + "!");
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
            MessageBoxManager.Instance.DisplayMessage("Invalid group name");
        });
    }
}
