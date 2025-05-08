using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftMouseClickHandler : MonoBehaviour
{
    [SerializeField] private GameplayInputManager _inputManager;
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
        _inputManager.LeftMouseClickReceived += OnLeftMouseClickReceived;

    }

    private void OnDestroy()
    {
        _inputManager.LeftMouseClickReceived -= OnLeftMouseClickReceived;
    }

    private void OnLeftMouseClickReceived()
    {
        if (player != null)
        {
            var currentItem = player.transform.Find("CurrentItem");
            if (currentItem != null)
            {
                var weapon = currentItem.GetComponent<Weapon>();
                if (weapon != null)
                {
                    weapon.Fire();
                }
            }
        }
    }
}
