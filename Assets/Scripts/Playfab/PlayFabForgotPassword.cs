using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class PlayFabForgotPassword : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField if_email;
    public void ForgotPassword()
    {
        var recoveryRequest = new SendAccountRecoveryEmailRequest
        {
            Email = if_email.text,
            TitleId = "55DD3"
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(recoveryRequest, ResultCallback, ErrorCallback);
    }

    private void ResultCallback(SendAccountRecoveryEmailResult result)
    {
        Debug.Log("Successfully sent recovery email!");
        MessageBoxManager.Instance.DisplayMessage("Successfully sent recovery email");
    }

    private void ErrorCallback(PlayFabError error)
    {
        Debug.LogError("Error sending recovery email \n" + error);
        MessageBoxManager.Instance.DisplayMessage("Error sending recovery email");
    }
}
