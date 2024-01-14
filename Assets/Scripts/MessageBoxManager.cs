using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBoxManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _messageBox;

    [SerializeField]
    private float _textDisplayDuration = 3f;

    private TMP_Text _messageBoxText;

    [HideInInspector]
    public static MessageBoxManager Instance;

    private void Start()
    {
        _messageBoxText = _messageBox.GetComponentInChildren<TMP_Text>();
    }

    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    public void DisplayMessage(string text)
    {
        if (_messageBox.activeInHierarchy)
        {
            _messageBox.SetActive(false);
            StopCoroutine(HideMessageBox());
        }
        _messageBox.SetActive(true);
        _messageBoxText.text = text;
        StartCoroutine(HideMessageBox());
    }

    private IEnumerator HideMessageBox()
    {
        yield return new WaitForSeconds(_textDisplayDuration);
        _messageBox.SetActive(false);
    }
}
