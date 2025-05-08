using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Environment;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerComponent : MonoBehaviour, IHurtable, IUser, IPushable, IPlacer
{
    public UnityEvent<float, float> OnHealthDecrease { get; } = new();
    public UnityEvent<float, float> OnHealthIncrease { get; } = new();

    
    [SerializeField]
    private float ItemPickUpRadius = 3;

    [SerializeField]
    private Transform handTransform;

    public Transform HandTransform => handTransform;

    [SerializeField]
    private Transform cameraTransform;

    public Transform CameraTransform => cameraTransform;

    public Transform SelfTransform => transform;
    
    public Inventory Inventory => inventory;
    private Inventory inventory;

    public CardInventory CardInventory;

    private QuakeCPMPlayerMovement QuakeMovenentController;

    [SerializeField]
    private float health = 100;
    
    [SerializeField]
    private float maxHealth = 100f;

    public float Health => health;
    public float MaxHealth => maxHealth;

    public Vector3 Velocity => QuakeMovenentController.GetVelocity();

    public UserType UserType => UserType.Player;

    public GameObject UserGameObject => gameObject;

    public void ConsumeDamage(float amount)
    {
        TakeDamage(new DamageInfo(amount));
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        health -= damageInfo.Amount;
        Debug.Log($"Player Hurt: health = {health}");
        if (health < 0)
        {
            Debug.Log("stop shooting! I`m already dead");
            SceneManager.UnloadSceneAsync("SampleScene");
            SceneManager.LoadSceneAsync("Menu");
            
        }
        else
        {
            OnHealthDecrease.Invoke(health, MaxHealth);
        }
    }

    private void Awake()
    {
        inventory = new Inventory();  

        CardInventory = new CardInventory(24);

        QuakeMovenentController = GetComponent<QuakeCPMPlayerMovement>();

        SingletonInputManager.instance.InputMap.Gameplay.PickItem.performed += PickItem_performed;
        SingletonInputManager.instance.InputMap.Gameplay.DropItem.performed += DropItem_performed;

        SingletonInputManager.instance.InputMap.Gameplay.UseItemPrimaryAction.performed += UseItemPrimaryAction_performed;
        SingletonInputManager.instance.InputMap.Gameplay.UseItemSecondaryAction.performed += UseItemSecondaryAction_performed;

        SingletonInputManager.instance.InputMap.Gameplay.ItemSelect.performed += ItemSelect_performed;
    }

    private void ItemSelect_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        var input = (int)obj.ReadValue<float>() - 1;
        if (input < 0 || input >= inventory.Capacity)
        {
            throw new ArgumentException($"input must be an integer in range 1 to {input + 1}");
        }
        inventory.SelectedIndex = input;
    }

    private void UseItemSecondaryAction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        inventory.SelectedItem?.TryUseSecondaryAction();
    }

    private void UseItemPrimaryAction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        inventory.SelectedItem?.TryUsePrimaryAction();
    }

    private void DropItem_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        TryDropItem();
    }

    private void PickItem_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        TryPickUpItemOrCard();
    }

    private bool TryPickUpItemOrCard()
    {
        //use layers or tags to optimize
        // should be remade to check chosest item first
        foreach( var item in Physics.OverlapSphere(transform.position, ItemPickUpRadius))
        {
            Debug.Log(item);
            if (item.TryGetComponent<ItemAvatar>(out var itemAvatar))
            {
                if (inventory.SelectedItem == null)
                {
                    inventory.SelectedItem = itemAvatar.PickUp();
                    inventory.SelectedItem.SetUser(this);
                    inventory.SelectedItem.OnSelect();
                    return true;
                }
            }
            if (item.TryGetComponent<CardAvatar>(out var cardAvatar))
            {
                if (CardInventory.TryAddCard(cardAvatar.Card))
                {
                    Debug.Log(cardAvatar.Card);
                    cardAvatar.PickUp();
                    return true;
                }
            }
            if (item.TryGetComponent<DoorKey>(out var doorKey))
            {
                doorKey.OnPickUp();
                return true;
            }
            if (item.TryGetComponent<Door>(out var door))
            {
                door.Interact();
                return true;
            }
        }
        return false;
    }

    private bool TryDropItem()
    {
        return inventory.TryDropCurrent();
    }

    public void Push(Vector3 impulse)
    {
        QuakeMovenentController.AddVelocity(impulse);   // believe that player mass is 1
    }

    public void Place(PlacementManager manager)
    {
        var position = manager.GetPosition(0);
        var positionInUnity = manager.GetTransformPosition(position);
        positionInUnity.y = manager.MazeBuilder.WallHeight;
        manager.AddOnPlacementMap(position, 31);
        transform.position = positionInUnity;
    }
}
