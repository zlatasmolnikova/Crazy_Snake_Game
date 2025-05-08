using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardBase : ICard
{
    public abstract Spell Spell {  get; }

    public abstract string Name {  get; }

    public abstract SpellType Type {  get; }

    public Texture2D Texture { get; protected set; }

    public Sprite Sprite { get; protected set; }

    public abstract string Description {  get; }

    public CardBase(Texture2D texture)
    {
        Texture = texture;
        Sprite = Sprite.Create(texture,
            new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
    }
}
