using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CanonBall : ProjectileBase, IDamaging, IUserSecure, IBouncing
{
    [SerializeField]
    private Detector detector;

    public DamageInfo DamageInfo { get; set; } = new DamageInfo(30);
    public int BounceLevel { get; set; } = 0;

    public override event Action<IProjectileInfo> OnProjectileEvent;

    private Rigidbody rb;

    public float ThrowingImpulse = 1;

    private float lifetime = 30;

    private float impulseDamageThreshold = 10;

    private float curTime = 0;

    private float minOffsetBetweenHits = 0.05f;

    private Dictionary<int, float> objectIdToLastHitTime = new();

    private float pushImpulseLimit = 20;

    private int? objProtectedId;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        detector.TriggerEnterEvent += OnDetectorTriggerEnterEvent;
    }

    public override void Fire(Vector3 origin, Vector3 direction, Vector3 baseVelocity = default)
    {
        transform.position = origin;
        transform.forward = direction;

        OnBeforeFire();
        //Debug.Log(rb);
        rb.AddForce(direction.normalized * ThrowingImpulse, ForceMode.Impulse);
    }

    public void OnBeforeFire()
    {
        if (BounceLevel >= 1)
        {
            GetComponent<Collider>().material.bounciness = 1;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        var id = collision.gameObject.GetInstanceID();

        if (objProtectedId != null && objProtectedId.Value == id)
        {
            return;
        }

        if (objectIdToLastHitTime.TryGetValue(id, out var time) && time + minOffsetBetweenHits > curTime)
        {
            objectIdToLastHitTime[id] = curTime;
            return;
        }

        if (collision.contactCount <= 0)
        {
            return;
        }

        var contact = collision.GetContact(0);

        if (contact.impulse.magnitude < impulseDamageThreshold)
        {
            return;
        }

        objectIdToLastHitTime[id] = curTime;

        OnProjectileEvent?.Invoke(
                    new CompositionBasedProjectileInfo(new[] {
                        new HitSomethingInfo(collision.gameObject, contact.point, contact.impulse.normalized, contact.normal)
                    }));

        AttemptHuring(collision.gameObject, contact.impulse * -1);

        StartCoroutine(DieWithTime());
    }

    private void AttemptHuring(GameObject obj, Vector3 impulse)
    {
        if (!obj.TryGetComponent<IHurtable>(out var hurtable))
        {
            return;
        }
        hurtable.TakeDamage(DamageInfo.SetAmount(DamageInfo.Amount * impulse.magnitude / ThrowingImpulse).WithImpulse(impulse));
    }

    private void OnDetectorTriggerEnterEvent(Collider collider)
    {
        if (objProtectedId.HasValue && objProtectedId.Value == collider.gameObject.GetInstanceID())
        {
            return;
        }

        if (rb.velocity.magnitude * rb.mass < impulseDamageThreshold) { return; }
        //Debug.Log(collider.gameObject);
        if (((1 << collider.gameObject.layer) | LayersStorage.Pierceable) != LayersStorage.Pierceable)
        {
            // game object not from pierceable layer
            return;
        }
        var scalar = Mathf.Min(rb.velocity.magnitude * rb.mass, pushImpulseLimit);
        AttemptHuring(collider.gameObject, rb.velocity * rb.mass);
        if (collider.gameObject.TryGetComponent<IPushable>(out var pushable))
        {
            pushable.Push(rb.velocity.normalized * scalar);
        }
    }

    private void Update()
    {
        if (curTime > 0.1f)
        {
            objProtectedId = null;
        }
        curTime += Time.deltaTime;
    }

    private IEnumerator DieWithTime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    public void EnsureProtectionOfObjectWith(int id)
    {
        objProtectedId = id;
    }
}
