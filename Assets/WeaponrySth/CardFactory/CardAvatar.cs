using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAvatar : MonoBehaviour
{
    public ICard Card { get; private set; }

    public void SetCard(ICard card)
    {
        if (Card != null)
        {
            throw new System.InvalidOperationException("card shall be set only once");
        }
        Card = card;
        var material = GetComponent<Renderer>().material;
        material.mainTexture = Card.Texture;
    }

    /// <summary>
    /// removes object from scene
    /// </summary>
    public ICard PickUp()
    {
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject);
        return Card;
    }
}
