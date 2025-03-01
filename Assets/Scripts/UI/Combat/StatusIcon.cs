using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour
{
    [SerializeField] private Image statusIcon;
    [SerializeField] private Image durationFillImage;
    [SerializeField] private TextMeshProUGUI stackCountText;
    private TickingEffect status;

    void Update()
    {
        if(durationFillImage.enabled == false)   
            return;

        durationFillImage.fillAmount = status.TimeLeft / status.Duration;
    }

    public void InitStatusIcon(TickingEffect status)
    {
        this.status = status;
        statusIcon.sprite = status.Icon;
        if(status is StackingEffect)
        {
            stackCountText.enabled = true;
            UpdateStackCountText();
        }
        
        if(status.isTicking)
            durationFillImage.enabled = true;
    }

    public void UpdateStackCountText()
    {
        StackingEffect stackingStatus = (StackingEffect)status;
        stackCountText.text = stackingStatus.stackCount.ToString();
    }

    public void ToggleEffectTimerVisual(bool toggle)
    {
        durationFillImage.enabled = toggle;
    }

    public void DisableStackCountText()
    {
        stackCountText.enabled = false;
    }
}
