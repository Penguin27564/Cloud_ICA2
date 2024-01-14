using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class InventoryManager : MonoBehaviour
{
    public int testVariable = 0;
    public static InventoryManager Instance;

    public Action OnBuyItem;
    public void GetVirtualCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        r =>
        {
            int coins = r.VirtualCurrency["CN"];
            UpdateMsg("Coins: " + coins);
        }, OnError);
    }

    public void GetCatalog()
    {
        var catreq = new GetCatalogItemsRequest
        {
            CatalogVersion = "terranweapons"
        };

        PlayFabClientAPI.GetCatalogItems(catreq,
        r =>
        {
            List<CatalogItem> items = r.Catalog;
            UpdateMsg("Catalog Items");

            foreach (CatalogItem i in items)
            {
                UpdateMsg(i.DisplayName + ", " + i.VirtualCurrencyPrices["CN"]);
            }
        }, OnError);
    }

    public void BuyItem()
    {
        var buyreq = new PurchaseItemRequest
        {
            // current sample is hardcoded, should make it more dynamic
            CatalogVersion = "terranweapons",
            ItemId = "Weapon01LC",
            VirtualCurrency = "CN",
            Price = 5
        };

        PlayFabClientAPI.PurchaseItem(buyreq,
        r =>
        {
            UpdateMsg("Bought!");
        }, OnError);
    }

    public void GetPlayerInventory(string catalog)
    {
        var userInv = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(userInv,
        r =>
        {
            List<ItemInstance> ii = r.Inventory;
            UpdateMsg("Player Inventory");

            foreach (ItemInstance i in ii)
            {
                UpdateMsg(i.DisplayName + ", " + i.ItemId + ", " + i.ItemInstanceId);
            }
        }, OnError);
    }

    private void Awake()
    {
        if (!Instance) Instance = this;
        GetVirtualCurrencies();
    }

    private void UpdateMsg(string msg)
    {
        Debug.Log(msg);
    }

    private void OnError(PlayFabError e)
    {
        UpdateMsg(e.GenerateErrorReport());
    }
}
