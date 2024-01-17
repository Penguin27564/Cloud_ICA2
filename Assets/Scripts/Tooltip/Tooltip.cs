using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField, contentField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;

    public RectTransform rectTransform;

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;

        layoutElement.enabled = Math.Max(headerField.preferredWidth, contentField.preferredWidth) >= layoutElement.preferredWidth;

        Vector2 position = Input.mousePosition;

        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;

        if (!rectTransform) 
        {
            rectTransform = GetComponent<RectTransform>();
        }

        rectTransform.pivot = new(pivotX, pivotY);

        transform.position = position;
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            layoutElement.enabled = Math.Max(headerField.preferredWidth, contentField.preferredWidth)
                                >= layoutElement.preferredWidth;
        }
    }
}
