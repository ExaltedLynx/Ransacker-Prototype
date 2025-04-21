using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEndBehavior : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<EnemyAnimHandler>().animInProgress = false;
        if (stateInfo.IsName("Death"))
        {
            animator.GetComponent<Enemy>().HandleEnemyDeath();
        }
    }
}
