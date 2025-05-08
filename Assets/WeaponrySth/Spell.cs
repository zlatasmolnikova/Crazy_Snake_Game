using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Spell
{
    // Projectiles
    Explosion = 0,
    GunShot = 1,
    Grenade = 2,
    Tracker = 3,
    CanonBall = 4,

    // Modifiers
    BouncinessIncrease = 101,
    DamageIncreaseConstant = 102,
    Piercing = 103,

    // Branching
    AscendTree = 201,
    AscendTreeTwice = 202,
}
