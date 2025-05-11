using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class CardInventory
{
    
    // Подбор ICard. params: selected index
    public UnityEvent<int> OnPickUpCard { get; } = new();
    
    // Выброс ICard. params: selected index
    public UnityEvent<int> OnDropCard { get; } = new();
    
    public List<ICard> Cards { get; }

    public int Capasity { get; }

    public int Count { get; private set; } = 0;

    public CardInventory(int capasity = 20)
    {
        Capasity = capasity;
        Cards = new List<ICard>();
        for (int i = 0; i < Capasity; i++)
        {
            Cards.Add(null);
        }
    }

    public bool TryAddCard(ICard card)
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            if (Cards[i] == null)
            {
                Cards[i] = card;
                OnPickUpCard.Invoke(i);
                Count++;
                
                return true;
            }
        }
        return false;
    }

    public void RemoveCard(ICard card)
    {
        var index = Cards.IndexOf(card);
        if (index != -1)
        {
            Cards.RemoveAt(index);
            OnDropCard.Invoke(index);
            Count--;
        }
    }
}
