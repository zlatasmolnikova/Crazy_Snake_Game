using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSomethingInfo : IHitSomethingInfo
{
    public GameObject Target { get; set; }

    public Vector3 Position { get; set; }

    public Vector3 ProjectileDirection { get; set; }

    public Vector3 SurfaceNormal { get; set; }

    public HitSomethingInfo(GameObject target, Vector3 position, Vector3 projectileDirection, Vector3 surfaceNormal)
    {
        Target = target;
        Position = position;
        ProjectileDirection = projectileDirection;
        SurfaceNormal = surfaceNormal;
    }
}
