using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Heal")]
public class HealEffectSO : EffectSO
{
    //[SerializeField] private HealEffect healEffect = new HealEffect();
    public override Effect GetEffectInstance()
    {
       return new HealEffect();
    } 
}

[Serializable]
public class HealEffect : Effect
{
    [SerializeField] private int healAmount;

    public int HealAmount => healAmount;

    public HealEffect() { }

    public HealEffect(HealEffect healEffect)
    {
        healAmount = healEffect.healAmount;
    }

    public override bool TryTriggerEffect()
    {
        Hero.Instance.Heal(healAmount);
        return true;
    }

    public override string GetEffectDesc()
    {
        return string.Format(effectDesc, healAmount);
    }
}
