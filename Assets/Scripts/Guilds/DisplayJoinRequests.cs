using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.GroupsModels;
using PlayFab.ClientModels;
using PlayFab.ProfilesModels;
using PlayFab.Json;
using TMPro;
using EntityKey = PlayFab.GroupsModels.EntityKey;
using UnityEditor.VersionControl;

public class DisplayJoinRequests : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _currentGuildName;

    [SerializeField]
    private UserElement _userElement;

    private RectTransform _rectTransform;
    private Dictionary<UserElement, EntityKey> _applicantList = new();

    public void AcceptApplication(string applicantDisplayName)
    {
        foreach (var applicant in _applicantList)
        {
            if (applicant.Key.displayName == applicantDisplayName)
            {
                GuildManager.Instance.AcceptApplication(applicant.Value);
                OnEnable();
                return;
            }
        }
        MessageBoxManager.Instance.DisplayMessage("Error when accepting application");
    }

    public void RejectApplication(string applicantDisplayName)
    {
        foreach (var applicant in _applicantList)
        {
            if (applicant.Key.displayName == applicantDisplayName)
            {
                GuildManager.Instance.RejectApplication(applicant.Value);
                OnEnable();
                return;
            }
        }
        MessageBoxManager.Instance.DisplayMessage("Error when rejecting application");
    }

    private void OnEnable()
    {
        ClearDisplay();
        PlayFabGroupsAPI.GetGroup(new GetGroupRequest()
        {
            GroupName = _currentGuildName.text
        },
        result =>
        {
            GuildManager.Instance.currentGroupKey = result.Group;
            GetApplications(result.Group);
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    private void GetApplications(EntityKey groupKey)
    {
        PlayFabGroupsAPI.ListGroupApplications(new ListGroupApplicationsRequest()
        {
            Group = groupKey
        },
        result =>
        {
            List<EntityKey> applicantIDs = new();
            foreach (var application in result.Applications)
            {
                applicantIDs.Add(application.Entity.Key);
            }

            if (applicantIDs.Count > 0) GetApplicantsPlayFabIDs(applicantIDs);
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    private void GetApplicantsPlayFabIDs(List<EntityKey> entityKeys)
    {
        var jsonConverter = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);

        var convertedEntityKeys = jsonConverter.DeserializeObject<List<PlayFab.ProfilesModels.EntityKey>>
                                (jsonConverter.SerializeObject(entityKeys));

        PlayFabProfilesAPI.GetProfiles(new GetEntityProfilesRequest()
        {
            Entities = convertedEntityKeys
        },
        result =>
        {
            List<string> playFabIDs = new();
            foreach (var profile in result.Profiles)
            {
                Debug.Log(profile.Lineage.MasterPlayerAccountId);
                playFabIDs.Add(profile.Lineage.MasterPlayerAccountId);
            }

            GetApplicantsDisplayNames(playFabIDs, entityKeys);
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void GetApplicantsDisplayNames(List<string> memberIDs, List<EntityKey> memberKeys)
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
                for(int i = 0; i < value.Count; i++)
                {
                    AddApplicant(value[i], memberIDs[i], memberKeys[i]);
                }

                DisplayApplicants();
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void AddApplicant(string name, string playfabID, EntityKey entityKey)
    {
        UserElement newElement = Instantiate(_userElement);
        newElement.SetName(name, playfabID);
        newElement.transform.SetParent(transform);
        newElement.transform.localScale = Vector3.one;
        _applicantList.Add(newElement, entityKey);
        newElement.gameObject.SetActive(false);
    }

    private void DisplayApplicants()
    {
        foreach (var element in _applicantList)
        {
            element.Key.gameObject.SetActive(true);
        }

        Vector2 contentSize = new(_rectTransform.sizeDelta.x, 110 * transform.childCount);
        _rectTransform.sizeDelta = contentSize;
    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void ClearDisplay()
    {
        _applicantList.Clear();
        foreach (Transform child in transform)
        {
           Destroy(child.gameObject);
        }
    }
}
