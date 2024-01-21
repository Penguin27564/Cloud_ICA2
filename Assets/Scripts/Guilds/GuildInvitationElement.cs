using System.Collections;
using System.Collections.Generic;
using PlayFab.GroupsModels;
using TMPro;
using UnityEngine;

public class GuildInvitationElement : MonoBehaviour
{
    public TMP_Text _groupName;
    public EntityKey _groupKey, _userKey;

    public void Accept()
    {
        GuildManager.Instance.AcceptInvite(_groupKey);
    }
    
    public void Reject()
    {
        GuildManager.Instance.DeclineInvite(_groupKey, _userKey);
    }

    private void RemoveElement()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        _groupName = GetComponentInChildren<TMP_Text>();
        GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        GuildManager.Instance.OnInviteInteraction += RemoveElement;
    }
}
