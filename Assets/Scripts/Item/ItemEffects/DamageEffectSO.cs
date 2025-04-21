using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Damage Effect")]
public class DamageEffectSO : EffectSO
{
    [SerializeField] private DamageEffect damageEffect = new DamageEffect();
    public override Effect GetEffectInstance() => new DamageEffect(damageEffect);
}

[Serializable]
public class DamageEffect : Effect
{
    [SerializeField] private int damage;
    public int Damage => damage;

    public DamageEffect() { }
    public DamageEffect(DamageEffect damageEffect)
    {
        damage = damageEffect.Damage;
    }

    public override bool TryTriggerEffect()
    {
        //Hero.Instance.Damage(damage, );
        return true;
    }

    public override string GetEffectDesc()
    {
        return string.Format(effectDesc, damage);
    }
}
