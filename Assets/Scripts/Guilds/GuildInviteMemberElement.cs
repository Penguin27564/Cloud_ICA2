using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuildInviteMemberElement : MonoBehaviour
{
    [SerializeField]
    private Color _baseColor, _invitedColor;
    private Image _image;

    public void Invite()
    {
        _image.color = _invitedColor;
    }

    public void UnInvite()
    {
        _image.color = _baseColor;
    }

    private void Start()
    {
        _image = GetComponent<Image>();
    }
}
