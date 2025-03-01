using UnityEngine;

[CreateAssetMenu(menuName = "Weakpoint Effects/Execute")]
public class ExecuteEffect : WeakpointEffect
{
    public override void Effect(Enemy enemy, EnemyPartHandler part)
    {
        enemy.ForceKillEnemy();
    }
}
