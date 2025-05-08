using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// i cant instantiate object from not Monobehaviour
/// </summary>
public class Instantiator : MonoBehaviour
{
    public GameObject InstantiatePrefab(GameObject gameObject)
    {
        return Instantiate(gameObject);
    }
}
