using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Tracker : ProjectileBase, IHurtable
{
    [SerializeField]
    private Transform tip;

    public float Health { get; private set; } = 1;

    public float MaxHealth => 1;

    public UnityEvent<float, float> OnHealthDecrease => throw new System.NotImplementedException();

    public UnityEvent<float, float> OnHealthIncrease => throw new System.NotImplementedException();

    public override event Action<IProjectileInfo> OnProjectileEvent;

    private float surfaceOffset = 0.1f;

    private float detectionRadius = 20;

    private float lifetime = 0.5f;

    private float minDistance = 0.3f;

    private bool dead = false;

    private Coroutine mainCoroutine;

    private void Start()
    {
        if (tip == null)
        {
            throw new Exception($"tip not set at {name}");
        }
    }

    public override void Fire(Vector3 origin, Vector3 direction, Vector3 baseVelocity = default)
    {
        transform.position = origin + direction.normalized * surfaceOffset;
        transform.forward = direction;

        mainCoroutine = StartCoroutine(StartTracking());
    }

    private IEnumerator StartTracking()
    {
        var recordDistance = detectionRadius + 1;
        GameObject closestObject = null;
        var priority = 0;
        foreach (var collider in Physics.OverlapSphere(transform.position, detectionRadius,
            LayersStorage.PossiblyHurtables))
        {
            var foundObject = collider.gameObject;
            
            var delta = foundObject.transform.position - transform.position;
            if (!foundObject.TryGetComponent<IHurtable>(out _))
            {
                continue;
            }

            var nextPriority = -1;

            if (foundObject.TryGetComponent<Tracker>(out _))
            {
                nextPriority = 0;
            } 
            else if(foundObject.TryGetComponent<IProjectile>(out _))
            {
                nextPriority = 2;
            }
            else if (foundObject.TryGetComponent<PlayerComponent>(out _))
            {
                nextPriority = 1;
            }
            else
            {
                nextPriority = 3;
            }

            if (priority < nextPriority 
                || (priority == nextPriority && delta.magnitude < recordDistance))
            {
                //Physics.Raycast(transform.position, transform.forward, )
                recordDistance = delta.magnitude;
                closestObject = foundObject;
                priority = nextPriority;
            }
        }

        if (closestObject != null)
        {
            var direction = closestObject.transform.position - transform.position;
            if (direction.magnitude < minDistance)
            {
                direction = transform.forward;
            }
            direction.Normalize();
            transform.forward = direction;
        }

        yield return new WaitForSeconds(lifetime);

        Die();
    }

    public void ConsumeDamage(float amount)
    {
        throw new NotImplementedException();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        Health -= damageInfo.Amount;
        if (Health <= 0)
        {
            if (mainCoroutine != null)
            {
                StopCoroutine(mainCoroutine);
            }
            Die();
        }
    }

    private void Die()
    {
        if (dead) return;
        dead = true;

        var projectileInfo = new CompositionBasedProjectileInfo(new[] {
            new ExpirationInfo(transform.position, transform.forward)
        });

        OnProjectileEvent?.Invoke(projectileInfo);
        Destroy(gameObject);
    }
}
