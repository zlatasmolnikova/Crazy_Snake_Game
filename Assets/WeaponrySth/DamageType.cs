using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags] public enum DamageType
{
    None = 0,
    ProjectileDamage = 1,
    ExplosionDamage = 1 << 1,
    MeleeDamage = 1 << 2,
}
