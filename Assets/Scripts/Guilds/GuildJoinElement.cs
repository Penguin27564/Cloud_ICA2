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
    private GameObject _membersPanel;

    public string guildName;

    private DisplayGuildMembers _displayGuildMemeberScript;

    public void Join()
    {
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

    public void ListGroupMembers()
    {
        _membersPanel.SetActive(true);
        _displayGuildMemeberScript.GetGroupByName(guildName);
    }

    private void OnEnable()
    {
        _guildNameText.text = guildName;
    }

    private void Start()
    {
        GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        _membersPanel = GameObject.Find("Join guilds Page").transform.Find("Guild member display").gameObject;
        _displayGuildMemeberScript = _membersPanel.GetComponentInChildren<DisplayGuildMembers>();
    }
}
