using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FriendElement : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _nameText;

    private RectTransform _rectTransform;
    
    public void AddElement(string name)
    {
        _nameText.text = name;
    }

    public void RemoveFriend()
    {
        FriendManager.Instance.RemoveFriendByDisplayName(_nameText.text);
    }

    public void AcceptFriendRequest()
    {
        FriendManager.Instance.AcceptFriendRequest();
    }

    public void RejectFriendRequest()
    {
        FriendManager.Instance.RejectFriendRequest();
    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchoredPosition3D = Vector3.zero;
    }
}
