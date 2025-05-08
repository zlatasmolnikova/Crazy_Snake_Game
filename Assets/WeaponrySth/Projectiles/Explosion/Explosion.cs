using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : ProjectileBase, IDamaging
{
    public static Spell Spell => Spell.Explosion;

    public override event Action<IProjectileInfo> OnProjectileEvent;

    [SerializeField]
    private GameObject externalSurface;

    private float initialVisibleRadius = 0.1f;

    private float damageRadius = 0.75f;

    private float maxVisibleRadius = 1f;

    private float lifetime = 0.2f;

    private float impulseModule = 7;

    private float time = 0;

    public DamageInfo DamageInfo { get; set; } = new DamageInfo(20);

    public override void Fire(Vector3 origin, Vector3 direction, Vector3 baseVelocity = default)
    {
        transform.position = origin;
        StartCoroutine(Explode());
    }

    public void SetParams(float damageRadius=1f, float impulseModule=7, float lifetime=0.2f, float? minVisisbleRadius=null, float? maxVisibleRadius = null)
    {
        if (minVisisbleRadius == null)
        {
            minVisisbleRadius = (damageRadius / this.damageRadius) * this.initialVisibleRadius;
        }
        this.initialVisibleRadius = (float)minVisisbleRadius;
        if (maxVisibleRadius == null)
        {
            maxVisibleRadius = (damageRadius / this.damageRadius) * this.maxVisibleRadius;
        }
        this.maxVisibleRadius = (float)maxVisibleRadius;

        this.lifetime = lifetime;
        this.impulseModule = impulseModule;
        this.damageRadius = damageRadius;
    }

    private IEnumerator Explode()
    {
        AdjustSurfaceSize();
        externalSurface.SetActive(true);

        DealDamageAndPush();

        yield return null;
        yield return StartCoroutine(ExpandSurface());

        OnProjectileEvent?.Invoke(
                new CompositionBasedProjectileInfo(new object[]
                {
                    new ExpirationInfo(transform.position, UnityEngine.Random.onUnitSphere)
                })
            );

        Destroy(gameObject);
    }

    private IEnumerator ExpandSurface()
    {
        while (time < lifetime)
        {
            time += Time.deltaTime;
            AdjustSurfaceSize();
            yield return null;
        }
    }

    private void AdjustSurfaceSize()
    {
        var k = (maxVisibleRadius - initialVisibleRadius) / Mathf.Sqrt(lifetime);
        externalSurface.transform.localScale = Vector3.one * (2 * Mathf.Min(k * Mathf.Sqrt(time) + initialVisibleRadius, maxVisibleRadius));
    }

    private void DealDamageAndPush()
    {
        //Debug.Log("called me?");
        foreach (var collision in Physics.OverlapSphere(
            transform.position, damageRadius, 
            LayersStorage.NotPierceableObstacles | LayersStorage.Pierceable))    //use more specific layer to optimize
        {
            if (collision.gameObject.TryGetComponent<IHurtable>(out var hurtable))
            {
                hurtable.TakeDamage(DamageInfo);
                //Debug.Log(hurtable.ToString());
            }

            var deltaDirection = (collision.gameObject.transform.position - transform.position).normalized;
            if (collision.gameObject.TryGetComponent<IPushable>(out var pushable))
            {
                pushable.Push(deltaDirection * 10);
            } 
            else if (collision.gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.AddForce(deltaDirection * impulseModule, ForceMode.Impulse);
            }
        }
    }
}
