using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float damage;
    public float range;

    public abstract void Fire();
}