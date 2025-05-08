using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCardFiller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var inventory = GameObject.FindAnyObjectByType<LaggyPistol>().CardInventory;
        var factory = GameObject.FindGameObjectWithTag("CardFactory").GetComponent<CardFactory>();
        inventory.TryAddCard(factory.CreateCard(Spell.Grenade));
        inventory.TryAddCard(factory.CreateCard(Spell.GunShot));
        inventory.TryAddCard(factory.CreateCard(Spell.Grenade));
        inventory.TryAddCard(factory.CreateCard(Spell.BouncinessIncrease));
    }
}
