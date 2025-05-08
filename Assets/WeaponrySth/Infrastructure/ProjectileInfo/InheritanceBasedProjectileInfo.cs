using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InheritanceBasedProjectileInfo : IProjectileInfo
{
    public bool TryGetProjectileInfo<T>(out T projectileInfo)
        where T : class
    {
        projectileInfo = this as T;
        if (projectileInfo != null)
        {
            return true;
        }
        return false;
    }
}
