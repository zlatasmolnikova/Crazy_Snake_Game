using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShotCard : CardBase
{
    public GunShotCard(Texture2D texture) : base(texture)
    {
    }

    public override Spell Spell => GunShot.Spell;

    public override string Name => "GunShot";

    public override SpellType Type => SpellType.Projectile;

    public override string Description => "instant bullet";
}
