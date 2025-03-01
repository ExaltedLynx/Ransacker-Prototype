using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleAnimBehavior : StateMachineBehaviour
{
    Enemy enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(enemy == null)
            enemy = animator.GetComponent<Enemy>();
        enemy.SetIdle();
    }
}
