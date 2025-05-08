using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerCard : CardBase
{
    public TrackerCard(Texture2D texture) : base(texture)
    {
    }

    public override Spell Spell => Spell.Tracker;

    public override string Name => "Tracker";

    public override SpellType Type => SpellType.Projectile;

    public override string Description => "Creates a small drone to aim at nearest mob with next projectile";
}
