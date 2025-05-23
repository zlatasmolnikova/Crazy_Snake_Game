using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAnimationEnd : StateMachineBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1f && !animator.IsInTransition(layerIndex))
        {
            animator.SetBool("canShoot", true);
        }
    }
}
