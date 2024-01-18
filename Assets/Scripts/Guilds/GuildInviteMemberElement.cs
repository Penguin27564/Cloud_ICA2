using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildInviteMemberElement : MonoBehaviour
{
    [SerializeField]
    private Color _baseColor, _invitedColor;

    public TMP_Text username;
    
    public bool inviteOnCreate = false;
    private Image _image;
    private CreateGuild _createGuildScript;

    public void Invite()
    {
        _image.color = _invitedColor;
        inviteOnCreate = true;
    }

    public void UnInvite()
    {
        _image.color = _baseColor;
        inviteOnCreate = false;
    }

    private void Start()
    {
        _image = GetComponent<Image>();
        _createGuildScript = GetComponentInParent<CreateGuild>();
        _createGuildScript.AddToUserList(this);
    }
}
