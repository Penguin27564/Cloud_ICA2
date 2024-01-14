using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class PFUserMgt : MonoBehaviour
{
    [SerializeField]
    private string _nextScene;

    [SerializeField]
    private TMP_Text msgbox;

    [SerializeField]
    private TMP_InputField if_username, if_email, if_password, if_displayName;

    private void UpdateMsg(string msg)
    {
        Debug.Log(msg);
        MessageBoxManager.Instance.DisplayMessage(msg);
    }
    // For Button Press
    public void OnButtonRegUser()
    {
        // Create request object
        var regReq = new RegisterPlayFabUserRequest 
        {
            Email = if_email.text,
            Password = if_password.text,
            Username = if_username.text,
        };

        // Execute request by calling PlayFab api
        PlayFabClientAPI.RegisterPlayFabUser(regReq, OnRegSucc, OnError);
    }

    private void OnRegSucc(RegisterPlayFabUserResult r)
    {
        msgbox.text = "Register Success! " + r.PlayFabId;
        var req = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = if_displayName.text
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(req, OnDisplayNameUpdate, OnError);
    }

    public void OnButtonLogin()
    {
        if(if_email.text.Contains("@"))
        {
            OnButtonLoginEmail();
        }
        else
        {
            OnButtonLoginUsername();
        }
    }

    public void OnButtonGuestLogin()
    {
        var loginReq = new LoginWithPlayFabRequest
            {
                Username = "GuestAccount",
                Password = "GuestAccount",

                // To get player profile and display name
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }
            };

            PlayFabClientAPI.LoginWithPlayFab(loginReq, OnLoginSucc, OnError);
    }

    private void OnButtonLoginEmail()
    {
        var loginReq = new LoginWithEmailAddressRequest
        {
            Email = if_email.text,
            Password = if_password.text,

            // To get player profile and display name
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithEmailAddress(loginReq, OnLoginSucc, OnError);
    }

    private void OnButtonLoginUsername()
    {
        var loginReq = new LoginWithPlayFabRequest
        {
            Username = if_email.text,
            Password = if_password.text,

            // To get player profile and display name
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithPlayFab(loginReq, OnLoginSucc, OnError);
    }

    private void OnError(PlayFabError e)
    {
        UpdateMsg(e.GenerateErrorReport());
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult r)
    {
        UpdateMsg("Display name updated!" + r.DisplayName);
    }

    private void OnLoginSucc(LoginResult r)
    {
        msgbox.text = "Login Success! " + r.PlayFabId;
        SceneManager.LoadScene(_nextScene);
    }
}
