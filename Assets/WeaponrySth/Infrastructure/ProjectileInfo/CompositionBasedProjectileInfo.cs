using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// not so abstract
public class CompositionBasedProjectileInfo : IProjectileInfo
{
    private readonly List<object> infos;

    public CompositionBasedProjectileInfo(IEnumerable<object> infos)
    {
        this.infos = new List<object>(infos);
    }

    public bool TryGetProjectileInfo<T>(out T projectileInfo) where T : class
    {
        // yes, linear search
        // i feel that infos list will be kinda small
        foreach (var info in infos)
        {
            if (info is T t)
            {
                projectileInfo = t;
                return true;
            }
        }
        projectileInfo = default;
        return false;
    }
}
