using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCard : CardBase
{
    public GrenadeCard(Texture2D texture) : base(texture)
    {
    }

    public override Spell Spell => GrenadeProjectile.Spell;

    public override string Name => "Grenade";

    public override SpellType Type => SpellType.Projectile;

    public override string Description => "a bomb that explodes upon touching something; dont shoot it!";
}
