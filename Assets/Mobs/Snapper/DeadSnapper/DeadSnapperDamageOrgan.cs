using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadSnapperDamageOrgan : MonoBehaviour
{
    private float lifetime = 1f;

    private DamageInfo damageInfo = new DamageInfo(5, DamageType.MeleeDamage);

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0f)
        {
            DestroyDamagingOrgan();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<IHurtable>(out var hurtable))
        {
            hurtable.TakeDamage(damageInfo);
        }
    }

    private void DestroyDamagingOrgan()
    {
        Destroy(gameObject);    //destroying only damaging object
    }
}
