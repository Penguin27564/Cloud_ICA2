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

    [SerializeField]
    private int _maxElements = 20;

    private RectTransform _rectTransform;

    private List<GameObject> _elementsToAdd = new();
    public void AddItem(float pos, string name, float score)
    {
        // Max leaderboard size
        if (transform.childCount >= _maxElements) return;

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
        Vector2 contentSize = new(0,  Mathf.Clamp(89 + 89 * transform.childCount, 0, (_maxElements + 1) * 89));
        _rectTransform.sizeDelta = contentSize;
    }

    public void ClearLeaderboard()
    {
        _elementsToAdd.Clear();
        foreach (Transform child in transform)
        {
           Destroy(child.gameObject);
        }
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
