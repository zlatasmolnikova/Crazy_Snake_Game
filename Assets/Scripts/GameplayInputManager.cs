using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayInputManager : MonoBehaviour
{
    public event System.Action<Vector2> RotationInputReceived;
    public event System.Action LeftMouseClickReceived;

    private InputMap _inputMap;
    private MouseGameplayInput _mouseInput;

    private void Awake()
    {
        _inputMap = new InputMap();
        _inputMap.Enable();

        InitMouseInput(_inputMap);
    }

    private void OnRotationInputReceived(Vector2 deltaVector)
    {
        RotationInputReceived?.Invoke(deltaVector);
    }

    private void OnLeftMouseClickReceived()
    {
        LeftMouseClickReceived?.Invoke();
    }

    private void InitMouseInput(InputMap map)
    {
        _mouseInput = new MouseGameplayInput(map);
        _mouseInput.RotationInputReceived += OnRotationInputReceived;
        _mouseInput.LeftMouseClickReceived += OnLeftMouseClickReceived;
        _mouseInput.InitInputs();
    }
}