using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendListDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject _noUsersText;

    [SerializeField]
    private FriendElement _friendElement;

    [SerializeField]
    private bool _isPending = false;

    private RectTransform _rectTransform;

    private List<GameObject> _elementsToAdd = new();

    public void AddItem(string name, string PlayfabID)
    {
        FriendElement newElement = Instantiate(_friendElement);

        newElement.AddElement(name);
        newElement.playFabId = PlayfabID;

        newElement.transform.SetParent(transform);
        newElement.transform.localScale = Vector3.one;
        _elementsToAdd.Add(newElement.gameObject);
        
        newElement.gameObject.SetActive(false);
    }

    public void DisplayFriendsList()
    {
        _noUsersText.SetActive(!(transform.childCount > 0));
        foreach (var element in _elementsToAdd)
        {
            element.SetActive(true);
        }
        Vector2 contentSize = new(220 * transform.childCount * 0.5f, _rectTransform.sizeDelta.y);
        _rectTransform.sizeDelta = contentSize;
    }

    public void ClearDisplay()
    {
        foreach (Transform child in transform)
        {
           Destroy(child.gameObject);
        }
    }

    private void OnEnable()
    {
        _elementsToAdd.Clear();
        ClearDisplay();
        FriendManager.Instance.GetFriends(_isPending);
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}
