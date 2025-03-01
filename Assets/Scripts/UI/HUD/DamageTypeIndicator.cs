using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class DamageTypeIndicator : MonoBehaviour
{
    [SerializeField] private Image damageTypeIndicator;
    [SerializeField] private Sprite slashingSprite;
    [SerializeField] private Sprite piercingSprite;
    [SerializeField] private Sprite bluntSprite;

    public bool indicatorEnabled { get; private set; } = false;

    public void SetDmgTypeIndicator(DamageType dmgType)
    {
        damageTypeIndicator.enabled = true;
        indicatorEnabled = true;
        switch (dmgType)
        {
            case DamageType.Slashing:
                damageTypeIndicator.sprite = slashingSprite;
                break;
            case DamageType.Piercing:
                damageTypeIndicator.sprite = piercingSprite;
                break;
            case DamageType.Blunt:
                damageTypeIndicator.sprite = bluntSprite;
                break;
        }
    }

    public void DisableDmgTypeIndicator()
    {
        damageTypeIndicator.enabled = false;
        indicatorEnabled = false;
    }
}
