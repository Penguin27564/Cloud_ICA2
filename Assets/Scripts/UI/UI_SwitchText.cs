using TMPro;
using UnityEngine;

public class UI_SwitchText : MonoBehaviour
{
    [SerializeField]
    private string _defaultText, _switchedText;

    [SerializeField]
    private TMP_Text _textToSwitch;

    public void SwitchText()
    {
        _textToSwitch.text = _textToSwitch.text == _defaultText ? _switchedText : _defaultText;
    }

    private void Awake()
    {
        if (!_textToSwitch) _textToSwitch = GetComponent<TMP_Text>();
        _textToSwitch.text = _defaultText;
    }
}
