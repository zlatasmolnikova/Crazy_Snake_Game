using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunShot : ProjectileBase, IDamaging, IPierceable, IBouncing
{
    public static Spell Spell => Spell.GunShot;

    public override event Action<IProjectileInfo> OnProjectileEvent;

    public DamageInfo DamageInfo { get; set; } = new DamageInfo(10);

    [SerializeField]
    private GameObject lineDetectorPrefab;

    [DoNotSerialize]
    public bool CanPierce { get; set; } = false;

    private int bounceLevel = 0;
    public int BounceLevel { 
        get => bounceLevel;
        set
        {
            if (value >= 0)
            {
                bounceLevel = value;
            }
        }
    }

    private int bounceCountLeft = 0;

    private readonly float range = 100;

    private LineRenderer lineRenderer;

    private bool used = false;

    private Vector3? overridenVisibleRayBegin;

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));   // may be use composition? nah, i`d win
        lineRenderer.material.color = Color.yellow;
    }

    private void Start()
    {
        if (lineDetectorPrefab == null)
        {
            throw new Exception("line detector not found");
        }
    }

    public override void Fire(Vector3 origin, Vector3 direction, Vector3 baseVelocity = default)
    {
        StartCoroutine(StartFire(origin, direction, baseVelocity));

        /*if (used)
        {
            throw new Exception("wtf maaan! its disposable");
        }
        used = true;

        BeforeFire();

        var expirationInfos = new List<object>();

        var detector = Instantiate(lineDetectorPrefab).GetComponent<LineDetector>();

        foreach (var hitInfo in detector.PerformAction(origin, direction, range, LayersStorage.NotPierceableObstacles, 
            LayersStorage.Pierceable, CanPierce, overridenVisibleRayBegin))
        {
            AttemptHurting(hitInfo.collider.gameObject);
            expirationInfos.Add(
                    new HitSomethingInfo(hitInfo.collider.gameObject,
                        hitInfo.point,
                        direction.normalized,
                        hitInfo.normal)
                    );
        }

        expirationInfos.Add(new ExpirationInfo(detector.ExpirationPos, direction.normalized));

        StartCoroutine(ActivateNextAndExpire(
                new CompositionBasedProjectileInfo(expirationInfos)
            ));*/
    }

    private IEnumerator StartFire(Vector3 origin, Vector3 direction, Vector3 baseVelocity = default)
    {
        if (used)
        {
            throw new Exception("wtf maaan! its disposable");
        }
        used = true;

        BeforeFire();

        var curDirection = direction.normalized;

        var isFirstIteration = true;

        var curOrigin = origin;

        while (bounceCountLeft-- >= 0)
        {
            var detector = MakePartFire(curOrigin, curDirection, out var expInfos, isFirstIteration ? overridenVisibleRayBegin : null);
            isFirstIteration = false;
            if (detector.HardHit != null)
            {
                var d = curDirection;
                var n = ((RaycastHit)detector.HardHit).normal.normalized;

                var reflectionDirection = d - 2 * Vector3.Dot(d, n) * n;
                curDirection = reflectionDirection;
            }
            curOrigin = detector.ExpirationPos;
            yield return new WaitForSeconds(0.03f);
            OnProjectileEvent?.Invoke(new CompositionBasedProjectileInfo(expInfos));
        }

        Destroy(gameObject);
    }

    private LineDetector MakePartFire(Vector3 origin, Vector3 direction, out List<object> expirationInfos, Vector3? visibleStartPos = null)
    {
        expirationInfos = new List<object>();

        var detector = Instantiate(lineDetectorPrefab).GetComponent<LineDetector>();

        foreach (var hitInfo in detector.PerformAction(origin, direction, range, LayersStorage.NotPierceableObstacles,
            LayersStorage.Pierceable, CanPierce, visibleStartPos))
        {
            AttemptHurting(hitInfo.collider.gameObject);
            expirationInfos.Add(
                    new HitSomethingInfo(hitInfo.collider.gameObject,
                        hitInfo.point,
                        direction.normalized,
                        hitInfo.normal)
                    );
        }

        expirationInfos.Add(new ExpirationInfo(detector.ExpirationPos, direction.normalized));

        return detector;
    }

    /*private void OldFire(Vector3 origin, Vector3 direction, Vector3 baseVelocity = default)
    {
        if (used)
        {
            throw new Exception("wtf maaan! its disposable");
        }
        used = true;

        BeforeFire();

        var rangeLimit = range;
        var expirationPos = origin + direction.normalized * range;
        var expirationInfos = new List<object>();

        if (CanPierce)
        {
            if (Physics.Raycast(origin, direction.normalized, out var hitOnHardSurface, rangeLimit, LayersStorage.NotPierceableObstacles))
            {
                rangeLimit = hitOnHardSurface.distance;
                expirationPos = hitOnHardSurface.point;
                AttemptHurting(hitOnHardSurface.collider.gameObject);
                expirationInfos.Add(
                        new HitSomethingInfo(hitOnHardSurface.collider.gameObject,
                            hitOnHardSurface.point,
                            direction.normalized,
                            hitOnHardSurface.normal)
                        );
                Debug.Log(hitOnHardSurface.collider.gameObject);
            }

            foreach (var hitSoft in Physics.RaycastAll(
                    origin, direction.normalized, rangeLimit, LayersStorage.Pierceable))
            {
                AttemptHurting(hitSoft.collider.gameObject);
                OnProjectileEvent?.Invoke(new CompositionBasedProjectileInfo(new[]
                {
                    new HitSomethingInfo(hitSoft.collider.gameObject,
                        hitSoft.point, direction.normalized, hitSoft.normal)
                }));
            }
        }
        else
        {
            if (Physics.Raycast(origin, direction.normalized, out var firstHit, rangeLimit, LayersStorage.Pierceable | LayersStorage.NotPierceableObstacles))
            {
                AttemptHurting(firstHit.collider.gameObject);
                expirationInfos.Add(
                        new HitSomethingInfo(firstHit.collider.gameObject,
                            firstHit.point,
                            direction.normalized,
                            firstHit.normal)
                        );
                expirationPos = firstHit.point;
            }
        }

        if (overridenVisibleRayBegin == null)
        {
            lineRenderer.SetPosition(0, origin);
        }
        else
        {
            lineRenderer.SetPosition(0, (Vector3)overridenVisibleRayBegin);
        }

        lineRenderer.SetPosition(1, expirationPos);

        expirationInfos.Add(new ExpirationInfo(expirationPos, direction.normalized));

        StartCoroutine(ShowLaserAndExpire(
                new CompositionBasedProjectileInfo(expirationInfos)
            ));
    }*/

    private void BeforeFire()
    {
        bounceCountLeft = BounceLevel * 2;
    }

    /// <summary>
    /// does not change actual ray tracing, only visible trace
    /// </summary>
    /// <param name="point"></param>
    public void SetVisibleRayBeginning(Vector3 point)
    {
        overridenVisibleRayBegin = point;
    }

    private bool AttemptHurting(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<IHurtable>(out var hurtable))
        {
            hurtable.TakeDamage(DamageInfo);
            return true;
        }
        return false;
    }

/*    private IEnumerator ShowLaserAndExpire(IProjectileInfo projectileInfo)
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f); // ”меньшенное врем€ видимости дл€ имитации вспышки
        OnProjectileEvent?.Invoke(projectileInfo);
        yield return new WaitForSeconds(0.05f);
        lineRenderer.enabled = false;
        
        Destroy(gameObject);
    }*/

    private IEnumerator ActivateNextAndExpire(IProjectileInfo projectileInfo)
    {
        yield return new WaitForSeconds(0.05f);
        OnProjectileEvent?.Invoke(projectileInfo);
        Destroy(gameObject);
    }
}
