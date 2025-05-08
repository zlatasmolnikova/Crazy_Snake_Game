using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastAccessInventoryUI : MonoBehaviour
{
    // [SerializeField]
    private PlayerComponent player;
    
    [SerializeField] 
    private Image[] FastAccessItemHolderImages;
    private GameObject[] ItemHolders;
    private Image[] ItemImages;
    
    [SerializeField]
    private GameObject itemHolderPrefab;
   
    public int totalCapacity { get; private set; }
    
    void Start()
    {
        player = FindObjectOfType<PlayerComponent>();
        Debug.Log($"Player in Inventory: {player}");
        LoadInventory();
        player.Inventory.OnCnangeIndex.AddListener(OnCnangeIndex);
        player.Inventory.OnPickUpItem.AddListener(OnPickUpItem);
        player.Inventory.OnDropItem.AddListener(OnDropItem);
    }

    private void LoadInventory()
    {
        totalCapacity = player.Inventory.Capacity;
        ItemHolders = new GameObject[totalCapacity];
        ItemImages = new Image[totalCapacity];
        FastAccessItemHolderImages = new Image[totalCapacity];
        
        for (var i = 0; i < totalCapacity; i++)
        {
            ItemHolders[i] = Instantiate(itemHolderPrefab, transform);
            FastAccessItemHolderImages[i] = ItemHolders[i].GetComponentsInChildren<Image>()[1];
            ItemImages[i] = ItemHolders[i].GetComponent<Image>();
        }
        
        FastAccessItemHolderImages[player.Inventory.SelectedIndex].color = Color.red;
    }
    
    private void OnDropItem(int index)
    {
        ItemImages[index].sprite = null;
        ItemImages[index].color = Color.clear;
    }


    private void OnPickUpItem(int index)
    {
        Debug.Log(ItemImages[index]);
        var sprite = player.Inventory.SelectedItem.GetItemAvatarSprite();
        if (sprite != null)
        {
            ItemImages[index].sprite = sprite;
            ItemImages[index].color = Color.white;
        }
    }

    private void OnCnangeIndex(int lastIndex, int index)
    {
        if (0 <= lastIndex && lastIndex < totalCapacity
            && 0 <= index && index < totalCapacity)
        {
            FastAccessItemHolderImages[lastIndex].color = Color.white;
            FastAccessItemHolderImages[index].color = Color.red;
        }
    }
}
