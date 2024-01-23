using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeManager : MonoBehaviour
{
    private static TradeManager _instance;

    public static TradeManager Instance
    {
        get
        {
            if (_instance == null) _instance = new TradeManager();
            return _instance;
        }
    }

    public TradeManager()
    {
        _instance = this;
    }

    [SerializeField]
    private GameObject _tradingScreen;

    public Action OnStartTrading;

    private void Awake()
    {
        OnStartTrading += EnableTradeUI;
    }

    private void EnableTradeUI()
    {
        _tradingScreen.SetActive(true);
    }
}
