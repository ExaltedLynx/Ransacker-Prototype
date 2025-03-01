using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Stun")]
public class StunEffectSO : EffectSO
{
    [SerializeField] private StunEffect stunEffect = new StunEffect();
    public override Effect GetEffectInstance() => new StunEffect(stunEffect);
}

[Serializable]
public class StunEffect : TickingEffect
{
    public StunEffect() { }
    public StunEffect(StunEffect stunEffect) : base(stunEffect) { }

    public StunEffect(float duration, Sprite icon, Enemy target) : base(duration, icon, target) { }

    public override bool TryTriggerEffect()
    {
        Enemy currentTarget = Hero.Instance.Target;
        if (currentTarget == null || currentTarget.GetAttackTimer().paused) { return false; }

        currentTarget.GetAttackTimer().PauseAttackTimer(Duration);
        currentTarget.ApplyStatusEffect(new StunEffect(this));
        return true;
    }

    public override string GetEffectDesc()
    {
        return string.Format(effectDesc, Duration);
    }


    public override bool Tick(float deltaTime)
    {
        TimeLeft -= deltaTime;
        if (TimeLeft > 0f)
            return false;

        return true;
    }
}
