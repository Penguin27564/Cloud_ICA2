using TMPro;
using UnityEngine;

public class LeaderboardTypeText : MonoBehaviour
{
    private TMP_Text _displayText;

    private PlayFabLeaderboard _pfLeaderboard;

    private void Start()
    {
        _displayText = GetComponent<TMP_Text>();
        _pfLeaderboard = PlayFabLeaderboard.Instance;
        _pfLeaderboard.OnUpdateLeaderboardType += UpdateDisplayText;
        UpdateDisplayText();
    }

    private void UpdateDisplayText()
    {
        if (_pfLeaderboard.leaderboardType == 0)
        {
            _displayText.text = "Global";
        }
        else if (_pfLeaderboard.leaderboardType == 1)
        {
            _displayText.text = "Proximity";
        }
        else if (_pfLeaderboard.leaderboardType == 2)
        {
            _displayText.text = "Friends";
        }
    }
}
