using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimHandler : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Enemy enemy;
    public bool animInProgress { get; set; }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void HandleNewState(Enemy.EnemyState state)
    {
        switch(state)
        {
            case Enemy.EnemyState.Spawning:

            break;
                
            case Enemy.EnemyState.Idle:
                //enemy.GetAttackTimer().toggleTimer(true); //on
            break;

            case Enemy.EnemyState.Attacking:
                enemy.GetAttackTimer().toggleTimer(false); //off
                animInProgress = true;
                StartEnemyAnim("StartAttack");

            break;

            case Enemy.EnemyState.Dying:
                enemy.GetAttackTimer().toggleTimer(false); //off
                animInProgress = true;
                StartEnemyAnim("StartDeath");
            break;
        }
    }

    public IEnumerator WaitUntilAnimComplete(Action delayedMethod)
    {
        yield return new WaitUntil(() => animInProgress = false);
        delayedMethod();
    }

    public void StartEnemyAnim(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }
}
