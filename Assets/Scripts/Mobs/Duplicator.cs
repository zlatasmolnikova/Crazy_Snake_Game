using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Duplicator : MonoBehaviour, IHurtable
{
    public float Health => 1;
    public float MaxHealth => 1;
    public UnityEvent<float, float> OnHealthDecrease { get; } = new();
    public UnityEvent<float, float> OnHealthIncrease { get; } = new();

    public void ConsumeDamage(float amount)
    {
        TakeDamage(new DamageInfo(amount));
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        Instantiate(gameObject).name = "duplicator clone";
    }
}
