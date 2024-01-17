using TMPro;
using UnityEngine;

public class UI_LeaderboardElement : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _positionText, _nameText, _scoreText;
    
    public void EnterStats(float pos, string name, float score)
    {
        _positionText.text = pos.ToString();
        _nameText.text = name;
        _scoreText.text = score.ToString();
    }
}
