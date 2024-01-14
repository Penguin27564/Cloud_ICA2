using UnityEngine;
using TMPro;

public class UI_TogglePassword : MonoBehaviour
{
    private TMP_InputField _passwordInput;
    private bool _hidePassword = true;

    public void TogglePassword()
    {
        _hidePassword = !_hidePassword;
        ChangeContentType();
    }

    private void Start()
    {
        _passwordInput = GetComponent<TMP_InputField>();
    }

    private void ChangeContentType()
    {
        if (_hidePassword)
        {
            _passwordInput.contentType = TMP_InputField.ContentType.Password;
        }
        else
        {
            _passwordInput.contentType = TMP_InputField.ContentType.Standard;
        }
        _passwordInput.ForceLabelUpdate();
    }
}
