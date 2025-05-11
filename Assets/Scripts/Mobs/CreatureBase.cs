using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CreatureBase : MonoBehaviour, IHurtable
{
    public UnityEvent<float, float> OnHealthDecrease { get; } = new();
    public UnityEvent<float, float> OnHealthIncrease { get; } = new();
    public float Health { get; set; }
    public float MaxHealth { get; private set; }
    public float Damage { get; set; }
    public float ViewAngle { get; set; }

    public CreatureBase(float health, float damage)
    {
        Health = health;
        MaxHealth = Health;
        Damage = damage;
    }

    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        ConsumeDamage(damageInfo.Amount);
    }

    public virtual void ConsumeDamage(float amount)
    {
        Health -= amount;
        OnHealthDecrease.Invoke(Health, MaxHealth);
        if (Health <= 0)
        {
            Die();
        }
    }

    public abstract void PerformAttack();

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
