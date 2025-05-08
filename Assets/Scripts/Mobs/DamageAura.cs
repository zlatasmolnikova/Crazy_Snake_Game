using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAura : MonoBehaviour
{
    public float Damage = 1;

    public float Radius = 3;


    void Start()
    {
        StartCoroutine(DealDamage());
    }

    private IEnumerator DealDamage()
    {
        while (true)
        {
            foreach (var item in Physics.OverlapBox(transform.position, Vector3.one * Radius))
            {
                if (item.gameObject.TryGetComponent<IHurtable>(out var hurtable))
                {
                    hurtable.TakeDamage(new DamageInfo(Damage));
                }
            };
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * Radius);
    }
}
