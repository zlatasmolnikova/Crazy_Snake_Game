using System;
using System.Collections.Generic;
using UnityEngine;

public class CardPlacer : MonoBehaviour, IPlacer
{
    private readonly Dictionary<SpellType, Dictionary<Spell, int>> instanceCount = new()
    {
        {
            SpellType.Projectile, new()
            {
                { Spell.Explosion, 1 },
                { Spell.GunShot, 1 },
                { Spell.Grenade, 1 },
                { Spell.Tracker, 1 },
                { Spell.CanonBall, 1 },
            }
        },
        {
            SpellType.Modifier, new()
            {
                { Spell.BouncinessIncrease, 1 },
                { Spell.DamageIncreaseConstant, 1 },
                { Spell.Piercing, 1 },
            }
        },
        {
            SpellType.Branching, new()
            {
                { Spell.AscendTree, 1 },
                { Spell.AscendTreeTwice, 1 },
            }
        }
    };

    private CardFactory cardFactory;

    public void Place(PlacementManager manager)
    {
        if (cardFactory == null)
            cardFactory = GameObject.FindGameObjectWithTag("CardFactory").GetComponent<CardFactory>();
        foreach (var (spellType, dict) in instanceCount)
        {
            foreach (var (spell, count)  in dict)
            {
                for (int i = 0; i < count; i++)
                {
                    PlaceCard(spell, manager);
                }
            }
        }
    }

    private void PlaceCard(Spell spell, PlacementManager manager)
    {
        var position = manager.GetPosition(0);
        var instance = cardFactory.CreateCard(spell);
        var positionInUnity = manager.GetTransformPosition(position) + manager.GetShiftInsideCell();
        cardFactory.CreateCardAvatar(instance, positionInUnity);
        manager.AddOnPlacementMap(position, 2);
    }

    
}