using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TradePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerInventory, _playerOffer, _requesteeItems, _playerRequest;
    
    private List<string> itemOffersID, itemRequestsID;

    public void AddOrRemoveOffer(string type)
    {
        if (type == "Red")
        {
            _playerInventory.transform.GetChild(0).gameObject
                            .SetActive(!_playerInventory.transform.GetChild(0).gameObject.activeInHierarchy);

            _playerOffer.transform.GetChild(0).gameObject
                            .SetActive(!_playerOffer.transform.GetChild(0).gameObject.activeInHierarchy);
        }
        else if (type == "Blue")
        {
            _playerInventory.transform.GetChild(1).gameObject
                            .SetActive(!_playerInventory.transform.GetChild(1).gameObject.activeInHierarchy);

            _playerOffer.transform.GetChild(1).gameObject
                            .SetActive(!_playerOffer.transform.GetChild(1).gameObject.activeInHierarchy);
        }
        else if (type == "Yellow")
        {
            _playerInventory.transform.GetChild(2).gameObject
                            .SetActive(!_playerInventory.transform.GetChild(2).gameObject.activeInHierarchy);

            _playerOffer.transform.GetChild(2).gameObject
                            .SetActive(!_playerOffer.transform.GetChild(2).gameObject.activeInHierarchy);
        }
    }

    public void AddOrRemoveRequest(string type)
    {
        if (type == "Red")
        {
            _requesteeItems.transform.GetChild(0).gameObject
                            .SetActive(!_requesteeItems.transform.GetChild(0).gameObject.activeInHierarchy);

            _playerRequest.transform.GetChild(0).gameObject
                            .SetActive(!_playerRequest.transform.GetChild(0).gameObject.activeInHierarchy);
        }
        else if (type == "Blue")
        {
            _requesteeItems.transform.GetChild(1).gameObject
                            .SetActive(!_requesteeItems.transform.GetChild(1).gameObject.activeInHierarchy);

            _playerRequest.transform.GetChild(1).gameObject
                            .SetActive(!_playerRequest.transform.GetChild(1).gameObject.activeInHierarchy);
        }
        else if (type == "Yellow")
        {
            _requesteeItems.transform.GetChild(2).gameObject
                            .SetActive(!_requesteeItems.transform.GetChild(2).gameObject.activeInHierarchy);

            _playerRequest.transform.GetChild(2).gameObject
                            .SetActive(!_playerRequest.transform.GetChild(2).gameObject.activeInHierarchy);
        }
    }

    private void AddToOffer()
    {
        // Add to a list of offered skin names
        List<string> offeredTypes = new();
        if (_playerOffer.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            offeredTypes.Add("Red");
        }
        if (_playerOffer.transform.GetChild(1).gameObject.activeInHierarchy)
        {
            offeredTypes.Add("Blue");
        }
        if (_playerOffer.transform.GetChild(2).gameObject.activeInHierarchy)
        {
            offeredTypes.Add("Yellow");
        }

        // Look through entire player's inventory
        foreach (var item in PFDataMgr.Instance.currentPlayerInventoryItems)
        {
            // If item id matches offered skin name
            if (offeredTypes.Contains(item.ItemId))
            {
                // Add to offers
                Debug.Log("Added " + item.ItemId);
                itemOffersID.Add(item.ItemInstanceId);
            }
        }
    }

    private void AddToRequests()
    {
        if (_playerRequest.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            itemRequestsID.Add("Red");
        }
        if (_playerRequest.transform.GetChild(1).gameObject.activeInHierarchy)
        {
            itemRequestsID.Add("Blue");
        }
        if (_playerRequest.transform.GetChild(2).gameObject.activeInHierarchy)
        {
            itemRequestsID.Add("Yellow");
        }
    }

    public void SendTrade()
    {
        itemOffersID.Clear();
        itemRequestsID.Clear();

        AddToOffer();
        AddToRequests();
        
        Debug.Log("Offers: " + itemOffersID.ToCommaSeparatedString());

        if (itemOffersID.Count == 0 && itemRequestsID.Count == 0)
        {
            MessageBoxManager.Instance.DisplayMessage("Empty trade");
            return;
        }

        TradeManager.Instance.SendTradeRequest(itemOffersID, itemRequestsID);
    }

    private void Awake()
    {
        itemOffersID = new();
        itemRequestsID = new();
    }

    private void OnEnable()
    {
        itemOffersID.Clear();
        itemRequestsID.Clear();
    }
}
