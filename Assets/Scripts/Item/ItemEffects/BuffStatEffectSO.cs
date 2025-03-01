using System;
using UnityEngine;
using static Stat;

[CreateAssetMenu(menuName = "Item Effects/Buff Stat")]
public class BuffStatEffectSO : EffectSO
{
    [SerializeField] private StatBuffEffect buffEffect = new StatBuffEffect();
    public override Effect GetEffectInstance() => new StatBuffEffect(buffEffect);
}

[Serializable]
public class StatBuffEffect : TickingEffect
{
    [SerializeField] private ConsumableItem source;
    [SerializeField] private float amount;
    [SerializeField] private StatType stat;
    [SerializeField] private CalcType calcType;
    public StatModifier statModifier { get; init; }

    public StatBuffEffect() { }
    public StatBuffEffect(StatBuffEffect buffEffect) : base(buffEffect)
    {
        source = buffEffect.source;
        amount = buffEffect.amount;
        stat = buffEffect.stat;
        calcType = buffEffect.calcType; 
        statModifier = new StatModifier(amount, stat, calcType);
    }

    public override bool TryTriggerEffect()
    {
        Hero.Instance.ApplyStatusEffect(source, new StatBuffEffect(this));
        return true;
    }

    public override string GetEffectDesc()
    {
        return string.Format(effectDesc, amount, Duration);
    }

    public override bool Tick(float deltaTime)
    {
        TimeLeft -= deltaTime;

        if (TimeLeft > 0f)
            return false;

        Hero.Instance.RemoveStatusEffect(source);
        return true;
    }
}
