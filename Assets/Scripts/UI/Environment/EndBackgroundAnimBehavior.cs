using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBackgroundAnimBehavior : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("end anim");
        BackgroundAnimController.animInProgress = false;
    }
}
