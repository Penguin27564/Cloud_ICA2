using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.GroupsModels;
using PlayFab.Json;
using EntityKey = PlayFab.GroupsModels.EntityKey;
using TMPro;

public class DisplayGuildMembers : MonoBehaviour
{
    [SerializeField]
    private GameObject _noUsersText, _adminUI;

    [SerializeField]
    private TMP_Text _guildName;

    [SerializeField]
    private UserElement _memberElement, _adminMemberElement;

    [SerializeField]
    private bool _getGuildOnEnable = true;

    private bool _isAdmin = true;
    private List<GameObject> _elementsToAdd = new();
    private RectTransform _rectTransform;

    public void AddItem(string name, string playfabID, EntityKey entityKey)
    {
        UserElement newElement = Instantiate(_isAdmin ? _adminMemberElement : _memberElement);
        newElement.SetName(name, playfabID, entityKey);
        newElement.transform.SetParent(transform);
        newElement.transform.localScale = Vector3.one;

        if (_getGuildOnEnable) GuildManager.Instance.currentGuildMembers.Add(newElement);
        else _elementsToAdd.Add(newElement.gameObject);
        
        newElement.gameObject.SetActive(false);
    }

    public void DisplayMembers()
    {
        if (_getGuildOnEnable)
        {
            _noUsersText.SetActive(!(transform.childCount > 0));
            foreach (var element in GuildManager.Instance.currentGuildMembers)
            {
                element.gameObject.SetActive(true);
            }

            Vector2 contentSize = new(220 * transform.childCount, _rectTransform.sizeDelta.y);
            _rectTransform.sizeDelta = contentSize;
        }
        else
        {
            
                        Debug.Log("TESTWLWIEJWLIF: DISPLAYING INFO");
            foreach (var element in _elementsToAdd)
            {
                element.SetActive(true);
            }

            Vector2 contentSize = new(_rectTransform.sizeDelta.x, 110 * transform.childCount);
            _rectTransform.sizeDelta = contentSize;
        }
    }

    public void ClearDisplay()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnEnable()
    {
        ClearDisplay();
        if (_getGuildOnEnable)
        {
            GuildManager.Instance.currentGuildMembers.Clear();
            GetGroup();
        }
        else
        {
            _elementsToAdd.Clear();
        }
    }

    public void GetGroupByName(string groupName)
    {
        PlayFabGroupsAPI.GetGroup(new GetGroupRequest
        {
            GroupName = groupName
        },
        result =>
        {
            Debug.Log("TESTWLWIEJWLIF: " + result.GroupName);
            GetMembers(result.Group);
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void GetGroup()
    {
        PlayFabGroupsAPI.ListMembership(new ListMembershipRequest
        {
        },
        result =>
        {
            GuildManager.Instance.currentGroupKey = result.Groups[0].Group;
            _guildName.text = result.Groups[0].GroupName;
            _isAdmin = result.Groups[0].Roles[0].RoleName == "Administrators";
            _adminUI.SetActive(_isAdmin);
            GetMembers(result.Groups[0].Group);
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void GetMembers(EntityKey groupID)
    {
        PlayFabGroupsAPI.ListGroupMembers(new ListGroupMembersRequest
        {
            Group = groupID
        },
        result =>
        {
            List<string> memberIDs = new();
            List<EntityKey> memberKeys = new();
            foreach (var role in result.Members)
            {
                foreach (var player in role.Members)
                {
                    if (player.Lineage["master_player_account"].Id != PFDataMgr.Instance.currentPlayerPlayFabID ||
                        !_getGuildOnEnable)
                    {
                        Debug.Log("TESTWLWIEJWLIF: ADDING MEMBER");

                        memberIDs.Add(player.Lineage["master_player_account"].Id);
                        memberKeys.Add(player.Key);
                    }
                }
            }
            GetMemberDisplayNames(memberIDs, memberKeys);
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void GetMemberDisplayNames(List<string> memberIDs, List<EntityKey> memberKeys)
    {
        var csrequest = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetDisplayNamesFromID",
            FunctionParameter = new
            {
                UserIDs = memberIDs
            }
        };

        PlayFabClientAPI.ExecuteCloudScript(csrequest,
        result =>
        {
            Dictionary<string, List<string>> dic = 
                PlayFabSimpleJson.DeserializeObject<Dictionary<string, List<string>>>(result.FunctionResult.ToString());

            if (dic.TryGetValue("Result", out List<string> value))
            {
                for (int i = 0; i < value.Count; i++)
                {
                        Debug.Log("TESTWLWIEJWLIF: ADDING MEMBER NAME");

                    AddItem(value[i], memberIDs[i], memberKeys[i]);
                }
                DisplayMembers();
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        GuildManager.Instance.OnMemberRemoved += OnEnable;
    }
}
