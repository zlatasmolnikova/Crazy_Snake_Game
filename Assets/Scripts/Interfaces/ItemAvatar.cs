using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAvatar : MonoBehaviour
{
    private IInventoryItem storedItem;

    private void Start()
    {
        storedItem = GetComponent<IInventoryItem>();
    }

    public IInventoryItem PickUp()
    {
        return storedItem;
    }
}
