using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Stackable Stun")]
public class StackableStunEffectSO : EffectSO
{
    [SerializeField] private StackableStunEffect stunEffect = new StackableStunEffect();
    public override Effect GetEffectInstance() => new StackableStunEffect(stunEffect);
}

[Serializable]
public class StackableStunEffect : StackingEffect
{
    [SerializeField] private int stacksPerHit;
    public int StacksPerHit => stacksPerHit;

    [SerializeField] private int stunDuration;
    public int StunDuration => stunDuration;

    public StackableStunEffect() { }

    public StackableStunEffect(StackableStunEffect stunEffect) : base(stunEffect) 
    {
        stacksPerHit = stunEffect.stacksPerHit;
        stunDuration = stunEffect.stunDuration;
    }

    public override bool TryTriggerEffect()
    {
        Enemy currentTarget = Hero.Instance.Target;
        if (currentTarget == null || currentTarget.GetAttackTimer().paused) { return false; }

        if(currentTarget.statuses.Count == 0)
        {
            stackCount = stacksPerHit;
            isTicking = true;
            currentTarget.ApplyStatusEffect(new StackableStunEffect(this));
        }
        else    
        {
            var stunEffect = (StackableStunEffect)currentTarget.statuses.Find(effect => effect is StackableStunEffect);
            if(stunEffect == null)
            {
                stackCount = stacksPerHit;
                isTicking = true;
                currentTarget.ApplyStatusEffect(new StackableStunEffect(this));
            }
            else
            {
                stunEffect.AddStacks(stacksPerHit);
                stunEffect.TimeLeft = stunEffect.Duration;

                if (stunEffect.stackCount == stunEffect.maxStacks)
                {
                    stunEffect.TimeLeft = stunEffect.stunDuration;
                    stunEffect.Duration = stunEffect.stunDuration;
                    stunEffect.DisableStacking();
                    currentTarget.GetAttackTimer().PauseAttackTimer(stunEffect.Duration);
                }
            }
        }
        return true;
    }

    public override string GetEffectDesc()
    {
        return string.Format(effectDesc, stunDuration);
    }

    protected override bool OnTick(float deltaTime)
    {
        TimeLeft -= deltaTime;
        if (TimeLeft > 0f)
            return false;

        return true;
    }
}
