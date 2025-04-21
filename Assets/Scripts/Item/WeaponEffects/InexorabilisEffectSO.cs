using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Effects/Inexorabilis Effect")]
public class InexorabilisEffectSO : EffectSO
{
    [SerializeField] private InexorabilisEffect inexorabilisEffect = new InexorabilisEffect();
    public override Effect GetEffectInstance() => new InexorabilisEffect(inexorabilisEffect);
}

[Serializable]
public class InexorabilisEffect : Effect
{
    public InexorabilisEffect() { }
    public InexorabilisEffect(InexorabilisEffect effect)
    {

    }

    public override string GetEffectDesc()
    {
        return string.Format(effectDesc);
    }

    public override bool TryTriggerEffect()
    {
        return true;
    }
}
