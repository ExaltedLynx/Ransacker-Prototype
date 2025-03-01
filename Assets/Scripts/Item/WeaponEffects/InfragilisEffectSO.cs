using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Effects/Infragilis")]
public class InfragilisEffectSO : EffectSO
{
    [SerializeField] private InfragilisEffect infragilisEffect = new InfragilisEffect();
    public override Effect GetEffectInstance() => new InfragilisEffect(infragilisEffect);
}

[Serializable]
public class InfragilisEffect : Effect
{
    public InfragilisEffect() { }
    public InfragilisEffect(InfragilisEffect infragilisEffect)
    {

    }

    public override string GetEffectDesc()
    {
        return string.Format(effectDesc);
    }

    public override bool TryTriggerEffect()
    {
        throw new NotImplementedException();
    }
}
