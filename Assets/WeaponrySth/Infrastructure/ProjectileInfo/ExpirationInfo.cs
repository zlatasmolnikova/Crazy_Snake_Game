using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpirationInfo : IExpirationInfo
{
    public Vector3 Position { get; set; }

    public Vector3 ProjectileDirection { get; set; }

    public ExpirationInfo(Vector3 position, Vector3 projectileDirection)
    {
        Position = position;
        ProjectileDirection = projectileDirection;
    }
}
