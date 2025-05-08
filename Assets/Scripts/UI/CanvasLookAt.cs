using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookAt : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform; 

    void Update()
    {
        transform.LookAt(targetTransform);
        transform.Rotate(0, 180, 0);
    }
}
