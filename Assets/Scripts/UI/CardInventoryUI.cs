using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInventoryUI : MonoBehaviour
{
    [SerializeField] 
    private GameObject WeaponInventoryPlaceHolder;
    [SerializeField] 
    private GameObject PlayerInventoryPlaceHolder;
    [SerializeField] 
    private WeaponInsideInventory[] WeaponHolders;
    
    // [SerializeField] private GameObject Player;
    private PlayerComponent player;
    private CardInventory cardInventory => player.CardInventory;
    private Inventory inventory => player.Inventory;

    private void Start()
    {
        player = FindObjectOfType<PlayerComponent>();
        Debug.Log($"Player in Card Inventory: {player}");
        Debug.Log(gameObject.activeSelf);
        gameObject.SetActive(false);
        Debug.Log(gameObject.activeSelf);
        
    }
    
    public void OpenInventory()
    {
        gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        player.gameObject.SetActive(false);
        Time.timeScale = 0f;
        for (var i = 0; i < WeaponHolders.Length; i++)
        {
            var cardBasedItem = inventory.Items[i] as ICardBasedItem;
            WeaponHolders[i].ReloadInventory(cardBasedItem);
        }
    }
    
    public void CloseInventory()  
    {
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.gameObject.SetActive(true);
        Time.timeScale = 1f;
    }
}
