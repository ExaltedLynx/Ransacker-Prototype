using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MerchantItem))]
public class MerchantTooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hasEnteredItem = false;
    [SerializeField] private MerchantItem merchantItem;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(GameManager.Instance.gameIsPaused || GameManager.Instance.shopTutorialEnabled)
            return;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            merchantItem.stockedItem.QuickEnableTooltip();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(DelayTooltip());
        }
        hasEnteredItem = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.Instance.gameIsPaused || GameManager.Instance.shopTutorialEnabled)
            return;

        hasEnteredItem = false;
        StopAllCoroutines();
        ItemTooltip.Instance.HideTooltip();
        BarterTooltip.Instance.HideTooltip();       
    }

    private IEnumerator DelayTooltip()
    {
        yield return new WaitForSeconds(0.2f);
        BarterTooltip.Instance.SetMerchantItem(merchantItem);
        BarterTooltip.Instance.ShowTooltip();
    }

    public void QuickEnableTooltip()
    {
        BarterTooltip.Instance.SetMerchantItem(merchantItem);
        BarterTooltip.Instance.ShowTooltip();
        //enabled = true;
    }

    public void HandleToggle()
    {
        if (enabled)
        {
            StopAllCoroutines();
            BarterTooltip.Instance.HideTooltip();
        }
        else
        {
            StartCoroutine(DelayTooltip());
        }
        enabled = !enabled;

    }
}
