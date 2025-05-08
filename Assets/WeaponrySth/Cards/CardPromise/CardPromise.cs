using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardPromise : MonoBehaviour
{
    [SerializeField]
    private Spell spell;

    private void Start()
    {
        var factory = GameObject.FindGameObjectWithTag("CardFactory").GetComponent<CardFactory>();
        factory.CreateCardAvatar(factory.CreateCard((Spell)spell), transform.position);
        Destroy(gameObject);
    }
}
