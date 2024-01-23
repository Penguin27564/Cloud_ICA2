using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header, content;

    public float showDelay = 1.0f;
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(ShowDelay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
        StopAllCoroutines();
    }

    public void OnDisable()
    {
        TooltipSystem.Hide();
        StopAllCoroutines();
    }

    private IEnumerator ShowDelay()
    {
        yield return new WaitForSeconds(showDelay);
        TooltipSystem.Show(content, header);
    }
}
