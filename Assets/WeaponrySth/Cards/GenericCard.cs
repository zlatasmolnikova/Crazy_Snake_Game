using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericCard : CardBase
{
    public GenericCard(Texture2D texture, Spell spell, string name, SpellType type, string description) : base(texture)
    {
        Spell = spell;
        Name = name;
        Type = type;
        Description = description;
    }

    public override Spell Spell { get; }

    public override string Name { get; }

    public override SpellType Type { get; }

    public override string Description { get; }
}
