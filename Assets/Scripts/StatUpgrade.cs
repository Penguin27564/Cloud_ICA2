using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class StatUpgrade : MonoBehaviour
{
    [SerializeField]
    private string _statName;

    [SerializeField]
    private int _cost;

    [SerializeField]
    private TMP_Text _statPriceText;

    public void BuyUpgrade()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        r =>
        {
            int coins = r.VirtualCurrency["CN"];
            if (coins >= _cost)
            {
                SubtractVirtualCurrency(_cost);
                // If player has enough, add stat increase
                PFDataMgr.Instance.AddStat(_statName);
                InventoryManager.Instance.OnBuyItem.Invoke();
            }
            else
            {
                MessageBoxManager.Instance.DisplayMessage("Not enough funds");
            }
        }, OnError);
    }

    private void SubtractVirtualCurrency(int amount)
    {
        var req = new SubtractUserVirtualCurrencyRequest
        {
            Amount = amount,
            VirtualCurrency = "CN"
        };

        PlayFabClientAPI.SubtractUserVirtualCurrency(req, OnSubtractCurrencySuccess, OnError);
    }

    private void OnSubtractCurrencySuccess(ModifyUserVirtualCurrencyResult r)
    {
        Debug.Log("Currency Added: " + r.ToString());
    }

    private void OnError(PlayFabError e)
    {
        Debug.Log("Error" + e.GenerateErrorReport());
        MessageBoxManager.Instance.DisplayMessage(e.GenerateErrorReport());
    }

    private void Start()
    {
        _statPriceText.text = "$" + _cost.ToString();
    }
}
