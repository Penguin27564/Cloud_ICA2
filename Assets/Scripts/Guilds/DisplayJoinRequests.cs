using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.GroupsModels;
using PlayFab.ClientModels;
using PlayFab.Json;
using TMPro;
using EntityKey = PlayFab.GroupsModels.EntityKey;
using PlayFab.ProfilesModels;
using UnityEngine.Rendering;
using System.Text;
using UnityEditor.Performance.ProfileAnalyzer;

public class DisplayJoinRequests : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _currentGuildName;

    [SerializeField]
    private UserElement _userElement;

    private RectTransform _rectTransform;
    private List<GameObject> _elementsToAdd = new();
    private void OnEnable()
    {
        ClearDisplay();
        PlayFabGroupsAPI.GetGroup(new GetGroupRequest()
        {
            GroupName = _currentGuildName.text
        },
        result =>
        {
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

            GetApplicantsDisplayNames(playFabIDs);
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void GetApplicantsDisplayNames(List<string> memberIDs)
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
                foreach (var name in value)
                {
                    AddApplicant(name);
                }

                DisplayApplicants();
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void AddApplicant(string name)
    {
        UserElement newElement = Instantiate(_userElement);
        newElement.SetName(name, "");
        newElement.transform.SetParent(transform);
        newElement.transform.localScale = Vector3.one;
        _elementsToAdd.Add(newElement.gameObject);
        newElement.gameObject.SetActive(false);
    }

    private void DisplayApplicants()
    {
        foreach (var element in _elementsToAdd)
        {
            element.SetActive(true);
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
        foreach (Transform child in transform)
        {
           Destroy(child.gameObject);
        }
    }
}
