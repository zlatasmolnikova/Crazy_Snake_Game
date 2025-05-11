using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject cheapPistolPrefab;

    [SerializeField]
    private GameObject canonPrefab;

    [SerializeField]
    private GameObject mimicPistolPrefab;

    // inner

    private System.Random generator = new System.Random();

    private CardFactory cardFactory;

    private void Awake()
    {
        generator.Next();
        generator.Next();
        generator.Next();
    }

    private void Start()
    {
        cardFactory = GameObject.FindGameObjectWithTag("CardFactory").GetComponent<CardFactory>();
    }

    public GameObject CreateRandomWeaponLevelOne(Vector3 position)
    {
        // 1
        if (UnityEngine.Random.value < 0.1f)
        {
            var mimic = Instantiate(mimicPistolPrefab, position, Quaternion.identity);
            var mimicCapacity = generator.Next(2, 10);
            var mimicInventory = new CardInventory(mimicCapacity);
            var spells = GetRandomSpellsLevelOne(generator.Next(1, mimicCapacity), Spell.GunShot);
            foreach (var spell in spells)
            {
                mimicInventory.TryAddCard(cardFactory.CreateCard(spell));
            }
            var mimicComponent = mimic.GetComponent<PistolMimic>();
            mimicComponent.SetCards(mimicInventory);
            mimicComponent.SetUseCardsFromInventory(true);
            return mimic;
        }
        // 0.9

        if (UnityEngine.Random.value < 0.2f)
        {
            return InsertRandomSpellsInWeaponLevelOne(canonPrefab, position, generator.NextDouble() < 0.5 ? Spell.CanonBall : Spell.Grenade); // 0.2 * 0.9
        }
        return InsertRandomSpellsInWeaponLevelOne(cheapPistolPrefab, position, Spell.GunShot);
    }

    private GameObject InsertRandomSpellsInWeaponLevelOne(GameObject weaponPrefab, Vector3 position, Spell? firstSpell)
    {
        var weapon = Instantiate(weaponPrefab, position, Quaternion.identity);
        var capacity = generator.Next(2, 16);
        var inventory = new CardInventory(capacity);

        foreach (var spell in GetRandomSpellsLevelOne(generator.Next(1, capacity), firstSpell))
        {
            //Debug.Log(inventory);
            inventory.TryAddCard(cardFactory.CreateCard(spell));
        }

        var weaponComponent = weapon.GetComponent<ICardBasedItem>();
        weaponComponent.SetCards(inventory);
        weaponComponent.SetUseCardsFromInventory(true);
        return weapon;
    }

    private List<Spell> GetRandomSpellsLevelOne(int amount, Spell? first = null)
    {
        
        var allSpells = Enum.GetValues(typeof(Spell)).Cast<Spell>().ToList();
        var result = new List<Spell>();

        if (first != null)
        {
            result.Add((Spell)first);
            amount--;
        }

        for (int i = 0; i < amount; i++)
        {
            result.Add(allSpells[generator.Next(0, allSpells.Count - 1)]);
        }
/*
        Debug.Log(result.Count);
        Debug.Log("here");*/

        return result;
    }
}
