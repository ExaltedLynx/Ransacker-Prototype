using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InventoryItem))]
public class ItemTooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private InventoryItem item;

    public void OnPointerEnter(PointerEventData eventData)
    {   
        if(GameManager.Instance.gameIsPaused)
            return;

        StopAllCoroutines();
        StartCoroutine(DelayTooltip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.Instance.gameIsPaused)
            return;

        StopAllCoroutines();
        ItemTooltip.Instance.HideTooltip();
    }

    private IEnumerator DelayTooltip()
    {
        yield return new WaitForSeconds(0.2f);
        ItemTooltip.Instance.SetDisplayedItem(item);
        ItemTooltip.Instance.ShowTooltip();
    }

    public void QuickEnableTooltip()
    {
        ItemTooltip.Instance.SetDisplayedItem(item);
        ItemTooltip.Instance.ShowTooltip();
        //enabled = true;
    }

    public void DisableTooltip()
    {
        StopAllCoroutines();
        ItemTooltip.Instance.HideTooltip();
    }

    public void HandleToggle()
    {
        
        if (enabled)
        {
            StopAllCoroutines();
            ItemTooltip.Instance.HideTooltip();
        }
        else
        {
            StartCoroutine(DelayTooltip());
        }
        enabled = !enabled;
    }
}
