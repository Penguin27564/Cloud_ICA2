using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System;

public class PFDataMgr : MonoBehaviour
{
    public static PFDataMgr Instance;

    public int playerMaxHealth;
    
    public float playerSpeed, playerFireRate;


    [SerializeField]
    private TMP_Text _healthText, _speedText, _fireRateText;

    // public void SetUserData()
    // {
    //     PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
    //     {
    //         Data = new Dictionary<string, string>()
    //         {
    //             {"XP", _XPInput.text.ToString()}
    //         }
    //     },
    //     result => Debug.Log("Successfully updated user data"),
    //     error => 
    //     {
    //         Debug.Log("Got error setting user data XP");
    //         Debug.Log(error.GenerateErrorReport());
    //     }
    //     );
    // }

    public void AddStat(string statName)
    {
        if (statName == "Health") SetUserHealth(playerMaxHealth + 1);
        else if (statName == "Speed") SetUserSpeed(playerSpeed + 0.2f);
        else if (statName == "FireRate") SetUserFireRate(playerFireRate + 0.2f);
        else Debug.LogError("Stat unable to be found");
    }

    private void SetUserHealth(int amount)
    {
        if (amount <= 1) amount = 1;
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"Health", (amount).ToString()}
            }
        },
        result => 
        {
            playerMaxHealth = amount;
             UpdateUI();
        },
        error => 
        {
            Debug.Log(error.GenerateErrorReport());
        }
        );
    }

    private void SetUserSpeed(float amount)
    {
        if (amount <= 1) amount = 1;
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"Speed", (amount).ToString()}
            }
        },
        result => 
        {
            Debug.Log("Successfully updated user speed");
            playerSpeed = amount;
            UpdateUI();
        },
        error => 
        {
            Debug.Log("Got error setting user data speed");
            Debug.Log(error.GenerateErrorReport());
        }
        );
    }

    private void SetUserFireRate(float amount)
    {
        if (amount <= 1) amount = 1;
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"FireRate", (amount).ToString()}
            }
        },
        result =>
        {
             Debug.Log("Successfully updated user fire rate");
             playerFireRate = amount;
             UpdateUI();
        },
        error => 
        {
            Debug.Log("Got error setting user data fire rate");
            Debug.Log(error.GenerateErrorReport());
        }
        );
    }

    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() 
        {
        },
        result => 
        {
            Debug.Log("Got user data");

            if (result.Data == null) return;

            // If player has Health stat
            if (result.Data.ContainsKey("Health"))
            {
                // Update stored health stat
                playerMaxHealth = int.Parse(result.Data["Health"].Value);
            }
            else
            {
                // Set health to default value
                Debug.LogError("No health found");
                SetUserHealth(1);
            }

            if (result.Data.ContainsKey("Speed"))
            {
                playerSpeed = float.Parse(result.Data["Speed"].Value);
                SetUserSpeed(playerSpeed);
            }
            else
            {
                Debug.LogError("No speed found");
                SetUserSpeed(1);
            }

            if (result.Data.ContainsKey("FireRate"))
            {
                playerFireRate = float.Parse(result.Data["FireRate"].Value);
                SetUserFireRate(playerFireRate);
            }
            else
            {
                Debug.LogError("No fire rate found");
                SetUserFireRate(1);
            }

            UpdateUI();
        },
        error =>  
        {
            Debug.Log("Got error retrieving user data: ");
            Debug.Log(error.GenerateErrorReport());
        }
        );
    }

    private void Awake()
    {
        if (!Instance) Instance = this;
        GetUserData();
        Debug.Log("PLAYER STATS: GETTING PLAYER STATS");
    }

    private void UpdateUI()
    {
        if (_healthText) _healthText.text = "Health: " + playerMaxHealth.ToString();
        if (_speedText) _speedText.text = "Speed: " + playerSpeed.ToString();
        if (_fireRateText) _fireRateText.text = "Fire Rate: " + playerFireRate.ToString();
    }
}
