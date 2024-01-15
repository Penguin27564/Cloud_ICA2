using TMPro;
using UnityEngine;

public class FriendSearchTypeText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _displayText;

    private FriendManager _friendManager;

    private void Start()
    {
        _displayText = GetComponent<TMP_Text>();
        _friendManager = FriendManager.Instance;
        _friendManager.OnUpdateSelectedFriendID += UpdateDisplayText;
        UpdateDisplayText();
    }

    private void UpdateDisplayText()
    {
        if (_friendManager.selectedFriendIdType == FriendManager.FriendIdType.PlayFabID)
        {
            _displayText.text = "Search by PlayFabID";
        }
        else if (_friendManager.selectedFriendIdType == FriendManager.FriendIdType.Username)
        {
            _displayText.text = "Search by username";
        }
        else if (_friendManager.selectedFriendIdType == FriendManager.FriendIdType.Displayname)
        {
            _displayText.text = "Search by display name";
        }
        else if (_friendManager.selectedFriendIdType == FriendManager.FriendIdType.Email)
        {
            _displayText.text = "Search by email";
        }
    }
}
