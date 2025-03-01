using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "Item Effects/Burn")]
public class BurnEffectSO : EffectSO
{
    [SerializeField] private BurnEffect burnEffect = new BurnEffect();
    public override Effect GetEffectInstance() => new BurnEffect(burnEffect);
}

[Serializable]
public class BurnEffect : TickingEffect
{
    [SerializeField] private int damage;
    public int Damage => damage;
    public BurnEffect() { }
    public BurnEffect(BurnEffect burnEffect) : base(burnEffect)
    {
        damage = burnEffect.Damage;
    }

    public override bool TryTriggerEffect()
    {
        Enemy heroTarget = Hero.Instance.Target;
        if(heroTarget == null)
            return false;

        Enemy rightEnemy = heroTarget.RightEnemy;
        Enemy leftEnemy = heroTarget.LeftEnemy;
        Target = heroTarget;
        Target.ApplyStatusEffect(new BurnEffect(this));

        if (leftEnemy != null)
        {
            Target = leftEnemy;
            leftEnemy.ApplyStatusEffect(new BurnEffect(this));
        }
        if (rightEnemy != null)
        {
            Target = rightEnemy;
            rightEnemy.ApplyStatusEffect(new BurnEffect(this));
        }
        return true;
    }

    public override string GetEffectDesc()
    {
        return string.Format(effectDesc, damage, Duration);
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
}
