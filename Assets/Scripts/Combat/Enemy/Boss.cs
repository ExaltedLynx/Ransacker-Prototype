using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : Enemy
{
    public DungeonType dungeonType { get; private set; } = DungeonType.dungeon0;

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
    }
}