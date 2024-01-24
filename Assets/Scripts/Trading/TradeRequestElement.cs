using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TradeRequestElement : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _userNameText;

    public string tradeID, requesterID;

    public void SetName(string _userName)
    {
        _userNameText.text = _userName;
    }

    private void Start()
    {
        GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
    }
}
