using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.GroupsModels;

public class DisplayGuildInvites : MonoBehaviour
{
    [SerializeField]
    private GuildInvitationElement _invitationElement;

    [SerializeField]
    private GameObject _noUsersText;

    private RectTransform _rectTransform;
    private List<GameObject> _inviteList = new();

    private void AddElement(string name, EntityKey entityKey, EntityKey userKey)
    {
        GuildInvitationElement newElement = Instantiate(_invitationElement);

        newElement._groupName.text = name;
        newElement._groupKey = entityKey;
        newElement._userKey = userKey;

        newElement.transform.SetParent(transform);
        newElement.transform.localScale = Vector3.one;

        _inviteList.Add(newElement.gameObject);
        newElement.gameObject.SetActive(false);
    }

    private void DisplayInvites()
    {
        _noUsersText.SetActive(!(transform.childCount > 0));
        foreach (var element in _inviteList)
        {
            element.SetActive(true);
        }

        Vector2 contentSize = new(220 * transform.childCount * 0.5f, _rectTransform.sizeDelta.y);
        _rectTransform.sizeDelta = contentSize;
    }

    private void GetGroups(List<GroupInvitation> invites)
    {
        foreach (var invite in invites)
        {
            PlayFabGroupsAPI.GetGroup(new GetGroupRequest()
            {
                Group = invite.Group
            },
            result =>
            {
                AddElement(result.GroupName, result.Group, PFDataMgr.Instance.currentPlayEntityKey);
                if (invite == invites[^1])
                {
                    DisplayInvites();
                }
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
            });
        }
    }

    private void GetInvites()
    {
        PlayFabGroupsAPI.ListMembershipOpportunities(new ListMembershipOpportunitiesRequest()
        {
        },
        result =>
        {
            GetGroups(result.Invitations);
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void OnEnable()
    {
        ClearDisplay();
        GetInvites();
    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void ClearDisplay()
    {
        _inviteList.Clear();
        foreach (Transform child in transform)
        {
           Destroy(child.gameObject);
        }
    }
}
