using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSwordsman : Enemy
{
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
    }

    protected override void DestroyEnemy()
    {
        base.DestroyEnemy();
    }

    public override void Attack()
    {
        base.Attack();
        Hero.Instance.Damage(damage, this);
    }
}
