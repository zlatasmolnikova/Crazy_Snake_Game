using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitSomethingInfo
{
    public GameObject Target {  get; }

    public Vector3 Position { get; }

    public Vector3 ProjectileDirection { get; }

    public Vector3 SurfaceNormal { get; }
}
