using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Effects/Flashpoint Effect")]
public class FlashpointEffectSO : EffectSO
{
    [SerializeField] private FlashpointEffect flashpointEffect = new FlashpointEffect();
    public override Effect GetEffectInstance() => new FlashpointEffect(flashpointEffect);
}

[Serializable]
public class FlashpointEffect : Effect
{
    [SerializeField] private int damage;
    public int Damage => damage;

    public FlashpointEffect() { }
    public FlashpointEffect(FlashpointEffect flashpointEffect)
    {
        damage = flashpointEffect.damage;
    }

    public override bool TryTriggerEffect()
    {
        Enemy currentTarget = Hero.Instance.Target;
        if (currentTarget == null) { return false; }

        Enemy rightEnemy = currentTarget.RightEnemy;
        Enemy leftEnemy = currentTarget.LeftEnemy;

        currentTarget.Damage(damage, Enums.DamageType.Fire);

        if (rightEnemy != null)
        {
            rightEnemy.Damage(damage, Enums.DamageType.Fire);
        }
        if (leftEnemy != null)
        {
            leftEnemy.Damage(damage, Enums.DamageType.Fire);
        }
        return true;
    }

    public override string GetEffectDesc()
    {
        return string.Format(effectDesc, Damage);
    }
}
