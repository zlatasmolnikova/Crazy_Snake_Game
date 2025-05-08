using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileInfo
{
    public bool TryGetProjectileInfo<T>(out T projectileInfo) where T : class;      //sorry bout that where
}
