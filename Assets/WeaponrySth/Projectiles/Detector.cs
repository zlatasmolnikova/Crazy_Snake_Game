using System;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public event Action<Collider> TriggerEnterEvent;

    private void OnTriggerEnter(Collider other)
    {
        TriggerEnterEvent?.Invoke(other);
    }
}
