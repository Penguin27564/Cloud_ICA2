using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab.GroupsModels;
using PlayFab;

public class GuildJoinElement : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _guildNameText;

    [HideInInspector]
    public string guildName;

    public void Join()
    {
        Debug.Log(GuildManager.Instance.currentGroupKey);
        if (GuildManager.Instance.currentGroupKey != null)
        {
            MessageBoxManager.Instance.DisplayMessage("Already in a guild");
            return;
        }
        PlayFabGroupsAPI.GetGroup(new GetGroupRequest()
        {
            GroupName = guildName
        },
        result =>
        {
            GuildManager.Instance.ApplyToGroup(result.Group.Id);
            MessageBoxManager.Instance.DisplayMessage("Applied to " + guildName + "!");
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
            MessageBoxManager.Instance.DisplayMessage("Invalid group name");
        });
    }

    private void OnEnable()
    {
        _guildNameText.text = guildName;
    }

    private void Start()
    {
        GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
    }
}
