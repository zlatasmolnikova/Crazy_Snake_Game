using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncinessIncreaseCard : CardBase
{
    public override Spell Spell => BouncyModifier.Spell;

    public override string Name => "Bouncy Modifier";

    public override SpellType Type => SpellType.Modifier;

    public override string Description => "Increase bounciness of projectile";

    public BouncinessIncreaseCard(Texture2D texture) : base(texture)
    {
    }
}
