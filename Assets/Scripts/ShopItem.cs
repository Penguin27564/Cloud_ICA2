using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Collections.Generic;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    private string _catalogVersion = "skins";

    [SerializeField]
    private string _itemID, _itemDisplayName;

    [SerializeField]
    private string _virtualCurrency = "CN";

    [SerializeField]
    private int _price = 5;

    [SerializeField]
    private TMP_Text _nameText, _priceText;

    private bool _canBuy;
    public void BuyItem()
    {
        _canBuy = true;

        var req = new GetUserInventoryRequest
        {
            
        };

        PlayFabClientAPI.GetUserInventory(req,
        r =>
        {
            List<ItemInstance> items = r.Inventory;

            foreach (ItemInstance i in items)
            {
                if (i.ItemId == _itemID)
                {
                    Debug.Log("Already Own skin");
                    MessageBoxManager.Instance.DisplayMessage("Already own skin");
                    _canBuy = false;
                }
            }
            // If they don't, allow them to buy
            if (_canBuy)
            {
                var buyreq = new PurchaseItemRequest
                {
                    CatalogVersion = _catalogVersion,
                    ItemId = _itemID,
                    VirtualCurrency = _virtualCurrency,
                    Price = _price
                };

                PlayFabClientAPI.PurchaseItem(buyreq,
                r =>
                {
                    PFDataMgr.Instance.GetUserInventory();
                    Debug.Log("Bought!");
                    MessageBoxManager.Instance.DisplayMessage("Bought!");
                    InventoryManager.Instance.OnBuyItem.Invoke();
                }, OnError);
            }
        }, OnError);
    }

    private void Start()
    {
        _nameText.text = _itemDisplayName;
        _priceText.text = "Cost: " + _price.ToString();
    }

    private void OnError(PlayFabError e)
    {
        Debug.Log(e.GenerateErrorReport());
        MessageBoxManager.Instance.DisplayMessage(e.GenerateErrorReport());
    }
}
