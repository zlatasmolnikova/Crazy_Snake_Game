using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class PiercingCard : CardBase
{
    public override string Name => "Piercing";

    public override SpellType Type => SpellType.Modifier;

    public override string Description => "makes projectile pierce enemies";

    public override Spell Spell => PiercingModifier.Spell;

    public PiercingCard(Texture2D texture) : base(texture)
    {
    }
}
