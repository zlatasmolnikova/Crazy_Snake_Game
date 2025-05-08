using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SnakeHitboxHandler : MonoBehaviour //, IHurtable
{
    public float Health { get; private set; }

    public float MaxHealth { get; private set; }

    public UnityEvent<float, float> OnHealthDecrease => throw new System.NotImplementedException();

    public UnityEvent<float, float> OnHealthIncrease => throw new System.NotImplementedException();

    public void ConsumeDamage(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        //if
    }
}
