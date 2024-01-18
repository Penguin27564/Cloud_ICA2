using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.GroupsModels;
using PlayFab.Json;
using EntityKey = PlayFab.GroupsModels.EntityKey;

public class DisplayGuildMembers : MonoBehaviour
{
    [SerializeField]
    private UserElement _memberElement;

    private bool _isAdmin = false;
    private List<GameObject> _elementsToAdd = new();
    private RectTransform _rectTransform;

    public void AddItem(string name)
    {
        UserElement newElement = Instantiate(_memberElement);
        //newElement.SetName(name);
        newElement.transform.SetParent(transform);
        newElement.transform.localScale = Vector3.one;
        _elementsToAdd.Add(newElement.gameObject);
        newElement.gameObject.SetActive(false);
    }

    public void DisplayMembers()
    {
        foreach (var element in _elementsToAdd)
        {
            element.SetActive(true);
        }

        Vector2 contentSize = new(220 * transform.childCount, _rectTransform.sizeDelta.y);
        _rectTransform.sizeDelta = contentSize;
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
        _elementsToAdd.Clear();
        ClearDisplay();
        GetGroup();
    }

    private void GetGroup()
    {
        Debug.Log("Getting group");
        PlayFabGroupsAPI.ListMembership(new ListMembershipRequest
        {
        },
        result =>
        {
            GetMembers(result.Groups[0].Group);
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void GetMembers(EntityKey groupID)
    {
        Debug.Log("Getting members");
        PlayFabGroupsAPI.ListGroupMembers(new ListGroupMembersRequest
        {
            Group = groupID
        },
        result =>
        {
            List<string> memberIDs = new();
            foreach (var role in result.Members)
            {
                foreach (var player in role.Members)
                {
                    memberIDs.Add(player.Lineage["master_player_account"].Id);
                }
            }

            GetMemberDisplayNames(memberIDs);
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void GetMemberDisplayNames(List<string> memberIDs)
    {
        Debug.Log("MemebrIDs: " + memberIDs.Count);
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
            //JsonObject jsonResult = (JsonObject)result.FunctionResult;
            Debug.Log(result.FunctionResult.ToString());
            //Debug.Log(jsonResult);

            //if (jsonResult.TryGetValue("Result", out object value))
            {
                // Turn back into list and add users here
                //Debug.Log(value.ToString());
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
    }
}
