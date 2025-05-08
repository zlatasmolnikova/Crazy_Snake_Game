using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// implements a common method;
/// DO NOT use to determine if something is a projectile - better use IProjectile
/// </summary>
public abstract class ProjectileBase : MonoBehaviour, IProjectile
{
    public abstract event Action<IProjectileInfo> OnProjectileEvent;

    public abstract void Fire(Vector3 origin, Vector3 direction, Vector3 baseVelocity = default);

    public virtual bool TryGetModificationInterface<T>(out T modifiable) where T : class
    {
        if (this is T t)
        {
            modifiable = t;
            return true;
        }
        modifiable = default;
        return false;
    }
}
