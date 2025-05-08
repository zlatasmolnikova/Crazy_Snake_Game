using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPControllerRemake : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;        //insert in editor

    private Vector2 movementInput = Vector2.zero;

    private Vector2 viewDirectionDelta = Vector2.zero;

    [SerializeField]
    private float speed = 100;

    [SerializeField]
    private float speedBias = 3;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        gameManager.InputMap.Gameplay.Movement.performed += HandleMoveventUpdate;
        gameManager.InputMap.Gameplay.ViewDirectionDelta.performed += HandleViewDirectionDeltaUpdate;

        gameManager.InputMap.Gameplay.Movement.canceled += HandleMoveventUpdate;
        gameManager.InputMap.Gameplay.ViewDirectionDelta.canceled += HandleViewDirectionDeltaUpdate;
    }

    private void HandleMoveventUpdate(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        movementInput = obj.ReadValue<Vector2>();
    }

    private void HandleViewDirectionDeltaUpdate(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        viewDirectionDelta = obj.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        //BaseLineMovement();
        var desiredAcceleration = movementInput.normalized * speed;
        var speedProjectionAbs = (desiredAcceleration[0] * rb.velocity[0] 
            + desiredAcceleration[1] * rb.velocity[2]) / (desiredAcceleration.magnitude + 1e-3f);

        Debug.Log(Sigmoid(-speedProjectionAbs - speedBias));

        var resultAcceleration2D = desiredAcceleration * ReverseSqr(speedProjectionAbs);

        //var acceleration = new Vector3(resultAcceleration2D[0], 0, resultAcceleration2D[1]);

        var acceleration = new Vector3(desiredAcceleration[0] * ReverseSqr(rb.velocity[0] * desiredAcceleration[0] / (Mathf.Abs(desiredAcceleration[0]) + 1e-3f)), 0,
            desiredAcceleration[1] * ReverseSqr(rb.velocity[2] * desiredAcceleration[1] / (Mathf.Abs(desiredAcceleration[1]) + 1e-3f)));

        Debug.Log(acceleration);

        rb.velocity += acceleration * Time.deltaTime * speed;

        /*var force = new Vector3(desiredAcceleration[0], 0, desiredAcceleration[1]);

        rb.AddForce(force, ForceMode.Force);*/
    }

    private float ReverseSqr(float x, float offset = 1f)
    {
        if (x < offset)
        {
            return 1 / (offset * offset);
        }
        return 1 / (x * x);
    }

    private float Sigmoid(float x, float clampAtDeviation = 10)
    {
        if (x >= clampAtDeviation)
        {
            return 1;
        }

        if (x <= -clampAtDeviation)
        {
            return 0;
        }

        return 1 / (1 + Mathf.Pow(Mathf.Exp(-x), 2));
    }

    private void BaseLineMovement()
    {
        var planeVelocity = new Vector2(rb.velocity[0], rb.velocity[2]);

        rb.velocity *= 0.8f;

        rb.AddForce(new Vector3(movementInput[0], 0, movementInput[1]) * Time.deltaTime * speed, ForceMode.VelocityChange);

    }
}
