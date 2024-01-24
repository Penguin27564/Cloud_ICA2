using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject _offerPanel, _requestPanel;
    public void SendTrade()
    {
        List<string> test = new();
        TradeManager.Instance.SendTradeRequest(test, test);
    }
}
