using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTestScript : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private void Update()
    {
        var dir = target.transform.position - transform.position;
        var rotation = Quaternion.LookRotation(dir);
        Debug.Log(rotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 5 * Time.deltaTime);
    }
}
