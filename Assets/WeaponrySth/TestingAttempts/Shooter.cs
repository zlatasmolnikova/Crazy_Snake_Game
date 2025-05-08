using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject GunShotProjectile;

    void Start()
    {
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            var instance = Instantiate(GunShotProjectile, transform.position, transform.rotation);
            if (instance.TryGetComponent<GunShot>(out var gunShot) 
                && gunShot.TryGetModificationInterface<IShootable>(out var mod))
            {
                mod.Fire(transform.position, transform.TransformDirection(Vector3.forward));
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}
