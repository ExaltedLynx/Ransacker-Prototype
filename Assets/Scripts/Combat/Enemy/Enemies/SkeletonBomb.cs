using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

//Every part is weak to fire which will immediately cause it to explode
//Maybe at a certain amount of damage taken will try to ignite itself and has to be killed before it does so
public class SkeletonBomb : Enemy
{
    bool willExplode = true;
    bool isExploding = false;
    float fuseDuration = 12f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (isExploding && attackTimer.TimeLeft <= 0f)
            ForceKillEnemy();
    }

    protected override void DestroyEnemy()
    {
        //Explodes on death dealing damage to every enemy and the player
        if(willExplode)
            Explode();

        base.DestroyEnemy();
    }

    public override void Damage(int amount, DamageType damageType)
    {
        if (damageType == DamageType.Fire)
        {
            ForceKillEnemy();
            return;
        }

        if(enemyHealth.getCurrentHealth() - amount < 1)
            willExplode = false;

        base.Damage(amount, damageType);

        if (isExploding == false && enemyHealth.getCurrentHealth() < enemyHealth.GetMaxHealth() / 2)
        {
            isExploding = true;
            attackTimer.SetStartTime(fuseDuration);
        }
    }

    public override void Attack()
    {
        if (isExploding)
            return;

       base.Attack();
       Hero.Instance.Damage(damage, this);
    }

    private void Explode()
    {
        var currentEnemies = EnemyController.Instance.Enemies;
        foreach (Enemy enemy in currentEnemies)
        {
            if (enemy != null && enemy != this)
                enemy.Damage(40, DamageType.Fire);
        }
        Hero.Instance.Damage(40, this);
    }
}
