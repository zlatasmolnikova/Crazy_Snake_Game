using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICard
{
    /// <summary>
    /// value for projectile factory
    /// </summary>
    public Spell Spell { get; }

    public string Name { get; }

    public SpellType Type { get; }

    public Texture2D Texture { get; }

    public Sprite Sprite { get; }

    public string Description { get; }
}
