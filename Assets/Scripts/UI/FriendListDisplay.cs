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
    private int _maxElements = 20;

    private List<GameObject> _elementsToAdd = new();

    public void AddItem(string name)
    {
        if (transform.childCount >= _maxElements) return;

        FriendElement newElement = Instantiate(_friendElement);
        newElement.AddElement(name);
        newElement.transform.SetParent(transform);
        newElement.transform.localScale = Vector3.one;
        _elementsToAdd.Add(newElement.gameObject);
        newElement.gameObject.SetActive(false);
        if (_noUsersText.activeInHierarchy) _noUsersText.SetActive(false);
    }

    public void DisplayFriendsList()
    {
        foreach (var element in _elementsToAdd)
        {
            element.SetActive(true);
        }
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
        FriendManager.Instance.GetFriends();
    }
}
