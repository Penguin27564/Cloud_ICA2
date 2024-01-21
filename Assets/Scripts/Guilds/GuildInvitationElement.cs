using System.Collections;
using System.Collections.Generic;
using PlayFab.GroupsModels;
using TMPro;
using UnityEngine;

public class GuildInvitationElement : MonoBehaviour
{
    public TMP_Text _groupName;
    public EntityKey _groupKey;

    public void Accept()
    {

    }
    
    public void Reject()
    {

    }

    private void Start()
    {
        _groupName = GetComponentInChildren<TMP_Text>();
        GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
    }
}
