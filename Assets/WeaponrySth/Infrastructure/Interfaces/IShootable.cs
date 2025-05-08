using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShootable
{
    public void Fire(Vector3 origin, Vector3 direction, Vector3 baseVelocity=default);
}
