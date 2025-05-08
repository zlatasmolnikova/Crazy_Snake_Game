using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolMimicGoreManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddExplosionForce(60, transform.position, 2, 1);
            }
        }
        
    }
}
