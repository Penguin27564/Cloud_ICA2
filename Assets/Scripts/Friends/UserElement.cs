using Microsoft.Unity.VisualStudio.Editor;
using PlayFab.GroupsModels;
using TMPro;
using UnityEngine;

public class UserElement : MonoBehaviour
{
    private TMP_Text _nameText;

    private RectTransform _rectTransform;

    [HideInInspector]
    public string playFabID, displayName;
    
    [HideInInspector]
    public EntityKey entityKey;

    public GameObject ownerImage, admiralImage, memberImage;

    public void SetName(string name, string PlayFabID = "", EntityKey userKey = null)
    {
        _nameText.text = name;
        playFabID = PlayFabID;
        displayName = name;
        entityKey = userKey;
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

    public void RejectFromGuild()
    {
        GetComponentInParent<DisplayJoinRequests>().RejectApplication(displayName);
    }

    public void RemoveMember()
    {
        if (ownerImage.activeInHierarchy)
        {
            MessageBoxManager.Instance.DisplayMessage("Unable to kick owner");
            return;
        }
        if (admiralImage.activeInHierarchy)
        {
            MessageBoxManager.Instance.DisplayMessage("Unable to kick admirals");
            return;
        }
        GuildManager.Instance.KickMember(entityKey);
    }

    public void Promote()
    {
        if (memberImage.activeInHierarchy && PFDataMgr.Instance.currentPlayerGuildRoleId == "admins")
        {
            GuildManager.Instance.ChangeMemberRole(entityKey, "admirals", "members", this);
        }
        else
        {
            MessageBoxManager.Instance.DisplayMessage("Unable to promote");
        }
    }

    public void Demote()
    {
        if (admiralImage.activeInHierarchy && PFDataMgr.Instance.currentPlayerGuildRoleId == "admins")
        {
            GuildManager.Instance.ChangeMemberRole(entityKey, "members", "admirals", this);
        }
        else
        {
            MessageBoxManager.Instance.DisplayMessage("Unable to demote");
        }
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
