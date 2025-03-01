using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Weakpoint : MonoBehaviour
{
    [SerializeField] public WeakpointEffect effect;
    [SerializeField] public EnemyPartHandler part;
    [SerializeField] private DamageType weaknessType;
    [Range(0f, 1f)]
    [SerializeField] private float damageThreshold;
    private int damageTaken = 0;

    public bool CheckWeakpointBreak(Enemy enemy)
    {
        if (enemy.GetHealth().getCurrentHealth() > 0 && (float)damageTaken / enemy.GetHealth().GetMaxHealth() >= damageThreshold)
        {
            effect.Effect(enemy, part);
            return true;
        }
        return false;
    }

    public void AddDamage(int amount, DamageType damageType)
    {
        if(damageType == weaknessType)
            damageTaken += amount;
    }
}

public abstract class WeakpointEffect : ScriptableObject
{
    public abstract void Effect(Enemy enemy, EnemyPartHandler partContext);
}
