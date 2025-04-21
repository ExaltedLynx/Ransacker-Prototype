using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weakpoint Effects/Break Part")]
public class BreakPartEffect : WeakpointEffect
{
    public override void Effect(Enemy enemy, EnemyPartHandler partContext)
    {
        partContext.TogglePartTargeting(false);
    }
}
