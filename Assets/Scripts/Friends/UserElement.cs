using TMPro;
using UnityEngine;

public class UserElement : MonoBehaviour
{
    private TMP_Text _nameText;

    private RectTransform _rectTransform;

    [HideInInspector]
    public string playFabID, displayName;

    public void SetName(string name, string PlayFabID = "")
    {
        _nameText.text = name;
        playFabID = PlayFabID;
        displayName = name;
    }

    public void AddFriend()
    {
        FriendManager.Instance.AddFriend(FriendManager.FriendIdType.Displayname, _nameText.text);
        Destroy(gameObject);
    }

    public void AcceptIntoGuild()
    {
        GetComponentInParent<DisplayJoinRequests>().AcceptApplication(displayName);
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
