using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldCooldownHandler : MonoBehaviour
{
    [SerializeField] Image cooldownFill;
    private float currentFatigue = 0;
    private float duration;
    private float timeLeft;
    public bool onCooldown { get; private set; } = false;

    void Update()
    {
        if(onCooldown)
        {
            cooldownFill.fillAmount = timeLeft / duration;
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
                ResetCooldown();
        }
    }

    public void AddBlockFatigue(float addedFatigue, float maxFatigue)
    {
        Debug.Log("Fatigue added: " + addedFatigue + " " + "Max Fatigue: " + maxFatigue);
        currentFatigue += addedFatigue;
        float fatiguePercent = currentFatigue / maxFatigue;
        cooldownFill.fillAmount = fatiguePercent;
        if(currentFatigue >= maxFatigue)
        {
            currentFatigue = 0;
            StartShieldCooldown();
        }
    }

    private void StartShieldCooldown()
    {
        duration = Hero.Instance.GetShieldCDDuration();
        timeLeft = duration;
        onCooldown = true;
        gameObject.SetActive(true);
        cooldownFill.enabled = true;
        Hero.Instance.DisableShieldBlocking();
    }

    private void ResetCooldown()
    {
        ResetBlockFatigue();
        timeLeft = duration;
        onCooldown = false;
    }

    public void DisableCooldown()
    {
        ResetBlockFatigue();
        timeLeft = duration;
        onCooldown = false;
        cooldownFill.enabled = false;
        gameObject.SetActive(false);
    }

    public void ResetBlockFatigue()
    {
        currentFatigue = 0;
        cooldownFill.fillAmount = 0;
    }

    public void HideShieldCooldown()
    {
        cooldownFill.enabled = false;
    }
}
