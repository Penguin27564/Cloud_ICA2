using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class PlayFabLeaderboard : MonoBehaviour
{
    private static PlayFabLeaderboard _instance;

    public static PlayFabLeaderboard Instance
    {
        get
        {
            if (_instance == null) _instance = new PlayFabLeaderboard();
            return _instance;
        }
    }

    public PlayFabLeaderboard()
    {
        _instance = this;
    }

    public Action OnUpdateLeaderboardType;

    public int leaderboardType = 0; //0 - Global, 1 - Proximity, 2 - Friend (might change to enum)

    [SerializeField]
    private UI_Leaderboard _leaderboardScript;

    [SerializeField]
    private TMP_InputField _currentScore;

    [SerializeField]
    private TMP_Text msgBox;
    
    [SerializeField]
    private int _maxResultsCount = 20;

    public void OnButtonSendLeaderboard()
    {
        var req = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Highscore",
                    Value = int.Parse(_currentScore.text)
                }
            }
        };

        UpdateMsg("Submitting Score: " + _currentScore.text);
        PlayFabClientAPI.UpdatePlayerStatistics(req, OnLeaderboardUpdate, OnError);
    }

    public void SetToGlobal()
    {
        leaderboardType = 0;
        OnButtonGetLeaderboard();
        OnUpdateLeaderboardType.Invoke();
    }

    public void SetToProximity()
    {
        leaderboardType = 1;
        OnButtonGetProximityLeaderboard();
        OnUpdateLeaderboardType.Invoke();
    }

    public void SetToFriends()
    {
        leaderboardType = 2;
        OnButtonGetFriendLB();
        OnUpdateLeaderboardType.Invoke();
    }

    public void OnButtonGetLeaderboard()
    {
        var leaderboardRequest = new GetLeaderboardRequest
        {
            StatisticName = "Highscore",
            StartPosition = 0,
            MaxResultsCount = _maxResultsCount
        };

        PlayFabClientAPI.GetLeaderboard(leaderboardRequest, OnLeaderboardGet, OnError);
    }
    
    private void OnLeaderboardGet(GetLeaderboardResult r)
    {
        _leaderboardScript.ClearLeaderboard();
        foreach (var item in r.Leaderboard)
        {
            _leaderboardScript.AddItem(item.Position, item.DisplayName, item.StatValue);
        }
        StartCoroutine(_leaderboardScript.DisplayLeaderboard());
    }

    public void OnButtonGetProximityLeaderboard()
    {
        var proximityLeaderboardRequest = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "Highscore",
            MaxResultsCount = _maxResultsCount
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(proximityLeaderboardRequest, OnProximityLeaderboardGet, OnError);
    }

    public void OnButtonGetFriendLB()
    {
        var friendLeaderboardRequest = new GetFriendLeaderboardRequest
        {
            StatisticName = "Highscore",
            MaxResultsCount = _maxResultsCount
        };

        PlayFabClientAPI.GetFriendLeaderboard(friendLeaderboardRequest, OnLeaderboardGet, OnError);
    }

    private void OnProximityLeaderboardGet(GetLeaderboardAroundPlayerResult r)
    {
        _leaderboardScript.ClearLeaderboard();
        foreach (var item in r.Leaderboard)
        {
            _leaderboardScript.AddItem(item.Position, item.DisplayName, item.StatValue);
        }
        StartCoroutine(_leaderboardScript.DisplayLeaderboard());
    }

    private void OnLeaderboardUpdate(UpdatePlayerStatisticsResult r)
    {
        UpdateMsg("Successful leaderboard sent: " + r.ToString());
    }

    private void OnError(PlayFabError e)
    {
        UpdateMsg("Error" + e.GenerateErrorReport());
    }

    private void UpdateMsg(string msg)
    {
        Debug.Log(msg);
        msgBox.text = msg;
    }
}
