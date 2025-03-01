using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Effects/Obsidian Shard")]
public class ObsidianShardEffectSO : EffectSO
{
    [SerializeField] private ObsidianShardEffect effect = new ObsidianShardEffect();
    public override Effect GetEffectInstance() => new ObsidianShardEffect(effect);
}

[Serializable]
public class ObsidianShardEffect : TickingEffect
{
    [SerializeField] private int damage;
    public int Damage => damage;
    public ObsidianShardEffect() { }
    public ObsidianShardEffect(ObsidianShardEffect effect) : base(effect)
    {
        damage = effect.Damage;
    }

    public override bool TryTriggerEffect()
    {
        Enemy target = Hero.Instance.Target;

        if (target == null)
            return false;

        if (target.statuses.Count > 0)
        {
            if(target.statuses.Find(effect => effect is ObsidianShardEffect) != null)
                return false;
        }
        else
        {
            Target = target;
            target.ApplyStatusEffect(new ObsidianShardEffect(this));
        }
        return true;
    }

    public override bool Tick(float deltaTime)
    {
        TimeLeft -= deltaTime;
        timeSinceLastTick += deltaTime;

        if (timeSinceLastTick >= tickRate)
        {
            Target.Damage(damage, Enums.DamageType.Fire);
            timeSinceLastTick = 0f;
            return false;
        }
        else if (TimeLeft > 0f)
            return false;

        return true;
    }

    public override string GetEffectDesc()
    {
        return string.Format(effectDesc, Damage, Duration);
    }
}
