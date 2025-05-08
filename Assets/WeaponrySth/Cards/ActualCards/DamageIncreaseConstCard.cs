using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIncreaseConstCard : CardBase
{
    public DamageIncreaseConstCard(Texture2D texture) : base(texture)
    {
    }

    public override Spell Spell => ConstantDamageIncreaseModifier.Spell;

    public override string Name => "Damage Increase (Constant)";

    public override SpellType Type => SpellType.Modifier;

    public override string Description => "Increases damage of projectile by constant value (5)";
}
