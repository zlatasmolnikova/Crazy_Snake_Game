using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroAppleEnemyDamageDetector : MonoBehaviour
{
    public event Action<Collider> OnTriggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEvent?.Invoke(other);
    }
}
