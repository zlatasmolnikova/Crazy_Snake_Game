using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GrenadeProjectile : ProjectileBase, IHurtable, IBouncing
{
    public static Spell Spell => Spell.Grenade;

    public override event Action<IProjectileInfo> OnProjectileEvent;

    private Rigidbody rb;

    private Collider myCollider;

    private bool exploded = false;

    public float Speed { get; set; } = 20f;

    private float explosionDamage = 40;

    public float Health { get; set; } = 1;
    

    public GameObject ExplosionPrefab;      // set in inspector

    public Detector Detector;       // set in inspector

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = rb.GetComponent<Collider>();
    }

    private void Start()
    {
        Detector.TriggerEnterEvent += OnDetectorTriggerEnterEvent;
    }

    public override void Fire(Vector3 origin, Vector3 direction, Vector3 baseVelocity = default)
    {
        transform.position = origin;
        transform.forward = direction.normalized;

        EnsureBouncy();

        rb.velocity = direction.normalized * Speed + baseVelocity;
        rb.AddTorque(UnityEngine.Random.insideUnitSphere * 10);
    }

    private int bounceLevel = 0;

    public int BounceLevel { 
        get => bounceLevel;
        set
        {
            bounceLevel = Math.Max(0, Math.Min(1, value));
            if (bounceLevel == 0)
            {
                myCollider.material.bounciness = 0.2f;
                myCollider.material.dynamicFriction = 0.5f;
                myCollider.material.staticFriction = 0.5f;
            }
            else if (bounceLevel == 1)
            {
                myCollider.material.bounciness = 1;
                myCollider.material.dynamicFriction = 0;
                myCollider.material.staticFriction = 0;
            }
        }
    }

    public float MaxHealth => throw new NotImplementedException();

    public UnityEvent<float, float> OnHealthDecrease => throw new NotImplementedException();

    public UnityEvent<float, float> OnHealthIncrease => throw new NotImplementedException();

    private void EnsureBouncy()
    {
        // this thing has side effects
        BounceLevel = BounceLevel;
    }

    private void Explode()
    {
        if (exploded)
        {
            return;
        }
        exploded = true;
        //Debug.Log("explode");
        if (!Instantiate(ExplosionPrefab).TryGetComponent<Explosion>(out var explosion))
        {
            throw new Exception("not an explosion");
        }
        explosion.SetParams(2, 8, 0.3f);
        explosion.DamageInfo = explosion.DamageInfo.SetAmount(explosionDamage);
        explosion.Fire(transform.position, transform.forward);
        OnProjectileEvent?.Invoke(
                new CompositionBasedProjectileInfo(new[]
                {
                    new ExpirationInfo(transform.position, transform.forward)
                })
            );
        Destroy(gameObject);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        // immune to explosion
        if (damageInfo.Type == DamageType.ExplosionDamage)
        {
            return;
        }

        //Health -= damageInfo.Amount;

        myCollider.enabled = false;
        Explode();
    }
    
    // used in detector
    private void OnDetectorTriggerEnterEvent(Collider other)
    {
        if (other.gameObject == gameObject
            || other.gameObject.TryGetComponent<PlayerComponent>(out _)) { return; }
        //Debug.Log(other.gameObject);

        //Debug.Log(other.gameObject.layer);
        if (BounceLevel > 0 && (LayersStorage.NotHurtableHard & (1 << other.gameObject.layer)) != 0)
        {
            return;
        }
        
        Detector.enabled = false;
        Explode();
    }

    public void ConsumeDamage(float amount)
    {
        throw new NotImplementedException();
    }
}
