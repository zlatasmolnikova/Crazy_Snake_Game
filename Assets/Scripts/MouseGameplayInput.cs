using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseGameplayInput
{
    public InputMap inputMap;
    private InputAction mouseMoveAction;
    private InputAction mouseClickAction;

    public event Action<Vector2> RotationInputReceived;
    public event Action LeftMouseClickReceived;

    public MouseGameplayInput(InputMap map)
    {
        inputMap = map;

        mouseMoveAction = inputMap.KeyboardAndMouse.deltaMouse;
        mouseClickAction = inputMap.KeyboardAndMouse.LeftMouseClick;
    }

    public void InitInputs()
    {
        mouseMoveAction.performed += context =>
        {
            var mouseDelta = context.ReadValue<Vector2>();
            RotationInputReceived?.Invoke(mouseDelta);
        };

        mouseClickAction.performed += context =>
        {
            LeftMouseClickReceived?.Invoke();
        };
    }
}
