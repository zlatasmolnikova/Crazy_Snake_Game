using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageInfo
{
    public float Amount;

    public DamageType Type;

    public Vector3 Impulse;

    // any other thing

    public DamageInfo(float amount = 0, DamageType type = DamageType.ProjectileDamage, Vector3? impulse = null)
    {
        Amount = amount;        // han we heal by damage?
        Type = type;
        Impulse = impulse != null ? (Vector3)impulse : Vector3.zero;
    }

    public DamageInfo SetAmount(float amount)
    {
        return new DamageInfo(amount, Type, Impulse);
    }

    public DamageInfo SetType(DamageType type)
    {
        return new DamageInfo(Amount, type, Impulse);
    }

    public DamageInfo WithImpulse(Vector3 impulse)
    {
        return new DamageInfo(Amount, Type, impulse);
    }
}
