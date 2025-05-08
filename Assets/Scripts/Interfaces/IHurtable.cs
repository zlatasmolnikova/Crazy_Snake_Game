using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IHurtable
{
    // though i`m not sure that passing in damageInfo is better than adjusting ConsumeDamage with additional default parameters
    public void TakeDamage(DamageInfo damageInfo);  

    public float Health { get; }
    public float MaxHealth { get; }
    public UnityEvent<float, float> OnHealthDecrease { get; }
    public UnityEvent<float, float> OnHealthIncrease { get; }
    
    public void ConsumeDamage(float amount);    // to little info; might be better to recieve sth like DamageInfo
} 
