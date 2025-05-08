using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCard : CardBase
{
    public ExplosionCard(Texture2D texture) : base(texture)
    {
    }

    public override Spell Spell => Explosion.Spell;

    public override string Name => "Explosion";

    public override SpellType Type => SpellType.Projectile;

    public override string Description => "Explodes";
}
