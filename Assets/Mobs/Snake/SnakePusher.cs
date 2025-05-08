using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SnakePusher : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var pushingImpulse = (transform.forward * 5 + transform.up) * 5;
        if (other.gameObject.TryGetComponent<IPushable>(out var pushable))
        {
            pushable.Push(pushingImpulse);
        }
        else if (other.gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
        {
            rigidbody.AddForce(pushingImpulse, ForceMode.Impulse);
        }
    }
}
