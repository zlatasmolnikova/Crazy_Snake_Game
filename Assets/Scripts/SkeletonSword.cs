using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSword : MonoBehaviour
{

    private PlayerComponent playerComponent;
    private float swordDamage;
    private Animator animator;

    void Start()
    {
        swordDamage = 1;
        playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
        animator = transform.root.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        var hash = stateInfo.shortNameHash;
        var attackStateHash = Animator.StringToHash("Attack");

        if (other.CompareTag("Player") && hash == attackStateHash)
        {
            playerComponent.ConsumeDamage(swordDamage);
        }
    }
}
