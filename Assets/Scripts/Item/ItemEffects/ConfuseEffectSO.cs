using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "Item Effects/Confuse")]
public class ConfuseEffectSO : EffectSO
{
    [SerializeField] private ConfuseEffect confuseEffect = new ConfuseEffect();
    public override Effect GetEffectInstance() => new ConfuseEffect(confuseEffect);
}

[Serializable]
public class ConfuseEffect : TickingEffect
{
    [SerializeField] private int damage;
    public int Damage => damage;

    public ConfuseEffect() { }
    public ConfuseEffect(ConfuseEffect confuseEffect) : base(confuseEffect)
    {
        damage = confuseEffect.damage;
    }

    public override bool TryTriggerEffect()
    {
        Enemy currentTarget = Hero.Instance.Target;
        if (currentTarget == null || currentTarget.GetAttackTimer().paused) { return false; }

        currentTarget.GetAttackTimer().PauseAttackTimer(Duration);
        Target = currentTarget;
        currentTarget.ApplyStatusEffect(new ConfuseEffect(this));
        return true;
    }

    public override string GetEffectDesc()
    {
        return string.Format(effectDesc, Duration, Damage);
    }

    public override bool Tick(float deltaTime)
    {
        TimeLeft -= deltaTime;
        timeSinceLastTick += deltaTime;

        if (timeSinceLastTick >= tickRate)
        {
            Target.Damage(damage, Enums.DamageType.Blunt);
            timeSinceLastTick = 0f;
            return false;
        }
        else if (TimeLeft > 0f)
            return false;

        return true;
    }
}
