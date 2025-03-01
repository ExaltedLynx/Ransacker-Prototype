using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyTooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Enemy displayedEnemy { get; set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.gameIsPaused || displayedEnemy == null)
            return;

        EnemyPreviewTooltip.Instance.DisplayedEnemy = displayedEnemy;
        StopAllCoroutines();
        StartCoroutine(DelayTooltip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.Instance.gameIsPaused || displayedEnemy == null)
            return;

        StopAllCoroutines();
        EnemyPreviewTooltip.Instance.HideTooltip();
    }

    private IEnumerator DelayTooltip()
    {
        yield return new WaitForSeconds(0.2f);
        EnemyPreviewTooltip.Instance.ShowTooltip();
    }

    public void DisableTooltip()
    {
        StopAllCoroutines();
        EnemyPreviewTooltip.Instance.HideTooltip();
    }

}
