using TMPro;
using UnityEngine;

public class UserElement : MonoBehaviour
{
    private TMP_Text _nameText;

    private RectTransform _rectTransform;

    public void SetName(string name)
    {
        _nameText.text = name;
    }

    public void AddFriend()
    {
        Debug.Log(_nameText.text);
        FriendManager.Instance.AddFriendByDisplayName(_nameText.text);
    }

    private void Awake()
    {
        _nameText = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchoredPosition3D = Vector3.zero;
    }
}
