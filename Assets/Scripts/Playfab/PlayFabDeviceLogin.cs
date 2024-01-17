using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class PlayFabDeviceLogin : MonoBehaviour
{
    [SerializeField]
    private string _nextScene;

    [SerializeField]
    private TMP_InputField if_deviceName;

    private bool _accountCreated;
    public void OnButtonDeviceLogin()
    {
        string customDeviceID = PlayerPrefs.GetString("DeviceID", SystemInfo.deviceUniqueIdentifier);
        PlayerPrefs.SetString("DeviceID", customDeviceID);

        
        string customDeviceName = PlayerPrefs.GetString("DeviceName", if_deviceName.text);
        PlayerPrefs.SetString("DeviceName", customDeviceName);

        var loginReq = new LoginWithCustomIDRequest
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            CustomId = customDeviceID
        };

        PlayFabClientAPI.LoginWithCustomID(loginReq, OnLoginSucc, OnError);

        PlayerPrefs.Save();
    }

    private void OnEnable()
    {
        if (PlayerPrefs.GetString("DeviceName") != "")
        {
            Debug.Log("Account already exists");
            _accountCreated = true;
            gameObject.SetActive(false);
            OnButtonDeviceLogin();
        }
    }

    private void OnLoginSucc(LoginResult r)
    {
        if (!_accountCreated)
        {
            var nameReq = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = if_deviceName.text
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(nameReq, OnDeviceNameUpdateSucc, OnError);

            // To enter user into the userlist
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "Highscore",
                        Value = 0
                    }
                }
            };
            PlayFabClientAPI.UpdatePlayerStatistics(request, 
            result => {}, OnError);
        }
        SceneManager.LoadScene(_nextScene);
    }

    private void OnDeviceNameUpdateSucc(UpdateUserTitleDisplayNameResult r)
    {
        Debug.Log("Updated Device Name: " + r.DisplayName);
    }

    private void OnError(PlayFabError e)
    {
        UpdateMsg(e.GenerateErrorReport());
    }

    private void UpdateMsg(string msg)
    {
        Debug.Log(msg);
        MessageBoxManager.Instance.DisplayMessage(msg);
    }
}
