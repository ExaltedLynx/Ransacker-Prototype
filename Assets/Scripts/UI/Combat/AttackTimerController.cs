using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackTimerController : MonoBehaviour
{
    [SerializeField] public Text valueText;
    [SerializeField] public Slider slider;

    [SerializeField] private AttackTimer timer;

    private void Start()
    {
        RefreshTimerVisual();
    }

    private void Update()
    {
        if (timer.paused)
            return;

        slider.value = slider.maxValue - timer.TimeLeft;
        valueText.text = timer.TimeLeft.ToString("N1");
    }

    public void setTimerInstance(AttackTimer timer)
    {
        this.timer = timer;
        slider.maxValue = timer.StartTime;

    }

    public void RefreshTimerVisual()
    {
        slider.maxValue = timer.StartTime;
    }
}
