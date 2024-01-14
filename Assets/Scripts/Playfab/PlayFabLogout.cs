using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFabLogout : MonoBehaviour
{
    public void UserLogout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        SceneManager.LoadScene("LoginReg");
    }
}