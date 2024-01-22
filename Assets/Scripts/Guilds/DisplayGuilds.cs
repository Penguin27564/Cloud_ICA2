using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Linq;

public class DisplayGuilds : MonoBehaviour
{
    [SerializeField]
    private GameObject _noUsersText;

    [SerializeField]
    private GuildJoinElement _guildElement;

    private List<GameObject> _elementsToAdd = new();
    private RectTransform _rectTransform;

    public void AddItem(string name)
    {
        GuildJoinElement newElement = Instantiate(_guildElement);
        newElement.guildName = name;
        newElement.transform.SetParent(transform);
        newElement.transform.localScale = Vector3.one;

        _elementsToAdd.Add(newElement.gameObject);
        
        newElement.gameObject.SetActive(false);
    }

    public void DisplayGuildList()
    {
        _noUsersText.SetActive(!(transform.childCount > 0));
        foreach (var element in _elementsToAdd)
        {
            element.SetActive(true);
        }

        Vector2 contentSize = new(220 * transform.childCount, _rectTransform.sizeDelta.y);
        _rectTransform.sizeDelta = contentSize;
    }

    public void ClearDisplay()
    {
        _elementsToAdd.Clear();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnEnable()
    {
        GuildManager.Instance.currentGuildMembers.Clear();
        ClearDisplay();
        GetGuilds();
    }

    private void GetGuilds()
    {
        var csrequest = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetGuildInfo"
        };

        PlayFabClientAPI.ExecuteCloudScript(csrequest,
        result =>
        {
            Dictionary<string, Dictionary<string, string>> dic = 
                PlayFabSimpleJson.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(result.FunctionResult.ToString());

            if (dic.TryGetValue("Result", out Dictionary<string, string> value))
            {
                foreach (var guild in value)
                {
                    AddItem(guild.Value);
                }
                
                DisplayGuildList();
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
