/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationHandler : MonoBehaviour
{
    [SerializeField] private GameplayInputManager _inputManager;

    [SerializeField] private Transform _rotationTarget;
    [SerializeField] private float _sensitivity = 1f;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 30f;

    private float horizontal = 0f;
    private float vertical = 0f;

    private void Start()
    {
        _inputManager.RotationInputReceived += OnRotationInputReceived;
    }

    private void OnDestroy()
    {
        _inputManager.RotationInputReceived -= OnRotationInputReceived;
    }

    private void OnRotationInputReceived(Vector2 delta)
    {
        var dt = Time.deltaTime;
        vertical -= _sensitivity * delta.y * dt;
        horizontal += _sensitivity * delta.x * dt;

        vertical = Mathf.Clamp(vertical, minVerticalAngle, maxVerticalAngle);
        _rotationTarget.eulerAngles = new Vector3(vertical, horizontal, 0f);
    }
}
*/