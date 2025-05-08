using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantDamageIncreaseModifier : IModifier
{
    public float DamageDelta = 5;

    public static Spell Spell => Spell.DamageIncreaseConstant;

    public bool TryModify(IModifiable modifiable)
    {
        if (modifiable.TryGetModificationInterface<IDamaging>(out var damaging))
        {
            damaging.DamageInfo = damaging.DamageInfo.SetAmount(damaging.DamageInfo.Amount + DamageDelta);
            return true;
        }
        return false;
    }
}
