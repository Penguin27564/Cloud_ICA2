using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Leaderboard : MonoBehaviour
{
    [SerializeField]
    private GameObject _leaderboardElement, _noUsersText;

    [SerializeField]
    private float _timeBetweenElementDisplay = 0.3f;

    private RectTransform _rectTransform;

    private List<GameObject> _elementsToAdd = new();
    public void AddItem(float pos, string name, float score)
    {
        GameObject newElement = Instantiate(_leaderboardElement);
        newElement.GetComponent<UI_LeaderboardElement>().EnterStats(pos + 1, name, score);
        newElement.transform.SetParent(transform);
        newElement.transform.localScale = Vector3.one;
        _elementsToAdd.Add(newElement);
        newElement.SetActive(false);
        if (_noUsersText.activeInHierarchy) _noUsersText.SetActive(false);
    }

    public IEnumerator DisplayLeaderboard()
    {
        foreach (var element in _elementsToAdd)
        {
            element.SetActive(true);
            yield return new WaitForSeconds(_timeBetweenElementDisplay);
        }
        Vector2 contentSize = new(0,  89 * transform.childCount - 89);
        _rectTransform.sizeDelta = contentSize;
    }

    public void ClearLeaderboard()
    {
        _elementsToAdd.Clear();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
            Debug.Log("Destroy child");
        }
        Debug.Log("CLEAR LEADERBOARD, CHILD COUNT: " + transform.childCount);
    }

    private void OnEnable()
    {
        ClearLeaderboard();
    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}
