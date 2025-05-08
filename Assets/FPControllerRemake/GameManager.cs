using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public InputMap InputMap => _inputMap.Value;

    private Lazy<InputMap> _inputMap = new Lazy<InputMap>(() => new InputMap());

    private void Awake()
    {
        InputMap.Gameplay.Movement.performed += Movement_performed;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        InputMap.Enable();
        InputMap.Gameplay.Enable();     // is this important? seems like not;
    }



    private void Movement_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //Debug.Log($"movement performed: {obj.ReadValue<Vector2>()}");
    }

    private void OnDisable()
    {
        InputMap.Disable();
    }
}
